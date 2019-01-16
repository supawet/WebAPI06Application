using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace WebAPI06Application
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            /*
            // I put my GetToken method in a Utils class. Change for wherever you placed your method.
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Utils.GetToken));
            //var sec = await kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]).ConfigureAwait(false);
            var sec = await kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]);

            //I put a variable in a Utils class to hold the secret for general application use.
            Utils.EncryptSecret = sec.Value
            */
        }
    }
}
