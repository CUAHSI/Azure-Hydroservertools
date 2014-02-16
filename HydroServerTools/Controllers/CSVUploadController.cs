using CsvHelper;
using HydroServerTools.Helper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

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
    [Authorize]
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
                    conn.Close();
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

        public ActionResult UploadData()
        {
            return View("UploadData");
        }

        public ActionResult Sites()
        {            
            return View("Sites");
        }
        public ActionResult Variables()
        {
            return View("Variables");
        }
        public ActionResult OffsetTypes()
        { 
            return View ("OffsetTypes");
        }
         public ActionResult ISOMetadata() 
         { 
             return View ("ISOMetadata");
         }
         public ActionResult Sources() 
         { 
             return View ("Sources");
         }
         public ActionResult Methods() 
         { 
             return View("Methods");
         }
         public ActionResult LabMethods() 
         { 
             return View("LabMethods");
         }
         public ActionResult Samples() 
         { 
             return View("Samples");
         }
         public ActionResult Qualifiers() 
         { 
             return View("Qualifiers");
         }
         public ActionResult QualityControlLevels() 
         { 
             return View("QualityControlLevels");
         }
         public ActionResult DataValues() 
         { 
             return View("DataValues");
         }
         public ActionResult GroupDescriptions() 
         { 
             return View("GroupDescriptions");
         }
         public ActionResult Groups() 
         { 
             return View("Groups");
         }
         public ActionResult DerivedFrom() 
         { 
             return View("DerivedFrom");
         }
         public ActionResult Categories() 
         { 
             return View("Categories");
         }
        public ContentResult _Breadcrumb(string name)
        {

            string[] vars = new string[14] { "Sites", "Variables", "OffsetTypes", "Sources", "Methods", "LabMethods", "Samples", "Qualifiers", "QualityControlLevels", "DataValues", "GroupDescriptions", "Groups", "DerivedFrom", "Categories" };//"ISOMetadata",
          
            StringBuilder html = new StringBuilder();
            html.Append("<div>");
            html.Append("<ol class='breadcrumb h6'>");
            foreach (var item in vars)
            {
                if (name.ToLower() == item.ToLower()) html.Append("<li class='active h5'><strong>" + item + "</strong></li>");

                else
                {                    
                    html.Append("<li><a href ='");
                    html.Append("/CSVUpload/");
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

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file, FormCollection collection)//
        {
            //string userName = HttpContext.User.Identity.Name;//  "martin.seul@yahoo.com";
            //var Db = new ApplicationDbContext();
            //var userEmail = Db.Users.First(u => u.UserName == userName).UserEmail

            //file = Request.Files[0];
            string viewName = collection["viewname"];
            
            if (!file.FileName.ToLower().EndsWith(".csv"))
            {
                ModelState.AddModelError(String.Empty, Ressources.FILETYPE_NOT_CSV);
                return View(viewName);              
            }
            //Get Connection string
            var connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name);
            var entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);
            
            if (String.IsNullOrEmpty(connectionName))
            { 
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);
                return View(viewName);
            }
            //Object T;
            try
            {
                #region Sites
                //  
                if (viewName.ToLower() == "sites")
                {
                    List<SiteModel> values = null;

                  
                    var siteViewModel = new SitesViewModel();
                    var listOfIncorrectRecords = new List<SiteModel>();
                    var listOfCorrectRecords = new List<SiteModel>();
                    var listOfDuplicateRecords = new List<SiteModel>();
                    var listOfEditedRecords = new List<SiteModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<SiteModel>(file);
                    }


                    if (values != null)
                    {
                        var sitesRepository = new SitesRepository();

                        sitesRepository.AddSites(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again

                    siteViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    siteViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    siteViewModel.listOfEditedRecords = listOfEditedRecords;
                    siteViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
                    return RedirectToAction("Sites", siteViewModel);
                    //return View(viewName, siteViewModel);
                } 
                #endregion
                
                #region Variables
                if (viewName.ToLower() == "variables")
                {
                    List<VariablesModel> values = null;

                    var variablesViewModel = new VariablesViewModel();
                    var listOfIncorrectRecords = new List<VariablesModel>();
                    var listOfCorrectRecords = new List<VariablesModel>();
                    var listOfDuplicateRecords = new List<VariablesModel>();
                    var listOfEditedRecords = new List<VariablesModel>();
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<VariablesModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new VariablesRepository();

                        repository.AddVariables(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();

                    variablesViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    variablesViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    variablesViewModel.listOfEditedRecords = listOfEditedRecords;
                    variablesViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    // redirect back to the index action to show the form once again
                    return View(viewName, variablesViewModel);
                } 
                #endregion

                #region ISOMetadata
                  
                if (viewName.ToLower() == "isometadata")
                {
                    List<ISOMetadataModel> values = null;
                    var listOfIncorrectRecords = new List<ISOMetadataModel>();
                    var listOfCorrectRecords = new List<ISOMetadataModel>();
                    var listOfDuplicateRecords = new List<ISOMetadataModel>();
                    var listOfEditedRecords = new List<ISOMetadataModel>();

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<ISOMetadataModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new ISOMetadataRepository();

                        repository.AddISOMetadata(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again
                    return View(viewName, listOfIncorrectRecords);
                } 
                #endregion

                #region OffsetTypes

                if (viewName.ToLower() == "offsettypes")
                {
                    List<OffsetTypesModel> values = null;
                    var offsetTypesViewModel = new OffsetTypesViewModel();
                    var listOfIncorrectRecords = new List<OffsetTypesModel>();
                    var listOfCorrectRecords = new List<OffsetTypesModel>();
                    var listOfDuplicateRecords = new List<OffsetTypesModel>();
                    var listOfEditedRecords = new List<OffsetTypesModel>();
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<OffsetTypesModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new OffsetTypesRepository();

                        repository.AddOffsetTypes(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again

                    offsetTypesViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    offsetTypesViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    offsetTypesViewModel.listOfEditedRecords = listOfEditedRecords;
                    offsetTypesViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    return View(viewName, offsetTypesViewModel);
                }
                
                #endregion   
                 
                #region Sources
                if (viewName.ToLower() == "sources")
                {
                    List<SourcesModel> values = null;
                    var sourcesModel = new SourcesViewModel();

                    var listOfIncorrectRecords = new List<SourcesModel>();
                    var listOfCorrectRecords = new List<SourcesModel>();
                    var listOfDuplicateRecords = new List<SourcesModel>();
                    var listOfEditedRecords = new List<SourcesModel>();
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<SourcesModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new SourcesRepository();

                        repository.AddSources(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();



                    sourcesModel.listOfCorrectRecords = listOfCorrectRecords;
                    sourcesModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    sourcesModel.listOfEditedRecords = listOfEditedRecords;
                    sourcesModel.listOfDuplicateRecords = listOfDuplicateRecords;


                    // redirect back to the index action to show the form once again
                    return View(viewName, sourcesModel);
                }
                
                #endregion

                #region  Methods
                if (viewName.ToLower() == "methods")
                {
                    List<MethodModel> values = null;
                    var methodsViewModel = new MethodsViewModel();
                    var listOfIncorrectRecords = new List<MethodModel>();
                    var listOfCorrectRecords = new List<MethodModel>();
                    var listOfDuplicateRecords = new List<MethodModel>();
                    var listOfEditedRecords = new List<MethodModel>();
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<MethodModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new MethodsRepository();

                        repository.AddMethods(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();

                    methodsViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    methodsViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    methodsViewModel.listOfEditedRecords = listOfEditedRecords;
                    methodsViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    // redirect back to the index action to show the form once again
                    return View(viewName, methodsViewModel);
                } 
                #endregion

                #region LabMethods
                if (viewName.ToLower() == "labmethods")
                {
                    List<LabMethodModel> values = null;
                    var labMethodsViewModel = new LabMethodsViewModel(); 
                    var listOfIncorrectRecords = new List<LabMethodModel>();
                    var listOfCorrectRecords = new List<LabMethodModel>();
                    var listOfDuplicateRecords = new List<LabMethodModel>();
                    var listOfEditedRecords = new List<LabMethodModel>();
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<LabMethodModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new LabMethodsRepository();

                        repository.AddLabMethods(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();

                    labMethodsViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    labMethodsViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    labMethodsViewModel.listOfEditedRecords = listOfEditedRecords;
                    labMethodsViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    return View(viewName, labMethodsViewModel);
                } 
                #endregion

                #region Samples
                if (viewName.ToLower() == "samples")
                {
                    List<SampleModel> values = null;
                    var sampleViewModel = new SamplesViewModel();
                    var listOfIncorrectRecords = new List<SampleModel>();
                    var listOfCorrectRecords = new List<SampleModel>();
                    var listOfDuplicateRecords = new List<SampleModel>();
                    var listOfEditedRecords = new List<SampleModel>();
                  
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        values = parseCSV<SampleModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new SamplesRepository();

                        repository.AddSamples(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again

                    sampleViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    sampleViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    sampleViewModel.listOfEditedRecords = listOfEditedRecords;
                    sampleViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    return View(viewName, sampleViewModel);
                } 
                #endregion

                #region Qualifiers
                if (viewName.ToLower() == "qualifiers")
                {
                    List<QualifiersModel> values = null;
                    var qualifiersViewModel = new QualifiersViewModel();
                    var listOfIncorrectRecords = new List<QualifiersModel>();
                    var listOfCorrectRecords = new List<QualifiersModel>();
                    var listOfDuplicateRecords = new List<QualifiersModel>();
                    var listOfEditedRecords = new List<QualifiersModel>();
                  
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<QualifiersModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new QualifiersRepository();

                        repository.AddQualifiers(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again

                    qualifiersViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    qualifiersViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    qualifiersViewModel.listOfEditedRecords = listOfEditedRecords;
                    qualifiersViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
                    return View(viewName, qualifiersViewModel);
                } 
                #endregion

                #region QualityControlLevels
                if (viewName.ToLower() == "qualitycontrollevels")
                {
                    List<QualityControlLevelModel> values = null;
                    var qualityControlLevelViewModel = new QualityControlLevelsViewModel();
                    var listOfIncorrectRecords = new List<QualityControlLevelModel>();
                    var listOfCorrectRecords = new List<QualityControlLevelModel>();
                    var listOfDuplicateRecords = new List<QualityControlLevelModel>();
                    var listOfEditedRecords = new List<QualityControlLevelModel>();
                 
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<QualityControlLevelModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new QualityControlLevelsRepository();

                        repository.AddQualityControlLevel(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again

                    qualityControlLevelViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    qualityControlLevelViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    qualityControlLevelViewModel.listOfEditedRecords = listOfEditedRecords;
                    qualityControlLevelViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    return View(viewName, qualityControlLevelViewModel);
                } 
                #endregion

                #region DataValues
                if (viewName.ToLower() == "datavalues")
                {
                    List<DataValuesModel> values = null;

                    var dataValuesViewModel = new DataValuesViewModel();
                    var listOfIncorrectRecords = new List<DataValuesModel>();
                    var listOfCorrectRecords = new List<DataValuesModel>();
                    var listOfDuplicateRecords = new List<DataValuesModel>();
                    var listOfEditedRecords = new List<DataValuesModel>();
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<DataValuesModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new DataValuesRepository();

                        repository.AddDataValues(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords);
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();

                    dataValuesViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    dataValuesViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    dataValuesViewModel.listOfEditedRecords = listOfEditedRecords;
                    dataValuesViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    // redirect back to the index action to show the form once again
                    return View(viewName, dataValuesViewModel);
                } 
                #endregion

                #region GroupDescriptions
                if (viewName.ToLower() == "groupdescriptions")
                {
                    List<GroupDescriptionModel> values = null;
                    var groupDescriptionsViewModel = new GroupDescriptionsViewModel();
                    var listOfIncorrectRecords = new List<GroupDescriptionModel>();
                    var listOfCorrectRecords = new List<GroupDescriptionModel>();
                    var listOfDuplicateRecords = new List<GroupDescriptionModel>();
                    var listOfEditedRecords = new List<GroupDescriptionModel>();
                  
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<GroupDescriptionModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new GroupDescriptionsRepository();

                        repository.AddGroupDescriptions(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();


                    // redirect back to the index action to show the form once again

                    groupDescriptionsViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    groupDescriptionsViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    groupDescriptionsViewModel.listOfEditedRecords = listOfEditedRecords;
                    groupDescriptionsViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
                    return View(viewName, groupDescriptionsViewModel);
                }
                
                #endregion 

                #region Groups
                if (viewName.ToLower() == "groups")
                {
                    List<GroupsModel> values = null;
                    var groupsViewModel = new GroupsViewModel();
                    var listOfIncorrectRecords = new List<GroupsModel>();
                    var listOfCorrectRecords = new List<GroupsModel>();
                    var listOfDuplicateRecords = new List<GroupsModel>();
                    var listOfEditedRecords = new List<GroupsModel>();
                  
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<GroupsModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new GroupsRepository();

                        repository.AddGroups(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again

                    groupsViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    groupsViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    groupsViewModel.listOfEditedRecords = listOfEditedRecords;
                    groupsViewModel.listOfDuplicateRecords = listOfDuplicateRecords;
                    return View(viewName, groupsViewModel);
                } 
                #endregion

                #region DerivedFrom
                if (viewName.ToLower() == "derivedfrom")
                {
                    List<DerivedFromModel> values = null;
                    var derivedFromViewModel = new DerivedFromViewModel();
                    var listOfIncorrectRecords = new List<DerivedFromModel>();
                    var listOfCorrectRecords = new List<DerivedFromModel>();
                    var listOfDuplicateRecords = new List<DerivedFromModel>();
                    var listOfEditedRecords = new List<DerivedFromModel>();
                  
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<DerivedFromModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new DerivedFromRepository();

                        repository.AddDerivedFrom(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again

					derivedFromViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    derivedFromViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    derivedFromViewModel.listOfEditedRecords = listOfEditedRecords;
                    derivedFromViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    return View(viewName, derivedFromViewModel);
                } 
                #endregion

                #region Categories
                if (viewName.ToLower() == "categories")
                {
                    List<CategoriesModel> values = null;
                    var categoriesViewModel = new CategoriesViewModel();
                    var listOfIncorrectRecords = new List<CategoriesModel>();
                    var listOfCorrectRecords = new List<CategoriesModel>();
                    var listOfDuplicateRecords = new List<CategoriesModel>();
                    var listOfEditedRecords = new List<CategoriesModel>();
                  
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {

                        values = parseCSV<CategoriesModel>(file);
                    }


                    if (values != null)
                    {
                        var repository = new CategoriesRepository();

                        repository.AddCategories(values, entityConnectionString, out listOfIncorrectRecords, out listOfCorrectRecords, out listOfDuplicateRecords, out listOfEditedRecords );
                    }

                    //var table = Utils.GetDatatableForModel<SiteModel>(sites[0]);

                    @ViewBag.numberOfCorrectRecords = listOfCorrectRecords.Count();
                    @ViewBag.numberOfDuplicateRecords = listOfDuplicateRecords.Count();
                    // redirect back to the index action to show the form once again

					categoriesViewModel.listOfCorrectRecords = listOfCorrectRecords;
                    categoriesViewModel.listOfIncorrectRecords = listOfIncorrectRecords;
                    categoriesViewModel.listOfEditedRecords = listOfEditedRecords;
                    categoriesViewModel.listOfDuplicateRecords = listOfDuplicateRecords;

                    return View(viewName, categoriesViewModel);
                } 
                #endregion
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, Ressources.IMPORT_FAILED );//+ ": " + ex.Message
            }
            return View(viewName);
        }

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
                reader.Close();
            }


            return s;
        }
	}
}