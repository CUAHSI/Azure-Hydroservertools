using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

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
        public ActionResult Datatable2()
        {
            ViewBag.Message = "Your Datatable2 page.";

            return View();
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
     
        
    }
}