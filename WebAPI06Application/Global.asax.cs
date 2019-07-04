using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

using System.Data.SqlClient;
using System.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider;
using System.Threading.Tasks;

namespace WebAPI06Application
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static ClientCredential _clientCredential;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            InitializeAzureKeyVaultProvider();
            /*
            // I put my GetToken method in a Utils class. Change for wherever you placed your method.
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Utils.GetToken));
            //var sec = await kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]).ConfigureAwait(false);
            var sec = await kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]);

            //I put a variable in a Utils class to hold the secret for general application use.
            Utils.EncryptSecret = sec.Value
            */
        }

        static void InitializeAzureKeyVaultProvider()
        {
            string clientId = ConfigurationManager.AppSettings["AuthClientId"];
            string clientSecret = ConfigurationManager.AppSettings["AuthClientSecret"];
            _clientCredential = new ClientCredential(clientId, clientSecret);

            SqlColumnEncryptionAzureKeyVaultProvider azureKeyVaultProvider =
              new SqlColumnEncryptionAzureKeyVaultProvider(GetToken);

            Dictionary<string, SqlColumnEncryptionKeyStoreProvider> providers =
              new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>();

            providers.Add(SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider);
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
        }

        public async static Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, _clientCredential);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the access token");

            return result.AccessToken;
        }
    }
}
