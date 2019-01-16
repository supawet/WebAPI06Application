using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebAPI06Application.Services;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace WebAPI06Application.Providers
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        #region[GrantResourceOwnerCredentials]
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            return Task.Factory.StartNew(() =>
            {
                var mobile_no = context.UserName;
                var pin = context.Password;
                var userService = new UserService(); // our created one
                var user = userService.ValidateUser(mobile_no, pin);
                if (user != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Sid, user.PIN),
                        //new Claim(ClaimTypes.Name, user.UnitHolder),
                        //new Claim(ClaimTypes.Email, user.Password)
                    };
                    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims,
                                Startup.OAuthOptions.AuthenticationType);

                    var properties = CreateProperties(Convert.ToString(user.PIN));
                    var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);
                    /*
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

                    context.Validated(identity);
                    */
                }
                else
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect");
                }
            });
        }
        #endregion

        #region[ValidateClientAuthentication]
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }
        #endregion

        #region[TokenEndpoint]
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
        #endregion

        #region[CreateProperties]
        public static AuthenticationProperties CreateProperties(string id_hash)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                // เพิ่ม properties ที่ return ตอนส่งค่า Token
                { "id", id_hash }
            };
            return new AuthenticationProperties(data);
        }
        #endregion
    }

}