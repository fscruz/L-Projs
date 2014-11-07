using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Configuration;
using System.Web.Services.Protocols;
using System.Diagnostics;

using PCA.Framework.Web.Soap.Customs;
using PCA.LawOffice.Messages;
using PCA.Framework.Helpers;
using System.Reflection;

namespace LOSystemTests.Communication
{
    public class WSProxy
    {

        private static readonly WSProxy _instance = new WSProxy();

        [DefaultValue(SoapExtensionCompressionVersion.Net1_1)]
        public static SoapExtensionCompressionVersion CompressionVersion
        {
            get;
            set;
        }

        [DefaultValue("https://devappwin01.lawoffice.com.br/servicepro01")]
        public String BackOfficeHost
        {
            get;
            set;
        }

        public LawOfficeContextData Context
        {
            get;
            set;
        }
        

        public static WSProxy Instance
        {
            get { return _instance; }
        }

        // To ensure compiler does not mark class as beforefieldinit
        static WSProxy()
        { }

        private WSProxy()
        {
            Initialize();
        }

        public static void Initialize()
        {

            #region Setup das classes de validacao de certificado e da soapextesion que será usada
            //Configura a policy de certificados digitais utilizada pelo LawOffice
            System.Net.ServicePointManager.CertificatePolicy = new LawOfficeCertificatePolicy();

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                    delegate(object sender, X509Certificate certificate,
                                            X509Chain chain,
                                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return LawOfficeCertificatePolicy.ServerCertificateValidationCheck(certificate, chain, sslPolicyErrors);
                    };

            PCA.Framework.Web.Services.Extensions.PCAWSCompress.DefaultCompressionLevel =
                PCA.Framework.Web.Services.Extensions.PCAWSCompress.CompressionLevel.Low;

            //Verifica qual tipo de soapExtension deve ser usado
            var soapExtensionTypeToUse = typeof(PCA.Framework.Web.Services.Extensions.PCAWSCompress);
            var soapExtensionConfig = System.Configuration.ConfigurationManager.AppSettings["LawOffice.Web.SoapExtensionType"];
            if (soapExtensionConfig != null)
            {
                soapExtensionTypeToUse = System.Type.GetType(soapExtensionConfig);
            }

#if DEBUG
            var xx = soapExtensionTypeToUse.GetConstructor(System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(null);

            var f = soapExtensionTypeToUse.GetField("CompressionVersion");
            var p = soapExtensionTypeToUse.GetProperty("CompressionVersion");
#endif


            var instSoapToCheckVersion = ReflectionHelper.Instantiate<SoapExtension>(soapExtensionTypeToUse);
            if (instSoapToCheckVersion != null)
            {
                WSProxy.CompressionVersion = ReflectionHelper.GetInstancePropertyValue<SoapExtensionCompressionVersion>("CompressionVersion",
                    instSoapToCheckVersion);
            }

            //Se nao foi configurado nenhum modelo de compactacao, usa o antigo
            if (WSProxy.CompressionVersion == SoapExtensionCompressionVersion.None)
            {
                //Por padrao usa o metodo antigo
                WSProxy.CompressionVersion = SoapExtensionCompressionVersion.Net1_1;
            }

            //Insere a soapExtension para fazer o tratamento dos DateTime.MinValue com os diferentes Fusos
            PCA.Framework.Web.Services.Extensions.Helpers.InjectExtensionToConfig(
                soapExtensionTypeToUse,
                1,
                (int)PriorityGroup.High);

            #endregion
        }

        public static void Config(System.Web.Services.Protocols.SoapHttpClientProtocol wsProxy, out LawOfficeContextHeader header, string serviceUrl)
        {
            Config(wsProxy, out header, serviceUrl, WSProxy.Instance.BackOfficeHost);
        }
        

        public static void Config(System.Web.Services.Protocols.SoapHttpClientProtocol wsProxy, out LawOfficeContextHeader header, string serviceUrl, string backOfficeHost)
        {
            //Define a url do webservice
            wsProxy.Url = string.Format("{0}/{1}", backOfficeHost, serviceUrl);

            StackFrame frame = new StackFrame(2);

#if DEBUG
            MethodBase method = frame.GetMethod();
            var stackMethod = String.Format("{0}.{1}", method.DeclaringType.BaseType.FullName, method.Name);

            //if (method.DeclaringType.BaseType == typeof(LawOfficeUserInterface.Reports.ReportJobBase))
            //{
            //    wsProxy.Timeout = 1800000; //30 minutos de timeout para geração de relatórios
            //}
            //else
            //{
                 //30 minutos de timeout para geração de relatórios

#endif          
                //Verifica se está obtendo os dados de relatórios
                //(ou se está fazendo logon - isso porque na implementação do novo financeiro
                //no primeiro logon, famzemos a geração do online de todas as regras
                //if (Reports.ReportJobBase.isGettingReportData || Principal.IsLoginOn)
                //{
                    wsProxy.Timeout = 1800000;
                //}
                //else
                //{
                //    //wsProxy.Timeout = 60000; // 1 minuto para timeout geral
                //    wsProxy.Timeout = 300000; // 5 minutos para timeout geral
                //}
#if DEBUG
            //}
#endif
            //Seta o authentication header
            header = new LawOfficeContextHeader();
            header.Context = new LawOfficeContext();
            //Verifica se já está logado
            if (WSProxy.Instance.Context != null)
            {
                header.Context.AuthCode = WSProxy.Instance.Context.AuthCode;
                header.Domain = WSProxy.Instance.Context.Domain;
                header.User = WSProxy.Instance.Context.ContaDeAcesso;
                header.Pwd = WSProxy.Instance.Context.EmailFuncionario;
                header.SessionID = WSProxy.Instance.Context.AuthCode.ToString();
                header.CompressionVersion = WSProxy.CompressionVersion;
            }
            else
            {
                header.Context.AuthCode = Guid.Empty;
                header.CompressionVersion = WSProxy.CompressionVersion;
            }

            //Origem da chamada
            //  header.Context.RequestInfo = new LawOfficeContextRequestInfo();
            //  header.Context.RequestInfo.WSOrigemChamada = WSTiposDeOrigemDeChamada.None;

            //Sender info
            header.Context.SenderContext = new LawOfficeSenderContext();
            header.Context.SenderContext.SenderName = wsProxy.GetType().Name;
            header.Context.SenderContext.BusinessContext = null;

            //Cursor.Current = Cursors.WaitCursor;
        }
    }

    /*
    /// <summary>
    /// Helper class to simplify common reflection tasks.
    /// </summary>
    public sealed class ReflectionHelper
    {
        private ReflectionHelper() { }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// /// <param name="type">Type of the member.</param>
        public static T GetStaticFieldValue<T>(string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }
            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="typeName"></param>
        public static T GetStaticFieldValue<T>(string fieldName, string typeName)
        {
            Type type = Type.GetType(typeName, true);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }
            return default(T);
        }

        /// <summary>
        /// Sets the value of the private static member.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public static void SetStaticFieldValue<T>(string fieldName, Type type, T value)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", fieldName));

            field.SetValue(null, value);
        }

        /// <summary>
        /// Sets the value of the private static member.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="typeName"></param>
        /// <param name="value"></param>
        public static void SetStaticFieldValue<T>(string fieldName, string typeName, T value)
        {
            Type type = Type.GetType(typeName, true);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", fieldName));

            field.SetValue(null, value);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        public static T GetPrivateInstanceFieldValue<T>(string fieldName, object source)
        {
            FieldInfo field = source.GetType().GetField(fieldName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(source);
            }
            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        /// <param name="value">The value to set the member to.</param>
        public static void SetPrivateInstanceFieldValue(string memberName, object source, object value)
        {
            FieldInfo field = source.GetType().GetField(memberName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", memberName));

            field.SetValue(source, value);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        public static T GetInstanceFieldValue<T>(string fieldName, object source)
        {
            FieldInfo field = source.GetType().GetField(fieldName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(source);
            }
            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        /// <param name="value">The value to set the member to.</param>
        public static void SetInstanceFieldValue(string memberName, object source, object value)
        {
            FieldInfo field = source.GetType().GetField(memberName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", memberName));

            field.SetValue(source, value);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        public static T GetInstancePropertyValue<T>(string propertyName, object source)
        {
            var property = source.GetType().GetProperty(propertyName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                return (T)property.GetValue(source, null);
            }
            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        /// <param name="value">The value to set the member to.</param>
        public static void SetInstancePropertyValue(string propertyName, object source, object value)
        {
            var property = source.GetType().GetProperty(propertyName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", propertyName));

            property.SetValue(source, value, null);
        }

        public static T Instantiate<T>(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            return (T)constructor.Invoke(null);
        }

        public static object Instantiate(string typeName)
        {
            return Instantiate(typeName, Type.EmptyTypes, null);
        }

        public static object Instantiate(string typeName, Type[] constructorArgumentTypes, params object[] constructorParameterValues)
        {
            return Instantiate(Type.GetType(typeName, true), constructorArgumentTypes, constructorParameterValues);
        }

        public static object Instantiate(Type type, Type[] constructorArgumentTypes, params object[] constructorParameterValues)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgumentTypes, null);
            return constructor.Invoke(constructorParameterValues);
        }

        /// <summary>
        /// Invokes a non-public static method.
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static TReturn InvokeNonPublicMethod<TReturn>(Type type, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, new Converter<object, Type>(delegate(object o) { return o.GetType(); }));

            MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, null, paramTypes, null);
            if (method == null)
                throw new ArgumentException(string.Format("Could not find a method with the name '{0}'", methodName), "method");

            return (TReturn)method.Invoke(null, parameters);
        }

        public static TReturn InvokeNonPublicMethod<TReturn>(object source, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, new Converter<object, Type>(delegate(object o) { return o.GetType(); }));

            MethodInfo method = source.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, paramTypes, null);
            if (method == null)
                throw new ArgumentException(string.Format("Could not find a method with the name '{0}'", methodName), "method");

            return (TReturn)method.Invoke(source, parameters);
        }

        public static TReturn InvokeProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");

            return (TReturn)propertyInfo.GetValue(source, null);
        }

        public static TReturn InvokeNonPublicProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance, null, typeof(TReturn), new Type[0], null);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");

            return (TReturn)propertyInfo.GetValue(source, null);
        }

        public static object InvokeNonPublicProperty(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");

            return propertyInfo.GetValue(source, null);
        }
    }
    */
}
