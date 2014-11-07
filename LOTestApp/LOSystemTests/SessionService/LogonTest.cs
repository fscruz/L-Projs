using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCA.LawOffice.Messages;

namespace LOSystemTests.SessionService
{
    [TestClass]
    public class LogonTest : ITestPrototype
    {
        public string DisplayName
        {
            get { return "Logon Test"; }
        }

        public string Test()
        {
            throw new NotImplementedException();
        }

        private LogonTest()
        {

        }
        

        public static void DoLogon(string user, string password, string domain, string versaoASerUsada, ref LawOfficeContextData context)
        {
            //Obtem e configura o endereço do backoffice
            //Verifica se está no modo asp

            //Consulta na retaguarda o endereço do BackOffice para o escritório informado
            //using (_RetaguardaServicoASPService.ServicoAsp wsAsp = new _RetaguardaServicoASPService.ServicoAsp())
            //{
            //    wsAsp.Url = Constants.C_URLRetaguardaServices + "/ServicoAsp.asmx";
            //    _backOfficeUrl = wsAsp.RetornaUrl(txtEscritorio.Text);
            //}

            //if (backOfficeUrl == null || backOfficeUrl.Trim().Length == 0) {

            //    throw new ArgumentException("backOfficeUrl cannot be null");
            //}

            try
            {
                using (var _wsSession = new SessionService.SessionService())
                {
                    //Never force logoff on tests
                    bool forceLogOff = false;
                    context = _wsSession.Logon(user.Trim(), MD5.ComputeHash(password), domain.Trim(), versaoASerUsada, System.Environment.MachineName, forceLogOff);
                }

                //this.DoLogon_Verify();
            }
            catch
            {
                //handle exceptions
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
