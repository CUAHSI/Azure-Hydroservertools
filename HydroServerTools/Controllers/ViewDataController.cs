using HydroServerTools.Helper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroServerToolsRepository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroServerTools.Controllers
{
    [Authorize]
    public class ViewDataController : Controller
    {
        //
        // GET: /View/
        public ActionResult Index()
        {
            return View();
        }
        //"Sites", "Variables", "OffsetTypes", "ISOMetadata", "Sources", "Methods", "LabMethods", "Samples", "Qualifiers", "QualityControlLevels", "DataValues", "GroupDescriptions", "Groups", "DerivedFrom", "Categories"};

        //public ActionResult ViewSites()
        //{

        //    string connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name.ToString());
            
        //    var entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);
                        
        //    var sitesRepository = new SitesRepository();

        //    return View(sitesRepository.GetAll(entityConnectionString));
          
        //}
        //public ActionResult ViewVariables()
        //{
            
        //    string connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name.ToString());
           
        //    var entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);

        //    var variablesRepository = new VariablesRepository();

        //    return View(variablesRepository.GetAll(entityConnectionString));
            
        //}

        public ActionResult ViewTableData(string identifier)
        {
           

            string connectionName = Utils.GetConnectionNameByUserEmail(HttpContext.User.Identity.Name.ToString());

            var entityConnectionString = Utils.GetDBConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString)) { ViewBag.Message = Ressources.HYDROSERVER_USERLOOKUP_FAILED; return View("Error"); }


            if (identifier.ToLower() == "sites") { var repo = new SitesRepository(); return View("ViewSites", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "variables") {var  repo = new VariablesRepository(); return View("ViewVariables", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "offsettypes") { var repo = new OffsetTypesRepository(); return View("ViewOffsetTypes", repo.GetAll(entityConnectionString)); };           
            if (identifier.ToLower() == "isometadata") { var repo = new ISOMetadataRepository(); return View("ViewISOMetadata", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "sources") { var repo = new SourcesRepository(); return View("ViewSources", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "methods") { var repo = new MethodsRepository(); return View("ViewMethods", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "labmethods") { var repo = new LabMethodsRepository(); return View("ViewLabMethods", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "samples") { var repo = new SamplesRepository(); return View("ViewSamples", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "qualifiers") { var repo = new QualifiersRepository(); return View("ViewQualifiers", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "qualitycontrollevels") { var repo = new QualityControlLevelsRepository(); return View("ViewQualityControlLevels", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "datavalues") { var repo = new DataValuesRepository(); return View("ViewDataValues", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "groupdescriptions") { var repo = new GroupDescriptionsRepository(); return View("ViewGroupDescriptions", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "groups") { var repo = new GroupsRepository(); return View("ViewGroups", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "derivedfrom") { var repo = new DerivedFromRepository(); return View("ViewDerivedFrom", repo.GetAll(entityConnectionString)); };
            if (identifier.ToLower() == "categories") { var repo = new CategoriesRepository(); return View("ViewCategories", repo.GetAll(entityConnectionString)); };



            return View();
        }

	}
}