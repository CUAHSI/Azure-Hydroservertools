
using CsvHelper;
using HydroServerTools.Helper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;

namespace HydroServerTools.Controllers.WebApi
{
    public class UploadController : ApiController
    {
        // Enable both Get and Post so that our jquery call can send data, and get a status
        [HttpGet]
        [HttpPost]
        [Authorize]
        public HttpResponseMessage Upload(string id)
        {
            try
            {
                // Get a reference to the file that our jQuery sent.  Even with multiple files, they will all be their own request and be the 0 index
                HttpPostedFile file = HttpContext.Current.Request.Files[0];

                //int count = 0;
                 if (Utils.IsLocalHostServer())
                {
                     //clear Session 
                     var httpContext = (HttpContextWrapper)Request.Properties["MS_HttpContext"];
                      httpContext.Session.Clear();
                }
                else
                 { 
                     //clear cache 
                    var httpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
                    //hack to provide unique id, work around the problem with the session and google ID
                     var identifier = HttpContext.Current.User.Identity.Name;
                     DataCache cache = new DataCache("default");
                     cache.Remove(identifier + "listOfCorrectRecords");
                     cache.Remove(identifier + "listOfIncorrectRecords");
                     cache.Remove(identifier + "listOfEditedRecords");
                     cache.Remove(identifier + "listOfDuplicateRecords");

                 }
                //var viewName = httpContext.Request.Form["identifier"];
                //var viewName = id;
                // do something with the file in this space 
                string message = string.Empty;
                if (file != null) 
                    ProcessData(file, id, out message);
                else 
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);


                if (message.Length> 0) //an error has occured
                {
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, err);
                }
                //if (!ModelState.IsValid)
                //{
                //    var error = ModelState.Values.Any(x => x.Errors.FirstOrDefault());
                //}
                // Now we need to wire up a response so that the calling script understands what happened
                HttpContext.Current.Response.ContentType = "text/plain";
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = new { name = file.FileName };

                HttpContext.Current.Response.Write(serializer.Serialize(result));
                HttpContext.Current.Response.StatusCode = 200;

                // For compatibility with IE's "done" event we need to return a result as well as setting the context.response
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                // Now we need to wire up a response so that the calling script understands what happened
                HttpError err = new HttpError(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
            }

        }


        private void ProcessData(HttpPostedFile file, string viewName, out string message)
        {
            
            //string viewName = "sites";
            string entityConnectionString = string.Empty;
            message = string.Empty;
            if (!file.FileName.ToLower().EndsWith(".csv"))
            {
                message = Ressources.FILETYPE_NOT_CSV;
                return;              
            }
            //Get Connection string
            if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                var connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.Current.User.Identity.Name);

                if (!String.IsNullOrEmpty(connectionName))
                {
                    entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);
                    if (string.IsNullOrEmpty(entityConnectionString))
                    {
                        message = Ressources.HYDROSERVER_USERLOOKUP_FAILED;
                        return;
                    }
                }
                else
                {
                    message = Ressources.HYDROSERVER_USERLOOKUP_FAILED;
                     
                    return;

                }
            }
            else
            {
                message = Ressources.HYDROSERVER_USERLOOKUP_FAILED;                 
                return;   
                
                //entityConnectionString = Utils.GetDBConnectionStringByName("Hydroservertest2");
             
            }



            //Object T;
            try
            {
                 #region Sites
                //  
                if (viewName.ToLower() == "sites")
                {
                    List<SiteModel> values = null;

                  
                   // var siteViewModel = new SitesViewModel();
                   // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<SiteModel>();
                    var listOfCorrectRecords = new List<SiteModel>();
                    var listOfDuplicateRecords = new List<SiteModel>();
                    var listOfEditedRecords = new List<SiteModel>();
                    var listOfErrors = new List<ErrorModel>();
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<SiteModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new SitesRepository();

                        repository.AddSites(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<SiteModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);
                    
                
                } 
                #endregion
                 #region Variables
                if (viewName.ToLower() == "variables")
                {
                    List<VariablesModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<VariablesModel>();
                    var listOfCorrectRecords = new List<VariablesModel>();
                    var listOfDuplicateRecords = new List<VariablesModel>();
                    var listOfEditedRecords = new List<VariablesModel>();
                    var listOfErrors = new List<ErrorModel>();
           
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<VariablesModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new VariablesRepository();

                        repository.AddVariables(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
           
                    }

                    PutRecordsInCache<VariablesModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region OffsetTypes
                if (viewName.ToLower() == "offsettypes")
                {
                    List<OffsetTypesModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<OffsetTypesModel>();
                    var listOfCorrectRecords = new List<OffsetTypesModel>();
                    var listOfDuplicateRecords = new List<OffsetTypesModel>();
                    var listOfEditedRecords = new List<OffsetTypesModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<OffsetTypesModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new OffsetTypesRepository();

                        repository.AddOffsetTypes(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<OffsetTypesModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                }
                 #endregion
                 #region Sources
                if (viewName.ToLower() == "sources")
                {
                    List<SourcesModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<SourcesModel>();
                    var listOfCorrectRecords = new List<SourcesModel>();
                    var listOfDuplicateRecords = new List<SourcesModel>();
                    var listOfEditedRecords = new List<SourcesModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<SourcesModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new SourcesRepository();

                        repository.AddSources(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<SourcesModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                }
                 #endregion
                 #region Methods
                if (viewName.ToLower() == "methods")
                {
                    List<MethodModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<MethodModel>();
                    var listOfCorrectRecords = new List<MethodModel>();
                    var listOfDuplicateRecords = new List<MethodModel>();
                    var listOfEditedRecords = new List<MethodModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<MethodModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new MethodsRepository();

                        repository.AddMethods(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<MethodModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region LabMethods 
                if (viewName.ToLower() == "labmethods")
                {
                    List<LabMethodModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<LabMethodModel>();
                    var listOfCorrectRecords = new List<LabMethodModel>();
                    var listOfDuplicateRecords = new List<LabMethodModel>();
                    var listOfEditedRecords = new List<LabMethodModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<LabMethodModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new LabMethodsRepository();

                        repository.AddLabMethods(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<LabMethodModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region Samples
                if (viewName.ToLower() == "samples")
                {
                    List<SampleModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<SampleModel>();
                    var listOfCorrectRecords = new List<SampleModel>();
                    var listOfDuplicateRecords = new List<SampleModel>();
                    var listOfEditedRecords = new List<SampleModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<SampleModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new SamplesRepository();

                        repository.AddSamples(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<SampleModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region Qualifiers
                if (viewName.ToLower() == "qualifiers")
                {
                    List<QualifiersModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<QualifiersModel>();
                    var listOfCorrectRecords = new List<QualifiersModel>();
                    var listOfDuplicateRecords = new List<QualifiersModel>();
                    var listOfEditedRecords = new List<QualifiersModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<QualifiersModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new QualifiersRepository();

                        repository.AddQualifiers(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<QualifiersModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region QualityControlLevels
                if (viewName.ToLower() == "qualitycontrollevels")
                {
                    List<QualityControlLevelModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<QualityControlLevelModel>();
                    var listOfCorrectRecords = new List<QualityControlLevelModel>();
                    var listOfDuplicateRecords = new List<QualityControlLevelModel>();
                    var listOfEditedRecords = new List<QualityControlLevelModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<QualityControlLevelModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new QualityControlLevelsRepository();

                        repository.AddQualityControlLevel(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<QualityControlLevelModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region Datavalues
                //  
                if (viewName.ToLower() == "datavalues")
                {
                    List<DataValuesModel> values = null;


                    //var siteViewModel = new SitesViewModel();
                    var listOfIncorrectRecords = new List<DataValuesModel>();
                    var listOfCorrectRecords = new List<DataValuesModel>();
                    var listOfDuplicateRecords = new List<DataValuesModel>();
                    var listOfEditedRecords = new List<DataValuesModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<DataValuesModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new DataValuesRepository();

                        repository.AddDataValues(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                        //update Seriescatalog
                        if (listOfCorrectRecords.Count() > 0)
                        {
                            var seriesCatalogRepository = new SeriesCatalogRepository();
                            seriesCatalogRepository.deleteAll(entityConnectionString);
                            seriesCatalogRepository.UpdateSeriesCatalog(entityConnectionString);
                        }
                    }

                    PutRecordsInCache<DataValuesModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);

                }
                #endregion
                 #region GroupDescriptions
                if (viewName.ToLower() == "groupdescriptions")
                {
                    List<GroupDescriptionModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<GroupDescriptionModel>();
                    var listOfCorrectRecords = new List<GroupDescriptionModel>();
                    var listOfDuplicateRecords = new List<GroupDescriptionModel>();
                    var listOfEditedRecords = new List<GroupDescriptionModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<GroupDescriptionModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new GroupDescriptionsRepository();

                        repository.AddGroupDescriptions(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<GroupDescriptionModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region Groups
                if (viewName.ToLower() == "groups")
                {
                    List<GroupsModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<GroupsModel>();
                    var listOfCorrectRecords = new List<GroupsModel>();
                    var listOfDuplicateRecords = new List<GroupsModel>();
                    var listOfEditedRecords = new List<GroupsModel>();
                    var listOfErrors = new List<ErrorModel>();
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<GroupsModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new GroupsRepository();

                        repository.AddGroups(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<GroupsModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region DerivedFrom
                if (viewName.ToLower() == "derivedfrom")
                {
                    List<DerivedFromModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<DerivedFromModel>();
                    var listOfCorrectRecords = new List<DerivedFromModel>();
                    var listOfDuplicateRecords = new List<DerivedFromModel>();
                    var listOfEditedRecords = new List<DerivedFromModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<DerivedFromModel>(file, viewName);
                    }


                    if (values != null)
                    {
                        var repository = new DerivedFromRepository();

                        repository.AddDerivedFrom(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<DerivedFromModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
                 #region Categories
                if (viewName.ToLower() == "categories")
                {
                    List<CategoriesModel> values = null;


                    // var siteViewModel = new SitesViewModel();
                    // Type t = typeof(SiteModel);

                    var listOfIncorrectRecords = new List<CategoriesModel>();
                    var listOfCorrectRecords = new List<CategoriesModel>();
                    var listOfDuplicateRecords = new List<CategoriesModel>();
                    var listOfEditedRecords = new List<CategoriesModel>();
                    var listOfErrors = new List<ErrorModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<CategoriesModel>(file, viewName);
                    }


                    if (values != null )
                    {
                        if (values.Count() == 0) return;

                        var repository = new CategoriesRepository();

                        repository.AddCategories(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords, out listOfErrors);
                    }

                    PutRecordsInCache<CategoriesModel>(listOfIncorrectRecords, listOfCorrectRecords, listOfDuplicateRecords, listOfEditedRecords);


                } 
                 #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
            return;
        }

        private static void PutRecordsInCache<T>(List<T> listOfIncorrectRecords, List<T> listOfCorrectRecords, List<T> listOfDuplicateRecords, List<T> listOfEditedRecords)
        {
            if (Utils.IsLocalHostServer())
            {
                //var httpContext = (HttpContextWrapper)Request.Properties["MS_HttpContext"];

                
                var httpContext = new HttpContextWrapper(HttpContext.Current);

                if (httpContext.Session["listOfCorrectRecords"] == null) httpContext.Session["listOfCorrectRecords"] = listOfCorrectRecords; else httpContext.Session["listOfCorrectRecords"] = listOfCorrectRecords;
                if (httpContext.Session["listOfIncorrectRecords"] == null) httpContext.Session["listOfIncorrectRecords"] = listOfIncorrectRecords; else httpContext.Session["listOfIncorrectRecords"] = listOfIncorrectRecords;
                if (httpContext.Session["listOfEditedRecords"] == null) httpContext.Session["listOfEditedRecords"] = listOfEditedRecords; else httpContext.Session["listOfEditedRecords"] = listOfEditedRecords;
                if (httpContext.Session["listOfDuplicateRecords"] == null) httpContext.Session["listOfDuplicateRecords"] = listOfDuplicateRecords; else httpContext.Session["listOfDuplicateRecords"] = listOfDuplicateRecords;

            }
            else
            {
                DataCache cache = new DataCache("default");
                var identifier = HttpContext.Current.User.Identity.Name;
                if (cache.Get(identifier + "listOfCorrectRecords") == null) cache.Add(identifier + "listOfCorrectRecords", listOfCorrectRecords); else cache.Put(identifier + "listOfCorrectRecords", listOfCorrectRecords);
                if (cache.Get(identifier + "listOfIncorrectRecords") == null) cache.Add(identifier + "listOfIncorrectRecords", listOfIncorrectRecords); else cache.Put(identifier + "listOfIncorrectRecords", listOfIncorrectRecords);
                if (cache.Get(identifier + "listOfEditedRecords") == null) cache.Add(identifier + "listOfEditedRecords", listOfEditedRecords); else cache.Put(identifier + "listOfEditedRecords", listOfEditedRecords);
                if (cache.Get(identifier + "listOfDuplicateRecords") == null) cache.Add(identifier + "listOfDuplicateRecords", listOfDuplicateRecords); else cache.Put(identifier + "listOfDuplicateRecords", listOfDuplicateRecords);

            }
        }

        private static List<T> parseCSV<T>(HttpPostedFile file, string viewName)
        {
            var s = new List<T>();
            var ms = new MemoryStream();
            StreamReader reader = null;
            var outFolder = "";

            if (file.FileName.ToLower().EndsWith(".zip"))
            {

                file.InputStream.Position = 0;

                ZipInputStream zipInputStream = new ZipInputStream(file.InputStream);
                ZipEntry zipEntry = zipInputStream.GetNextEntry();
                while (zipEntry != null)
                {
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    MemoryStream streamWriter = new MemoryStream();

                    StreamUtils.Copy(zipInputStream, ms, buffer);

                    zipEntry = zipInputStream.GetNextEntry();

                }
                ms.Position = 0;
                reader = new StreamReader(ms, Encoding.GetEncoding("iso-8859-1"));

            }
            else
            {
                reader = new StreamReader(file.InputStream, Encoding.GetEncoding("iso-8859-1"));
            }

            

            var csvReader = new CsvHelper.CsvReader(reader);


            //while (csvReader.Read())
            //{
            //var intField = csvReader.GetField<int>(0);
            csvReader.Configuration.IsHeaderCaseSensitive = false;
            csvReader.Configuration.WillThrowOnMissingField = false;
            csvReader.Configuration.SkipEmptyRecords = true;

            //while (csvReader.Read())
            //{
               
            //    break;
            //}
            
            try
            {
                s = csvReader.GetRecords<T>().ToList();

                var missingMandatoryFields = Utils.ValidateFields<T>(csvReader.FieldHeaders.ToList());
                if (missingMandatoryFields.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in missingMandatoryFields)
                    {
                        sb.Append(item);
                        sb.Append(",");
                    }
                    var f = sb.ToString().TrimEnd(',');
                    throw new System.ArgumentException(String.Format(Ressources.IMPORT_FAILED_MISSINGMANDATORYFIELDS, f));
                }
                
            }
            catch (CsvMissingFieldException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //clean up ressources
                ms.Close();
                reader.Close();
            }


            return s;
        }
    }
}
