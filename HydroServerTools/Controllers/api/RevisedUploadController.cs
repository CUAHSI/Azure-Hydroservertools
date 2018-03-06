using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using HydroServerTools.Utilities;
using HydroServerTools.Validators;

using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.ModelMaps;
using HydroServerToolsRepository.Repository;
using HydroServerToolsUtilities;
using System.Xml.Serialization;
using HydroServerTools.Models;

namespace HydroServerTools.Controllers.api
{
    public class RevisedUploadController : ApiController
    {
        public class TableNames
        {

            //Properties...
            public List<string> tableNames { get; set; }
        }

        //NOTE: Static members are thread-specific in web api!!
        //      One HttpRuntime.Cache instance exists for the Application Domain...

        //Utilities...
        private ConcurrentDictionary<string, FileContext> getFileContexts()
        {
            var key = "uploadIdsToFileContexts";
            var cache = HttpRuntime.Cache;

            //Concurrent dictionary maps current uploadIds to current FileContext instances... 
            ConcurrentDictionary<string, FileContext> filecontexts = cache.Get(key) as ConcurrentDictionary<string, FileContext>;
#if (DEBUG)
            if (null == filecontexts)
            {
                //Cache creation expected at Application_Start() - thow an exception!!
                throw new Exception("HttpRuntime.Cache object: " + key + " NOT found!!!");
            }
#endif
            return filecontexts;
        }

        private ConcurrentDictionary<string, ValidationContext<CsvValidator>> getValidationContexts()
        {
            var key = "uploadIdsToValidationContexts";
            var cache = HttpRuntime.Cache;

            //Concurrent dictionary maps current uploadIds to current ValidationContext instances... 
            ConcurrentDictionary<string, ValidationContext<CsvValidator>> validationcontexts = cache.Get(key) as ConcurrentDictionary<string, ValidationContext<CsvValidator>>;
#if (DEBUG)
            if (null == validationcontexts)
            {
                //Cache creation expected at Application_Start() - thow an exception!!
                throw new Exception("HttpRuntime.Cache object: " + key + " NOT found!!!");
            }
#endif
            return validationcontexts;
        }

        private ConcurrentDictionary<string, RepositoryContext> getRepositoryContexts()
        {
            var key = "uploadIdsToRepositoryContexts";
            var cache = HttpRuntime.Cache;

            //Concurrent dictionary maps current uploadIds to current RepositoryContext instances... 
            ConcurrentDictionary<string, RepositoryContext> repositorycontexts = cache.Get(key) as ConcurrentDictionary<string, RepositoryContext>;
#if (DEBUG)
            if (null == repositorycontexts)
            {
                //Cache creation expected at Application_Start() - thow an exception!!
                throw new Exception("HttpRuntime.Cache object: " + key + " NOT found!!!");
            }
#endif
            return repositorycontexts;
        }

        private ConcurrentDictionary<string, StatusContext> getStatusContexts()
        {
            var key = "uploadIdsToStatusContexts";
            var cache = HttpRuntime.Cache;

            //Concurrent dictionary maps current uploadIds to current StatusContext instances... 
            ConcurrentDictionary<string, StatusContext> statuscontexts = cache.Get(key) as ConcurrentDictionary<string, StatusContext>;
#if (DEBUG)
            if (null == statuscontexts)
            {
                //Cache creation expected at Application_Start() - thow an exception!!
                throw new Exception("HttpRuntime.Cache object: " + key + " NOT found!!!");
            }
#endif
            return statuscontexts;
        }

        private ConcurrentDictionary<string, DbLoadContext> getDbLoadContexts()
        {
            var key = "uploadIdsToDbLoadContexts";
            var cache = HttpRuntime.Cache;

            //Concurrent dictionary maps current uploadIds to current StatusContext instances... 
            ConcurrentDictionary<string, DbLoadContext> dbloadcontexts = cache.Get(key) as ConcurrentDictionary<string, DbLoadContext>;
#if (DEBUG)
            if (null == dbloadcontexts)
            {
                //Cache creation expected at Application_Start() - thow an exception!!
                throw new Exception("HttpRuntime.Cache object: " + key + " NOT found!!!");
            }
#endif
            return dbloadcontexts;
        }

        //Members...
#if (DEBUG)
        private static Object fileLockObject = new Object();

        private struct DebugData
        {
            public string currentUploadId { get; set; }
            public string validationQualifier { get; set; }
            //public string fileNames { get; set; }
            public string fileName { get; set; }
            public bool isFirstChunk { get; set; }
            public bool isLastChunk { get; set; }
            public long? from { get; set; }
            public long? to { get; set; }
            public long? length { get; set; }
            public string filePathAndName { get; set; }

            public override String ToString()
            {
                return String.Format("uploadId: {0}, qualifier: {1}, file: {2}, first: {3}, last: {4}, from: {5}, to: {6}, length: {7}, path: {8}",
                                        currentUploadId, validationQualifier, fileName, isFirstChunk, isLastChunk, from, to, length, filePathAndName);
            }

        }
#endif

        //Get method...
        //GET api/revisedupload/get/{uploadId}
        //Web API feature: Have to name the input variable 'uploadId' to satisfy the router!!!
        //See WebApiConfig.cs for custom route...
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string uploadId)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(uploadId))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            var fileContexts = getFileContexts();
            var validationContexts = getValidationContexts();
            if ((!fileContexts.ContainsKey(uploadId)) || (!validationContexts.ContainsKey(uploadId)))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Unknown uploadId - return early
                response.ReasonPhrase = "Unknown upload id...";
                return response;
            }

            //Input parameter(s) valid - retrieve file and validation context...
            FileContext fileContext = fileContexts[uploadId];
            using (await fileContext.FileSemaphore.UseWaitAsync())
            {
                ValidationContext<CsvValidator> validationContext = validationContexts[uploadId];
                using ( await validationContext.ValidationResultSemaphore.UseWaitAsync())
                {
                    //Note the difference in type between the two collections...
                    var valiDATORResults = validationContext.ValidationResults;                     //CsvValidator
                    var valiDATIONResults = new List<ValidationResult<CsvValidationResults>>();     //CsvValidationResults

                    //Copy CsvValidator instances to CsvValidationResults instances...
                    HttpStatusCode httpStatusCode = HttpStatusCode.OK;  //Assume success...
                    foreach (var valiDATORResult in valiDATORResults)
                    {
                        var csvValiDATIONResults = new CsvValidationResults(valiDATORResult.FileValidator.ValidationResults);

                        valiDATIONResults.Add(new ValidationResult<CsvValidationResults>(valiDATORResult.FileName, csvValiDATIONResults));

                        ////if (valiDATORResult.ValidationComplete)
                        ////{
                        //    //Validation complete - add to results...
                        //    var csvValiDATIONResults = new CsvValidationResults(valiDATORResult.FileValidator);

                        //    valiDATIONResults.Add(new ValidationResult<CsvValidationResults>(valiDATORResult.FileName, csvValiDATIONResults));
                        ////}
                        ////else
                        ////{
                        ////    //Validation not yet complete - re-set status code...
                        ////    httpStatusCode = HttpStatusCode.PartialContent;     //206
                        ////    //break;
                        ////}
                    }

                    //Convert list to JSON, if indicated...
                    //string jsonData = (HttpStatusCode.OK == httpStatusCode) ? Newtonsoft.Json.JsonConvert.SerializeObject(valiDATIONResults) : Newtonsoft.Json.JsonConvert.SerializeObject("{}");
                    string jsonData = (0 < valiDATIONResults.Count) ? Newtonsoft.Json.JsonConvert.SerializeObject(valiDATIONResults) : Newtonsoft.Json.JsonConvert.SerializeObject("{}");

                    response.StatusCode = httpStatusCode;
                    response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                }
            }

            //Processing complete - return response
            return response;
        }

        //Get DB Load Results method...
        //GET api/revisedupload/get/dbloadresults/{uploadId}
        //Web API feature: Have to name the input variable 'uploadId' to satisfy the router!!!
        //See WebApiConfig.cs for custom route...
        [HttpGet]
        public async Task<HttpResponseMessage> GetDbLoadResults(string uploadId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;  //Assume success...

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(uploadId))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            var dbLoadContexts = getDbLoadContexts();
            if (!dbLoadContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Unknown uploadId - return early
                response.ReasonPhrase = "Unknown upload id...";
                return response;
            }

            //Input parameter(s) valid - retrieve db load context
            DbLoadContext dbLoadContext = dbLoadContexts[uploadId];
            using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
            {
                //Db load context found - convert db load results to JSON, if indicated...
                var results = dbLoadContext.DbLoadResults;
                string jsonData = (0 < results.Count) ? Newtonsoft.Json.JsonConvert.SerializeObject(results) : Newtonsoft.Json.JsonConvert.SerializeObject("{}");

                response.StatusCode = httpStatusCode;
                response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            }

            //Processing complete - return response
            return response;
        }

        //Get Rejected Items method...
        //GET api/revisedupload/get/rejecteditems/{uploadId}/{tableName}
        //See WebApiConfig.cs for custom route...
        [HttpGet]
        public async Task<HttpResponseMessage> GetRejectedItems(string uploadId, string tableName)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;  //Assume success...

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery AJAX!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(uploadId) || String.IsNullOrWhiteSpace(tableName))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            //Retrieve the associated DbLoadContext...
            var dbLoadContexts = getDbLoadContexts();
            if (!dbLoadContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No dbLoadContext for upload id: {0}", uploadId);
                return response;
            }

            //Check context for input table name...
            DbLoadContext dbLoadContext = dbLoadContexts[uploadId];
            using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
            {
                bool bFound = false;    //Assume failure
                var dbLoadResults = dbLoadContext.DbLoadResults;
                foreach (var dbLoadResult in dbLoadResults)
                {
                    if (tableName.ToLowerInvariant() == dbLoadResult.TableName.ToLowerInvariant())
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    response.StatusCode = HttpStatusCode.NotFound;    //Invalid table name - return early
                    response.ReasonPhrase = String.Format("Unknown table name {0} for upload id: {1}", tableName, uploadId);
                    return response;
                }
            }

            //Retrieve the associated repository context...
            var repositoryContexts = getRepositoryContexts();
            if (!repositoryContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No repositoryContext for upload id: {0}", uploadId);
                return response;
            }

            //Retrieve the associated model type...
            RepositoryContext repositoryContext = repositoryContexts[uploadId];
            Dictionary<String, Type> modelTypes = await repositoryContext.ModelTypeByTableName(tableName);

            Type modelType = modelTypes["tSourceType"];
            Type proxyType = modelTypes["tProxyType"];

            //Retrieve the associated StatusContext...
            var statusContexts = getStatusContexts();
            if (!statusContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No status context for table name {0} for upload id: {1}", tableName, uploadId);
                return response;
            }

            //Retrieve the associated status messages...
            StatusContext statusContext = statusContexts[uploadId];
            List<StatusMessage> listStatusMessages = new List<StatusMessage>();
            using (await statusContext.StatusMessagesSemaphore.UseWaitAsync())
            {
                var keyVal = (null != proxyType) ? proxyType.Name : modelType.Name;
                if (statusContext.StatusMessages.ContainsKey(keyVal))
                {
                    var msgQueue = statusContext.StatusMessages[keyVal];
                    foreach (var msg in msgQueue)
                    {
                        StatusMessage statMsg = new StatusMessage(msg);
                        listStatusMessages.Add(statMsg);
                    }
                }
            }

            //Retrieve the list of UpdateableItem<> from the incorrect records binary file...
            string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");
            string binFilePathAndName = pathProcessed + uploadId + "-" + modelType.Name + "-IncorrectRecords.bin";
            Type tGenericList = typeof(List<>);
            Type tUpdateableItem = typeof(UpdateableItem<>);
            Type gUpdateableItem = tUpdateableItem.MakeGenericType(modelType);
            Type updateableItemsListType = tGenericList.MakeGenericType(gUpdateableItem);
            System.Collections.IList iUpdateableItemsList = (System.Collections.IList)Activator.CreateInstance(updateableItemsListType);

            using (await repositoryContext.RepositorySemaphore.UseWaitAsync())
            {
                try
                {
                    using (var fileStream = new FileStream(binFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                    {
                        //De-serialize to generic list...
                        BinaryFormatter binFor = new BinaryFormatter();
                        //iList = (System.Collections.IList)binFor.Deserialize(fileStream);
                        iUpdateableItemsList = (System.Collections.IList)binFor.Deserialize(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    //File not found - return early
                    string msg = ex.Message;

                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = String.Format("Incorrect records file not found: {0}", binFilePathAndName);
                    return response;
                }
            }

            //Retrieve required and optional property names for model type...
            Type tGenericMap = typeof(GenericMap<>);
            Type tModelMap = tGenericMap.MakeGenericType(modelType);
            List<string> listRequiredPropertyNames = null;
            List<string> listOptionalPropertyNames = null;

            //Construct a map type instance...
            ConstructorInfo constructorInfo = tModelMap.GetConstructor(Type.EmptyTypes);
            if ( null != constructorInfo)
            {
                object mapTypeInstance = constructorInfo.Invoke(new object[] { });

                MethodInfo miGetRequiredPropertyNames = tModelMap.GetMethod("GetRequiredPropertyNames");
                listRequiredPropertyNames = miGetRequiredPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;

                MethodInfo miGetOptionalPropertyNames = tModelMap.GetMethod("GetOptionalPropertyNames");
                listOptionalPropertyNames = miGetOptionalPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;
            }
            
            if (null == listRequiredPropertyNames && null == listOptionalPropertyNames)
            {
                //Cannot retrieve property names - return early
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = String.Format("Cannot find property names for map type {0} for upload id {1}", tModelMap.Name, uploadId);
                return response;
            }

            //All rejected items data retrieved - assemble and return to caller...
            Type tRejectedItemsData = typeof(RejectedItemsData<>);
            Type rejectedItemsModelType = tRejectedItemsData.MakeGenericType(modelType);

            IRejectedItemsData iRejectedItemsData = (IRejectedItemsData)Activator.CreateInstance(rejectedItemsModelType);

            iRejectedItemsData.TableName = tableName;
            iRejectedItemsData.StatusMessages = listStatusMessages;
            //iRejectedItemsData.RejectedItems = iList;
            iRejectedItemsData.RejectedItems = iUpdateableItemsList;
            iRejectedItemsData.RequiredPropertyNames = listRequiredPropertyNames;
            iRejectedItemsData.OptionalPropertyNames = listOptionalPropertyNames;
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(iRejectedItemsData);
#if (DEBUG)
            lock (fileLockObject)
            {
                using (System.IO.FileStream output = new System.IO.FileStream(@"C:\CUAHSI\IRejectedItemsData.json", FileMode.Create))
                {
                    //ms.CopyTo(output);
                    //output.Flush();
                    //output.Close();

                    //ms.Seek(0, SeekOrigin.Begin);

                    using (StreamWriter sw = new StreamWriter(output))
                    {
                        Newtonsoft.Json.JsonTextWriter jw = new Newtonsoft.Json.JsonTextWriter(sw);
                        jw.WriteRaw(jsonData);
                        jw.Flush();
                        jw.Close();
                    }

                    //output.Flush();
                    output.Close();
                }
            }
#endif
            response.StatusCode = httpStatusCode;
            response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            //Processing complete - return response
            return response;
        }

        //Get Rejected Items File method...
        //GET api/revisedupload/get/rejecteditemsfile/{uploadId}/{tableName}
        //See WebApiConfig.cs for custom route...
        [HttpGet]
        public async Task<HttpResponseMessage> GetRejectedItemsFile(string uploadId, string tableName)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;  //Assume success...

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery AJAX!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(uploadId) || String.IsNullOrWhiteSpace(tableName))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            //Retrieve the associated DbLoadContext...
            var dbLoadContexts = getDbLoadContexts();
            if (!dbLoadContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No dbLoadContext for upload id: {0}", uploadId);
                return response;
            }

            //Check context for input table name...
            DbLoadContext dbLoadContext = dbLoadContexts[uploadId];
            using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
            {
                bool bFound = false;    //Assume failure
                var dbLoadResults = dbLoadContext.DbLoadResults;
                foreach (var dbLoadResult in dbLoadResults)
                {
                    if (tableName.ToLowerInvariant() == dbLoadResult.TableName.ToLowerInvariant())
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    response.StatusCode = HttpStatusCode.NotFound;    //Invalid table name - return early
                    response.ReasonPhrase = String.Format("Unknown table name {0} for upload id: {1}", tableName, uploadId);
                    return response;
                }
            }

            //Retrieve the associated repository context...
            var repositoryContexts = getRepositoryContexts();
            if (!repositoryContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No repositoryContext for upload id: {0}", uploadId);
                return response;
            }

            //Retrieve the associated model type...
            RepositoryContext repositoryContext = repositoryContexts[uploadId];
            Dictionary<String, Type> modelTypes = await repositoryContext.ModelTypeByTableName(tableName);

            Type modelType = modelTypes["tSourceType"];

            //Retrieve required and optional property names for model type...
            Type tGenericMap = typeof(GenericMap<>);
            Type tModelMap = tGenericMap.MakeGenericType(modelType);
            List<string> listRequiredPropertyNames = null;
            List<string> listOptionalPropertyNames = null;

            //Construct a map type instance...
            ConstructorInfo constructorInfo = tModelMap.GetConstructor(Type.EmptyTypes);
            if (null != constructorInfo)
            {
                object mapTypeInstance = constructorInfo.Invoke(new object[] { });

                MethodInfo miGetRequiredPropertyNames = tModelMap.GetMethod("GetRequiredPropertyNames");
                listRequiredPropertyNames = miGetRequiredPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;

                MethodInfo miGetOptionalPropertyNames = tModelMap.GetMethod("GetOptionalPropertyNames");
                listOptionalPropertyNames = miGetOptionalPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;
            }

            if (null == listRequiredPropertyNames && null == listOptionalPropertyNames)
            {
                //Cannot retrieve property names - return early
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = String.Format("Cannot find property names for map type {0} for upload id {1}", tModelMap.Name, uploadId);
                return response;
            }

            //Retrieve validation context...
            var validationContexts = getValidationContexts();
            if (!validationContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No validationContext for upload id: {0}", uploadId);
                return response;
            }

            //Retrieve the associated file encoding...
            Encoding fileEncoding = Encoding.GetEncoding("iso-8859-1"); //Default usage...
            ValidationContext<CsvValidator> validationContext = validationContexts[uploadId];
            using (await validationContext.ValidationResultSemaphore.UseWaitAsync())
            {
                foreach (var validationRes in validationContext.ValidationResults)
                {
                    if (modelType == validationRes.FileValidator.ValidatedModelType)
                    {
                        fileEncoding = validationRes.FileValidator.FileEncoding;
                    }
                }
            }

            //Retrieve the list of UpdateableItem<> from the incorrect records binary file...
            string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");
            string binFilePathAndName = pathProcessed + uploadId + "-" + modelType.Name + "-IncorrectRecords.bin";

            using (await repositoryContext.RepositorySemaphore.UseWaitAsync())
            {
                try
                {
                    using (var fileStream = new FileStream(binFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                    {
                        //De-serialize to generic list...
                        BinaryFormatter binFor = new BinaryFormatter();
                        var iUpdateableItemsList = binFor.Deserialize(fileStream);

                        //Prepare call to RepositoryContext method to stream list to model type...
                        Type resContextType = repositoryContext.GetType();
                        MethodInfo miStreamItemsToModelList = resContextType.GetMethod("StreamItemsToModelList");
                        MethodInfo miStreamItemsToModelList_G = miStreamItemsToModelList.MakeGenericMethod(modelType);

                        //Call the async method via reflection...
                        //Source: https://stackoverflow.com/questions/43426533/how-to-invoke-async-method-in-c-sharp-by-using-reflection-and-wont-cause-deadlo
                        var task = (Task)miStreamItemsToModelList_G.Invoke(repositoryContext, new object[] { iUpdateableItemsList,
                                                                                                             fileEncoding,
                                                                                                             listRequiredPropertyNames,
                                                                                                             listOptionalPropertyNames });
                        await task;

                        //Retrieve result...
                        //using (Stream streamResult = task.GetType().GetProperty("Result").GetValue(task) as Stream)
                        Stream streamResult = task.GetType().GetProperty("Result").GetValue(task) as Stream;
                        if (null != streamResult)
                        {

                            //Reset stream position - add stream to response content...
                            streamResult.Position = 0;
                            response.Content = new StreamContent(streamResult);  
                            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                            response.Content.Headers.ContentDisposition.FileName = "Rejected-" + modelType.Name + ".csv";
                            response.Content.Headers.ContentLength = streamResult.Length;

                            response.StatusCode = httpStatusCode;
                        }
                        else
                        {
                            //Streaming error...
                            response.StatusCode = HttpStatusCode.InternalServerError;
                            response.ReasonPhrase = String.Format("Unable to stream rejected items to output for upload id: {0}", uploadId);
                            return response;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //File not found - return early
                    string msg = ex.Message;

                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = String.Format("Incorrect records file not found: {0}", binFilePathAndName);
                    return response;
                }
            }

            //Processing complete - return response
            return response;
        }

        //Request DB Table Counts method...
        //Post api/revisedupload/post/requestdbtablecounts/{tablenames}
        //See WebApiConfig.cs for custom route...
        [HttpPost]
        public async Task<HttpResponseMessage> RequestDbTableCounts()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;    //Assume success...

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery AJAX!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Retrieve request data...
            HttpContent httpContent = Request.Content;

            string content = await httpContent.ReadAsStringAsync();
            TableNames tableNames = Newtonsoft.Json.JsonConvert.DeserializeObject<TableNames>(content) ;

            if (null == tableNames)
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            //Retrieve user name - build connection string...
            var userName = HttpContext.Current.User.Identity.Name;
            if (String.IsNullOrWhiteSpace(userName))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Username not found - return early
                response.ReasonPhrase = Resources.HYDROSERVER_USERLOOKUP_FAILED;
                return response;
            }

            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(userName);
            if (String.IsNullOrWhiteSpace(entityConnectionString))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Connection string not found - return early
                response.ReasonPhrase = Resources.CONNECTION_STRING_NOT_FOUND;
                return response;
            }

            //Create Repository instance...
            var repository = new Repository(entityConnectionString);

            //Retieve counts for input table names...
            Dictionary<string, int> tableNamesToRecordCounts = repository.GetTableRecordCounts(tableNames.tableNames);

            //Return results...
            if (0 < tableNamesToRecordCounts.Count)
            {
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(tableNamesToRecordCounts);
                response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            }

            //Processing complete - return response
            return response;
        }

        //Put method
        //PUT api/revisedupload/put/{uploadId}
        //Web API feature: Have to name the input variable 'uploadId' to satisfy the router!!!
        //See WebApiConfig.cs for custom route...
        [HttpPut]
        public async Task<HttpResponseMessage> Put(string uploadId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;    //Assume success...

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery AJAX!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(uploadId))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            var repositoryContexts = getRepositoryContexts();
            RepositoryContext repositoryContext = null;
            var userName = HttpContext.Current.User.Identity.Name;
            //Retrieve/create associated context instance...
            if (!repositoryContexts.TryGetValue(uploadId, out repositoryContext))
            {
                //Not found - Attempt to build connection string...
                
                if (String.IsNullOrWhiteSpace(userName))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;    //Username not found - return early
                    response.ReasonPhrase = Resources.HYDROSERVER_USERLOOKUP_FAILED;
                    return response;
                }

                var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(userName);
                if (String.IsNullOrWhiteSpace(entityConnectionString))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;    //Connection string not found - return early
                    response.ReasonPhrase = Resources.CONNECTION_STRING_NOT_FOUND;
                    return response;
                }

                //Create new instance...
                repositoryContext = new RepositoryContext(entityConnectionString);
                repositoryContexts.TryAdd(uploadId, repositoryContext);

                //Get the instance again...
                // IIS threading alert... Since the TryAdd(...) call occurs outside  
                // any Semaphore 'using' block it is not thread-safe!!  Thus another 
                // IIS thread may have already added a context for the uploadId.  
                // Getting the instance again ensures all IIS threads reference the 
                // ***same*** context...
                repositoryContexts.TryGetValue(uploadId, out repositoryContext);
            }

            //Check for repository context...
            if (null == repositoryContext)
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //No repository context - return early
                response.ReasonPhrase = "No current upload Id found in request... (from Put)";
                return response;
            }

            var statusContexts = getStatusContexts();
            StatusContext statusContext = null;

            //Retrieve/create associated context instance...
            if (!statusContexts.TryGetValue(uploadId, out statusContext))
            {
                //Not found - create new instance...
                statusContext = new StatusContext();
                statusContexts.TryAdd(uploadId, statusContext);

                //Get the instance again...
                // IIS threading alert... Since the TryAdd(...) call occurs outside  
                // any Semaphore 'using' block it is not thread-safe!!  Thus another 
                // IIS thread may have already added a context for the uploadId.  
                // Getting the instance again ensures all IIS threads reference the 
                // ***same*** context...
                statusContexts.TryGetValue(uploadId, out statusContext);
            }

            //Check for status context...
            if (null == statusContext)
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //No repository context - return early
                response.ReasonPhrase = "Cannot find/create status context for current upload Id... (from Put)";
                return response;
            }

            var dbLoadContexts = getDbLoadContexts();
            DbLoadContext dbLoadContext = null;

            //Retrieve/create associated context instance...
            if (!dbLoadContexts.TryGetValue(uploadId, out dbLoadContext))
            {
                //Not found - create new instance...
                dbLoadContext = new DbLoadContext();
                dbLoadContexts.TryAdd(uploadId, dbLoadContext);

                //Get the instance again...
                // IIS threading alert... Since the TryAdd(...) call occurs outside  
                // any Semaphore 'using' block it is not thread-safe!!  Thus another 
                // IIS thread may have already added a context for the uploadId.  
                // Getting the instance again ensures all IIS threads reference the 
                // ***same*** context...
                dbLoadContexts.TryGetValue(uploadId, out dbLoadContext);
            }

            //Check for db load context...
            if (null == dbLoadContext)
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //No repository context - return early
                response.ReasonPhrase = "Cannot find/create db load context for current upload Id... (from Put)";
                return response;
            }

            //Construct 'validated' path to binary files...
            string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");
            string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");

            //Invoke DB load processing on validated binary files...
            //NOTE: Member semaphore is referenced periodically within the LoadDb(...) call...
            await repositoryContext.LoadDb(uploadId, pathValidated, pathProcessed, statusContext, dbLoadContext);

            //Retrieve db load results, if any - return to client in response data...
            using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
            {
                var dbLoadResults = dbLoadContext.DbLoadResults;
                if (0 < dbLoadResults.Count) 
                {
                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dbLoadResults);
                    response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                }
            }
            //update updateTracking table to indicate changes
            HydroServerToolsUtils.InsertTrackUpdates(userName);

            //Processing complete - return response

            return response;
        }

        //Put (updated) Rejected Items method...
        //PUT api/revisedupload/put/rejecteditems/
        //See WebApiConfig.cs for custom route...
        [HttpPut]
        public async Task<HttpResponseMessage> PutRejectedItems()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;    //Assume success...

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery AJAX!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Retrieve data from request body...
            HttpContent httpContent = Request.Content;
            IUpdateableItemsData<object> iupdateableItemsData = null;

            try
            {
                //Load contents into internal buffer (to allow multiple reads) and read...
                //Source: https://stackoverflow.com/questions/26942514/multiple-calls-to-httpcontent-readasasync
                await httpContent.LoadIntoBufferAsync();
                iupdateableItemsData = await httpContent.ReadAsAsync<UpdateableItemsData<object>>();
            }
            catch (Exception ex)
            {
                //Read content error - return early...

                //Find the 'inner-most' exception
                Exception innerException = ex;
                while (null != innerException.InnerException)
                {
                    innerException = innerException.InnerException;
                }

                response.ReasonPhrase = String.Format("Cannot retrieve request data.  Error: {0}", innerException.Message);
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }

            if (null == iupdateableItemsData)
            {
                response.ReasonPhrase = "Request data empty!!";
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            //Data retrieved - validate uploadId and table name...
            var uploadId = iupdateableItemsData.UploadId;
            var tableName = iupdateableItemsData.TableName;

            //Retrieve the associated DbLoadContext...
            var dbLoadContexts = getDbLoadContexts();
            if (!dbLoadContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No dbLoadContext for upload id: {0}", uploadId);
                return response;
            }

            //Check context for input table name...
            DbLoadContext dbLoadContext = dbLoadContexts[uploadId];
            using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
            {
                bool bFound = false;    //Assume failure
                var dbLoadResults = dbLoadContext.DbLoadResults;
                foreach (var dbLoadResult in dbLoadResults)
                {
                    if (tableName.ToLowerInvariant() == dbLoadResult.TableName.ToLowerInvariant())
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    response.StatusCode = HttpStatusCode.NotFound;    //Invalid table name - return early
                    response.ReasonPhrase = String.Format("Unknown table name {0} for upload id: {1}", tableName, uploadId);
                    return response;
                }
            }

            //Find the associated StatusContext...
            var statusContexts = getStatusContexts();
            if (!statusContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No status context for table name {0} for upload id: {1}", tableName, uploadId);
                return response;
            }

            StatusContext statusContext = statusContexts[uploadId];

            //Retrieve associated file context..
            var fileContexts = getFileContexts();
            if (!fileContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Unknown uploadId - return early
                response.ReasonPhrase = "Unknown upload id...";
                return response;
            }

            FileContext fileContext = fileContexts[uploadId];

            //Find the associated repository context...
            var repositoryContexts = getRepositoryContexts();
            if (!repositoryContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No repositoryContext for upload id: {0}", uploadId);
                return response;
            }

            //Retrieve associated model type from table name...
            RepositoryContext repositoryContext = repositoryContexts[uploadId];
            Dictionary<String, Type> modelTypes = await repositoryContext.ModelTypeByTableName(tableName);

            Type modelType = modelTypes["tSourceType"];
            Type proxyType = modelTypes["tProxyType"];

            //Set generic type for UpdatedItemsData for deserialization of request content...
            //NOTE: To correctly deserialize the <tModelType> items in the UpdatedItems list
            //      a call to the Newtonsoft JsonSerializer is required.  A call to 
            //      httpContent.ReadAsAsync(gUpdatedItemsDataType) runs OK but leaves all 
            //      fields in the <tModelType> items null...
            Type updateableItemsDataType = typeof(UpdateableItemsData<>);
            Type gUpdateableItemsDataType = updateableItemsDataType.MakeGenericType(modelType);

            try
            {
                //Re-read request content with correct model type...
                //Source: https://stackoverflow.com/questions/26942514/multiple-calls-to-httpcontent-readasasync
                using (Stream stream = await httpContent.ReadAsStreamAsync())
                {
                    //Deserialize directly from a stream to optimize memory usage...
                    //Source: https://www.newtonsoft.com/json/help/html/Performance.htm
                    stream.Seek(0, SeekOrigin.Begin);   //rewind stream...
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        using (Newtonsoft.Json.JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                        {
                            Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                            var iupdateableItemsData2 = jsonSerializer.Deserialize(jsonReader, gUpdateableItemsDataType);

                            //Prepare call to RepositoryContext method to update the database...
                            string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");
                            Type resContextType = repositoryContext.GetType();
                            MethodInfo miUpdateDbTable = resContextType.GetMethod("UpdateDbTable");
                            MethodInfo miUpdateDbTable_G = miUpdateDbTable.MakeGenericMethod(modelType);

                            //Call the async method via reflection...
                            //Source: https://stackoverflow.com/questions/43426533/how-to-invoke-async-method-in-c-sharp-by-using-reflection-and-wont-cause-deadlo
                            var task = (Task)miUpdateDbTable_G.Invoke(repositoryContext, new object[] { iupdateableItemsData2,
                                                                                                        pathProcessed,
                                                                                                        statusContext,
                                                                                                        dbLoadContext,
                                                                                                        proxyType });
                            await task;

                            //Retrieve result...
                            TableUpdateResult tableUpdateResult = task.GetType().GetProperty("Result").GetValue(task) as TableUpdateResult;
                            UpdateResults updateResults = null;

                            if ( null != tableUpdateResult)
                            {
                                //Revise contents of binary files per method 'Add...' outcome
                                Type fileContextType = fileContext.GetType();
                                MethodInfo miUpdateBinaryFiles = fileContextType.GetMethod("UpdateBinaryFiles");
                                MethodInfo miUpdateBinaryFiles_G = miUpdateBinaryFiles.MakeGenericMethod(modelType);

                                //Call the async method via reflection...
                                //Source: https://stackoverflow.com/questions/43426533/how-to-invoke-async-method-in-c-sharp-by-using-reflection-and-wont-cause-deadlo
                                var task1 = (Task)miUpdateBinaryFiles_G.Invoke(fileContext, new object[] { iupdateableItemsData2,
                                                                                                           pathProcessed,
                                                                                                           tableUpdateResult});
                                await task1;

                                //Allocate update results, serialize to response content...
                                updateResults = new UpdateResults(uploadId);
                                updateResults.TableUpdateResults.Add(tableUpdateResult);
                            }

                            string jsonData = (null != updateResults) ? Newtonsoft.Json.JsonConvert.SerializeObject(updateResults) : Newtonsoft.Json.JsonConvert.SerializeObject("{}");
                            response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Find the 'inner-most' exception
                Exception innerException = ex;
                while (null != innerException.InnerException)
                {
                    innerException = innerException.InnerException;
                }

                response.StatusCode = HttpStatusCode.InternalServerError;    //Error - return early
                response.ReasonPhrase = String.Format("PutRejectedItems error: '{0}' for upload id: {1}", innerException.Message, uploadId);
                return response;
            }

            //Processing complete - return response
            return response;
        }

        //Post method...
        public async Task<HttpResponseMessage> Post()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;    //Assume success

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery file download!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Map to a relative path...
            //Source: http://bytutorial.com/blogs/aspnet/alternative-way-of-using-server-mappath-in-aspnet-web-api
            string pathUploads = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");
#if (DEBUG)
            DebugData debugData = new DebugData();
#endif
            //Retrieve form data via request context...
            List<FileNameAndType> fileNamesAndTypes = null;
            string currentUploadId = String.Empty;
            string validationQualifier = String.Empty;
            if ( HttpContext.Current.Request.Form.HasKeys() )
            {
                var form = HttpContext.Current.Request.Form;
                currentUploadId = form["currentUploadId"];
                validationQualifier = form["validationQualifier"];
                try
                {
                    fileNamesAndTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileNameAndType>>(form["fileNamesAndTypes"]);
                }
                catch (Newtonsoft.Json.JsonException jEx)
                {
                    //Deserialization error - log and return error...
                    response.StatusCode = HttpStatusCode.BadRequest;

                    //Find the 'inner-most' exception...
                    Exception innerException = jEx;
                    while (null != innerException.InnerException)
                    {
                        innerException = innerException.InnerException;
                    }

                    response.ReasonPhrase = jEx.Message;

                    return response;
                }
            }
#if (DEBUG)
            debugData.currentUploadId = currentUploadId;
            debugData.validationQualifier = validationQualifier;
#endif
            var fileContexts = getFileContexts();
            FileContext fileContext = null;
            if (null != fileNamesAndTypes && (! String.IsNullOrWhiteSpace(currentUploadId)))
            {
                //Form data found - retrieve/create associated context instance...
                if ( !fileContexts.TryGetValue(currentUploadId, out fileContext))
                {
                    //Not found - create new instance...
                    fileContext = new FileContext(currentUploadId, fileNamesAndTypes);
                    fileContexts.TryAdd(currentUploadId, fileContext);

                    //Get the instance again...
                    //IIS threading alert... Since the TryAdd(...) call occurs outside the 
                    // fileContext.FileSemaphore 'using' block it is not thread-safe!!
                    // Thus another IIS thread may have already added a file context 
                    // for the currentUploadId.  Getting the instance again ensures
                    // all IIS threads reference the ***same*** file context...
                    fileContexts.TryGetValue(currentUploadId, out fileContext);
                }
            }

            //Check for file context...
            if (null == fileContext)
            {
                //File context not found...
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ReasonPhrase = "File context not created/found...(from POST)";
                return response;
            }

            using (await fileContext.FileSemaphore.UseWaitAsync())
            {
                //File context found - add file name(s), if indicated...
                var contextFileNamesAndTypes = fileContext.FileNamesAndTypes;
                foreach (var fileNameAndType in fileNamesAndTypes)
                {

                    var result = contextFileNamesAndTypes.Where(item => item.fileName == fileNameAndType.fileName &&
                                                                        item.fileType == fileNameAndType.fileType);
                    if ( (null != result) && (0 >= result.Count()))
                    {
                        contextFileNamesAndTypes.Add(fileNameAndType);
                    }
                }
                
                //Retrieve request content...
                HttpContent httpContent = Request.Content;
                if (httpContent.IsMimeMultipartContent())
                {
                    //Process the multipart content...
                    try
                    {
                        //Read contents into stream...
                        var mpmProvider = new MultipartMemoryStreamProvider();
                        await httpContent.ReadAsMultipartAsync(mpmProvider);

                        //Check headers...
                        var headers = httpContent.Headers;
                        var hdrContentDisposition = headers.ContentDisposition;
                        var hdrContentRange = headers.ContentRange;

                        //Chunk logic based on: https://stackoverflow.com/questions/26546296/how-to-implement-a-web-api-controller-to-accept-chunked-uploads-using-jquery-fil
                        bool isFirstChunk = true;    //Assume the first (or perhaps only) chunk...
                        bool isLastChunk = true;    //Assume the last (only) chunk...

                        if (null != hdrContentRange)
                        {
                            //Chunked content...
                            //long? chunkEnd = hdrContentRange.To;
                            //long? totalLength = hdrContentRange.Length;

                            //isLastChunk = ((chunkEnd + 1) >= totalLength) ? true : false;
                            if (hdrContentRange.HasRange)
                            {
                                //Is this the first chunk?
                                isFirstChunk = hdrContentRange.From > 0 ? false : true;

                                //Is this the last chunk?
                                isLastChunk = ((hdrContentRange.To + 1) >= hdrContentRange.Length);
#if (DEBUG)
                                debugData.from = hdrContentRange.From;
                                debugData.to = hdrContentRange.To;
                                debugData.length = hdrContentRange.Length;
#endif
                            }
                        }
#if (DEBUG)
                        debugData.isFirstChunk = isFirstChunk;
                        debugData.isLastChunk = isLastChunk;
#endif
                        //For each content  element in the contents...
                        char[] charArray = { '\"' };
                        foreach (var content in mpmProvider.Contents)
                        {
                            //Skip 'form' data...
                            var name = content.Headers.ContentDisposition.Name.Trim(charArray);
                            if ("fileNamesAndTypes" == name || 
                                "currentUploadId" == name   ||
                                "validationQualifier" == name)
                            {
                                continue;
                            }

                            //Read current content into stream...
                            using (var contentStream = await content.ReadAsStreamAsync())
                            {
                                //Reposition to beginning of stream...
                                contentStream.Seek(0, SeekOrigin.Begin);

                                //Retrieve file name, construct file path and name...
                                var fileName = content.Headers.ContentDisposition.FileName.Trim(charArray);
                                var filePathAndName = pathUploads + fileContext.PrefixedFileName(fileName);
#if (DEBUG)
                                debugData.fileName = fileName;
                                debugData.filePathAndName = filePathAndName;
#endif
                                //Retrieve file encoding...
                                var contentType = String.Empty;
                                foreach (var fileNameAndType in fileContext.FileNamesAndTypes)
                                {
                                    if (fileName == fileNameAndType.fileName)
                                    {
                                        contentType = fileNameAndType.fileType;
                                        break;
                                    }
                                }

                                Encoding currentEncoding = EncodingContext.GetFileEncoding(contentType, filePathAndName);

                                //Create output file --OR-- open file for append...
                                using (var fileStream = new FileStream(filePathAndName, isFirstChunk ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.None, 65536, true))
                                {
                                    //Copy content to file...
                                    await contentStream.CopyToAsync(fileStream, 65536);

                                    //NOTE: DO NOT use StreamReader/StreamWriter here.  
                                    //      Apparently their use introduces invalid NUL characters in the file stream...

                                    //char[] buffer = new char[65536];
                                    //using (var streamReader = new StreamReader(contentStream, currentEncoding))
                                    //{
                                    //    using (var streamWriter = new StreamWriter(fileStream, currentEncoding))
                                    //    {
                                    //        while (!streamReader.EndOfStream)
                                    //        {
                                    //            await streamReader.ReadAsync(buffer, 0, 65536);
                                    //            await streamWriter.WriteAsync(buffer);
                                    //        }
                                    //    }
                                    //}

                                    //Close all streams...
                                    fileStream.Close();
                                    contentStream.Close();
                                }

                                //If the last chunk, queue a validation task for the completed file
                                if (isLastChunk)
                                {
                                    await ValidateFileContentsAsync(currentUploadId, contentType, fileName, filePathAndName, validationQualifier);
                                }
                            }
                        }

                        //For now - return success...
                        //Return with content to avoid unexpected end of JSON input error...
                        //if (isLastChunk)
                        //{
                        //    //result = Ok();
                        //    //Return with content to avoid unexpected end of JSON input error...
                        //    result = Ok(HttpStatusCode.OK);
                        //}
                        //else
                        //{
                        //    result = Ok(HttpStatusCode.Continue);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //Find the 'inner-most' exception
                        Exception innerException = ex;
                        while (null != innerException.InnerException)
                        {
                            innerException = innerException.InnerException;
                        }

                        response.StatusCode = HttpStatusCode.InternalServerError;
                        response.ReasonPhrase = innerException.Message;
                        return response;
                    }
                }
                else
                {
                    //Unexpected content - set error return...
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ReasonPhrase = "Multipart content expected...";
                    return response;
                }
            }

#if (DEBUG)
            lock (fileLockObject)
            {
                using (StreamWriter swUrl = new StreamWriter(@"C:\CUAHSI\UploaderPostValues.txt", true))
                {
                    swUrl.WriteLine(debugData.ToString());
                    swUrl.Flush();
                    swUrl.Close();
                }
            }
#endif

            //Processing complete - return response...
            return response;
        }


        //Delete the input file per the input uploadId...
        //DELETE api/revisedupload/delete/file/{uploadId}/{fileName}
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteFile(string uploadId, string fileName)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;    //Assume success

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery file download!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(uploadId) || String.IsNullOrWhiteSpace(fileName))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            //Retrieve file context..
            var fileContexts = getFileContexts();
            if (!fileContexts.ContainsKey(uploadId))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Unknown uploadId - return early
                response.ReasonPhrase = "Unknown upload id...";
                return response;
            }

            //Input parameter(s) valid - retrieve file context...
            FileContext fileContext = fileContexts[uploadId];
            using (await fileContext.FileSemaphore.UseWaitAsync())
            {
                //Map file path...
                string pathUploads = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");

                //Construct file path and name, delete file...
                var filePathAndName = pathUploads + fileContext.PrefixedFileName(fileName);
                try
                {
                    File.Delete(filePathAndName);
                }
                catch (Exception ex)
                {
                    //File path invalid (or too long)? - For now take no action
                    var msg = ex.Message;
                }

                //Remove entry from file names...
                var contextFileNamesAndTypes = fileContext.FileNamesAndTypes;
                FileNameAndType target = null;
                foreach (var fileNameAndType in contextFileNamesAndTypes)
                {
                    if (fileName == fileNameAndType.fileName)
                    {
                        target = fileNameAndType;
                        break;
                    }
                }

                if (null != target)
                {
                    contextFileNamesAndTypes.Remove(target);
                }
            }

            //Retrieve validation context...
            var validationContexts = getValidationContexts();
            if (validationContexts.ContainsKey(uploadId))
            {
                //Validation context found - scan validation results...
                ValidationContext<CsvValidator> validationContext = validationContexts[uploadId];
                using (await validationContext.ValidationResultSemaphore.UseWaitAsync())
                {
                    ValidationResult<CsvValidator> validationResult = null;
                    foreach (var validationRes in validationContext.ValidationResults)
                    {
                        if (fileName == validationRes.FileName)
                        {
                            validationResult = validationRes;
                            break;
                        }
                    }

                    if ( null != validationResult)
                    {
                        //Validation result found - map file path...
                        string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");
                        var csvValidator = validationResult.FileValidator;

                        var modeltype = csvValidator.ValidatedModelType;
                        
                        if (null != modeltype)
                        {
                            //File validated - construct file path and name, delete binary file...
                            var binFilePathAndName = pathValidated + uploadId + "-" + modeltype.Name + "-validated.bin";
                            try
                            {
                                File.Delete(binFilePathAndName);
                            }
                            catch (Exception ex)
                            {
                                //File path invalid (or too long)? - For now take no action
                                var msg = ex.Message;
                            }
                        }

                        //Remove entry from file names...
                        var validationResults = validationContext.ValidationResults;
                        validationResults.Remove(validationResult);
                    }
                }

            }

            //Processing complete - return
            return response;
        }

        //Delete all entries and files associated with the input uploadId...
        //DELETE api/revisedupload/delete/uploadId/{uploadId}
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteUploadId(string uploadId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;    //Assume success

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery file download!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(uploadId))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            //For input uploadId, remove associated file context and uploaded files, if any...
            var wildCard = uploadId + "*.*";

            var fileContexts = getFileContexts();
            FileContext fileContext = null;
            if ( fileContexts.TryRemove(uploadId, out fileContext))
            {
                using (await fileContext.FileSemaphore.UseWaitAsync())
                {
                    //Source: https://forums.asp.net/t/1899755.aspx?How+to+delete+files+with+wildcard+
                    string pathUploads = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");
                    DirectoryInfo directoryInfo = new DirectoryInfo(pathUploads);
                    foreach (FileInfo fI in directoryInfo.GetFiles(wildCard))
                    {
                        fI.Delete();
                    }
                }
            }

            //For input uploadId, remove associated validation context and validated files, if any...
            var validationContexts = getValidationContexts();
            ValidationContext<CsvValidator> validationContext = null;
            if (validationContexts.TryRemove(uploadId, out validationContext))
            {
                using (await validationContext.ValidationResultSemaphore.UseWaitAsync())
                {
                    //Source: https://forums.asp.net/t/1899755.aspx?How+to+delete+files+with+wildcard+
                    string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");
                    DirectoryInfo directoryInfo = new DirectoryInfo(pathValidated);
                    foreach (FileInfo fI in directoryInfo.GetFiles(wildCard))
                    {
                        fI.Delete();
                    }
                }
            }

            //For input uploadId, remove associated db load context and rsults files, if any...
            var dbLoadContexts = getDbLoadContexts();
            DbLoadContext dbLoadContext = null;
            if (dbLoadContexts.TryRemove(uploadId, out dbLoadContext))
            {
                using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
                {
                    //Source: https://forums.asp.net/t/1899755.aspx?How+to+delete+files+with+wildcard+
                    string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");
                    DirectoryInfo directoryInfo = new DirectoryInfo(pathProcessed);
                    foreach (FileInfo fI in directoryInfo.GetFiles(wildCard))
                    {
                        fI.Delete();
                    }
                }
            }

            //Processing complete - return
            return response;
        }

        //An asynchronous method for file content validation...
        //ASSUMPTION: Referenced file is available for read access
        //Source: https://www.dotnetperls.com/async
        //private static async void ValidateFileContentsAsync(string uploadId, string fileName, string filePathAndName)
        //private static async Task ValidateFileContentsAsync(string uploadId, string fileName, string filePathAndName)
        private async Task ValidateFileContentsAsync(string uploadId, string contentType, string fileName, string filePathAndName, string validationQualifier)
        {
            //Await a task to ensure this method runs asynchronously
            await Task.Run( async () =>
            {
                //Validate/initialize input parameters...
                var validationContexts = getValidationContexts();
                ValidationContext<CsvValidator> validationContext = null;
                //NOTE: input contentType may be empty...
                if ((!String.IsNullOrWhiteSpace(uploadId)) && 
                    (!String.IsNullOrWhiteSpace(fileName)) && 
                    (!String.IsNullOrWhiteSpace(filePathAndName)) &&
                    (!String.IsNullOrWhiteSpace(validationQualifier)))
                {
                    //Input parameters valid - retrieve/create associated context instance...
                    if ( !validationContexts.TryGetValue(uploadId, out validationContext))
                    {
                        //Not found - create new instance...
                        validationContext = new ValidationContext<CsvValidator>();
                        validationContexts.TryAdd(uploadId, validationContext);

                        //Get the instance again...
                        //IIS threading alert... Since the TryAdd(...) call occurs outside
                        // the validationContext.ValidationResultSemaphore 'using' block 
                        // it is not thread-safe!!  Thus another IIS thread may have 
                        // already added a validation context for the uploadId.  Getting 
                        // the instance again ensures all IIS threads reference the 
                        // ***same*** validation context...
                        validationContexts.TryGetValue(uploadId, out validationContext);
                    }

                    //Attempt to retrieve validator for input file name...
                    using (await validationContext.ValidationResultSemaphore.UseWaitAsync())
                    {
                        ValidationResult<CsvValidator> validationResult = null;
                        foreach (var validationRes in validationContext.ValidationResults)
                        {
                            if (fileName == validationRes.FileName)
                            {
                                validationResult = validationRes;
                                break;
                            }
                        }

                        if (null == validationResult)
                        {
                            validationResult = new ValidationResult<CsvValidator>(fileName, new CsvValidator(contentType, filePathAndName));
                            validationContext.ValidationResults.Add(validationResult);
                        }

                        if (null != validationResult)
                        {
                            //Validation result found - retrieve validator
                            var csvValidator = validationResult.FileValidator;
                            if (null != csvValidator)
                            {
                                //Validator found - set validation qualifier...
                                csvValidator.ValidationQualifier = validationQualifier;

                                //Validate file contents...
                                try
                                {
                                    bool vfcResult = await csvValidator.ValidateFileContents();
                                    //validationResult.ValidationComplete = true;
                                    if (vfcResult)
                                    {
                                        //Success - write validated records to relative path...
                                        string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");
                                        var validatedRecords = csvValidator.ValidatedRecords;
                                        var modeltype = csvValidator.ValidatedModelType;

                                        var binFilePathAndName = pathValidated + uploadId + "-" + modeltype.Name + "-validated.bin";

                                        try
                                        {
                                            //For the output file stream...
                                            //using (var fileStream = new FileStream(binFilePathAndName, FileMode.Create))
                                            using (var fileStream = new FileStream(binFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                                            {
                                                //Serialize validated records to file stream as binary...
                                                BinaryFormatter binFor = new BinaryFormatter();
                                                binFor.Serialize(fileStream, validatedRecords);
                                                fileStream.Flush();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //For now take no action...
                                            string msg = ex.Message;
                                            int n = 5;

                                            ++n;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //For now - take no action...
                                    var message = ex.Message;

                                    int n = 5;

                                    ++n;
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
