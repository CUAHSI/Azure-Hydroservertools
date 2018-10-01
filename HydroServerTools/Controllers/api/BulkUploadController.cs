using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

#if (!USE_BINARY_FORMATTER)
using Newtonsoft.Json;
#endif

using HydroServerToolsUtilities;
using HydroServerToolsUtilities.Validators;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.ModelMaps;
using System.Reflection;
using System.Text;
using System.Web;

using RazorEngine;
using RazorEngine.Templating;
using System.Web.Routing;

namespace HydroServerTools.Controllers.api
{
    public class BulkUploadController : ApiController
    {
        //private members...
        private static readonly string _strHdrNetworkApiKey = "X-Api-BulkUpload-NetworkApiKey";
        private static readonly string _strHdrValidationQualifier = "X-Api-BulkUpload-ValidationQualifier";
        private static readonly string _strHdrResponseHumanReadable = "X-Api-BulkUpload-ResponseHumanReadable";

        private static readonly string _postErrorMsgTemplate = "API method: bulkupload/post fails with error code: {0}. Reason: {1}";
        private static readonly string _hdrNotFoundTemplate = "Header '{0}' not found...";

        private static readonly Dictionary<string, string> _itemTypesToFileStubs = new Dictionary<string, string>()
                                                                                    { { "inserted", "CorrectRecords" }, 
                                                                                      { "updated", "EditedRecords" },
                                                                                      { "duplicate", "DuplicateRecords" },
                                                                                      { "rejected", "IncorrectRecords" }
                                                                                    };

        //Private methods...
        //An asynchronous method for file content validation...
        //ASSUMPTION: Referenced file is available for read access
        //Source: https://www.dotnetperls.com/async
        private async Task ValidateFileContentsAsync(string uploadId, string contentType, string fileName, string filePathAndName, string validationQualifier)
        {
            //Await a task to ensure this method runs asynchronously
            await Task.Run(async () =>
            {
                //Validate/initialize input parameters...
                //NOTE: input contentType may be empty...
                if ((!String.IsNullOrWhiteSpace(uploadId)) &&
                    (!String.IsNullOrWhiteSpace(fileName)) &&
                    (!String.IsNullOrWhiteSpace(filePathAndName)) &&
                    (!String.IsNullOrWhiteSpace(validationQualifier)))
                {
                    //Input parameters valid - retrieve/create associated context instance...
                    ValidationContext<CsvValidator> validationContext = null;

                    validationContext = CacheCollections.GetContext<ValidationContext<CsvValidator>>(uploadId);
                    if (null == validationContext)
                    {
                        //Not found - create and attempt to add to collection...
                        validationContext = new ValidationContext<CsvValidator>();
                        if (!CacheCollections.AddContext<ValidationContext<CsvValidator>>(uploadId, validationContext))
                        {
                            //Not added - context possibly already added by another IIS thread
                            validationContext = CacheCollections.GetContext<ValidationContext<CsvValidator>>(uploadId);
                            if (null == validationContext)
                            {
                                //Not found - retun early
                                //TO DO - How to communicate this outcome to the caller??
                                return;
                            }
                        }
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
                            //Not found - create...
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
                                    if (vfcResult)
                                    {
                                        //Success - write validated records to relative path...
                                        string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");
                                        var validatedRecords = csvValidator.ValidatedRecords;
                                        var modeltype = csvValidator.ValidatedModelType;
#if (USE_BINARY_FORMATTER)
                                        var binFilePathAndName = pathValidated + uploadId + "-" + modeltype.Name + "-validated.bin";
#else
                                        var binFilePathAndName = pathValidated + uploadId + "-" + modeltype.Name + "-validated.json";
#endif
                                        try
                                        {
                                            //For the output file stream...
                                            using (var fileStream = new FileStream(binFilePathAndName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 65536 * 16, true))
                                            {
                                                //Move to end of stream...
                                                if (fileStream.CanSeek)
                                                {
                                                    fileStream.Seek(0, SeekOrigin.End);
                                                }
#if (USE_BINARY_FORMATTER)
                                                //Serialize validated records to file stream as binary...
                                                BinaryFormatter binFor = new BinaryFormatter();
                                                binFor.Serialize(fileStream, validatedRecords);

                                                fileStream.Flush();
#else
                                                //Serialize validated records to file stream as JSON...
                                                using (StreamWriter sw = new StreamWriter(fileStream))
                                                {
                                                    using (JsonTextWriter jtw = new JsonTextWriter(sw))
                                                    {
                                                        JsonSerializer jsonSerializer = new JsonSerializer();
                                                        foreach (var validatedRecord in validatedRecords)
                                                        {
                                                            jsonSerializer.Serialize(jtw, validatedRecord);
                                                        }

                                                        await jtw.FlushAsync();
                                                    }
                                                }
#endif
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

        // GetItemsFile
        //Return a non-zero length items file as produced by db insertion/update processing...
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetItemsFile(string itemsType, string uploadId, string tableName)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;  //Assume success...

            //Write an empty JSON object to the response
            //  To avoid 'Unexpected end of JSON input' error in jQuery AJAX!!
            response.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            //Validate/initialize input parameters
            if (String.IsNullOrWhiteSpace(itemsType) || 
                String.IsNullOrWhiteSpace(uploadId) || 
                String.IsNullOrWhiteSpace(tableName) ||
                (! _itemTypesToFileStubs.ContainsKey(itemsType.ToLowerInvariant())))
            {
                response.StatusCode = HttpStatusCode.PreconditionFailed;    //Missing/invalid parameter(s) - return early
                response.ReasonPhrase = "Invalid parameter(s)";
                return response;
            }

            //Retrieve DbLoad context...
            DbLoadContext dbLoadContext = CacheCollections.GetContext<DbLoadContext>(uploadId);
            if (null == dbLoadContext)
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No dbLoadContext for upload id: {0}", uploadId);
                return response;
            }

            //Check context for input table name...
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
            RepositoryContext repositoryContext = CacheCollections.GetContext<RepositoryContext>(uploadId);
            if (null == repositoryContext)
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No repositoryContext for upload id: {0}", uploadId);
                return response;
            }

            //Retrieve the associated model type...
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
            ValidationContext<CsvValidator> validationContext = CacheCollections.GetContext<ValidationContext<CsvValidator>>(uploadId);
            if (null == validationContext)
            {
                response.StatusCode = HttpStatusCode.NotFound;    //Unknown uploadId - return early
                response.ReasonPhrase = String.Format("No validationContext for upload id: {0}", uploadId);
                return response;
            }

            //Retrieve the associated file encoding...
            System.Text.Encoding fileEncoding = System.Text.Encoding.GetEncoding("iso-8859-1"); //Default usage...
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

            //Retrieve the associated file stub...
            var fileStub = _itemTypesToFileStubs[itemsType.ToLowerInvariant()];

            //Retrieve the list of UpdateableItem<> from the indicated records file...
            string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");
#if (USE_BINARY_FORMATTER)
            string binFilePathAndName = pathProcessed + uploadId + "-" + modelType.Name + "-" + fileStub + ".bin";
#else
            string binFilePathAndName = pathProcessed + uploadId + "-" + modelType.Name + "-" + fileStub + ".json";
#endif

            using (await repositoryContext.RepositorySemaphore.UseWaitAsync())
            {
                try
                {
                    int bufferSize = 65536 * 16;
                    using (var fileStream = new FileStream(binFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true))
                    {
#if (USE_BINARY_FORMATTER)
                        //De-serialize binary file to generic list...
                        BinaryFormatter binFor = new BinaryFormatter();
                        var iUpdateableItemsList = binFor.Deserialize(fileStream);
#else
                        //De-serialize JSON file to generic list...
                        Type tGenericList = typeof(List<>);
                        Type tUpdateableItem = typeof(UpdateableItem<>);
                        Type gUpdateableItem = tUpdateableItem.MakeGenericType(modelType);
                        Type updateableItemsListType = tGenericList.MakeGenericType(gUpdateableItem);
                        System.Collections.IList iUpdateableItemsList = (System.Collections.IList)Activator.CreateInstance(updateableItemsListType);

                        using (StreamReader sr = new StreamReader(fileStream))
                        {
                            //Find the generic Deserialize method: public T Deserialize<T>(JsonReader reader);
                            //Source: https://forums.asp.net/t/1664599.aspx?+Ask+How+to+get+generic+method+using+reflection+
                            JsonSerializer jsonSerializer = new JsonSerializer();
                            var serializerType = typeof(JsonSerializer);

                            //var methodInfos = serializerType.GetMethods();
                            //var miDeserialize = methodInfos.Where(m => m.IsGenericMethod && m.Name.Equals("Deserialize", StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                            //if (null != miDeserialize)
                            //{
                                //MethodInfo miDeserialize_g = miDeserialize.MakeGenericMethod(updateableItemsListType);

                                using (JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
                                {
                                    ////iUpdateableItemsList = (System.Collections.IList)miDeserialize_g.Invoke(jsonSerializer, new object[] { sr });

                                    //iUpdateableItemsList = (System.Collections.IList)jsonSerializer.Deserialize(jsonReader, updateableItemsListType);
                                    jsonReader.SupportMultipleContent = true;
                                    while (await jsonReader.ReadAsync())
                                    {
                                        iUpdateableItemsList.Add(jsonSerializer.Deserialize(jsonReader, gUpdateableItem));
                                    }
                                }
                            //}
                        }
#endif
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
                        Stream streamResult = task.GetType().GetProperty("Result").GetValue(task) as Stream;
                        if (null != streamResult)
                        {
                            //Reset stream position - add stream to response content...
                            streamResult.Position = 0;
                            response.Content = new StreamContent(streamResult);
                            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                            response.Content.Headers.ContentDisposition.FileName = itemsType + "-" + modelType.Name + ".csv";
                            response.Content.Headers.ContentLength = streamResult.Length;

                            response.StatusCode = httpStatusCode;
                        }
                        else
                        {
                            //Streaming error...
                            response.StatusCode = HttpStatusCode.InternalServerError;
                            response.ReasonPhrase = String.Format("Unable to stream {0} items to output for upload id: {1}", itemsType, uploadId);
                            return response;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //File not found - return early
                    string msg = ex.Message;

                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = String.Format("{0} items file not found: {1}", itemsType, binFilePathAndName);
                    return response;
                }
            }

            //Processing complete - return response
            return response;
        }

        // POST
        //Retrieve file contents --OR-- file 'chunk' contents from request.
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
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

            //Default to human readable response content...
            bool bRespHumanReadable = true;

            //Check for multipart content...
            HttpContent httpContent = Request.Content;
            if (httpContent.IsMimeMultipartContent())
            {
                //Multipart content -retrieve response human readable parameter, if any...
                IEnumerable<string> values;
                if (Request.Headers.TryGetValues(_strHdrResponseHumanReadable, out values))
                {
                    bool bResult = false;

                    var val = values.First();
                    if ( bool.TryParse(values.First(), out bResult))
                    {
                        bRespHumanReadable = bResult;
                    }
                }

                //Retrieve Upload Id and user name
                string networkApiKey = String.Empty;
                string validationQualifier = String.Empty;
                string userName;

                values = null;
                if (Request.Headers.TryGetValues(_strHdrNetworkApiKey, out values))
                {
                    networkApiKey = values.First();

                    //Retrieve user name from network API key...
                    userName = HydroServerToolsUtils.GetGoogleEmailForNetworkApiKey(networkApiKey);
                    if (String.IsNullOrWhiteSpace(userName))
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;    //User name not found - return early
                        var reason = String.Format("Google e-mail account not found for network API key: {0}", networkApiKey);

                        response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                        if (bRespHumanReadable)
                        {
                            response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                        }

                        return response;
                    }

                    values = null;
                    if (Request.Headers.TryGetValues(_strHdrValidationQualifier, out values))
                    {
                        validationQualifier = values.First();

                        try
                        {
                            //Process the multipart content...
                            var mpmProvider = new MultipartMemoryStreamProvider();
                            await httpContent.ReadAsMultipartAsync(mpmProvider);

                            char[] charArray = { '\"' };
                            bool bContent = false;      //Assume no content found...

                            //NOTE: Following logic returns error response on first content error found...
                            foreach (var content in mpmProvider.Contents)
                            {
                                //Check headers...
                                var headers = content.Headers;
                                var hdrContentDisposition = headers.ContentDisposition;
                                var hdrContentRange = headers.ContentRange;
                                var hdrContentType = headers.ContentType;

                                //Check content disposition...
                                if (null != hdrContentDisposition && (null != hdrContentDisposition.FileName))
                                {
                                    //File content - retrieve file name...
                                    var fileName = hdrContentDisposition.FileName.Trim(charArray);

                                    //Check for file 'chunk'...
                                    //Chunk logic based on: https://stackoverflow.com/questions/26546296/how-to-implement-a-web-api-controller-to-accept-chunked-uploads-using-jquery-fil
                                    bool isFirstChunk = true;    //Assume the first (or perhaps only) chunk...
                                    bool isLastChunk = true;    //Assume the last (only) chunk...

                                    if ((null != hdrContentRange) && hdrContentRange.HasRange)
                                    {
                                        //Chunked content - First chunk?
                                        isFirstChunk = hdrContentRange.From > 0 ? false : true;

                                        //Last chunk?
                                        isLastChunk = ((hdrContentRange.To + 1) >= hdrContentRange.Length);
                                    }

                                    //Check content type...
                                    if ((null != hdrContentType) && (null != hdrContentType.MediaType))
                                    {
                                        //Retrieve content type...
                                        string contentType = hdrContentType.MediaType;

                                        //Retrieve/create associated file context...
                                        FileContext fileContext = null;

                                        fileContext = CacheCollections.GetContext<FileContext>(networkApiKey);
                                        if (null == fileContext)
                                        {
                                            //Not found - create and attempt to add to collection...
                                            FileNameAndType fileNameAndType = new FileNameAndType(fileName, contentType);
                                            fileContext = new FileContext(networkApiKey, new List<FileNameAndType>() { fileNameAndType });

                                            if (!CacheCollections.AddContext<FileContext>(networkApiKey, fileContext))
                                            {
                                                //Not added - context possibly already added by another IIS thread
                                                //            get the instance again...
                                                fileContext = CacheCollections.GetContext<FileContext>(networkApiKey);
                                                if (null == fileContext)
                                                {
                                                    //File context not found - return error...
                                                    response.StatusCode = HttpStatusCode.NotFound;
                                                    var reason = String.Format("File context not created/found for network API key: {0}, file: {1}", networkApiKey, fileName);

                                                    response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                                                    if (bRespHumanReadable)
                                                    {
                                                        response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                                                    }
                                                    
                                                    return response;
                                                }
                                            }
                                        }

                                        using (await fileContext.FileSemaphore.UseWaitAsync())
                                        {
                                            //File context found - add file name(s), if indicated...
                                            var contextFileNamesAndTypes = fileContext.FileNamesAndTypes;

                                            var result = contextFileNamesAndTypes.Where(item => item.fileName == fileName &&
                                                                                                item.fileType == contentType);
                                            if ((null != result) && (0 >= result.Count()))
                                            {
                                                contextFileNamesAndTypes.Add(new FileNameAndType(fileName, contentType));
                                            }

                                            //Construct file path and name...
                                            var filePathAndName = pathUploads + fileContext.PrefixedFileName(fileName);

                                            //Read content...
                                            using (var contentStream = await content.ReadAsStreamAsync())
                                            {
                                                //Reposition to beginning of stream...
                                                bContent = true;
                                                contentStream.Seek(0, SeekOrigin.Begin);

                                                //Create output file --OR-- open file for append...
                                                int bufferSize = 65536 * 16;
                                                using (var fileStream = new FileStream(filePathAndName, isFirstChunk ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.None, bufferSize, true))
                                                {
                                                    //Copy content to file...
                                                    await contentStream.CopyToAsync(fileStream, bufferSize);

                                                    //Close all streams...
                                                    fileStream.Close();
                                                    contentStream.Close();
                                                }

                                                //If the last chunk, queue a validation task for the completed file
                                                if (isLastChunk)
                                                {
                                                    await ValidateFileContentsAsync(networkApiKey, contentType, fileName, filePathAndName, validationQualifier);

                                                    //Retrieve validation context...
                                                    ValidationContext<CsvValidator> validationContext = null;

                                                    validationContext = CacheCollections.GetContext<ValidationContext<CsvValidator>>(networkApiKey);
                                                    if (null != validationContext)
                                                    {
                                                        //Validation context found - retrieve file validation results...
                                                        using (await validationContext.ValidationResultSemaphore.UseWaitAsync())
                                                        {
                                                            var valiDATORResults = validationContext.ValidationResults;     //Type: CsvValidator
                                                            foreach (var valiDATORResult in valiDATORResults)
                                                            {
                                                                if (fileName.ToLower() == valiDATORResult.FileName.ToLower())
                                                                {
                                                                    //Validator results found for current file - translate to validation results
                                                                    var csvValiDATIONResults = new CsvValidationResults(valiDATORResult.FileValidator.ValidationResults);   //Type: CsvValidationResults

                                                                    if ("unknown" == csvValiDATIONResults.CandidateTypeName.ToLower() ||
                                                                         0 < csvValiDATIONResults.InvalidHeaderNames.Count ||
                                                                         0 < csvValiDATIONResults.MissingRequiredHeaderNames.Count)
                                                                    {
                                                                        //Validation error(s) - return error response 
                                                                        response.StatusCode = (HttpStatusCode) 422; //WebDAV Unprocessable Entity
                                                                        var reason = String.Format("Validation error(s) for file: '{0}'", fileName);

                                                                        response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                                                                        if (bRespHumanReadable)
                                                                        {
                                                                            //Report results in a rendered Razor view...
                                                                            // Sources: https://itechmanager.com/2013/09/10/web-api-and-returning-a-razor-view/
                                                                            //          https://medium.com/@DomBurf/templated-html-emails-using-razorengine-6f150bb5f3a6
                                                                            string viewPath = @"~/Views/Shared/_LayoutBulkUpload.cshtml";
                                                                            string template = String.Empty;

                                                                            if (!Engine.Razor.IsTemplateCached(viewPath, null))
                                                                            {
                                                                                string fullViewPath = HttpContext.Current.Server.MapPath(viewPath);
                                                                                template = File.ReadAllText(fullViewPath);
                                                                                Engine.Razor.AddTemplate(viewPath, template);
                                                                            }

                                                                            DynamicViewBag viewBag = new DynamicViewBag();

                                                                            //Add file name to view bag...
                                                                            viewBag.AddValue("fileName", fileName);

                                                                            //Retrieve request scheme, host and port...
                                                                            var httpContext = System.Web.HttpContext.Current;
                                                                            var urlBase = httpContext.Request.Url.Scheme + "://" +
                                                                                          httpContext.Request.Url.Host + ":" +
                                                                                          httpContext.Request.Url.Port + "/";

                                                                            //Add base URL to view bag...
                                                                            viewBag.AddValue("urlBase", urlBase);

                                                                            //Add service name to view bag...
                                                                            var userId = HydroServerToolsUtils.GetUserIdFromUserName(userName);
                                                                            var serviceName = HydroServerToolsUtils.GetServiceNameForUserID(userId);

                                                                            viewBag.AddValue("serviceName", serviceName);

                                                                            string templateName = "ValidationSummary";
                                                                            string parsedView = String.Empty;

                                                                            //Check engine cache for template - add to cache if not found...
                                                                            //Source: https://stackoverflow.com/questions/42119846/how-to-runcomplie-on-updated-template-using-same-key-in-razorengine
                                                                            if (Engine.Razor.IsTemplateCached(templateName, null))
                                                                            {
                                                                                parsedView = Engine.Razor.Run(templateName, csvValiDATIONResults.GetType(), csvValiDATIONResults, viewBag);
                                                                            }
                                                                            else
                                                                            {
                                                                                viewPath = HttpContext.Current.Server.MapPath(@"~/Views/BulkUpload/ValidationSummary.cshtml");
                                                                                template = File.ReadAllText(viewPath);
                                                                                Engine.Razor.AddTemplate(templateName, template);

                                                                                parsedView = Engine.Razor.RunCompile(template, templateName, csvValiDATIONResults.GetType(), csvValiDATIONResults, viewBag);
                                                                            }

                                                                            response.Content = new StringContent(parsedView);
                                                                            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                                                                        }
                                                                        else
                                                                        {
                                                                            string jsonData = JsonConvert.SerializeObject(csvValiDATIONResults);

                                                                            response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                                                                        }

                                                                        return response;
                                                                    }

                                                                    break;  //Uploaded file passes validation - break...
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //Validation context not found...
                                                        response.StatusCode = HttpStatusCode.NotFound;
                                                        var reason = String.Format("Validation context missing for network API key: '{0}'", networkApiKey);

                                                        response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                                                        if (bRespHumanReadable)
                                                        {
                                                            response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                                                        }

                                                         return response;
                                                    }
                                                }
                                            }
                                        }

                                        //Check for file validation completion (when out of all 'using' blocks)
                                        ValidationContext<CsvValidator> validationContext2 = null;

                                        validationContext2 = CacheCollections.GetContext<ValidationContext<CsvValidator>>(networkApiKey);
                                        if (null != validationContext2)
                                        {
                                            var valiDATORResults2 = validationContext2.ValidationResults;     //Type: CsvValidator
                                            foreach (var valiDATORResult2 in valiDATORResults2)
                                            {
                                                if (valiDATORResult2.ValidationComplete && (fileName.ToLower() == valiDATORResult2.FileName.ToLower()))
                                                {
                                                    //valiDATORResult2.FileValidator.ValidatedModelType
                                                    //File validation complete - retrieve repository context...
                                                    Type modelType = valiDATORResult2.FileValidator.ValidatedModelType;

                                                    RepositoryContext repositoryContext = CacheCollections.GetContext<RepositoryContext>(networkApiKey);
                                                    if (null == repositoryContext)
                                                    {
                                                        //Repository context not found - attempt to build connection string...
                                                        var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(userName);
                                                        if (String.IsNullOrWhiteSpace(entityConnectionString))
                                                        {
                                                            response.StatusCode = HttpStatusCode.BadRequest;    //Connection string not found - return early
                                                            var reason = String.Format("Database connection string not found for user: {0}", userName);

                                                            response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                                                            if (bRespHumanReadable)
                                                            {
                                                                response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                                                            }
                                                            
                                                            return response;
                                                        }

                                                        //Create new context instance...
                                                        repositoryContext = new RepositoryContext(entityConnectionString);
                                                        if (!CacheCollections.AddContext<RepositoryContext>(networkApiKey, repositoryContext))
                                                        {
                                                            //Not added - context possibly already added by another IIS thread
                                                            //            get the instance again...
                                                            repositoryContext = CacheCollections.GetContext<RepositoryContext>(networkApiKey);
                                                            if (null == repositoryContext)
                                                            {
                                                                //Repository context not found - return error...
                                                                response.StatusCode = HttpStatusCode.NotFound;
                                                                var reason = String.Format("Repository context not created/found for user: {0}", userName);

                                                                response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                                                                if (bRespHumanReadable)
                                                                {
                                                                    response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                                                                }
                                                                
                                                                return response;
                                                            }
                                                        }
                                                    }

                                                    //Retrieve status context...
                                                    StatusContext statusContext = CacheCollections.GetContext<StatusContext>(networkApiKey);
                                                    if (null == statusContext)
                                                    {
                                                        statusContext = new StatusContext();
                                                        if (!CacheCollections.AddContext<StatusContext>(networkApiKey, statusContext))
                                                        {
                                                            //Not added - context possibly already added by another IIS thread
                                                            //            get the instance again...
                                                            statusContext = CacheCollections.GetContext<StatusContext>(networkApiKey);
                                                            if ( null == statusContext)
                                                            {
                                                                //Status context not found - return error...
                                                                response.StatusCode = HttpStatusCode.NotFound;
                                                                var reason = String.Format("Status context not created/found for network API key: {0}", networkApiKey);

                                                                response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                                                                if (bRespHumanReadable)
                                                                {
                                                                    response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                                                                }
                                                                return response;
                                                            }
                                                        }
                                                    }

                                                    //Retrieve DbLoad context...
                                                    DbLoadContext dbLoadContext = CacheCollections.GetContext<DbLoadContext>(networkApiKey);
                                                    if (null == dbLoadContext)
                                                    {
                                                        dbLoadContext = new DbLoadContext();
                                                        if (!CacheCollections.AddContext<DbLoadContext>(networkApiKey, dbLoadContext))
                                                        {
                                                            //Not added - context possibly already added by another IIS thread
                                                            //            get the instance again...
                                                            dbLoadContext = CacheCollections.GetContext<DbLoadContext>(networkApiKey);
                                                            if (null == dbLoadContext)
                                                            {
                                                                //DbLoad context not found - return error...
                                                                response.StatusCode = HttpStatusCode.NotFound;
                                                                var reason = String.Format("DbLoad context not created/found for network API key: {0}", networkApiKey);

                                                                response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                                                                if (bRespHumanReadable)
                                                                {
                                                                    response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                                                                }
                                                                
                                                                return response;
                                                            }
                                                        }
                                                    }

                                                    //Construct 'validated' path to files...
                                                    string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");
                                                    string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");

                                                    //Invoke DB load processing on validated files...
                                                    bool bPurgeValidated = true;    //purge validated file(s) to prevent multiple db insertion attempts...
                                                    await repositoryContext.LoadDbBis(networkApiKey, pathValidated, pathProcessed, statusContext, dbLoadContext, bPurgeValidated);

                                                    //Retrieve db load results, if any - return to client in response data...
                                                    using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
                                                    {
                                                        var tableName = repositoryContext.GetTableName(modelType.Name);
                                                        var dbLoadResults = dbLoadContext.DbLoadResults;
                                                        if ((0 < dbLoadResults.Count) && (!String.IsNullOrWhiteSpace(tableName)))
                                                        {
                                                            var dbLoadResult = dbLoadResults.Find(dblr => tableName.ToLower() == dblr.TableName.ToLower());
                                                            if (null != dbLoadResult)
                                                            {
                                                                if (bRespHumanReadable)
                                                                {
                                                                    //NOTE: Tried using UrlHelper to programmatically create complete URL's for 
                                                                    //      API calls as explained in the source references, but encountered incomplete/erroneous results!!
                                                                    //
                                                                    //Report results in a rendered Razor view...
                                                                    // Sources: https://itechmanager.com/2013/09/10/web-api-and-returning-a-razor-view/
                                                                    //          https://medium.com/@DomBurf/templated-html-emails-using-razorengine-6f150bb5f3a6
                                                                    string viewPath = @"~/Views/Shared/_LayoutBulkUpload.cshtml";
                                                                    string template = String.Empty;

                                                                    if (!Engine.Razor.IsTemplateCached(viewPath, null))
                                                                    {
                                                                        string fullViewPath = HttpContext.Current.Server.MapPath(viewPath);
                                                                        template = File.ReadAllText(fullViewPath);
                                                                        Engine.Razor.AddTemplate(viewPath, template);
                                                                    }

                                                                    DynamicViewBag viewBag = new DynamicViewBag();

                                                                    //Add file name to view bag...
                                                                    viewBag.AddValue("fileName", fileName);

                                                                    //Add networkApiKey to view bag...
                                                                    viewBag.AddValue("networkApiKey", networkApiKey);

                                                                    //Retrieve request scheme, host and port...
                                                                    var httpContext = System.Web.HttpContext.Current;
                                                                    var urlBase = httpContext.Request.Url.Scheme + "://" +
                                                                                  httpContext.Request.Url.Host + ":" +
                                                                                  httpContext.Request.Url.Port + "/";

                                                                    //Add base URL to view bag...
                                                                    viewBag.AddValue("urlBase", urlBase);

                                                                    //Add service name to view bag...
                                                                    var userId = HydroServerToolsUtils.GetUserIdFromUserName(userName);
                                                                    var serviceName = HydroServerToolsUtils.GetServiceNameForUserID(userId);

                                                                    viewBag.AddValue("serviceName", serviceName);

                                                                    string templateName = "DatabaseLoadSummary";
                                                                    string parsedView = String.Empty;

                                                                    //Check engine cache for template - add to cache if not found...
                                                                    //Source: https://stackoverflow.com/questions/42119846/how-to-runcomplie-on-updated-template-using-same-key-in-razorengine
                                                                    if (Engine.Razor.IsTemplateCached(templateName, null))
                                                                    {
                                                                        parsedView = Engine.Razor.Run(templateName, dbLoadResult.GetType(), dbLoadResult, viewBag);
                                                                    }
                                                                    else
                                                                    {
                                                                        viewPath = HttpContext.Current.Server.MapPath(@"~/Views/BulkUpload/DatabaseLoadSummary.cshtml");
                                                                        template = File.ReadAllText(viewPath);
                                                                        Engine.Razor.AddTemplate(templateName, template);

                                                                        parsedView = Engine.Razor.RunCompile(template, templateName, dbLoadResult.GetType(), dbLoadResult, viewBag);
                                                                    }

                                                                    response.Content = new StringContent(parsedView);
                                                                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                                                                }
                                                                else
                                                                {
                                                                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dbLoadResult);
                                                                    response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                                                                }
                                                            }
                                                        }
                                                    }

                                                    //update updateTracking table to indicate changes
                                                    HydroServerToolsUtils.InsertTrackUpdates(userName);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!bContent)
                            {
                                //No content - return early with error response...
                                response.StatusCode = HttpStatusCode.PreconditionFailed;
                                var reason = "No content found...";

                                response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                                if (bRespHumanReadable)
                                {
                                    response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                                }

                                return response;
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

                            response.StatusCode = HttpStatusCode.InternalServerError;
                            var reason = String.Format("Internal Server Error: {0}", innerException.Message);

                            response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;

                            if (bRespHumanReadable)
                            {
                                response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                            }
                            else
                            {
                                //Return call stack as response content, if available...
                                if (!String.IsNullOrWhiteSpace(innerException.StackTrace))
                                {
                                    string responseContent = String.Format("Stack trace: {0}", innerException.StackTrace);
                                    response.Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "text/plain");
                                }
                            }

                            return response;
                        }
                    }
                    else
                    {
                        //Unexpected content - set error return...
                        response.StatusCode = HttpStatusCode.PreconditionFailed;
                        var reason = String.Format(_hdrNotFoundTemplate, _strHdrValidationQualifier);

                        response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                        if (bRespHumanReadable)
                        {
                            response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                        }
                        
                        return response;
                    }
                }
                else
                {
                    //Unexpected content - set error return...
                    response.StatusCode = HttpStatusCode.PreconditionFailed;
                    var reason = String.Format(_hdrNotFoundTemplate, _strHdrNetworkApiKey);

                    response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                    if (bRespHumanReadable)
                    {
                        response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                    }
                    
                    return response;
                }
            }
            else
            {
                //Not multipart content - set error return...
                response.StatusCode = HttpStatusCode.PreconditionFailed;
                var reason = "Multipart content expected...";

                response.ReasonPhrase = bRespHumanReadable ? String.Format(_postErrorMsgTemplate, response.StatusCode, reason) : reason;
                if (bRespHumanReadable)
                {
                    response.Content = new StringContent(response.ReasonPhrase, System.Text.Encoding.UTF8, "text/plain");
                }
            }

            //Processing complete - return response...
            return response;
        }
    }
}
