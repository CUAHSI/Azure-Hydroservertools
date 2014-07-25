
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using Microsoft.ApplicationServer.Caching;
using MvcFileUploader;
using MvcFileUploader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace HydroServerTools.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            var tableValueCounts = new DatabaseTableValueCountModel();

            //string entityConnectionString = HydroServerToolsUtils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name.ToString());

            string entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name.ToString());


            if (!String.IsNullOrEmpty(entityConnectionString))
            {
                //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

                var databaseRepository = new DatabaseRepository();
                
                tableValueCounts = databaseRepository.GetDatabaseTableValueCount(entityConnectionString);
                
                return View(tableValueCounts);
                
            }
            else
            {
                return RedirectToAction("GoogleForm");
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Manage()
        {
            ViewBag.Message = "";

            return View();
        }
        public ActionResult GoogleForm()
        {
            ViewBag.Message = "";
            //var user = User.Identity.Name;
            return View();
        }
        public ContentResult _GoogleFormIframe()
        {
            var sb = new StringBuilder();
            sb.Append("<iframe src='https://docs.google.com/forms/d/10c-ZpmxZQX9kXIixDZMV5sNJblyhebn13-CnD1xdwjg/viewform?embedded=true&entry.1429452739=");
            sb.Append(User.Identity.Name);
            sb.Append("' width='100%' height='100%' frameborder='0' marginheight='0' marginwidth='0'>Loading...</iframe>");
            return Content(sb.ToString());
        }
     
        public ActionResult JqueryUpload()
        {
            ViewBag.Message = "JqueryUpload";
            return View();
        }

        public ActionResult SimpleUpload()
        {
            ViewBag.Message = "SimpleUpload";
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            HttpPostedFileBase myFile = Request.Files["MyFile"];
            bool isUploaded = false;
            string message = "File upload failed";

            if (myFile != null && myFile.ContentLength != 0)
            {
                string pathForSaving = Server.MapPath("~/Uploads");
                message = "File uploaded successfully!";
                  
            }
            return Json(new { isUploaded = isUploaded, message = message }, "text/html");
        }


        public ActionResult Steps()
        {
            ViewBag.Message = "My Steps page";
            return View();
        }
        public ActionResult VerticalTab()
        {
            ViewBag.Message = "My VerticalTabs page";
            return View();
        }

        public ActionResult ViewTableData(string identifier)
        {
      
            return View();
        }
        public ActionResult Demo(bool? inline, string ui = "bootstrap")
        {
            return View(inline);
        }

        public ActionResult JQupload(bool? inline, string ui = "bootstrap")
        {
            return View(inline);
        }
        public ActionResult JQuploaddemo()
        {
            return View();
        }
        public ActionResult JQueryApI()
        {
            return View();
        }
        public ActionResult Templates()
        {
            return View();
        }
        public ActionResult ChangeHistory()
        {
            return View();
        }
        public ActionResult ControlledVocabularies()
        {
            return View();
        }
      
        [Authorize]
        public ActionResult ClearTablesHandler(FormCollection collection)
        {
            string entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name.ToString());

            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);
            //"Sites", "Variables", "OffsetTypes", "ISOMetadata", "Sources", "Methods", "LabMethods", "Samples", "Qualifiers", "QualityControlLevels", "DataValues", "GroupDescriptions", "Groups", "DerivedFrom", "Categories"};

            //Sites
 
            try
            {
                if (collection.HasKeys())
                {
                    //do in this order to remove foreign keys in order
                    if (collection.AllKeys.Contains("categories")) { var repo = new CategoriesRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("derivedfrom")) { var repo = new DerivedFromRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("groups")) { var repo = new GroupsRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("groupdescriptions")) { var repo = new GroupDescriptionsRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("datavalues")) { var repo = new DataValuesRepository(); repo.deleteAll(entityConnectionString); }                  
                    if (collection.AllKeys.Contains("qualitycontrollevels")) { var repo = new QualityControlLevelsRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("qualifiers")) { var repo = new QualifiersRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("samples")) { var repo = new SamplesRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("labmethods")) { var repo = new LabMethodsRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("methods")) { var repo = new MethodsRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("sources")) { var repo = new SourcesRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("offsettypes")) { var repo = new OffsetTypesRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("variables")) { var repo = new VariablesRepository(); repo.deleteAll(entityConnectionString); }
                    if (collection.AllKeys.Contains("sites")) { var repo = new SitesRepository(); repo.deleteAll(entityConnectionString); }

                    
                }
            }
            catch (DbUpdateException ex)
            {
                //HttpContext.Response.ContentType = "text/plain";
                Response.StatusCode = (int)HttpStatusCode.BadRequest; 
                return Json(new { success = false });

            }
           
            // Now we need to wire up a response so that the calling script understands what happened
            HttpContext.Response.ContentType = "text/plain";
            HttpContext.Response.StatusCode = 200;

            // For compatibility with IE's "done" event we need to return a result as well as setting the context.response
            return Json(new { success = true });

        }
        [HttpPost]
        public ActionResult Progress()
        {
            DataCache cache = new DataCache("default");
            var identifier = MvcApplication.InstanceGuid + User.Identity.Name;
            string StatusMessage = "Uploading...";
            if (cache.Get(identifier + "processStatus") != null)
            {
                if (cache.Get(identifier + "processStatus") != null)   StatusMessage = (string)cache.Get(identifier + "processStatus");
            }

            return Json(StatusMessage);
        }
    }
}