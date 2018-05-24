using System;

namespace Owin.Security.Providers.Hydroshare
{
    public static class HydroshareAuthenticationExtensions
    {
        public static IAppBuilder UseHydroshareAuthentication(this IAppBuilder app,
            HydroshareAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            app.Use(typeof(HydroshareAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseHydroshareAuthentication(this IAppBuilder app, string clientId, string clientSecret)
        {
            return app.UseHydroshareAuthentication(new HydroshareAuthenticationOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            });
        }
    }
}