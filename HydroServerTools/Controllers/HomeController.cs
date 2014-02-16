using HydroServerTools.Helper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HydroServerTools.Controllers
{
  
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
        [HttpPost]
        public ContentResult UploadFiles()
        {
            
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;

                string savedFileName = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(hpf.FileName));
                //hpf.SaveAs(savedFileName); // Save the file

               
            }
            // Returns json
            return Content("{\"name\":\"}", "application/json");
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
                                        c.LocalProjectionID,  
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

    }
}