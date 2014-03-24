using CsvHelper;
using HydroServerTools.Helper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using jQuery.DataTables.Mvc;
using Microsoft.ApplicationServer.Caching;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HydroServerTools.Controllers
{
    //[Authorize]
    public class CSVUploadController : Controller
    {

         //
        // GET: /CSVUpload/
        public ActionResult Index(ExternalLoginConfirmationViewModel model)
        {
            
            return View();
        }
        public ActionResult TestConnection(ConnectionModel model)
        {

            if (ModelState.IsValid)
            {

                var connectionString = Helper.Utils.BuildEFConnnectionString(model);

                //Get Connection
                var conn = new EntityConnection(connectionString);
                                
                //Initialize vars
                string message = string.Empty;
                string status = "failed";
                //test connectionstring
                try
                {
                    conn.Open();
                    message = HISRessources.CONNECTION_SUCCESS;
                    status = "success";
                   // conn.Close();
                }

                catch (EntityException ex)
                {
                    if (ex.InnerException.Message.StartsWith("A network-related"))
                        message = HISRessources.CONNECTION_FAILED_SERVERNAME;
                    else if (ex.InnerException.Message.StartsWith("Cannot open database"))
                        message = HISRessources.CONNECTION_FAILED_DATASOURCENAME;
                    else
                    {
                        message = HISRessources.CONNECTION_FAILED_LOGIN;
                    }

                }
                catch (Exception ex)
                {
                    model.Message = "An error occured. Please notify the technical team.";
                }
                finally
                {
                    model.Message = message;
                    model.Status = status;
                    conn.Close();
                      
                }


                
            }
            //ViewBag.Message = "tes";

            return View("Index", model);
        }

        //public ActionResult UploadData()
        //{
        //    return View("UploadData");
        //}

        [HttpGet]
        public ActionResult Sites()
        {
            var model = new SiteModel();
            return View("Sites", model);
        }
        [HttpGet]
        public ActionResult Variables()
        {
            var model = new VariablesModel();
            return View("Variables", model);
        }
        [HttpGet]
        public ActionResult OffsetTypes()
        {
            var model = new OffsetTypesModel();
            return View("OffsetTypes", model);
        }
        [HttpGet]
        public ActionResult ISOMetadata()
        {
            return View("ISOMetadata");
        }
        [HttpGet]
        public ActionResult Sources()
        {
            var model = new SourcesModel();
            return View("Sources", model);
        }
        [HttpGet]
        public ActionResult Methods()
        {
            var model = new MethodModel();
            return View("Methods", model);
        }
        [HttpGet]
        public ActionResult LabMethods()
        {
            var model = new LabMethodModel();
            return View("LabMethods", model);
        }
        [HttpGet]
        public ActionResult Samples()
        {
            var model = new SampleModel();
            return View("Samples", model);
        }
        [HttpGet]
        public ActionResult Qualifiers()
        {
            var model = new QualifiersModel();
            return View("Qualifiers", model);
        }
        [HttpGet]
        public ActionResult QualityControlLevels()
        {
            var model = new QualityControlLevelModel();
            return View("QualityControlLevels", model);
        }
        [HttpGet]
        public ActionResult DataValues()
        {
            var model = new DataValuesModel();
            return View("DataValues", model);
        }
        [HttpGet]
        public ActionResult GroupDescriptions()
        {
            var model = new GroupDescriptionModel();
            return View("GroupDescriptions", model);
        }
        [HttpGet]
        public ActionResult Groups()
        {
            var model = new GroupsModel();
            return View("Groups", model);
        }
        [HttpGet]
        public ActionResult DerivedFrom()
        {
            var model = new DerivedFromModel();
            return View("DerivedFrom", model);
        }
        [HttpGet]
        public ActionResult Categories()
        {
            var model = new CategoriesModel();
            return View("Categories", model);
        }
        [HttpGet]

         public ActionResult UploadData(string id) 
         {
             ViewBag.name = id;
             return View();
         }
        [HttpGet]
         public ContentResult _Breadcrumb(string id)
         {

             string[] vars = new string[14] { "Sites", "Variables", "OffsetTypes", "Sources", "Methods", "LabMethods", "Samples", "Qualifiers", "QualityControlLevels", "DataValues", "GroupDescriptions", "Groups", "DerivedFrom", "Categories" };//"ISOMetadata",

             StringBuilder html = new StringBuilder();
             html.Append("<div  class='container'>");
             html.Append("<ol class='breadcrumb h6'>");
             foreach (var item in vars)
             {
                 if (id.ToString().ToLower() == item.ToLower()) html.Append("<li class='active h5'><strong>" + item + "</strong></li>");

                 else
                 {
                     html.Append("<li>");
                     html.Append("<a href ='/CSVUpload/UploadData/");
                     html.Append(item);
                     html.Append("' >");
                     html.Append(item);
                     html.Append("</a></li>");
                 }
             }
             html.Append("</ol>");
             html.Append("</div>");
             return Content(html.ToString());
         }

        [HttpGet]
         public ContentResult _Pager(string name)
         {
             string[] vars = new string[14] { "Sites", "Variables", "OffsetTypes", "Sources", "Methods", "LabMethods", "Samples", "Qualifiers", "QualityControlLevels", "DataValues", "GroupDescriptions", "Groups", "DerivedFrom", "Categories" };//"ISOMetadata",

             StringBuilder html = new StringBuilder();
             //<li>@Html.ActionLink("Import Sites", "UploadData", "CSVUpload", new { id = "Sites" }, null)</li>
             //var index = Array.FindIndex(vars, r => r.Contains(name));
             int index = Array.IndexOf(vars, vars.Where(x => x.Contains(name)).FirstOrDefault());
             //int index = vars.Select((v, i) => new { Index = i, Value = v })
             //        .Where(p => p.Value == name)
             //        .Select(p => p.Index).FirstOrDefault();
             html.Append("<div class='container'>");
             html.Append("<ul class='pager'>");
             if (index > 0)
             {
                 html.Append("<li class='previous'>");
                 html.Append("<a href=/CSVUpload/UploadData/");
                 html.Append(vars[index - 1]);
                 html.Append(">&larr; Previous</a></li>");
             }
             if (index < vars.Length-1)
             {
                 html.Append("<li class='next'>");
                 html.Append("<a href=/CSVUpload/UploadData/");
                 html.Append(vars[index + 1]);
                 html.Append(">Next &rarr;</a></li>");
                 html.Append("</ul>");
             }
             html.Append("</div>");
             return Content(html.ToString());
         }

        //[HttpPost]
        //public ActionResult Import(HttpPostedFileBase file, FormCollection collection)//
        //{
        //    //string userName = HttpContext.User.Identity.Name;//  "martin.seul@yahoo.com";
        //    //var Db = new ApplicationDbContext();
        //    //var userEmail = Db.Users.First(u => u.UserName == userName).UserEmail

        //    //file = Request.Files[0];
        //    string viewName = collection["viewname"];
            
        //    if (!file.FileName.ToLower().EndsWith(".csv"))
        //    {
        //        ModelState.AddModelError(String.Empty, Ressources.FILETYPE_NOT_CSV);
        //        return View(viewName);              
        //    }
        //    //Get Connection string
        //    var connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name);
        //    var entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);
            
        //    if (String.IsNullOrEmpty(connectionName))
        //    { 
        //        ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);
        //        return View(viewName);
        //    }
        //    //Object T;
        //    try
        //    {
        //        #region Sites
        //        //  
        //        if (viewName.ToLower() == "sites")
        //        {
        //            List<SiteModel> values = null;

                  
        //            var siteViewModel = new SitesViewModel();
        //            var listOfIncorrectRecords = new List<SiteModel>();
        //            var listOfCorrectRecords = new List<SiteModel>();
        //            var listOfDuplicateRecords = new List<SiteModel>();
        //            var listOfEditedRecords = new List<SiteModel>();

        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<SiteModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var sitesRepository = new SitesRepository();

        //                sitesRepository.AddSites(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            siteViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            siteViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            siteViewModel.listOfEditedRecords = listOfEditedRecords;
        //            siteViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
        //            return RedirectToAction("Sites", siteViewModel);
        //            //return View(viewName, siteViewModel);
        //        } 
        //        #endregion
                
        //        #region Variables
        //        if (viewName.ToLower() == "variables")
        //        {
        //            List<VariablesModel> values = null;

        //            var variablesViewModel = new VariablesViewModel();
        //            var listOfIncorrectRecords = new List<VariablesModel>();
        //            var listOfCorrectRecords = new List<VariablesModel>();
        //            var listOfDuplicateRecords = new List<VariablesModel>();
        //            var listOfEditedRecords = new List<VariablesModel>();
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<VariablesModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new VariablesRepository();

        //                repository.AddVariables(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();

        //            variablesViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            variablesViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            variablesViewModel.listOfEditedRecords = listOfEditedRecords;
        //            variablesViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            // redirect back to the index action to show the form once again
        //            return View(viewName, variablesViewModel);
        //        } 
        //        #endregion

        //        #region ISOMetadata
                  
        //        if (viewName.ToLower() == "isometadata")
        //        {
        //            List<ISOMetadataModel> values = null;
        //            var listOfIncorrectRecords = new List<ISOMetadataModel>();
        //            var listOfCorrectRecords = new List<ISOMetadataModel>();
        //            var listOfDuplicateRecords = new List<ISOMetadataModel>();
        //            var listOfEditedRecords = new List<ISOMetadataModel>();

        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<ISOMetadataModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new ISOMetadataRepository();

        //                repository.AddISOMetadata(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again
        //            return View(viewName, listOfIncorrectRecords);
        //        } 
        //        #endregion

        //        #region OffsetTypes

        //        if (viewName.ToLower() == "offsettypes")
        //        {
        //            List<OffsetTypesModel> values = null;
        //            var offsetTypesViewModel = new OffsetTypesViewModel();
        //            var listOfIncorrectRecords = new List<OffsetTypesModel>();
        //            var listOfCorrectRecords = new List<OffsetTypesModel>();
        //            var listOfDuplicateRecords = new List<OffsetTypesModel>();
        //            var listOfEditedRecords = new List<OffsetTypesModel>();
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<OffsetTypesModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new OffsetTypesRepository();

        //                repository.AddOffsetTypes(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            offsetTypesViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            offsetTypesViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            offsetTypesViewModel.listOfEditedRecords = listOfEditedRecords;
        //            offsetTypesViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            return View(viewName, offsetTypesViewModel);
        //        }
                
        //        #endregion   
                 
        //        #region Sources
        //        if (viewName.ToLower() == "sources")
        //        {
        //            List<SourcesModel> values = null;
        //            var sourcesModel = new SourcesViewModel();

        //            var listOfIncorrectRecords = new List<SourcesModel>();
        //            var listOfCorrectRecords = new List<SourcesModel>();
        //            var listOfDuplicateRecords = new List<SourcesModel>();
        //            var listOfEditedRecords = new List<SourcesModel>();
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<SourcesModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new SourcesRepository();

        //                repository.AddSources(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();



        //            sourcesModel.listOfCorrectRecords = listOfCorrectRecords;
        //            sourcesModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            sourcesModel.listOfEditedRecords = listOfEditedRecords;
        //            sourcesModel.listOfDuplicateRecords = listOfDuplicateRecords;


        //            // redirect back to the index action to show the form once again
        //            return View(viewName, sourcesModel);
        //        }
                
        //        #endregion

        //        #region  Methods
        //        if (viewName.ToLower() == "methods")
        //        {
        //            List<MethodModel> values = null;
        //            var methodsViewModel = new MethodsViewModel();
        //            var listOfIncorrectRecords = new List<MethodModel>();
        //            var listOfCorrectRecords = new List<MethodModel>();
        //            var listOfDuplicateRecords = new List<MethodModel>();
        //            var listOfEditedRecords = new List<MethodModel>();
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<MethodModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new MethodsRepository();

        //                repository.AddMethods(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();

        //            methodsViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            methodsViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            methodsViewModel.listOfEditedRecords = listOfEditedRecords;
        //            methodsViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            // redirect back to the index action to show the form once again
        //            return View(viewName, methodsViewModel);
        //        } 
        //        #endregion

        //        #region LabMethods
        //        if (viewName.ToLower() == "labmethods")
        //        {
        //            List<LabMethodModel> values = null;
        //            var labMethodsViewModel = new LabMethodsViewModel(); 
        //            var listOfIncorrectRecords = new List<LabMethodModel>();
        //            var listOfCorrectRecords = new List<LabMethodModel>();
        //            var listOfDuplicateRecords = new List<LabMethodModel>();
        //            var listOfEditedRecords = new List<LabMethodModel>();
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<LabMethodModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new LabMethodsRepository();

        //                repository.AddLabMethods(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();

        //            labMethodsViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            labMethodsViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            labMethodsViewModel.listOfEditedRecords = listOfEditedRecords;
        //            labMethodsViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            return View(viewName, labMethodsViewModel);
        //        } 
        //        #endregion

        //        #region Samples
        //        if (viewName.ToLower() == "samples")
        //        {
        //            List<SampleModel> values = null;
        //            var sampleViewModel = new SamplesViewModel();
        //            var listOfIncorrectRecords = new List<SampleModel>();
        //            var listOfCorrectRecords = new List<SampleModel>();
        //            var listOfDuplicateRecords = new List<SampleModel>();
        //            var listOfEditedRecords = new List<SampleModel>();
                  
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                values = parseCSV<SampleModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new SamplesRepository();

        //                repository.AddSamples(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            sampleViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            sampleViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            sampleViewModel.listOfEditedRecords = listOfEditedRecords;
        //            sampleViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            return View(viewName, sampleViewModel);
        //        } 
        //        #endregion

        //        #region Qualifiers
        //        if (viewName.ToLower() == "qualifiers")
        //        {
        //            List<QualifiersModel> values = null;
        //            var qualifiersViewModel = new QualifiersViewModel();
        //            var listOfIncorrectRecords = new List<QualifiersModel>();
        //            var listOfCorrectRecords = new List<QualifiersModel>();
        //            var listOfDuplicateRecords = new List<QualifiersModel>();
        //            var listOfEditedRecords = new List<QualifiersModel>();
                  
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<QualifiersModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new QualifiersRepository();

        //                repository.AddQualifiers(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            qualifiersViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            qualifiersViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            qualifiersViewModel.listOfEditedRecords = listOfEditedRecords;
        //            qualifiersViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
        //            return View(viewName, qualifiersViewModel);
        //        } 
        //        #endregion

        //        #region QualityControlLevels
        //        if (viewName.ToLower() == "qualitycontrollevels")
        //        {
        //            List<QualityControlLevelModel> values = null;
        //            var qualityControlLevelViewModel = new QualityControlLevelsViewModel();
        //            var listOfIncorrectRecords = new List<QualityControlLevelModel>();
        //            var listOfCorrectRecords = new List<QualityControlLevelModel>();
        //            var listOfDuplicateRecords = new List<QualityControlLevelModel>();
        //            var listOfEditedRecords = new List<QualityControlLevelModel>();
                 
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<QualityControlLevelModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new QualityControlLevelsRepository();

        //                repository.AddQualityControlLevel(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            qualityControlLevelViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            qualityControlLevelViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            qualityControlLevelViewModel.listOfEditedRecords = listOfEditedRecords;
        //            qualityControlLevelViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            return View(viewName, qualityControlLevelViewModel);
        //        } 
        //        #endregion

        //        #region DataValues
        //        if (viewName.ToLower() == "datavalues")
        //        {
        //            List<DataValuesModel> values = null;

        //            var dataValuesViewModel = new DataValuesViewModel();
        //            var listOfIncorrectRecords = new List<DataValuesModel>();
        //            var listOfCorrectRecords = new List<DataValuesModel>();
        //            var listOfDuplicateRecords = new List<DataValuesModel>();
        //            var listOfEditedRecords = new List<DataValuesModel>();
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<DataValuesModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new DataValuesRepository();

        //                repository.AddDataValues(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();

        //            dataValuesViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            dataValuesViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            dataValuesViewModel.listOfEditedRecords = listOfEditedRecords;
        //            dataValuesViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            // redirect back to the index action to show the form once again
        //            return View(viewName, dataValuesViewModel);
        //        } 
        //        #endregion

        //        #region GroupDescriptions
        //        if (viewName.ToLower() == "groupdescriptions")
        //        {
        //            List<GroupDescriptionModel> values = null;
        //            var groupDescriptionsViewModel = new GroupDescriptionsViewModel();
        //            var listOfIncorrectRecords = new List<GroupDescriptionModel>();
        //            var listOfCorrectRecords = new List<GroupDescriptionModel>();
        //            var listOfDuplicateRecords = new List<GroupDescriptionModel>();
        //            var listOfEditedRecords = new List<GroupDescriptionModel>();
                  
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<GroupDescriptionModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new GroupDescriptionsRepository();

        //                repository.AddGroupDescriptions(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();


        //            // redirect back to the index action to show the form once again

        //            groupDescriptionsViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            groupDescriptionsViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            groupDescriptionsViewModel.listOfEditedRecords = listOfEditedRecords;
        //            groupDescriptionsViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
        //            return View(viewName, groupDescriptionsViewModel);
        //        }
                
        //        #endregion 

        //        #region Groups
        //        if (viewName.ToLower() == "groups")
        //        {
        //            List<GroupsModel> values = null;
        //            var groupsViewModel = new GroupsViewModel();
        //            var listOfIncorrectRecords = new List<GroupsModel>();
        //            var listOfCorrectRecords = new List<GroupsModel>();
        //            var listOfDuplicateRecords = new List<GroupsModel>();
        //            var listOfEditedRecords = new List<GroupsModel>();
                  
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<GroupsModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new GroupsRepository();

        //                repository.AddGroups(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            groupsViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            groupsViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            groupsViewModel.listOfEditedRecords = listOfEditedRecords;
        //            groupsViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
        //            return View(viewName, groupsViewModel);
        //        } 
        //        #endregion

        //        #region DerivedFrom
        //        if (viewName.ToLower() == "derivedfrom")
        //        {
        //            List<DerivedFromModel> values = null;
        //            var derivedFromViewModel = new DerivedFromViewModel();
        //            var listOfIncorrectRecords = new List<DerivedFromModel>();
        //            var listOfCorrectRecords = new List<DerivedFromModel>();
        //            var listOfDuplicateRecords = new List<DerivedFromModel>();
        //            var listOfEditedRecords = new List<DerivedFromModel>();
                  
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<DerivedFromModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new DerivedFromRepository();

        //                repository.AddDerivedFrom(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            derivedFromViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            derivedFromViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            derivedFromViewModel.listOfEditedRecords = listOfEditedRecords;
        //            derivedFromViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            return View(viewName, derivedFromViewModel);
        //        } 
        //        #endregion

        //        #region Categories
        //        if (viewName.ToLower() == "categories")
        //        {
        //            List<CategoriesModel> values = null;
        //            var categoriesViewModel = new CategoriesViewModel();
        //            var listOfIncorrectRecords = new List<CategoriesModel>();
        //            var listOfCorrectRecords = new List<CategoriesModel>();
        //            var listOfDuplicateRecords = new List<CategoriesModel>();
        //            var listOfEditedRecords = new List<CategoriesModel>();
                  
        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<CategoriesModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var repository = new CategoriesRepository();

        //                repository.AddCategories(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            categoriesViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            categoriesViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            categoriesViewModel.listOfEditedRecords = listOfEditedRecords;
        //            categoriesViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

        //            return View(viewName, categoriesViewModel);
        //        } 
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(String.Empty, Ressources.IMPORT_FAILED );//+ ": " + ex.Message
        //    }
        //    return View(viewName);
        //}

        //[HttpPost]
        //public virtual ActionResult Import2(HttpPostedFileBase file, FormCollection collection)//
        //{
        //    //string userName = HttpContext.User.Identity.Name;//  "martin.seul@yahoo.com";
        //    //var Db = new ApplicationDbContext();
        //    //var userEmail = Db.Users.First(u => u.UserName == userName).UserEmail

        //    //file = Request.Files[0];
        //    string viewName = collection["viewname"];

        //    if (!file.FileName.ToLower().EndsWith(".csv"))
        //    {
        //        ModelState.AddModelError(String.Empty, Ressources.FILETYPE_NOT_CSV);
        //        return View(viewName);
        //    }
        //    //Get Connection string
        //    var connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name);
        //    var entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);

        //    if (String.IsNullOrEmpty(connectionName))
        //    {
        //        ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);
        //        return View(viewName);
        //    }
        //    //Object T;
        //    try
        //    {
        //        #region Sites
        //        //  
        //        if (viewName.ToLower() == "sites")
        //        {
        //            List<SiteModel> values = null;


        //            //var siteViewModel = new SitesViewModel();
        //            var listOfIncorrectRecords = new List<SiteModel>();
        //            var listOfCorrectRecords = new List<SiteModel>();
        //            var listOfDuplicateRecords = new List<SiteModel>();
        //            var listOfEditedRecords = new List<SiteModel>();

        //            // Verify that the user selected a file
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                values = parseCSV<SiteModel>(file);
        //            }


        //            if (values != null)
        //            {
        //                var sitesRepository = new SitesRepository();

        //                sitesRepository.AddSites(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
        //            }

        //            //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

        //            @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
        //            @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
        //            // redirect back to the index action to show the form once again

        //            //siteViewModel.listOfCorrectRecords = listOfCorrectRecords;
        //            //siteViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
        //            //siteViewModel.listOfEditedRecords = listOfEditedRecords;
        //            //siteViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
        //            //return RedirectToAction("Sites", siteViewModel);
        //            //return View(viewName, siteViewModel);
        //            bool isUploaded = false;
        //            string message = "File upload succeeded";

        //            return Json(new { isUploaded = isUploaded, message = message }, "text/html");
        //        }
        //        #endregion


        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(String.Empty, Ressources.IMPORT_FAILED);//+ ": " + ex.Message
        //    }
        //    return View(viewName);
        //}

        private static List<T> parseCSV<T>(HttpPostedFileBase file)
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

            //while (csvReader.Read())
            //{
            try
            {
                s = csvReader.GetRecords<T>().ToList();
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
                if (reader != null) reader.Close();
            }


            return s;
        }

        [HttpPost]
        public JsonResult Sites(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<SiteModel>();
            List<SiteModel> items = new List<SiteModel>();

            listOfRecords = (List<SiteModel>)Utils.GetRecordsFromCache<SiteModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
               //filter results only for text columns


               if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                       (c.SiteCode != null && c.SiteCode.ToLower().Contains(searchString))
                                    || (c.SiteName != null && c.SiteName.ToLower().Contains(searchString))
                                    || (c.Latitude != null && c.Latitude.Contains(searchString))
                                    || (c.Longitude != null && c.Longitude.Contains(searchString))
                                    || (c.LatLongDatumSRSName != null && c.LatLongDatumSRSName.ToLower().Contains(searchString))
                                    || (c.Elevation_m != null && c.Elevation_m.Contains(searchString))
                                    || (c.LocalX != null && c.LocalX.Contains(searchString))
                                    || (c.LocalY != null && c.LocalY.Contains(searchString))
                                           //|| c.LocalProjectionID.Contains(searchString)                                   
                                    || (c.PosAccuracy_m != null && c.PosAccuracy_m.Contains(searchString))
                                    || (c.State != null && c.State.ToLower().Contains(searchString))
                                    || (c.County != null && c.County.ToLower().Contains(searchString))
                                    || (c.Comments != null && c.Comments.ToLower().Contains(searchString))
                                    || (c.SiteType != null && c.SiteType.ToLower().Contains(searchString))
                        );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SiteName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SiteName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Latitude).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Latitude).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "3":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Longitude).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Longitude).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "4":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LatLongDatumSRSName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LatLongDatumSRSName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "5":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Elevation_m).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Elevation_m).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "6":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.VerticalDatum).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.VerticalDatum).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "7":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LocalX).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LocalX).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "8":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LocalY).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LocalY).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "9":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LocalProjectionSRSName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LocalProjectionSRSName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "10":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.PosAccuracy_m).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.PosAccuracy_m).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "11":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "12":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.County).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.County).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "13":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Comments).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Comments).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "14":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SiteType).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SiteType).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }
            
          
            

            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                         select new[] { 
                    c.SiteCode,
                    c.SiteName,  
                    c.Latitude,  
                    c.Longitude,  
                    c.LatLongDatumSRSName,  
                    c.Elevation_m,  
                    c.VerticalDatum,  
                    c.LocalX,  
                    c.LocalY,  
                    c.LocalProjectionSRSName,  
                    c.PosAccuracy_m,  
                    c.State,  
                    c.County,  
                    c.Comments,  
                    c.SiteType
                };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult Variables(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<VariablesModel>();
            List<VariablesModel> items = new List<VariablesModel>();

            listOfRecords = (List<VariablesModel>)Utils.GetRecordsFromCache<VariablesModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                       //c.VariableID != null && c.VariableID.ToString().Contains(searchString.ToLower())
                                      c.VariableCode != null && c.VariableCode.Contains(searchString.ToLower())
                                    || c.VariableName != null && c.VariableName.ToLower().Contains(searchString.ToLower())
                                    || c.Speciation != null && c.Speciation.Contains(searchString.ToLower())
                                    || c.VariableUnitsName != null && c.VariableUnitsName.ToString().Contains(searchString.ToLower())
                                    || c.SampleMedium != null && c.SampleMedium.ToLower().Contains(searchString.ToLower())
                                    || c.ValueType != null && c.ValueType.ToLower().Contains(searchString.ToLower())
                                    || c.IsRegular != null && c.IsRegular.ToString().ToLower().Contains(searchString.ToLower())
                                    || c.TimeSupport != null && c.TimeSupport.ToString().ToLower().Contains(searchString.ToLower())
                                    || c.TimeUnitsName != null && c.TimeUnitsName.ToLower().Contains(searchString.ToLower())
                                    || c.DataType != null && c.DataType.ToLower().Contains(searchString.ToLower())
                                    || c.GeneralCategory != null && c.GeneralCategory.ToLower().Contains(searchString.ToLower())
                                    || c.NoDataValue != null && c.NoDataValue.ToString().ToLower().Contains(searchString.ToLower())
                        );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.VariableName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.VariableName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Speciation).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Speciation).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "3":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.VariableUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.VariableUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "4":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SampleMedium).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SampleMedium).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "5":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.ValueType).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.ValueType).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "6":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.IsRegular).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.IsRegular).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "7":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.TimeSupport).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.TimeSupport).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "8":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.TimeUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.TimeUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "9":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.DataType).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.DataType).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "10":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.GeneralCategory).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.GeneralCategory).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "11":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.NoDataValue).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.NoDataValue).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                            //c.VariableID,
                            c.VariableCode,
                            c.VariableName,
                            c.Speciation,
                            c.VariableUnitsName,
                            c.SampleMedium,
                            c.ValueType,
                            c.IsRegular,
                            c.TimeSupport,
                            c.TimeUnitsName,
                            c.DataType,
                            c.GeneralCategory,
                            c.NoDataValue
                };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult OffsetTypes(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<OffsetTypesModel>();
            List<OffsetTypesModel> items = new List<OffsetTypesModel>();

            listOfRecords = (List<OffsetTypesModel>)Utils.GetRecordsFromCache<OffsetTypesModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                      c.OffsetTypeID != null && c.OffsetTypeID.ToString().ToLower().Contains(searchString.ToLower())
                                    || c.OffsetUnitsName != null && c.OffsetUnitsName.ToLower().Contains(searchString.ToLower())
                                    || c.OffsetDescription != null && c.OffsetDescription.ToLower().Contains(searchString.ToLower())
                        );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.OffsetTypeID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.OffsetTypeID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.OffsetUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.OffsetUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.OffsetDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.OffsetDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.OffsetTypeID).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                        c.OffsetTypeID,
                                        c.OffsetUnitsName,
                                        c.OffsetDescription
                                    };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult Sources(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<SourcesModel>();
            List<SourcesModel> items = new List<SourcesModel>();

            listOfRecords = (List<SourcesModel>)Utils.GetRecordsFromCache<SourcesModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                       c.SourceID != null && c.SourceID.ToString().ToLower().Contains(searchString.ToLower())
                                    || c.Organization != null && c.Organization.ToLower().Contains(searchString.ToLower())
                                    || c.SourceDescription != null && c.SourceDescription.ToLower().Contains(searchString.ToLower())
                                    || c.SourceLink != null && c.SourceLink.ToLower().Contains(searchString.ToLower())
                                    || c.ContactName != null && c.ContactName.ToLower().Contains(searchString.ToLower())
                                    || c.Phone != null && c.Phone.ToLower().Contains(searchString.ToLower())
                                    || c.Email != null && c.Email.ToLower().Contains(searchString.ToLower())
                                    || c.Address != null && c.Address.ToLower().Contains(searchString.ToLower())
                                    || c.City != null && c.City.ToLower().Contains(searchString.ToLower())
                                    || c.State != null && c.State.ToLower().Contains(searchString.ToLower())
                                    || c.ZipCode != null && c.ZipCode.ToLower().Contains(searchString.ToLower())
                                    || c.Citation != null && c.Citation.ToLower().Contains(searchString.ToLower())
                                    || c.TopicCategory != null && c.TopicCategory.ToLower().Contains(searchString.ToLower())
                                    || c.Title != null && c.Title.ToLower().Contains(searchString.ToLower())
                                    || c.Abstract != null && c.Abstract.ToLower().Contains(searchString.ToLower())
                                    || c.ProfileVersion != null && c.ProfileVersion.ToLower().Contains(searchString.ToLower())
                                    || c.MetadataLink != null && c.MetadataLink.ToLower().Contains(searchString.ToLower())
                        );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SourceID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SourceID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Organization).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Organization).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SourceDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SourceDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "3":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SourceLink).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SourceLink).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "4":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.ContactName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.ContactName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "5":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Phone).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Phone).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "6":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Email).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Email).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "7":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Address).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Address).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "8":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.City).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.City).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "9":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "10":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.ZipCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.ZipCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "11":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "12":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Citation).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Citation).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "13":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Title).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Title).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "14":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Abstract).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Abstract).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "15":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.ProfileVersion).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.ProfileVersion).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "16":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.MetadataLink).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.MetadataLink).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.SourceID).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }


            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                        c.SourceID,
                                        c.Organization,
                                        c.SourceDescription,
                                        c.SourceLink,
                                        c.ContactName,
                                        c.Phone,
                                        c.Email,
                                        c.Address,
                                        c.City,
                                        c.State,
                                        c.ZipCode,
                                        c.Citation,
                                        c.TopicCategory,
                                        c.Title,
                                        c.Abstract,
                                        c.ProfileVersion,
                                        c.MetadataLink
                                    };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult Methods(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<MethodModel>();
            List<MethodModel> items = new List<MethodModel>();

            listOfRecords = (List<MethodModel>)Utils.GetRecordsFromCache<MethodModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                       c.MethodID != null && c.MethodID.ToString().ToLower().Contains(searchString.ToLower())
                                    || c.MethodDescription != null && c.MethodDescription.ToLower().Contains(searchString.ToLower())
                                    || c.MethodLink != null && c.MethodLink.ToLower().Contains(searchString.ToLower())
                                );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.MethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.MethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.MethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.MethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;

                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                            c.MethodID,
                            c.MethodDescription,
                            c.MethodLink
                };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult LabMethods(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<LabMethodModel>();
            List<LabMethodModel> items = new List<LabMethodModel>();

            listOfRecords = (List<LabMethodModel>)Utils.GetRecordsFromCache<LabMethodModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                           c.LabMethodID != null && c.LabMethodID.ToString().ToLower().Contains(searchString.ToLower())
                                        || c.LabName != null && c.LabName.ToLower().Contains(searchString.ToLower())
                                        || c.LabOrganization != null && c.LabOrganization.ToLower().Contains(searchString.ToLower())
                                        || c.LabMethodName != null && c.LabMethodName.ToLower().Contains(searchString.ToLower())
                                        || c.LabMethodDescription != null && c.LabMethodDescription.ToLower().Contains(searchString.ToLower())
                                        || c.LabMethodLink != null && c.LabMethodLink.ToLower().Contains(searchString.ToLower())
                                    );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LabMethodID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LabMethodID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LabName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LabName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LabOrganization).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LabOrganization).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "3":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "4":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LabMethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LabMethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "5":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LabMethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LabMethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                               
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.LabMethodID).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                        c.LabMethodID,
                                        c.LabName,
                                        c.LabOrganization,
                                        c.LabMethodName,
                                        c.LabMethodDescription,
                                        c.LabMethodLink
                                     };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult Samples(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<SampleModel>();
            List<SampleModel> items = new List<SampleModel>();

            listOfRecords = (List<SampleModel>)Utils.GetRecordsFromCache<SampleModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                           c.SampleID != null && c.SampleID.ToString().ToLower().Contains(searchString.ToLower())
                                        || c.SampleType != null && c.SampleType.ToLower().Contains(searchString.ToLower())
                                        || c.LabSampleCode != null && c.LabSampleCode.ToLower().Contains(searchString.ToLower())
                                        || c.LabMethodName != null && c.LabMethodName.Contains(searchString.ToLower())
                                );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SampleID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SampleID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.SampleType).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.SampleType).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LabSampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LabSampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "3":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                               
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.SampleID).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                        c.SampleID,
                                        c.SampleType,
                                        c.LabSampleCode,
                                        c.LabMethodName
                                    };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult Qualifiers(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<QualifiersModel>();
            List<QualifiersModel> items = new List<QualifiersModel>();

            listOfRecords = (List<QualifiersModel>)Utils.GetRecordsFromCache<QualifiersModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                       c.QualifierID != null && c.QualifierID.ToString().ToLower().Contains(searchString.ToLower())
                                    || c.QualifierCode != null && c.QualifierCode.ToLower().Contains(searchString.ToLower())
                                    || c.QualifierDescription != null && c.QualifierDescription.ToLower().Contains(searchString.ToLower())
                                );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.QualifierID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.QualifierID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.QualifierDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.QualifierDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;

                              
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                        c.QualifierID,
                                        c.QualifierCode,
                                        c.QualifierDescription
                                     };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult QualityControlLevels(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<QualityControlLevelModel>();
            List<QualityControlLevelModel> items = new List<QualityControlLevelModel>();

            listOfRecords = (List<QualityControlLevelModel>)Utils.GetRecordsFromCache<QualityControlLevelModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                        c.QualityControlLevelCode != null && c.QualityControlLevelCode.ToLower().Contains(searchString.ToLower())
                                    || c.Definition != null && c.Definition.ToLower().Contains(searchString.ToLower())
                                    || c.Explanation != null && c.Explanation.ToLower().Contains(searchString.ToLower())
                                    );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Definition).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Definition).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.Explanation).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.Explanation).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                               
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                            c.QualityControlLevelCode,
                            c.Definition,
                            c.Explanation
                };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult Datavalues(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var listOfRecords = new List<DataValuesModel>();
            var items = new List<DataValuesModel>();

            listOfRecords = Utils.GetRecordsFromCache<DataValuesModel>(id, "default");
            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = (List<DataValuesModel>)listOfRecords.
                            FindAll(c =>
                                        (c.CensorCode != null && c.CensorCode.ToLower().Contains(searchString))
                                    || (c.DataValue != null && c.DataValue.ToLower().Contains(searchString))
                                    || (c.DateTimeUTC != null && c.DateTimeUTC.Contains(searchString))
                                    || (c.DerivedFromID != null && c.DerivedFromID.Contains(searchString))
                                    || (c.LocalDateTime != null && c.LocalDateTime.ToLower().Contains(searchString))
                                    || (c.MethodDescription != null && c.MethodDescription.Contains(searchString))
                                    || (c.OffsetTypeID != null && c.OffsetTypeID.Contains(searchString))
                                    || (c.QualifierID != null && c.QualifierID.Contains(searchString))
                                    || (c.QualityControlLevelCode != null && c.QualityControlLevelCode.Contains(searchString))
                                    || (c.SampleID != null && c.SampleID.ToLower().Contains(searchString))
                                    || (c.SiteCode != null && c.SiteCode.ToLower().Contains(searchString))
                                    || (c.SourceID != null && c.SourceID.ToLower().Contains(searchString))
                                    || (c.UTCOffset != null && c.UTCOffset.ToLower().Contains(searchString))
                                    || (c.ValueAccuracy != null && c.ValueAccuracy.ToLower().Contains(searchString))
                                    || (c.VariableCode != null && c.VariableCode.ToLower().Contains(searchString))
                        );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();
                        }
                    }
                    else
                        items = (List<DataValuesModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                }

            }



            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);


            var rst = from c in items
                      select new[] { 
                        c.ValueID,
                        c.DataValue,
                        c.ValueAccuracy,
                        c.LocalDateTime,
                        c.UTCOffset,
                        c.DateTimeUTC,
                        c.SiteCode,
                        //c.VariableID,
                        c.VariableCode,
                        c.OffsetValue,
                        c.OffsetTypeID,
                        c.CensorCode,
                        c.QualifierID,
                        c.MethodID,
                        c.MethodDescription,
                        c.SourceID,
                        c.SampleID,
                        c.DerivedFromID,
                        c.QualityControlLevelCode
                };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult GroupDescriptions(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<GroupDescriptionModel>();
            List<GroupDescriptionModel> items = new List<GroupDescriptionModel>();

            listOfRecords = (List<GroupDescriptionModel>)Utils.GetRecordsFromCache<GroupDescriptionModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                       c.GroupID.ToString().ToLower().Contains(searchString.ToLower())
                                    || c.GroupDescription != null && c.GroupDescription.ToLower().Contains(searchString.ToLower())
                                );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.GroupDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.GroupDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                               
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                    c.GroupID,
                                    c.GroupDescription
                                    };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult Groups(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<GroupsModel>();
            List<GroupsModel> items = new List<GroupsModel>();

            listOfRecords = (List<GroupsModel>)Utils.GetRecordsFromCache<GroupsModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                       c.GroupID != null && c.GroupID.ToString().ToLower().Contains(searchString.ToLower())
                                     || c.ValueID != null && c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
                                    );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                        c.GroupID,
                                        c.ValueID
                                    };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult DerivedFrom(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<DerivedFromModel>();
            List<DerivedFromModel> items = new List<DerivedFromModel>();

            listOfRecords = (List<DerivedFromModel>)Utils.GetRecordsFromCache<DerivedFromModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                       c.DerivedFromId != null && c.DerivedFromId.ToString().ToLower().Contains(searchString.ToLower())
                                    || c.ValueID != null && c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
                        );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.DerivedFromId).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.DerivedFromId).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.DerivedFromId).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                        c.DerivedFromId,
                                        c.ValueID
                                    };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult Categories(JQueryDataTablesModel jQueryDataTablesModel, int id)
        {
            int totalRecordCount = 0;
            int searchRecordCount = 0;

            var startIndex = jQueryDataTablesModel.iDisplayStart;
            var pageSize = jQueryDataTablesModel.iDisplayLength;

            var listOfRecords = new List<CategoriesModel>();
            List<CategoriesModel> items = new List<CategoriesModel>();

            listOfRecords = (List<CategoriesModel>)Utils.GetRecordsFromCache<CategoriesModel>(id, "default");

            var sortedColumns = jQueryDataTablesModel.GetSortedColumns();

            //initial value
            if (listOfRecords != null)
            {
                totalRecordCount = searchRecordCount = listOfRecords.Count();
                //filter results only for text columns


                if (listOfRecords.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(jQueryDataTablesModel.sSearch))
                    {
                        var searchString = jQueryDataTablesModel.sSearch.ToLower();
                        var filteredItems = listOfRecords.
                            Where(c =>
                                      c.VariableCode.ToString().ToLower().Contains(searchString.ToLower())
                                   || c.DataValue.ToString().ToLower().Contains(searchString.ToLower())
                                   || c.CategoryDescription.ToLower().Contains(searchString.ToLower())
                                 );

                        if (filteredItems != null)
                        {
                            searchRecordCount = filteredItems.Count();
                            items = filteredItems.Skip(startIndex).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        // items = (List<SiteModel>)listOfRecords.OrderBy(a => a.SiteCode).Skip(jQueryDataTablesModel.iDisplayStart).Take(jQueryDataTablesModel.iDisplayLength).ToList();

                        //List<SiteModel> sortedItems = null;

                        foreach (var sortedColumn in sortedColumns)
                        {
                            switch (sortedColumn.PropertyName.ToLower())
                            {
                                case "0":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "1":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.DataValue).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.DataValue).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                                case "2":
                                    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                                    { items = listOfRecords.OrderBy(a => a.CategoryDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    else
                                    { items = listOfRecords.OrderByDescending(a => a.CategoryDescription).Skip(startIndex).Take(pageSize).ToList(); }
                                    break;
                               
                            }
                        }

                        if (items == null) items = listOfRecords.OrderByDescending(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList();


                    }
                }

            }




            //var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
            //    pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
            //    totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var rst = from c in items
                      select new[] { 
                                        c.VariableCode,
                                        c.DataValue,
                                        c.CategoryDescription
                                    };

            return this.DataTablesJson(items: rst,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

    
	}
}