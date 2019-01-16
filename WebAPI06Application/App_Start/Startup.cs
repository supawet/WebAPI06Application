using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Owin;
using Microsoft.Owin;
using WebAPI06Application.Providers;
using Microsoft.Owin.Security.OAuth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace WebAPI06Application
{
    public partial class Startup
    {
        /// <summary>  
        /// OAUTH options property.  
        /// </summary>  
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        /// <summary>  
        /// Public client ID property.  
        /// </summary>  
        public static string PublicClientId { get; private set; }

        static Startup()
        {
            // Configure the application for OAuth based flow  
            PublicClientId = "self";

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/token"),
                Provider = new OAuthProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                // Note: Remove the following line before you deploy to production:
                AllowInsecureHttp = true
            };
        }
        public void ConfigureAuth(IAppBuilder app)
        {
            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthBearerTokens(OAuthOptions);
            //app.UseOAuthAuthorizationServer(OAuthOptions);
        }
    }
}