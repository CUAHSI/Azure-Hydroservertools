
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using jQuery.DataTables.Mvc;
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
        [HttpGet]
        public ActionResult Sites()
        {
            return View("Sites");
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

        [HttpPost]
        public JsonResult Search(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var sitesRepository = new SitesRepository();
            var items = sitesRepository.GetSites(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
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

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult VariablesSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new VariablesRepository();
            var items = repository.GetVariables(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
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

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult OffsetTypesSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
           // var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new OffsetTypesRepository();
            var items = repository.GetOffsetTypes(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            //c.OffsetTypeID,
                            c.OffsetTypeCode,
                            c.OffsetUnitsName,
                            c.OffsetDescription
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult SourcesSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new SourcesRepository();
            var items = repository.GetSources(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            c.SourceCode,
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

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult SamplesSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new SamplesRepository();
            var items = repository.GetSamples(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            
                            c.SampleType,
                            c.LabSampleCode,
                            c.LabMethodName,
                            c.LabMethodID
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult MethodsSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new MethodsRepository();
            var items = repository.GetMethods(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            c.MethodCode,
                            c.MethodDescription,
                            c.MethodLink
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult LabMethodsSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new LabMethodsRepository();
            var items = repository.GetLabMethods(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] {                             
                            c.LabName,
                            c.LabOrganization,
                            c.LabMethodName,
                            c.LabMethodDescription,
                            c.LabMethodLink
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult QualifiersSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new QualifiersRepository();
            var items = repository.GetQualifiers(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] {                            
                            c.QualifierCode,
                            c.QualifierDescription                        
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult QualityControlLevelsSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new QualityControlLevelsRepository();
            var items = repository.GetQualityControlLevels(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            c.QualityControlLevelCode,
                            c.Definition,
                            c.Explanation
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }
   
        [HttpPost]
        public JsonResult GroupDescriptionSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new GroupDescriptionsRepository();
            var items = repository.GetGroupDescriptions(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            c.GroupID,
                            c.GroupDescription
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult GroupsSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }

            var repository = new GroupsRepository();
            var items = repository.GetGroups(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            c.GroupID,
                            c.ValueID
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult DerivedFromSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }

            var repository = new DerivedFromRepository();
            var items = repository.GetDerivedFrom(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            c.DerivedFromId,
                            c.ValueID
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

          [HttpPost]
        public JsonResult CategoriesSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new CategoriesRepository();
            var items = repository.GetCategories(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            c.VariableCode,
                            c.DataValue,
                            c.CategoryDescription
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }
   

        [HttpPost]
        public JsonResult DatavaluesSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
           var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new DataValuesRepository();
            var items = repository.GetDatavalues(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            c.ValueID,
                            c.DataValue,
                            c.ValueAccuracy,
                            c.LocalDateTime,
                            c.UTCOffset,
                            c.DateTimeUTC,
                            c.SiteCode,
                            c.VariableCode,
                            c.OffsetValue,
                            c.OffsetTypeID,
                            c.CensorCode,
                            c.QualifierID,
                            c.MethodID,
                            c.SourceID,
                            c.SampleID,
                            c.DerivedFromID,
                            c.QualityControlLevelCode
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult SeriesCatalogSearch(JQueryDataTablesModel jQueryDataTablesModel, string identifier)
        {
            int totalRecordCount;
            int searchRecordCount;

            //Get Connection string
            var entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name);
            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString))
            {
                ModelState.AddModelError(String.Empty, Ressources.HYDROSERVER_USERLOOKUP_FAILED);

            }


            var repository = new SeriesCatalogRepository();
            var items = repository.GetSeriesCatalog(entityConnectionString, startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            var result = from c in items
                         select new[] { 
                            //c.SeriesID,
                            //c.SiteID,
                            c.SiteCode,
                            c.SiteName,
                            c.SiteType,
                            //c.VariableID,
                            c.VariableCode,
                            c.VariableName,
                            c.Speciation,
                            //c.VariableUnitsID,
                            c.VariableUnitsName,
                            c.SampleMedium,
                            c.ValueType,
                            c.TimeSupport,
                            //c.TimeUnitsID,
                            c.TimeUnitsName,
                            c.DataType,
                            c.GeneralCategory,
                            //c.MethodID,
                            c.MethodDescription,
                            //c.SourceID,
                            c.Organization,
                            c.SourceDescription,
                            c.Citation,
                            //c.QualityControlLevelID,
                            c.QualityControlLevelCode,
                            c.BeginDateTime,
                            c.EndDateTime,
                            //c.BeginDateTimeUTC,
                            //c.EndDateTimeUTC,
                            c.ValueCount
                };

            return this.DataTablesJson(items: result,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        public ActionResult ViewTableData(string identifier)
        {


            string entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(HttpContext.User.Identity.Name.ToString());

            //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

            if (String.IsNullOrEmpty(entityConnectionString)) { ViewBag.Message = Ressources.HYDROSERVER_USERLOOKUP_FAILED; return View("Error"); }


            if (identifier.ToLower() == "sites") { var repo = new SiteModel(); return View("ViewSites", repo); };
            if (identifier.ToLower() == "variables") {var  repo = new VariablesModel(); return View("ViewVariables", repo); };
            if (identifier.ToLower() == "offsettypes") { var repo = new OffsetTypesModel(); return View("ViewOffsetTypes", repo); };
            if (identifier.ToLower() == "isometadata") { var repo = new ISOMetadataModel(); return View("ViewISOMetadata", repo); };
            if (identifier.ToLower() == "sources") { var repo = new SourcesModel(); return View("ViewSources", repo); };
            if (identifier.ToLower() == "methods") { var repo = new MethodModel(); return View("ViewMethods", repo); };
            if (identifier.ToLower() == "labmethods") { var repo = new LabMethodModel(); return View("ViewLabMethods", repo); };
            if (identifier.ToLower() == "samples") { var repo = new SampleModel(); return View("ViewSamples", repo); };
            if (identifier.ToLower() == "qualifiers") { var repo = new QualifiersModel(); return View("ViewQualifiers", repo); };
            if (identifier.ToLower() == "qualitycontrollevels") { var repo = new QualityControlLevelModel(); return View("ViewQualityControlLevels", repo); };
            if (identifier.ToLower() == "datavalues") { var repo = new DataValuesModel(); return View("ViewDataValues", repo); };
            if (identifier.ToLower() == "groupdescriptions") { var repo = new GroupDescriptionModel(); return View("ViewGroupDescription", repo); };
            if (identifier.ToLower() == "groups") { var repo = new GroupsModel(); return View("ViewGroups", repo); };
            if (identifier.ToLower() == "derivedfrom") { var repo = new DerivedFromModel(); return View("ViewDerivedFrom", repo); };
            if (identifier.ToLower() == "categories") { var repo = new CategoriesModel(); return View("ViewCategories", repo); };
            if (identifier.ToLower() == "seriescatalog") { var repo = new SeriesCatalogModel(); return View("ViewSeriesCatalog", repo); };


            return View();
        }

	}
}