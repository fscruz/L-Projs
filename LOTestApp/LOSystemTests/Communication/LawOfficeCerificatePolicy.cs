using System;
using PCA.Framework;
using System.Security.Cryptography.X509Certificates;

namespace LOSystemTests.Communication
{
        public class LawOfficeCertificatePolicy : System.Net.ICertificatePolicy
        {
            public bool CheckValidationResult(System.Net.ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Net.WebRequest request, int certificateProblem)
            {
                try
                {
                    if (certificateProblem == 0)
                    {
                        return true;
                    }
                    else if (certificateProblem == -2146762481)
                    { 
                        //Constante CertCN_NO_MATCH
                        //Verifica se é o certificado da PCA
                        string name = certificate.Subject;
                        //Localiza o CommonName
                        int index = name.IndexOf("CN=");
                        //Obtem o CommonName
                        string commonName = name.Substring(index + 3);

                        //Valida se é o certificado de exceção
                        if (commonName.ToLower().StartsWith("www.selector.com.br"))
                        {
                            return true;
                        }
                    }
                    else if (certificateProblem == -2146762487)
                    {
                        //certificado de testes
                        return true;
                        //@@throw new BusinessException("teste de erros");
                    }
                }
                catch { }

                return false;
            }


            public static bool ServerCertificateValidationCheck(X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                //return true; // **** Always accept

                // If the certificate is a valid, signed certificate, return true.
                if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                {
                    return true;
                }

                //certificate => Subject	"CN=LAWOFFICE.LAWSOFT.COM.BR, OU=Desenvolvimento, O=LAWSOFT S.A., L=SAO PAULO, S=SP, C=BR"	string

                // If there are errors in the certificate chain, look at each error to determine the cause.
                if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                {
                    if (chain != null && chain.ChainStatus != null)
                    {
                        foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                        {
                            if ((certificate.Subject == certificate.Issuer) &&
                               (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                            {
                                // Self-signed certificates with an untrusted root are valid. 
                                continue;
                            }
                            else
                            {
                                if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                                {
                                    // If there are any other errors in the certificate chain, the certificate is invalid,
                                    // so the method returns false.
                                    return false;
                                }
                            }
                        }
                    }

                    // When processing reaches this line, the only errors in the certificate chain are 
                    // untrusted root errors for self-signed certificates. These certificates are valid
                    // for default Exchange server installations, so return true.
                    return true;
                }
                else
                {
                    // In all other cases, return false.
                    return false;
                }
            }
        }
}
