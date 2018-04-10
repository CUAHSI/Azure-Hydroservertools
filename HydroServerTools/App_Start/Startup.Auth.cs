using HydroServerTools.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using Owin;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;


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


            app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);

            var oauthAuthorizationServerOptions = new OAuthAuthorizationServerOptions
            {
                AuthorizeEndpointPath = new PathString("//www.hydroshare.org/o/authorize"),
                TokenEndpointPath = new PathString("//www.hydroshare.org/o/access_token"),
                ApplicationCanDisplayErrors = true,
#if DEBUG
                AllowInsecureHttp = true,
#endif
                // Authorization server provider which controls the lifecycle of Authorization Server
                Provider = new OAuthAuthorizationServerProvider
                {
                    OnValidateClientRedirectUri = ValidateClientRedirectUri,
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,
                    OnGrantClientCredentials = GrantClientCredetails
                },

                // Authorization code provider which creates and receives the authorization code.
                AuthorizationCodeProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateAuthenticationCode,
                    OnReceive = ReceiveAuthenticationCode,
                },

                // Refresh token provider which creates and receives refresh token.
                RefreshTokenProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateRefreshToken,
                    OnReceive = ReceiveRefreshToken,
                }
            };

            app.UseOAuthAuthorizationServer(oauthAuthorizationServerOptions);

            var oauthBearerAuthenticationOptions = new OAuthBearerAuthenticationOptions
            {
                AccessTokenProvider = new DropboxAccessTokenProvider()
            };
            app.UseOAuthBearerAuthentication(oauthBearerAuthenticationOptions);

        }

        public sealed class DropboxAccessTokenProvider : AuthenticationTokenProvider
        {
            public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.dropbox.com/1/account/info");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.Token);

                    var response = await client.SendAsync(request);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return;
                    }

                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                    var identity = new ClaimsIdentity("Dropbox");
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, payload.Value<string>("uid")));

                    context.SetTicket(new AuthenticationTicket(identity, new AuthenticationProperties()));
                }
            }
        }

        private Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(
               context.UserName, OAuthDefaults.AuthenticationType),
               context.Scope.Select(x => new Claim("urn:oauth:scope", x))
               );

            context.Validated(identity);

            return Task.FromResult(0);
        }

        private Task GrantClientCredetails(OAuthGrantClientCredentialsContext context)
        {
            return Task.FromResult(0);
        }


        private Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            //if (context.ClientId == Clients.Client1.Id)
            //{
            //    context.Validated(Clients.Client1.RedirectUrl);
            //}
            //else if (context.ClientId == Clients.Client2.Id)
            //{
            //    context.Validated(Clients.Client2.RedirectUrl);
            //}
            return Task.FromResult(0);
        }

        private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
                context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                //if (clientId == Clients.Client1.Id && clientSecret == Clients.Client1.Secret)
                //{
                //    context.Validated();
                //}
                //else if (clientId == Clients.Client2.Id && clientSecret == Clients.Client2.Secret)
                //{
                //    context.Validated();
                //}
            }
            return Task.FromResult(0);
        }


        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        private void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            _authenticationCodes[context.Token] = context.SerializeTicket();
        }

        private void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }

        private void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            context.SetToken(context.SerializeTicket());
        }

        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }

    }
}