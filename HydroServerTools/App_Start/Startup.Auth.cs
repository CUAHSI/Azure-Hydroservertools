using HydroServerTools.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Owin.Security.Providers.Hydroshare;
using System;

namespace HydroServerTools
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
       public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
           //add signalR
            //app.MapSignalR();


            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, User>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });          

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();

           var googleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "208043537148-ds7a83pm5ssa61kj2jpg7rpojqrqchfj.apps.googleusercontent.com",
                ClientSecret = "ah5IH-1uSPb0LAgusAGy5AZM",
                CallbackPath = new PathString("/signin-google")                
            };


           //app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);

            app.UseHydroshareAuthentication(
     "lDePFI1H2g4etu8xCaka5S0Gz2ttF6oQLm8CeVA7", "uxOWEVJiZTotyENmI9bVyZE30sSGIxoj1CYS6n7gdgFIW3FY6HSzYZGmUXM6oHhit4VnOMYMnsF2SUHQfri2WvtwLw5FSkZlSbzf6q2ef5OWG6U220ZdUAQZlhhzfk6O");


        }

    }
}