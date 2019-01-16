using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using Owin;
using Microsoft.Owin;
using WebAPI06Application.Providers;
using Microsoft.Owin.Security.OAuth;

namespace WebAPI06Application
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services  
            // Configure Web API to use only bearer token authentication.  
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Asset Allocate route
            /*
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            */
            config.Routes.MapHttpRoute(
                name: "WealthPlan",
                routeTemplate: "api/WealthPlan/{id}",
                defaults: new { controller = "WealthPlan", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
                );

            config.Routes.MapHttpRoute(
                name: "Signup",
                routeTemplate: "api/Signup/{id}",
                defaults: new { controller = "Signup", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
                );

            config.Routes.MapHttpRoute(
                name: "Signup_Verify",
                routeTemplate: "api/Signup_Verify/{id}",
                defaults: new { controller = "Signup_Verify", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
                );

            config.Routes.MapHttpRoute(
                name: "Login",
                routeTemplate: "api/Login/{id}",
                defaults: new { controller = "Login", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
                );

            /*
            // WebAPI when dealing with JSON & JavaScript!  
            // Setup json serialization to serialize classes to camel (std. Json format)  
            var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            formatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            // Adding JSON type web api formatting.  
            config.Formatters.Clear();
            config.Formatters.Add(formatter);
            */
        }
    }
}
