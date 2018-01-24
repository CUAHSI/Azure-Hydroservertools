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

            //Get Rejected Items Route...
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiGetRejectedItems",
                routeTemplate: "api/revisedupload/get/rejecteditems/{uploadId}/{tableName}",
                defaults: new { controller = "RevisedUpload", action = "GetRejectedItems" });

            //Put (updated) Rejected Items Route...
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiPutRejectedItems",
                routeTemplate: "api/revisedupload/put/rejecteditems",
                defaults: new { controller = "RevisedUpload", action = "PutRejectedItems" });

            //Define a custom route taking a parameter that is not named 'Id' 
            // (thus different from the DefaultApi)
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiPut",
                routeTemplate: "api/revisedupload/put/{uploadId}",
                defaults: new { controller = "RevisedUpload", action = "Put" });

            //Define a custom route requesting db table counts 
            // Action: Post to allow data placement in the request body
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiRequestDbCounts",
                routeTemplate: "api/revisedupload/post/requestdbtablecounts",
                defaults: new { controller = "RevisedUpload", action = "RequestDbTableCounts" });

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

            //Delete route taking one parameter...
            config.Routes.MapHttpRoute(
                name: "RevisedUploadApiDeleteUploadId",
                routeTemplate: "api/revisedupload/delete/uploadId/{uploadId}",
                defaults: new { controller = "RevisedUpload", action = "DeleteUploadId" });

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
