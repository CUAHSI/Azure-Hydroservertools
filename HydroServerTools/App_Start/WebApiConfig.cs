using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace HydroServerTools
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //Define a custom route taking a parameter that is not named 'Id' 
            // (thus different from the DefaultApi)
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiGet",
                routeTemplate: "api/revisedupload/get/{uploadId}",
                defaults: new { controller = "RevisedUpload", action = "Get" });

            //Get Summary Report Route
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiGetDbLoadResults",
                routeTemplate: "api/revisedupload/get/dbloadresults/{uploadId}",
                defaults: new { controller = "RevisedUpload", action = "GetDbLoadResults" });

            //Define a custom route taking a parameter that is not named 'Id' 
            // (thus different from the DefaultApi)
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiPut",
                routeTemplate: "api/revisedupload/put/{uploadId}",
                defaults: new { controller = "RevisedUpload", action = "Put" });

            //Not sure if this route is strictly necessary...
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiPost",
                routeTemplate: "api/revisedupload/post",
                defaults: new { controller = "RevisedUpload", action = "Post" });

            //Delete route taking two parameters...
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiDeleteFile",
                routeTemplate: "api/revisedupload/delete/file/{uploadId}/{fileName}",
                defaults: new { controller = "RevisedUpload", action = "DeleteFile" });

            RouteTable.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
               ).RouteHandler = new SessionRouteHandler();
        }
        //added to support Session
        public class SessionControllerHandler : HttpControllerHandler, IRequiresSessionState
        {
            public SessionControllerHandler(RouteData routeData)
                : base(routeData)
            { }
        }
        public class SessionRouteHandler : IRouteHandler
        {
            IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
            {
                return new SessionControllerHandler(requestContext.RouteData);
            }
        }
    }
}
