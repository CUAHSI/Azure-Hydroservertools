using HydroServerTools.Helper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
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
            return View();
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
        public ActionResult Datatable1()
        {
            var model = new ConnectionModel();
            model.ServerName = "mseul-cuahsi";
            model.DataSourceName = "LittleBear_1_1_1";
            model.Username = "martin";
            model.Password = "ms";
            
            var connectionString = Helper.Utils.BuildEFConnnectionString(model);


            var sitesRepository = new SitesRepository();
            
            ViewBag.Message = "Your Datatable page.";
            return View(sitesRepository.GetAll(connectionString));
        }
        public ActionResult Datatable2(string identifier)
        {
            ViewBag.Message = "Your Datatable2 page.";

            if (identifier.ToLower() == "sites") { var model = new SiteModel(); return View("DataTable2", model); };
      

            return View();
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
        public ActionResult AjaxHandler(jQueryDataTableParamModel param, string identifier)
        {

            var repo = new SitesRepository(); 
            
            string connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name.ToString());

            var entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString)) { ViewBag.Message = Ressources.HYDROSERVER_USERLOOKUP_FAILED; return View("Error"); }


            var items = repo.GetAll(entityConnectionString);


            var itemsToDisplay = items.Take(1);
            var list = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach(var item in itemsToDisplay)
            {
                var itemProps = item.GetType().GetProperties();
               
                foreach (var itemProp in itemProps)
                {                           
                    if(itemProp.GetValue(item, null) != null) 
                    {
                        sb.Append("\"");
                        sb.Append(itemProp.GetValue(item, null).ToString());
                        sb.Append("\"");
                    }
                    sb.Append(",");
                }

                list.Add(sb.ToString().TrimEnd(','));                
            }

            var result = from c in itemsToDisplay
                         select
                                    new[] { 
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
           //var s = "[{\"SiteCode\": \"wq1\",\"SiteName\": \"Fyrisån Flottsund\",\"Latitude\": \"59.786938\",\"Longitude\": \"17.659863\",\"LatLongDatumSRSName\": \"1\",\"Elevation_m\": \"1\",\"VerticalDatum\": \"1\",\"LocalX\": \"1\",\"LocalY\": \"1\",\"LocalProjectionID\": \"1\",\"PosAccuracy_m\": \"1\",\"State\": \"Rock\",\"County\": \"\",\"Comments\": \"1\",\"SiteType\": \"1\"}]";

           var jsonReturn = JsonConvert.SerializeObject(result);

           var json = new JavaScriptSerializer().Serialize(list);

           var test = "\"sEcho\": \"1\", \"iTotalRecords\": \"1\", \"iTotalDisplayRecords\": \"10\",  \"aaData\": \"[[\"wq1\",\"Fyrisån Flottsund\",\"59.786938\",\"17.659863\",null,null,null,null,null,null,null,\"Rock\",\"\",null,null],[\"wq2\",\"Fyrisån Klastorp\",\"59.886636\",\"17.578593\",null,null,null,null,null,null,null,\"\",\"\",null,null],[\"wq3\",\"Sävjaån Kuggebro\",\"59.831468\",\"17.691587\",null,null,null,null,null,null,null,\"\",\"\",null,null],[\"wq7\",\"Sävjaån Lejsta\",\"59.954735\",\"17.900163\",null,null,null,null,null,null,null,\"\",\"\",null,null],[\"wq127\",\"Kolbäcksån Strömsholm\",\"59.525675\",\"16.270192\",null,null,null,null,null,null,null,\"\",\"\",null,null],[\"wq129\",\"Sagån Målhammar\",\"59.600104\",\"16.890664\",null,null,null,null,null,null,null,\"\",\"\",null,null],[\"wq131\",\"Örsundaån Örsundsbro\",\"59.736638\",\"17.310296\",null,null,null,null,null,null,null,\"\",\"\",null,null],[\"wq133\",\"Botorpström Brunnsö\",\"57.663568\",\"16.495821\",null,null,null,null,null,null,null,\"\",\"\",null,null],[\"wq134\",\"Ljungbyån Ljungbyholm\",\"56.631532\",\"16.172673\",null,null,null,null,null,null,null,\"\",\"\",null,null],[\"wq135\",\"Mörrumsån Mörrum\",\"56.189214\",\"14.750275\",null,null,null,null,null,null,null,\"\",\"\",null,null]]";

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = "1",// itemsToDisplay.Count(), 
                iTotalDisplayRecords = "1",//param.iDisplayLength,
                //aaData = new List<string[]>() {
                //    new string[] {"USU-LBR-Mendon","'Little Bear River at Mendon Road near Mendon',' Utah'","41.718473","-111.946402","2","1345","WGS84","421276.323","4618952.04","105","NULL","Utah","Cache","Located below county road bridge at Mendon Road crossing","Stream"}
                   
                //    }
                aaData = result

            },
            JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public ActionResult ClearTablesHandler(FormCollection collection)
        {
             string connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name.ToString());

            var entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);
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

    }
}