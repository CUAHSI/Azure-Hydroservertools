using jQuery.DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Collections.Concurrent;

using HydroServerTools.Utilities;
using HydroServerToolsUtilities.Validators;

using HydroServerToolsUtilities;
using System.Threading.Tasks;
using System.Threading;

namespace HydroServerTools
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //Cancellation Token instance...
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public static Guid InstanceGuid { get; set;}
        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Lets MVC know that anytime there is a JQueryDataTablesModel as a parameter in an action to use the
            // JQueryDataTablesModelBinder when binding the model.
            ModelBinders.Binders.Add(typeof(JQueryDataTablesModel), new JQueryDataTablesModelBinder());
            //InstanceGuid = Guid.NewGuid();

            //13-Aug-2018 - BC - Add CacheCollections Initialize() call here...
            CacheCollections.Initialize();

            //TO DO - re-refactor this code and RevisedUploadController to use CacheCollections... 

            //At web application startup - add ConcurrentDictionary instances to the runtime cache for the following context types:
            //  - file, 
            //  - validation
            //  - repository
            //  - status
            //  - db load
            //
            //NOTE: One HttpRuntime.Cache instance exists for the Application Domain...
            //Sources: https://docs.microsoft.com/en-us/aspnet/web-forms/overview/data-access/caching-data/caching-data-at-application-startup-cs
            //         https://stackoverflow.com/questions/27575213/how-to-cache-in-asp-net
            CacheCollections.Initialize();

            //TO DO - re-refactor RevisedUploadController to use CacheCollections... 

            //Start the 'keep alives' checks interval task...
            //DO NOT await the task!!
            TimeSpan tsKeepAliveInterval = new TimeSpan(1, 0, 0);   //One hour 'keep-alive' interval...   
            //TimeSpan tsKeepAliveInterval = new TimeSpan(0, 2, 0);   //TEST two minute 'keep-alive' interval...   
            TimeSpan tsDelayInterval = new TimeSpan(0, 0, 10);      //ten second interval between checks...

            var uploadIdKeepAlives = CacheCollections.GetCollection<DateTime>("KeepAliveDateTimes");

            CheckKeepAlives(uploadIdKeepAlives, tsKeepAliveInterval, tsDelayInterval, _cancellationTokenSource.Token);
        }

        //Disable forms authentication redirect...
        //Source: https://stackoverflow.com/questions/18519446/prevent-asp-net-from-redirecting-to-login-aspx
        protected void Application_EndRequest(Object sender, EventArgs eventArgs)
        {
            HttpApplication context = (HttpApplication)sender;
            context.Response.SuppressFormsAuthenticationRedirect = true;

            //Signal 'keep alive' interval task to end...
            _cancellationTokenSource.Cancel();
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started  
            


        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends.   
            // Note: The Session_End event is raised only when the sessionstate mode  
            // is set to InProc in the Web.config file. If session mode is set to StateServer   
            // or SQLServer, the event is not raised. 
            

        }

        //Define an interval task for uploadId 'keep alive' checking...
        //Source: https://stackoverflow.com/questions/30462079/run-async-method-regularly-with-specified-interval
        private async Task CheckKeepAlives(ConcurrentDictionary<string, DateTime> uploadIdKeepAlives, TimeSpan tsKeepAliveInterval, TimeSpan tsDelayInterval, CancellationToken cancellationToken)
        {
            //Set paths...
            string pathUploads = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");
            string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");
            string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");

            //Loop until told to stop...
            while (true)
            {
                //Perform keep alive checks here...
                DateTime dtValue = DateTime.Now;
                DateTime dtNow = DateTime.Now;
                List<string> expiredUploadIds = new List<string>();
                var uploadIds = uploadIdKeepAlives.Keys;

                //For each uploadId...
                foreach (var uploadId in uploadIds)
                {
                    //Check 'keep-alive' date/time against current date/time
                    //ASSUMPTION: NO 'keep-alive' date/times are in the future...
                    if (uploadIdKeepAlives.TryGetValue(uploadId, out dtValue))
                    {
                        if (dtNow.Subtract(dtValue) >= tsKeepAliveInterval)
                        {
                            //'Keep-alive' expired - retain uploadId...
                            expiredUploadIds.Add(uploadId);
                        }
                    }
                }

                if (0 < expiredUploadIds.Count)
                {
                    //Retrieve collections from cache...
                    var cache = HttpRuntime.Cache;
                    var key = "uploadIdsToFileContexts";
                    ConcurrentDictionary<string, FileContext> fileContexts = cache.Get(key) as ConcurrentDictionary<string, FileContext>;

                    key = "uploadIdsToValidationContexts";
                    ConcurrentDictionary<string, ValidationContext<CsvValidator>> validationContexts = cache.Get(key) as ConcurrentDictionary<string, ValidationContext<CsvValidator>>;

                    key = "uploadIdsToDbLoadContexts";
                    ConcurrentDictionary<string, DbLoadContext> dbloadContexts = cache.Get(key) as ConcurrentDictionary<string, DbLoadContext>;


                    //For each expired uploadId...
                    foreach (var uploadId in expiredUploadIds)
                    {
                        if (uploadIdKeepAlives.TryRemove(uploadId, out dtValue))
                        {
                            //Call helper method to remove the collections' files etc... 
                            UploadIdHelper uploadIdHelper = new UploadIdHelper(uploadId);

                            await uploadIdHelper.DeleteFromCollections(fileContexts, pathUploads, validationContexts, pathValidated, dbloadContexts, pathProcessed);
                        }
                    }

                }

                //Wait for the input interval...
                await Task.Delay(tsDelayInterval, cancellationToken);
            }
        }

    }
}
