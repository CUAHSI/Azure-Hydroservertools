using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;

using HydroServerToolsUtilities.Validators;

namespace HydroServerToolsUtilities
{
    //A simple class for the management of Http.Cache collections...
    //NOTE: One Httpruntime.Cache instance exists for the Application Domain...
    //Sources: https://docs.microsoft.com/en-us/aspnet/web-forms/overview/data-access/caching-data/caching-data-at-application-startup-cs
    //         https://stackoverflow.com/questions/27575213/how-to-cache-in-asp-net
    public static class CacheCollections
    {
        //Private members...
        private const string _keyDbLoadContexts = "DbLoadContexts";
        private const string _keyFileContext = "FileContexts";
        private const string _keyKeepAliveDateTimes = "KeepAliveDateTimes";
        private const string _keyRepositoryContexts = "RepositoryContexts";
        private const string _keyStatusContexts = "StatusContexts";
        private const string _keyValidationContexts = "ValidationContexts";

        private const string _keyUploadIdsToDbLoadContexts = "uploadIdsToDbLoadContexts";
        private const string _keyUploadIdsToFileContexts = "uploadIdsToFileContexts";
        private const string _keyUploadIdsToKeepAliveDateTimes = "uploadIdsToKeepAliveDateTimes";
        private const string _keyUploadIdsToRepositoryContexts = "uploadIdsToRepositoryContexts";
        private const string _keyUploadIdsToStatusContexts = "uploadIdsToStatusContexts";
        private const string _keyUploadIdsToValidationContexts = "uploadIdsToValidationContexts";

        private static ConcurrentDictionary<string, string> _friendlyKeysToInternalKeys =
            new ConcurrentDictionary<string, string>(new Dictionary<string, string>() { //{_keyDbLoadContexts, _keyUploadIdsToDbLoadContexts },
                                                                                        //{_keyFileContext, _keyUploadIdsToFileContexts },
                                                                                        {_keyKeepAliveDateTimes, _keyUploadIdsToKeepAliveDateTimes } //,
                                                                                        //{_keyRepositoryContexts, _keyUploadIdsToRepositoryContexts },
                                                                                        //{_keyStatusContexts, _keyUploadIdsToStatusContexts },
                                                                                        //{_keyValidationContexts, _keyUploadIdsToValidationContexts} 
                                                                                      });

        private static ConcurrentDictionary<string, Type> _keysToContextTypes =
            new ConcurrentDictionary<string, Type>(new Dictionary<string, Type>() { {_keyUploadIdsToDbLoadContexts, typeof (DbLoadContext) },
                                                                                    {_keyUploadIdsToFileContexts, typeof (FileContext) },
                                                                                    {_keyUploadIdsToKeepAliveDateTimes, typeof (DateTime) },
                                                                                    {_keyUploadIdsToRepositoryContexts, typeof (RepositoryContext) },
                                                                                    {_keyUploadIdsToStatusContexts, typeof (StatusContext) },
                                                                                    {_keyUploadIdsToValidationContexts, typeof (ValidationContext<CsvValidator>)} });

        //Public methods...

        //Initialize cache collections...
        //NOTE: Call once during application start...
        public static void Initialize()
        {
            //Retrieve Http cache reference...
            var cache = HttpRuntime.Cache;

            //Check initialization flag...
            var initKey = "dbcontextInit";
            object obj = cache.Get(initKey);
            bool bInit = (null != obj) ? ((bool)obj) : false;

            if (!bInit)
            {
                //Not yet initialized... 

                //For each collection pair...
                foreach (var kvp in _keysToContextTypes)
                {
                    var key = kvp.Key;
                    var type = kvp.Value;

                    //Create a ConcurrentDictionary instance to pair uploadIds with context instances...
                    //Source: https://www.c-sharpcorner.com/UploadFile/d3e4b1/dynamically-creating-generic-listt-and-generic-dictionary/
                    Type cdType = typeof(ConcurrentDictionary<,>);
                    Type[] typeParams = { typeof(string), type };
                    Type genericType = cdType.MakeGenericType(typeParams);

                    var instance = Activator.CreateInstance(genericType);
                    cache.Insert(key, instance);
                }

                //Allocate log contexts...
                var bulkUploadLogContext = new DbLogContext("BulkUploadLog", "AdoNetAppenderBulkUploadLog", "local-DBLog", "deploy-DBLog");
                var bulkUploadErrorContext = new DbErrorContext("BulkUploadError", "AdoNetAppenderBulkUploadError", "local-DBLog", "deploy-DBLog");
                var hydroServerLogContext = new DbLogContext("HydroServerLog", "AdoNetAppenderHydroServerLog", "local-DBLog", "deploy-DBLog");
                var hydroServerErrorContext = new DbErrorContext("HydroServerError", "AdoNetAppenderHydroServerError", "local-DBLog", "deploy-DBLog");

                cache.Insert("BulkUploadLog", bulkUploadLogContext);
                cache.Insert("BulkUploadError", bulkUploadErrorContext);

                cache.Insert("HydroServerLog", hydroServerLogContext);
                cache.Insert("HydroServerError", hydroServerErrorContext);

                //Set initialization flag...
                cache.Insert(initKey, true);
            }
        }

        //Add a context...
        public static bool AddContext<typeContext>(string uploadId, typeContext context)
        {
            bool bResult = false;   //assume failure...

            //Validate/initialize input parameters...
            if ((!String.IsNullOrWhiteSpace(uploadId)) && (null != context))
            {
                //Input parameters valid - find context collection...

                //For each collection pair...
                foreach (var kvp in _keysToContextTypes)
                {
                    var key = kvp.Key;
                    var type = kvp.Value;

                    if (type.Equals(typeof(typeContext)))
                    {
                        //Retrieve Http cache reference...
                        var cache = HttpRuntime.Cache;
                        var collection = cache.Get(key) as ConcurrentDictionary<string, typeContext>;
                        if (null != collection)
                        {
                            bResult = collection.TryAdd(uploadId, context);
                        }
                    }
                }
            }

            //Processing complete - return
            return bResult;
        }

        //Retrieve a context...
        public static typeContext GetContext<typeContext>(string uploadId)
        {
            typeContext result = default(typeContext);

            //Validate/initialize input parameters...
            if (!String.IsNullOrWhiteSpace(uploadId))
            {
                //Input parameter(s) valid - find context collection...

                //For each collection pair...
                foreach (var kvp in _keysToContextTypes)
                {
                    var key = kvp.Key;
                    var type = kvp.Value;

                    if (type.Equals(typeof (typeContext)))
                    {
                        //Retrieve Http cache reference...
                        var cache = HttpRuntime.Cache;

                        var collection = cache.Get(key) as ConcurrentDictionary<string, typeContext>;
                        if (null != collection)
                        {
                            collection.TryGetValue(uploadId, out result);
                            break;
                        }
                    }
                }
            }

            //Processing complete - return
            return result;
        }

        //Get Bulk Upload Log Context
        public static DbLogContext GetBulkUploadLogContext()
        {
            //Retrieve Http cache reference...
            var cache = HttpRuntime.Cache;

            var result = cache.Get("BulkUploadLog") as DbLogContext;

            return result;
        }

        //Get Bulk Upload Error Context 
        public static DbErrorContext GetBulkUploadErrorContext()
        {
            //Retrieve Http cache reference...
            var cache = HttpRuntime.Cache;

            var result = cache.Get("BulkUploadError") as DbErrorContext;

            return result;
        }

        //Get HydroServer Log Context
        public static DbLogContext GetHydroServerLogContext()
        {
            //Retrieve Http cache reference...
            var cache = HttpRuntime.Cache;

            var result = cache.Get("HydroServerLog") as DbLogContext;

            return result;
        }

        //Get HydroServer Error Context 
        public static DbErrorContext GetHydroServerErrorContext()
        {
            //Retrieve Http cache reference...
            var cache = HttpRuntime.Cache;

            var result = cache.Get("HydroServerError") as DbErrorContext;

            return result;
        }

        //Retrieve a collection...
        public static ConcurrentDictionary<string, typeValue> GetCollection<typeValue>(string friendlyKey)
        {
            ConcurrentDictionary<string, typeValue> result = null;

            //Validate/initialize input parameters...
            if (!String.IsNullOrWhiteSpace(friendlyKey))
            {
                string internalKey = String.Empty;
                if (_friendlyKeysToInternalKeys.TryGetValue(friendlyKey, out internalKey))
                {
                    var cache = HttpRuntime.Cache;

                    result = cache.Get(internalKey) as ConcurrentDictionary<string, typeValue>;
                }
            }

            //Processing complete - return result...
            return result;
        }

    }
}
