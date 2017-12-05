using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;

using HydroServerTools.Utilities;
using HydroServerTools.Validators;

using HydroserverToolsBusinessObjects;

namespace HydroServerTools.Controllers.api
{
    public class RevisedUploadController : ApiController
    {
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


        //Members...
#if (DEBUG)
        private static Object fileLockObject = new Object();

        private struct DebugData
        {
            public string currentUploadId { get; set; }
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
                return String.Format("uploadId: {0}, file: {1}, first: {2}, last: {3}, from: {4}, to: {5}, length: {6}, path: {7}",
                                        currentUploadId, fileName, isFirstChunk, isLastChunk, from, to, length, filePathAndName);
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
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
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
                    //if (fileContext.FileNames.Count != validationContext.ValidationResults.Count)
                    //{
                    //    //Validation not yet complete - return 'partial content'
                    //    response.StatusCode = HttpStatusCode.PartialContent;
                    //    response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject("{}"), 
                    //                                         System.Text.Encoding.UTF8, 
                    //                                         "application/json");

                    //    return response;
                    //}

                    //Note the difference in type between the two collections...
                    var valiDATORResults = validationContext.ValidationResults;                     //CsvValidator
                    var valiDATIONResults = new List<ValidationResult<CsvValidationResults>>();     //CsvValidationResults

                    //Copy CsvValidator instances to CsvValidationResults instances...
                    HttpStatusCode httpStatusCode = HttpStatusCode.OK;  //Assume success...
                    foreach (var valiDATORResult in valiDATORResults)
                    {
                        //if (valiDATORResult.ValidationComplete)
                        //{
                            //Validation complete - add to results...
                            var csvValiDATIONResults = new CsvValidationResults(valiDATORResult.FileValidator);

                            valiDATIONResults.Add(new ValidationResult<CsvValidationResults>(valiDATORResult.FileName, csvValiDATIONResults));
                        //}
                        //else
                        //{
                        //    //Validation not yet complete - re-set status code...
                        //    httpStatusCode = HttpStatusCode.PartialContent;     //206
                        //    //break;
                        //}
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

        //Put method
        //PUT api/revisedupload/put/{uploadId}
        //Web API feature: Have to name the input variable 'uploadId' to satisfy the router!!!
        //See WebApiConfig.cs for custom route...
        [HttpPut]
        public async Task<HttpResponseMessage> Put(string uploadId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;    //Assume success...

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(uploadId))
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            var repositoryContexts = getRepositoryContexts();
            RepositoryContext repositoryContext = null;

            //Retrieve/create associated context instance...
            if (!repositoryContexts.TryGetValue(uploadId, out repositoryContext))
            {
                //Not found - Attempt to build connection string...
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

                //Create new instance...
                repositoryContext = new RepositoryContext(entityConnectionString);
                repositoryContexts.TryAdd(uploadId, repositoryContext);

                //Get the instance again...
                //IIS threading alert... Since the TryAdd(...) call occurs outside  
                // the repositoryContext.RepositorySemaphore 'using' block it is not 
                // thread-safe!!  Thus another IIS thread may have already added a 
                // repository context for the uploadId.  Getting the instance again 
                // ensures all IIS threads reference the ***same*** repository context...
                repositoryContexts.TryGetValue(uploadId, out repositoryContext);
            }

            //Check for repository context...
            if (null == repositoryContext)
            {
                response.StatusCode = HttpStatusCode.BadRequest;    //No repository context - return early
                response.ReasonPhrase = "No current upload Id found in request... (from Put)";
                return response;
            }

            using (await repositoryContext.RepositorySemaphore.UseWaitAsync())
            {
                //Construct 'validated' path to binary files...
                string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");

                //Invoke DB load processing on validated binary files...
                await repositoryContext.LoadDb(uploadId, pathValidated);

                //TO DO - Retrieve/retain information for Summary Report...
                int n = 5;

                ++n;
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
            string[] fileNames = null;
            string currentUploadId = String.Empty;
            if ( HttpContext.Current.Request.Form.HasKeys() )
            {
                var form = HttpContext.Current.Request.Form;
                fileNames = form["fileNames"].Split(new char[] { ',' });
                currentUploadId = form["currentUploadId"];
            }
#if (DEBUG)
            debugData.currentUploadId = currentUploadId;
#endif
            var fileContexts = getFileContexts();
            FileContext fileContext = null;
            if (null != fileNames && (! String.IsNullOrWhiteSpace(currentUploadId)))
            {
                //Form data found - retrieve/create associated context instance...
                if ( !fileContexts.TryGetValue(currentUploadId, out fileContext))
                {
                    //Not found - create new instance...
                    fileContext = new FileContext(currentUploadId, fileNames.ToList());
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
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ReasonPhrase = "No current upload Id found in request...(from POST)";
                return response;
            }

            using (await fileContext.FileSemaphore.UseWaitAsync())
            {
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
                            if ("fileNames" == name || "currentUploadId" == name)
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

                                //Create output file --OR-- open file for append...
                                //using (await fileContext.FileSemaphore.UseWaitAsync())
                                //{
                                    using (var fileStream = new FileStream(filePathAndName, isFirstChunk ? FileMode.Create : FileMode.Append))
                                    {
                                        //Copy content to file...
                                        await contentStream.CopyToAsync(fileStream);

                                        //Close all streams...
                                        fileStream.Close();
                                        contentStream.Close();
                                    }

                                    //If the last chunk, queue a validation task for the completed file
                                    if (isLastChunk)
                                    {
                                        await ValidateFileContentsAsync(currentUploadId, fileName, filePathAndName);
                                    }
                                //}
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

        //An asynchronous method for file content validation...
        //ASSUMPTION: Referenced file is available for read access
        //Source: https://www.dotnetperls.com/async
        //private static async void ValidateFileContentsAsync(string uploadId, string fileName, string filePathAndName)
        //private static async Task ValidateFileContentsAsync(string uploadId, string fileName, string filePathAndName)
        private async Task ValidateFileContentsAsync(string uploadId, string fileName, string filePathAndName)
        {
            //Await a task to ensure this method runs asynchronously
            await Task.Run( async () =>
            {
                //Validate/initialize input parameters...
                var validationContexts = getValidationContexts();
                ValidationContext<CsvValidator> validationContext = null;
                if ((!String.IsNullOrWhiteSpace(uploadId)) && 
                    (!String.IsNullOrWhiteSpace(fileName)) && 
                    (!String.IsNullOrWhiteSpace(filePathAndName)))
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
                            validationResult = new ValidationResult<CsvValidator>(fileName, new CsvValidator(filePathAndName));
                            validationContext.ValidationResults.Add(validationResult);
                        }

                        if (null != validationResult)
                        {
                            //Validation result found - retrieve validator
                            var csvValidator = validationResult.FileValidator;
                            if (null != csvValidator)
                            {
                                //Validator found - validate file contents...
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
                                            using (var fileStream = new FileStream(binFilePathAndName, FileMode.Create))
                                            {
                                                //For a memory stream...
                                                using (var memoryStream = new MemoryStream())
                                                {
                                                    //Serialize validated records to binary...
                                                    BinaryFormatter binFor = new BinaryFormatter();
                                                    binFor.Serialize(memoryStream, validatedRecords);

                                                    //Read stream contents into byte array...
                                                    byte[] byteArray = memoryStream.ToArray();

                                                    //Write byte array asynchronously to output file stream...
                                                    await fileStream.WriteAsync(byteArray, 0, byteArray.Length);
                                                }
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
