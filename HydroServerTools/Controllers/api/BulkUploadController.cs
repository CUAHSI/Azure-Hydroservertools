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

namespace HydroServerTools.Controllers.api
{
    public class BulkUploadController : ApiController
    {
        //private members...
        static string _strHdrUploadId = "X-Api-BulkUpload-UploadId";
        static string _strHdrValidationQualifier = "X-Api-BulkUpload-ValidationQualifier";

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

        // GET
        //Return a newly generated Upload Id
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;    //Assume success...

            //TO DO - Authentication of input JWT...
            //        Return early (with error) if no JWT or JWT invalid/expired

            //Generate new Upload Id...
            var uploadId = Guid.NewGuid().ToString();

            //TO DO - pair Upload Id with JWT... 

            //Add a customer header, pass in parameters to task...
            //Sources: https://stackoverflow.com/questions/32017686/add-a-custom-response-header-in-apicontroller
            //         https://stackoverflow.com/questions/41520513/create-a-task-with-an-actiont-t-n-multiple-parameters
            void action(HttpResponseMessage resp, string upldId) => resp.Headers.Add(_strHdrUploadId, upldId);
            Task task = new Task(() => action(response, uploadId));

            await task;

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

            //TO DO - Authentication of input JWT...
            //        Return early (with error) if no JWT or JWT invalid/expired

            //Map to a relative path...
            //Source: http://bytutorial.com/blogs/aspnet/alternative-way-of-using-server-mappath-in-aspnet-web-api
            string pathUploads = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");

            //Check for multipart content...
            HttpContent httpContent = Request.Content;
            if (httpContent.IsMimeMultipartContent())
            {
                //Multipart content - retrieve Upload Id
                IEnumerable<string> values;
                string uploadId = String.Empty;
                string validationQualifier = String.Empty;

                if (Request.Headers.TryGetValues(_strHdrUploadId, out values))
                {
                    uploadId = values.First();
                    //TO DO - verify uploadId maps to the input JWT...

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

                                        fileContext = CacheCollections.GetContext<FileContext>(uploadId);
                                        if (null == fileContext)
                                        {
                                            //Not found - create and attempt to add to collection...
                                            FileNameAndType fileNameAndType = new FileNameAndType(fileName, contentType);
                                            fileContext = new FileContext(uploadId, new List<FileNameAndType>() { fileNameAndType });

                                            if (!CacheCollections.AddContext<FileContext>(uploadId, fileContext))
                                            {
                                                //Not added - context possibly already added by another IIS thread
                                                //            get the instance again...
                                                fileContext = CacheCollections.GetContext<FileContext>(uploadId);
                                                if (null == fileContext)
                                                {
                                                    //File context not found - return error...
                                                    response.StatusCode = HttpStatusCode.BadRequest;
                                                    response.ReasonPhrase = String.Format("File context not created/found for {0}", fileName);
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
                                                    await ValidateFileContentsAsync(uploadId, contentType, fileName, filePathAndName, validationQualifier);

                                                    //Retrieve validation context...
                                                    ValidationContext<CsvValidator> validationContext = null;

                                                    validationContext = CacheCollections.GetContext<ValidationContext<CsvValidator>>(uploadId);
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
                                                                        response.StatusCode = HttpStatusCode.NotAcceptable;
                                                                        response.ReasonPhrase = String.Format("Validation error(s) for file: '{0}'", fileName);
                                                                        string jsonData = JsonConvert.SerializeObject(csvValiDATIONResults);

                                                                        response.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
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
                                                        response.ReasonPhrase = String.Format("Validation context missing for upload Id: '{0}'", uploadId);
                                                        return response;
                                                    }
                                                }
                                            }
                                        }

                                        //Check for file validation completion (when out of all 'using' blocks)
                                        ValidationContext<CsvValidator> validationContext2 = null;

                                        validationContext2 = CacheCollections.GetContext<ValidationContext<CsvValidator>>(uploadId);
                                        if (null != validationContext2)
                                        {
                                            var valiDATORResults2 = validationContext2.ValidationResults;     //Type: CsvValidator
                                            foreach (var valiDATORResult2 in valiDATORResults2)
                                            {
                                                if (valiDATORResult2.ValidationComplete && (fileName.ToLower() == valiDATORResult2.FileName.ToLower()))
                                                {
                                                    //File validation complete - retrieve repository context...
                                                    //TO DO - Retrieve user name from JWT...
                                                    var userName = "anightcoder.brian@gmail.com";

                                                    RepositoryContext repositoryContext = CacheCollections.GetContext<RepositoryContext>(uploadId);
                                                    if (null == repositoryContext)
                                                    {
                                                        //Repository context not found - attempt to build connection string...
                                                        var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(userName);
                                                        if (String.IsNullOrWhiteSpace(entityConnectionString))
                                                        {
                                                            response.StatusCode = HttpStatusCode.BadRequest;    //Connection string not found - return early
                                                            response.ReasonPhrase = Resources.CONNECTION_STRING_NOT_FOUND;
                                                            return response;
                                                        }

                                                        //Create new context instance...
                                                        repositoryContext = new RepositoryContext(entityConnectionString);
                                                        if (!CacheCollections.AddContext<RepositoryContext>(uploadId, repositoryContext))
                                                        {
                                                            //Not added - context possibly already added by another IIS thread
                                                            //            get the instance again...
                                                            repositoryContext = CacheCollections.GetContext<RepositoryContext>(uploadId);
                                                            if (null == repositoryContext)
                                                            {
                                                                //Repository context not found - return error...
                                                                response.StatusCode = HttpStatusCode.BadRequest;
                                                                response.ReasonPhrase = String.Format("Repository context not created/found for user: {0}", userName);
                                                                return response;
                                                            }
                                                        }
                                                    }

                                                    //Retrieve status context...
                                                    StatusContext statusContext = CacheCollections.GetContext<StatusContext>(uploadId);
                                                    if (null == statusContext)
                                                    {
                                                        statusContext = new StatusContext();
                                                        if (!CacheCollections.AddContext<StatusContext>(uploadId, statusContext))
                                                        {
                                                            //Not added - context possibly already added by another IIS thread
                                                            //            get the instance again...
                                                            statusContext = CacheCollections.GetContext<StatusContext>(uploadId);
                                                            if ( null == statusContext)
                                                            {
                                                                //Status context not found - return error...
                                                                response.StatusCode = HttpStatusCode.BadRequest;
                                                                response.ReasonPhrase = String.Format("Status context not created/found for upload Id: {0}", uploadId);
                                                                return response;
                                                            }
                                                        }
                                                    }

                                                    //Retrieve DbLoad context...
                                                    DbLoadContext dbLoadContext = CacheCollections.GetContext<DbLoadContext>(uploadId);
                                                    if (null == dbLoadContext)
                                                    {
                                                        dbLoadContext = new DbLoadContext();
                                                        if (!CacheCollections.AddContext<DbLoadContext>(uploadId, dbLoadContext))
                                                        {
                                                            //Not added - context possibly already added by another IIS thread
                                                            //            get the instance again...
                                                            dbLoadContext = CacheCollections.GetContext<DbLoadContext>(uploadId);
                                                            if (null == dbLoadContext)
                                                            {
                                                                //DbLoad context not found - return error...
                                                                response.StatusCode = HttpStatusCode.BadRequest;
                                                                response.ReasonPhrase = String.Format("DbLoad context not created/found for upload Id: {0}", uploadId);
                                                                return response;
                                                            }
                                                        }
                                                    }

                                                    //Construct 'validated' path to files...
                                                    string pathValidated = System.Web.Hosting.HostingEnvironment.MapPath("~/Validated/");
                                                    string pathProcessed = System.Web.Hosting.HostingEnvironment.MapPath("~/Processed/");

                                                    //Invoke DB load processing on validated files...
                                                    await repositoryContext.LoadDbBis(uploadId, pathValidated, pathProcessed, statusContext, dbLoadContext);

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
                                                }
                                            }
                                        }
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

                            response.StatusCode = HttpStatusCode.InternalServerError;
                            response.ReasonPhrase = innerException.Message;
                            return response;
                        }
                    }
                    else
                    {
                        //Unexpected content - set error return...
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.ReasonPhrase = String.Format("Header '{0}' not found!!", _strHdrValidationQualifier);
                        return response;
                    }
                }
                else
                {
                    //Unexpected content - set error return...
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ReasonPhrase = String.Format("Header '{0}' not found!!", _strHdrUploadId);
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

            //Processing complete - return response...
            return response;
        }
    }
}
