//////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography.X509Certificates;

//////////////////////////////////////////////////////////////////////

namespace IdServer.Helpers
{
    public class LoadCert
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        public X509Certificate2 LoadCertificate(IHostingEnvironment _env)
        {
            X509Certificate2 cert = null;
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    // Replace below with your cert's thumbprint
                    "‎90b03404c8e3ac400efabaa40ee17c1965cfaa69",
                    false);
                // Get the first cert with the thumbprint
                if (certCollection.Count > 0)
                {
                    cert = certCollection[0];
                    //log.LogInformation($"Successfully loaded cert from registry: {cert.Thumbprint}");
                }
            }

            // Fallback to local file for development
            if (cert == null)
            {
                cert = new X509Certificate2(Path.Combine(_env.ContentRootPath,"rentd.pfx"),"password");
                //log.LogError("Could not load cert (Thumbprint: {thumbprint}) from registry");
            }
            return cert;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
