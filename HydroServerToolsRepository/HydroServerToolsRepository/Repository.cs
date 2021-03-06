﻿using AutoMapper;
using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository;
using ODM_1_1_1EFModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using TB.ComponentModel;

using System.Data.Entity;
using System.Threading.Tasks;

using HydroServerToolsUtilities;

using EntityFramework.Metadata.Extensions; 

namespace HydroServerToolsRepository.Repository
{
    //A simple class implementing the IRepository methods...
    public class Repository: IRepository
    {
        //Dictionary of table names to ODM types...
        //private static Dictionary<string, object> tableNamesToDbSets = new Dictionary<string, object>();

        //Properties...
        private string ConnectionString { get; set; }

        private ODM_1_1_1Entities OdmEntities { get; set; }

        //Constructors 

        //Default constructor - private to prevent use...
        private Repository() { }

        //Initializing - throw exception on empty connection string
        //TO DO - RegEx check for connection string format?
        public Repository(string connectionString)
        {
            //Validate/initialize input parameters...
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString", "empty/null input value...");
            }

            //Assign input value to member...
            ConnectionString = connectionString;

            //Instantiate entities member...
            OdmEntities = new ODM_1_1_1Entities(connectionString);
        }

        //For the input table name(s), return a dictionary of table names and record counts...
        public Dictionary<string, int> GetTableRecordCounts(List<string> tableNames)
        {
            Dictionary<string, int> tableNamesToRecordCounts = new Dictionary<string, int>();

            //Validate/initialize input parameters...
            if (null != tableNames)
            {
                //For each input table name...
                foreach (var tableName in tableNames)
                {
                    var lowerTableName = tableName.ToLowerInvariant();
                    int count = 0;

                    switch (lowerTableName)
                    {
                        case "sites":
                            {
                              var items = from item in OdmEntities.Sites select item;
                              count = items.Count();
                            }
                            break;    
                        case "variables":
                            {
                                var items = from item in OdmEntities.Sites select item;
                                count = items.Count();
                            }
                            break;
                        case "offsettypes":
                            {
                                var items = from item in OdmEntities.OffsetTypes select item;
                                count = items.Count();
                            }
                            break;
                        case "sources":
                            {
                                var items = from item in OdmEntities.Sources select item;
                                count = items.Count();
                            }
                            break;
                        case "methods":
                            {
                                var items = from item in OdmEntities.Methods select item;
                                count = items.Count();
                            }
                            break;
                        case "labmethods":
                            {
                                var items = from item in OdmEntities.LabMethods select item;
                                count = items.Count();
                            }
                            break;
                        case "samples":
                            {
                                var items = from item in OdmEntities.Samples select item;
                                count = items.Count();
                            }
                            break;
                        case "qualifiers":
                            {
                                var items = from item in OdmEntities.Qualifiers select item;
                                count = items.Count();
                            }
                            break;
                        case "qualitycontrollevels":
                            {
                                var items = from item in OdmEntities.QualityControlLevels select item;
                                count = items.Count();
                            }
                            break;
                        case "datavalues":
                            {
                                var items = from item in OdmEntities.DataValues select item;
                                count = items.Count();
                            }
                            break;
                        case "groupdescriptions":
                            {
                                var items = from item in OdmEntities.GroupDescriptions select item;
                                count = items.Count();
                            }
                            break;
                        case "groups":
                            {
                                var items = from item in OdmEntities.Groups select item;
                                count = items.Count();
                            }
                            break;
                        case "derivedfrom":
                            {
                                var items = from item in OdmEntities.DerivedFroms select item;
                                count = items.Count();
                            }
                            break;
                        case "categories":
                            {
                                var items = from item in OdmEntities.Categories select item;
                                count = items.Count();
                            }
                            break;
                        default:
                            //Take no action
                            break;
                    }

                    //Adjust count value for selected tables, if indicated...
                    if (1 == count && ("methods" == lowerTableName || "labmethods" == lowerTableName))
                    {
                        count = 0;
                    }

                    //if (6 == count && "qualitycontrollevels" == lowerTableName)
                    //{
                    //    count = 0;
                    //}

                    tableNamesToRecordCounts[tableName] = count;
                }
            }

            //Processing complete - return
            return tableNamesToRecordCounts;
        }
    }
    
    //  Sites
    public class SitesRepository : ISitesRepository
    {
        public const string CacheName = "default";
        

        public List<SiteModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.
            //EntityConnection conn = new EntityConnection(connectionString);


            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from site in context.Sites
                        select site;
            var sites = new List<SiteModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<Site, SiteModel>(item);

                sites.Add(model);
            }
            return sites;
        }

        public List<SiteModel> GetSites(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<SiteModel>();

            if (context.Sites.Count() != 0)
            {
                totalRecordCount = context.Sites.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            //var test = DatatablesHelper.FilterSitesTable(context, searchString, pageSize);

            if (!string.IsNullOrWhiteSpace(searchString))
            {

                var allItems = context.Sites.ToList();
                var rst = allItems.
                          Where(c =>
                               c.SiteCode != null && c.SiteCode.ToLower().Contains(searchString.ToLower())
                            || c.SiteName != null && c.SiteName.ToLower().Contains(searchString.ToLower())
                            || c.Latitude.ToString().ToLower().Contains(searchString.ToLower())
                            || c.Longitude.ToString().ToLower().Contains(searchString.ToLower())
                            || c.SpatialReference.SRSName != null && c.SpatialReference.SRSName.ToLower().Contains(searchString.ToLower())
                            || c.Elevation_m != null && c.Elevation_m.ToString().ToLower().Contains(searchString.ToLower())
                            || c.LocalX != null && c.LocalX.ToString().ToLower().Contains(searchString.ToLower())
                            || c.LocalY != null && c.LocalY.ToString().ToLower().Contains(searchString.ToLower())
                            || c.VerticalDatum != null && c.VerticalDatum.ToString().ToLower().Contains(searchString.ToLower())
                                   //|| c.LocalProjectionID != null && c.LocalProjectionID.ToString().Contains(searchString.ToLower())
                            || c.PosAccuracy_m != null && c.PosAccuracy_m.ToString().ToLower().Contains(searchString.ToLower())
                            || c.State != null && c.State.ToLower().Contains(searchString.ToLower())
                            || c.County != null && c.County.ToLower().Contains(searchString.ToLower())
                            || c.Comments != null && c.Comments.ToLower().Contains(searchString.ToLower())
                            || c.SiteType != null && c.SiteType.ToLower().Contains(searchString.ToLower())
                            );


                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<Site, SiteModel>(item);

                    model.LatLongDatumSRSName = context.SpatialReferences
                                       .Where(a => a.SpatialReferenceID == item.LatLongDatumID)
                                       .Select(a => a.SRSName)
                                       .FirstOrDefault();
                    result.Add(model);
                }
            }

            else
            {
                List<Site> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.SiteName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.SiteName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.Latitude).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.Latitude).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.Longitude).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.Longitude).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "4":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.SpatialReference.SRSName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.SpatialReference.SRSName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "5":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.Elevation_m).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.Elevation_m).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "6":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.VerticalDatum).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.VerticalDatum).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "7":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.LocalX).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.LocalX).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "8":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.LocalY).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.LocalY).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "9":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.LocalProjectionID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.LocalProjectionID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "10":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.PosAccuracy_m).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.PosAccuracy_m).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "11":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "12":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.County).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.County).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "13":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.Comments).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.Comments).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "14":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sites.OrderBy(a => a.SiteType).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sites.OrderByDescending(a => a.SiteType).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                    }
                }

                if (sortedItems == null) sortedItems = context.Sites.OrderByDescending(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<Site, SiteModel>(item);
                    //model.LatLongDatumSRSName = from r in context.SpatialReferences
                    //             where r.SpatialReferenceID == item.LatLongDatumID
                    //             select r.SRSName.ToString()
                    //             .FirstOrDefault();



                    model.LatLongDatumSRSName = context.SpatialReferences
                                         .Where(a => a.SpatialReferenceID == item.LatLongDatumID)
                                         .Select(a => a.SRSName)
                                         .FirstOrDefault();


                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddSites(List<SiteModel> itemList, string entityConnectionString, string instanceIdentifier, List<SiteModel> listOfIncorrectRecords, List<SiteModel> listOfCorrectRecords, List<SiteModel> listOfDuplicateRecords, List<SiteModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords )
            {
                ArgumentNullException ex = new ArgumentNullException("SitesRepository.AddSites(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //var objContext = ((IObjectContextAdapter)context).ObjectContext;

            var LatLongDatum = context.SpatialReferences.ToDictionary(p => p.SRSName.Trim(), p => p.SpatialReferenceID);
            var LatLongDatumSRSID = context.SpatialReferences.Where(p => p.SRSID != null).ToDictionary(p => p.SRSID, p => p.SpatialReferenceID);

            var VerticalDatumCV = context.VerticalDatumCVs.ToList();
            var SiteTypeCV = context.SiteTypeCVs.ToList();

            //get all sites
            var sitesInDatabase = context.Sites.Select(p => p.SiteCode.ToLower()).ToList();

            var maxCount = itemList.Count;
            var count = 0;

            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "Sites");
            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (SiteModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {
                //var item = new ODM_1_1_1EFModel.Site();

                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof (SiteModel).Name, statusMessage);
                    }
                    count++;                    
                    //var model = Mapper.Map<SiteModel, Site>(item);


                    var model = new Site();
                    bool isRejected = false;

                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();
   

                    //SiteCode
                    if (!string.IsNullOrWhiteSpace(item.SiteCode))
                    {
                        if (RepositoryUtils.containsNotOnlyAllowedCharacters(item.SiteCode))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "SiteCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.SiteCode = item.SiteCode;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SiteCode")); listOfErrors.Add(err); isRejected = true;
                    }

                    //SiteName
                    if (!string.IsNullOrWhiteSpace(item.SiteName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.SiteName))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "SiteName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.SiteName = item.SiteName;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SiteName")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Latitude
                    if (!string.IsNullOrWhiteSpace(item.Latitude))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.Latitude, out result);

                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "Latitude")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            if (result >= -90 && result <= 90) model.Latitude = result;
                            else
                            {
                                var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDRANGE, "Latitude", "-90 to +90")); listOfErrors.Add(err); isRejected = true;
                            }
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Latitude")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Longitude
                    if (!string.IsNullOrWhiteSpace(item.Longitude))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.Longitude, out result);

                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "Longitude")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            if (result >= -180 && result <= 180) model.Longitude = result;
                            else
                            {
                                var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDRANGE, "Longitude", "-180 to +180")); listOfErrors.Add(err); isRejected = true;
                            }
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Longitude")); listOfErrors.Add(err); isRejected = true;
                    }
                    //#####################
                    //LatLongDatumID
                    if (!string.IsNullOrWhiteSpace(item.LatLongDatumSRSName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LatLongDatumSRSName))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LatLongDatumSRSName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            if (item.LatLongDatumSRSName.ToLower() == "unknown")
                            {
                                var unknownID = context.SpatialReferences.Where(p => p.SRSName.ToLower() == "unknown").Select(p => p.SpatialReferenceID).FirstOrDefault();
                                model.LatLongDatumID = unknownID;
                                item.LatLongDatumID = unknownID.ToString();// write back to viewmodel to not have to convert again when values are committed to DB
                            }
                            else
                            {
                                int result;
                                bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.LatLongDatumSRSName, out result);
                                if (canConvert)//user used SSRID
                                {
                                    
                                    var LatLongDatumID = LatLongDatumSRSID
                                     .Where(a => a.Key == result)
                                     .Select(a => a.Value)
                                     .SingleOrDefault();
                                    if (LatLongDatumID != 0)
                                    {
                                        model.LatLongDatumID = LatLongDatumID;
                                        item.LatLongDatumID = LatLongDatumID.ToString();// write back to viewmodel to not have to convert again when values are committed to DB
                                        item.LatLongDatumSRSName = LatLongDatum.Where(a => a.Value == LatLongDatumID).Select(a => a.Key).FirstOrDefault();
                                    }
                                    else
                                    {
                                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "LatLongDatumSRSName")); listOfErrors.Add(err); isRejected = true;
                                    }
                                }
                                else
                                {
                                    var LatLongDatumID = LatLongDatum
                                     .Where(a => a.Key.ToLower() == item.LatLongDatumSRSName.ToLower())
                                     .Select(a => a.Value)
                                     .SingleOrDefault();
                                    if (LatLongDatumID != 0)
                                    {
                                        model.LatLongDatumID = LatLongDatumID;
                                        item.LatLongDatumID = LatLongDatumID.ToString();// write back to viewmodel to not have to convert again when values are comitted to DB
                                        item.LatLongDatumSRSName = LatLongDatum.Where(a => a.Value == LatLongDatumID).Select(a => a.Key).FirstOrDefault();
                                    }
                                    else
                                    {
                                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "LatLongDatumSRSName")); listOfErrors.Add(err); isRejected = true;
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "LatLongDatumSRSName")); listOfErrors.Add(err); isRejected = true;
                    }

                    //#####################
                    //Elevation_m
                    if (!string.IsNullOrWhiteSpace(item.Elevation_m))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.Elevation_m, out result);

                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "Elevation_m")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            //    if (result >= -180 && result <= 180) 
                            model.Elevation_m = result;
                            //    else
                            //    {
                            //        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDRANGE, "Longitude", "-180 to +180")); listOfErrors.Add(err); isRejected = true;
                            //    }
                        }
                    }
                    else
                    {
                        model.Elevation_m = null;
                    }
                    //VerticalDatum
                    if (!string.IsNullOrWhiteSpace(item.VerticalDatum))
                    {
                        if (RepositoryUtils.containsInvalidCharacters(item.VerticalDatum))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "VerticalDatum")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            var verticalDatum = VerticalDatumCV
                                           .Where(a => a.Term.ToString().ToLower() == item.VerticalDatum.ToLower()).FirstOrDefault();

                            if (verticalDatum == null)
                            {
                                var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.VerticalDatum, "VerticalDatum"));
                                listOfErrors.Add(err); isRejected = true;
                            }
                            else
                            {
                                model.VerticalDatum = verticalDatum.Term;
                                item.VerticalDatum = verticalDatum.Term; 
                            }

                        }
                    }
                    else
                    {
                        model.VerticalDatum = null;
                        item.VerticalDatum = null;
                    }
                    //LocalX
                    if (!string.IsNullOrWhiteSpace(item.LocalX))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.LocalX, out result);

                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "LocalX")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.LocalX = result;
                        }
                    }
                    else
                    {
                        model.LocalX = null;
                    }
                    //LocalY
                    if (!string.IsNullOrWhiteSpace(item.LocalY))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.LocalY, out result);

                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "LocalY")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.LocalY = result;
                        }
                    }
                    else
                    {
                        model.LocalY = null;
                    }
                    //LocalProjectionID
                    if (!string.IsNullOrWhiteSpace(item.LocalProjectionSRSName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LocalProjectionSRSName))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LocalProjectionSRSName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            if (item.LatLongDatumSRSName.ToLower() == "unknown")
                            {
                                var unknownID = context.SpatialReferences.Where(p => p.SRSName.ToLower() == "unknown").Select(p => p.SpatialReferenceID).FirstOrDefault();
                                model.LatLongDatumID = unknownID;
                                item.LatLongDatumID = unknownID.ToString();// write back to viewmodel to not have to convert again when values are committed to DB
                            }
                            else
                            {
                                int result;
                                bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.LocalProjectionSRSName, out result);
                                if (canConvert)//user used SSRID
                                {
                                    var localDatumID = LatLongDatumSRSID
                                     .Where(a => a.Key == result)
                                     .Select(a => a.Value)
                                     .SingleOrDefault();
                                    if (localDatumID != 0)
                                    {
                                        model.LocalProjectionID = localDatumID;
                                        item.LocalProjectionID = localDatumID.ToString();// write back to viewmodel to not have to convert again when values are comitted to DB
                                        item.LocalProjectionSRSName = LatLongDatum.Where(a => a.Value == localDatumID).Select(a => a.Key).FirstOrDefault();
                                    }
                                    else
                                    {
                                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "LocalProjectionSRSName")); listOfErrors.Add(err); isRejected = true;
                                    }
                                }
                                else
                                {
                                    var localDatumID = LatLongDatum
                                     .Where(a => a.Key.ToLower() == item.LocalProjectionSRSName.ToLower())
                                     .Select(a => a.Value)
                                     .SingleOrDefault();
                                    if (localDatumID != 0)
                                    {
                                        model.LocalProjectionID = localDatumID;
                                        item.LocalProjectionID = localDatumID.ToString();// write back to viewmodel to not have to convert again when values are comitted to DB
                                        item.LocalProjectionSRSName = LatLongDatum.Where(a => a.Value == localDatumID).Select(a => a.Key).FirstOrDefault();
                                    }
                                    else
                                    {
                                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "LocalProjectionSRSName")); listOfErrors.Add(err); isRejected = true;
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        model.LocalProjectionID = null;
                    }
                    //PosAccuracy_m
                    if (!string.IsNullOrWhiteSpace(item.PosAccuracy_m))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.PosAccuracy_m, out result);

                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "PosAccuracy_m")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            //    if (result >= -180 && result <= 180) 
                            model.PosAccuracy_m = result;
                            //    else
                            //    {
                            //        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDRANGE, "Longitude", "-180 to +180")); listOfErrors.Add(err); isRejected = true;
                            //    }
                        }
                    }
                    else
                    {
                        model.PosAccuracy_m = null;
                    }
                    //State
                    if (!string.IsNullOrWhiteSpace(item.State))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.State))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "State")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.State = item.State;
                        }
                    }
                    else
                    {
                        model.State = null;
                        item.State = null;
                    }
                    //County
                    if (!string.IsNullOrWhiteSpace(item.County))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.County))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "County")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.County = item.County;
                        }
                    }
                    else
                    {
                        model.County = null;
                        item.County = null;
                    }
                    //Comments
                    if (!string.IsNullOrWhiteSpace(item.Comments))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Comments))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Comments")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.Comments = item.Comments;
                        }
                    }
                    else
                    {
                        model.Comments = null;
                        item.Comments = null;
                    }
                    //SiteType
                    if (!string.IsNullOrWhiteSpace(item.SiteType))
                    {
                        //var siteType = SiteTypeCV
                        //               .Exists(a => a.Term.ToString().ToLower() == item.SiteType.ToLower());

                        var siteType = SiteTypeCV
                                      .Where(a => a.Term.ToString().ToLower() == item.SiteType.ToLower()).FirstOrDefault();

                        if (siteType == null)
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.SiteType, "SiteType"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            //using this insures correct spelling 
                            model.SiteType = siteType.Term;
                            item.SiteType = siteType.Term;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SiteType")); listOfErrors.Add(err); isRejected = true;
                    }

                    //general rules check If one of these fields are included, then so should the rest: LocalX, LocalY, LocalSRSName 

                    if (model.LocalX != null || model.LocalY != null || model.LocalProjectionID != null)
                    {
                        if (!(model.LocalX != null && model.LocalY != null && model.LocalProjectionID != null))
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_LOCALVALUE_NOT_COMPLETE)); listOfErrors.Add(err); isRejected = true;
                        }
                    }
                    if (model.Elevation_m != null )
                    {
                        if (model.VerticalDatum == null)
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_ELEVATION_VERTICALDATUM)); listOfErrors.Add(err); isRejected = true;                 
                        }
                    }

                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof (SiteModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }
                    //check for duplicates first in database then in upload if a duplicate site is found the record will be rejected.

                    //check in list
                    var doesExist = sitesInDatabase.Find(p =>p == item.SiteCode.ToLower());                    
                    
                    
                    //var j = context.Sites.Find(s.SiteCode);

                    if (doesExist == null)
                    {
                        
                        
                        var existInUpload = listOfCorrectRecords.Exists(a => a.SiteCode.ToLower() == item.SiteCode.ToLower());
                        if (!existInUpload)   
                        {
                            //context.Sites.Add(model);
                        //context.SaveChanges();
                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddSites", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE,"SiteCode")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(SiteModel).Name, statMsg);
                            }
                        }
                    }
                    else
                    {
                       // var editedFields = new List<string>();
                        //retrieve all DatabaseRepository from db
                        var existingItem = context.Sites.Where(a => a.SiteCode.ToLower() == item.SiteCode.ToLower()).FirstOrDefault();

                        //if (existingItem.SiteCode != model.SiteCode) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "SiteCode", existingItem.SiteCode.ToString(), item.SiteCode.ToString())); }
                        if (existingItem.Latitude != model.Latitude) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "Latitude", existingItem.Latitude.ToString(), item.Latitude.ToString())); }
                        if (existingItem.Longitude != model.Longitude) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "Longitude", existingItem.Longitude.ToString(), item.Longitude.ToString())); }
                        if (existingItem.LatLongDatumID != model.LatLongDatumID) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "LatLongDatumSRSName", existingItem.SpatialReference.SRSName.ToString(), item.LatLongDatumSRSName.ToString())); }
                        if (model.Elevation_m != null && existingItem.Elevation_m != model.Elevation_m) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "Elevation_m", existingItem.Elevation_m.ToString(), item.Elevation_m.ToString())); }
                        if (model.VerticalDatum != null && existingItem.VerticalDatum != model.VerticalDatum) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "VerticalDatum", existingItem.VerticalDatum != null ? existingItem.VerticalDatum.ToString() : String.Empty, item.VerticalDatum.ToString())); }
                        if (model.LocalX != null && existingItem.LocalX != model.LocalX) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "LocalX", existingItem.LocalX.ToString(), item.LocalX.ToString())); }
                        if (model.LocalY != null && existingItem.LocalY != model.LocalY) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "LocalY", existingItem.LocalY.ToString(), item.LocalY.ToString())); }
                        if (model.LocalProjectionID != null && existingItem.LocalProjectionID != model.LocalProjectionID) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "LocalProjectionSRSName", existingItem.SpatialReference1 != null ? existingItem.SpatialReference1.ToString() : String.Empty, item.LocalProjectionSRSName.ToString())); }
                        if (model.PosAccuracy_m != null && existingItem.PosAccuracy_m != model.PosAccuracy_m) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "PosAccuracy_m", existingItem.PosAccuracy_m.ToString(), item.PosAccuracy_m.ToString())); }
                        if (model.State != null && existingItem.State != model.State) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "State", existingItem.State, item.State)); }
                        if (model.County != null && existingItem.County != model.County) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "County", existingItem.County, item.County)); }
                        if (model.Comments != null && existingItem.Comments != model.Comments) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "Comments", existingItem.Comments, item.Comments)); }
                        if (model.SiteType != null && existingItem.SiteType != model.SiteType) { listOfUpdates.Add(new UpdateFieldsModel("Sites", "SiteType", existingItem.SiteType, item.SiteType)); }


                        if (listOfUpdates.Count() > 0)
                        {

                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name, 0, 1, 0, 0);
                            }

                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var erMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(erMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(SiteModel).Name, erMessage);
                                }
                            }
                            item.Errors += sb.ToString();
                           
                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name, 0, 0, 0, 1);
                            }
                        }
                        //var modifiedEntries = this.ObjectStateManager.GetObjectStateEntries(EntityState.Modified);

                    }
                    ////check if entry with this key exists
                    //object value;

                    //var key = Utils.GetEntityKey(objectSet, d);

                    //if (!objContext.TryGetObjectByKey(key, out value))
                    //{
                    //     try
                    //     {
                    // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                    //objContext.Connection.Open();
                    //objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[Sites] ON");
                    //objContext.AddObject(objectSet.Name, d);

                    //objContext.SaveChanges();

                    //    objContext.Connection.Close();
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw;
                    //}
                    //}
                    //else
                    //{

                    //context.MyEntities.Attach(myEntity);
                    //    listOfDuplicateRecords.Add(s);
                    //}
                   


                }
                //Datum not in CV
                //catch (KeyNotFoundException ex)
                catch (KeyNotFoundException)
                {

                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name, 0, 0, 1, 0);
                    }
                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(SiteModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Sites
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.Sites.Remove(row);
            //}
            try
            {
                context.Sites.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }

    //  Variables
    public class VariablesRepository : IVariablesRepository
    {
        public const string CacheName = "default";

        public List<VariablesModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.Variables
                        select obj;
            var modelList = new List<VariablesModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<Variable, VariablesModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<VariablesModel> GetVariables(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<VariablesModel>();

            //if (context.Variables.Count() != null)
            if (0 < context.Variables.Count())
            {
                totalRecordCount = context.Variables.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
               var allItems = context.Variables.ToList();
                var rst = allItems.
                            Where(c =>
                                   c.VariableCode != null && c.VariableCode.ToLower().Contains(searchString.ToLower())
                                || c.VariableName != null && c.VariableName.ToLower().Contains(searchString.ToLower())
                                || c.Speciation != null && c.Speciation.Contains(searchString.ToLower())
                                || c.Unit != null && c.VariableUnitsID.ToString().Contains(searchString.ToLower())
                                || c.SampleMedium != null && c.SampleMedium.ToLower().Contains(searchString.ToLower())
                                || c.ValueType != null && c.ValueType.ToLower().Contains(searchString.ToLower())
                                //|| c.IsRegular != null && c.IsRegular.ToString().ToLower().Contains(searchString.ToLower())
                                || c.IsRegular.ToString().ToLower().Contains(searchString.ToLower())
                                //|| c.TimeSupport != null && c.TimeSupport.ToString().ToLower().Contains(searchString.ToLower())
                                || c.TimeSupport.ToString().ToLower().Contains(searchString.ToLower())
                                || c.Unit != null && c.Unit.UnitsName.ToLower().Contains(searchString.ToLower())
                                || c.DataType != null && c.DataType.ToLower().Contains(searchString.ToLower())
                                || c.GeneralCategory != null && c.GeneralCategory.ToLower().Contains(searchString.ToLower())
                                //|| c.NoDataValue != null && c.NoDataValue.ToString().ToLower().Contains(searchString.ToLower())
                                || c.NoDataValue.ToString().ToLower().Contains(searchString.ToLower())
                           );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)

                    searchRecordCount = rst.Count();
                foreach (var item in rst)
                {

                    var model = Mapper.Map<Variable, VariablesModel>(item);

                    model.VariableUnitsName = context.Units
                      .Where(a => a.UnitsID == item.VariableUnitsID)
                      .Select(a => a.UnitsName)
                      .FirstOrDefault();

                    model.TimeUnitsName = context.Units
                       .Where(a => a.UnitsID == item.TimeUnitsID)
                       .Select(a => a.UnitsName)
                       .FirstOrDefault();

                    result.Add(model);
                }
            }

            else
            {
                List<Variable> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        //case "0":
                        //    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                        //    { sortedItems = context.Variables.OrderBy(a => a.VariableID).Skip(startIndex).Take(pageSize).ToList(); }
                        //    else
                        //    { sortedItems = context.Variables.OrderByDescending(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                        //    break;
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.VariableName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.VariableName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.Speciation).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.Speciation).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.Unit1.UnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.Unit1.UnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "4":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.SampleMedium).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.SampleMedium).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "5":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.ValueType).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.ValueType).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "6":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.IsRegular).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.IsRegular).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "7":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.TimeSupport).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.TimeSupport).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "8":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.Unit1.UnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.Unit1.UnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "9":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.DataType).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.DataType).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "10":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.GeneralCategory).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.GeneralCategory).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "11":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Variables.OrderBy(a => a.NoDataValue).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Variables.OrderByDescending(a => a.NoDataValue).Skip(startIndex).Take(pageSize).ToList(); }
                            break;

                    }
                }

                if (sortedItems == null) sortedItems = context.Variables.OrderByDescending(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<Variable, VariablesModel>(item);
                    //model.LatLongDatumSRSName = from r in context.SpatialReferences
                    //             where r.SpatialReferenceID == item.LatLongDatumID
                    //             select r.SRSName.ToString()
                    //             .FirstOrDefault();



                    model.VariableUnitsName = context.Units
                       .Where(a => a.UnitsID == item.VariableUnitsID)
                       .Select(a => a.UnitsName)
                       .FirstOrDefault();

                    model.TimeUnitsName = context.Units
                       .Where(a => a.UnitsID == item.TimeUnitsID)
                       .Select(a => a.UnitsName)
                       .FirstOrDefault();

                    result.Add(model);
                }
            }
            return result;
        }


        public async Task AddVariables(List<VariablesModel> itemList, string entityConnectionString, string instanceIdentifier, List<VariablesModel> listOfIncorrectRecords, List<VariablesModel> listOfCorrectRecords, List<VariablesModel> listOfDuplicateRecords, List<VariablesModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("VariablesRepository.AddVariables(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            //read CV in to list for fster searching
            var variableCV = context.VariableNameCVs.ToList();
            var speciationCV = context.SpeciationCVs.ToList();
            var units = context.Units.Distinct().ToDictionary(p => p.UnitsName.Trim(), p => p.UnitsID);
            var sampleMediumCV = context.SampleMediumCVs.ToList();
            var valueTypeCV = context.ValueTypeCVs.ToList();
            var dataTypeCV = context.DataTypeCVs.ToList();
            var generalCategoryCV = context.GeneralCategoryCVs.ToList();

            //get all variables
            var variablesInDatabase = context.Variables.Select(p => p.VariableCode.ToLower()).ToList();

            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "Variables");
            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (VariablesModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(VariablesModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {
                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if ( null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof (VariablesModel).Name, statusMessage);
                    }

                    count++;

                    //var model = Mapper.Map<VariablesModel, Variable>(item);
                    var model = new Variable();
                    //set deafults
                    model.Speciation = "Not Applicable";
                    model.SampleMedium = "Unknown";
                    model.ValueType = "Unknown";
                    model.IsRegular = false;
                    model.TimeSupport = 0;
                    model.TimeUnitsID = 103;
                    model.DataType = "Unknown";
                    model.GeneralCategory = "Unknown";
                    model.NoDataValue = -9999;

                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();

                    //need to look up Id's for VariableName, Speciation, VariableUnitsName, SampleMedium, ValueType, DataType, GeneralCategory, TimeUnitsName
                    //User has no concept of ID's
                    bool isRejected = false;
                    //VariableCode
                    if (!string.IsNullOrWhiteSpace(item.VariableCode))
                    {
                        if (RepositoryUtils.containsNotOnlyAllowedCharacters(item.VariableCode))
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "VariableCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.VariableCode = item.VariableCode;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "VariableCode")); listOfErrors.Add(err); isRejected = true;
                    }
                    //VariableName
                    if (!string.IsNullOrWhiteSpace(item.VariableName))
                    {
                        var variableName = variableCV
                                          .Where(a => a.Term.ToString().ToLower() == item.VariableName.ToLower()).SingleOrDefault();

                        if (variableName == null)
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.VariableName, "variableName"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.VariableName = variableName.Term;
                        }
                        
                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "VariableName")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Speciation
                    if (!string.IsNullOrWhiteSpace(item.Speciation))
                    {
                        var speciation = speciationCV
                                          .Where(a => a.Term.ToString().ToLower() == item.Speciation.ToLower()).SingleOrDefault();

                        if (speciation == null)
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.Speciation, "Speciation"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.Speciation = item.Speciation;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Speciation")); listOfErrors.Add(err); isRejected = true;
                    }
                    //VariableUnitsName
                    if (!string.IsNullOrWhiteSpace(item.VariableUnitsName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.VariableUnitsName))
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "VariableUnitsName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            
                            int result;
                            bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.VariableUnitsName, out result);
                            if (canConvert)//user used id
                            {

                                if (result != 0)
                                {
                                    model.VariableUnitsID = result;
                                    item.VariableUnitsID = result.ToString();
                                }
                                else
                                {
                                    var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "VariableUnitsName")); listOfErrors.Add(err); isRejected = true;
                                }
                            }
                            else
                            {
                                    var variableUnitsID = units
                                        .Where(a => a.Key.ToLower() == item.VariableUnitsName.ToLower())
                                        .Select(a => a.Value)
                                        .SingleOrDefault();
                                    if (variableUnitsID != 0)
                                    {
                                        model.VariableUnitsID = variableUnitsID;
                                        item.VariableUnitsID = variableUnitsID.ToString();
                                    }
                                    else
                                    {
                                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "VariableUnitsName")); listOfErrors.Add(err); isRejected = true;
                                    }
                            }                            
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "VariableUnitsName")); listOfErrors.Add(err); isRejected = true;
                    }
                    //SampleMedium
                    if (!string.IsNullOrWhiteSpace(item.SampleMedium))
                    {
                        var sampleMedium = sampleMediumCV
                                            .Where(a => a.Term.ToString().ToLower() == item.SampleMedium.ToLower()).FirstOrDefault();
                        if (sampleMedium == null)
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.SampleMedium, "SampleMedium"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.SampleMedium = sampleMedium.Term;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SampleMedium")); listOfErrors.Add(err); isRejected = true;
                    }
                    //ValueType
                    if (!string.IsNullOrWhiteSpace(item.ValueType))
                    {
                        var valueType = valueTypeCV
                                           .Where(a => a.Term.ToString().ToLower() == item.ValueType.ToLower()).FirstOrDefault();
                        if (valueType == null)
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.ValueType, "ValueType"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.ValueType = valueType.Term;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "ValueType")); listOfErrors.Add(err); isRejected = true;
                    }
                    //IsRegular
                    if (!string.IsNullOrWhiteSpace(item.IsRegular))
                    {
                        bool result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<bool>(item.IsRegular, out result);
                        
                        if (!canConvert)
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.IsRegular, "IsRegular"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.IsRegular = result;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "IsRegular")); listOfErrors.Add(err); isRejected = true;
                    }
                    //TimeSupport
                    if (!string.IsNullOrWhiteSpace(item.TimeSupport))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.TimeSupport, out result);
                        
                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.TimeSupport, "TimeSupport"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.TimeSupport = result;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "TimeSupport")); listOfErrors.Add(err); isRejected = true;
                    }
                    //TimeUnitsID
                    if (!string.IsNullOrWhiteSpace(item.TimeUnitsName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.TimeUnitsName))
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "TimeUnitsName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {

                            int result;
                            bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.TimeUnitsName, out result);
                            if (canConvert)//user used id
                            {

                                if (result != 0)
                                {
                                    model.TimeUnitsID = result;
                                    item.TimeUnitsID = result.ToString();                                
                                }
                                else
                                {
                                    var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "TimeUnitsName")); listOfErrors.Add(err); isRejected = true;
                                }
                            }
                            else
                            {
                                var timeUnitsID = units
                                   .Where(a => a.Key.ToLower() == item.TimeUnitsName.ToLower())
                                   .Select(a => a.Value)
                                   .SingleOrDefault();
                                if (timeUnitsID != 0)
                                {
                                    model.TimeUnitsID = timeUnitsID;
                                    item.TimeUnitsID = timeUnitsID.ToString();
                                }
                                else
                                {
                                    var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "TimeUnitsName")); listOfErrors.Add(err); isRejected = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "TimeUnitsName")); listOfErrors.Add(err); isRejected = true;
                    }
                    //DataType
                    if (!string.IsNullOrWhiteSpace(item.DataType))
                    {
                        var dataType = dataTypeCV
                                           .Where(a => a.Term.ToString().ToLower() == item.DataType.ToLower()).FirstOrDefault();
                        if (dataType == null)
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.DataType, "DataType"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.DataType = dataType.Term;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "DataType")); listOfErrors.Add(err); isRejected = true;
                    }
                    //GeneralCategory
                    if (!string.IsNullOrWhiteSpace(item.GeneralCategory))
                    {
                        var generalCategory = generalCategoryCV
                                            .Where(a => a.Term.ToString().ToLower() == item.GeneralCategory.ToLower()).FirstOrDefault();
                        if (generalCategory == null)
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.GeneralCategory, "GeneralCategory"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.GeneralCategory = generalCategory.Term;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "GeneralCategory")); listOfErrors.Add(err); isRejected = true;
                    }
                    //NoDataValue
                    if (!string.IsNullOrWhiteSpace(item.NoDataValue))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.NoDataValue, out result);
                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_FAILED_NOVALIDDATA, "NoDataValue"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.NoDataValue = result;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "NoDataValue")); listOfErrors.Add(err); isRejected = true;
                    }

                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(VariablesModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(VariablesModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }
                                        

                    //lookup duplicates
                    //var objectSet = objContext.CreateObjectSet<ODM_1_1_1EFModel.Variable>().EntitySet;//.EntitySet;
                    //check if item with this variablecode exists in the database
                    var doesExist = variablesInDatabase.Find(p => p == item.VariableCode.ToLower());                    
                  
                    if (doesExist == null)
                    {
                        var existInUpload = listOfCorrectRecords.Exists(a => a.VariableCode == item.VariableCode);
                        if (!existInUpload)
                        {
                            //context.Sites.Add(model);
                            //context.SaveChanges();
                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(VariablesModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE, "VariableCode")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(VariablesModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(VariablesModel).Name, statMsg);
                            }
                        }
                    }
                    else
                    {
                        var existingItem = context.Variables.Where(a => a.VariableCode == item.VariableCode).FirstOrDefault();
                    
 
                        //if (existingItem.VariableCode != model.VariableCode) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "VariableCode", existingItem.VariableCode.ToString(), item.VariableCode.ToString())); }
                        if (model.VariableName != null && existingItem.VariableName != model.VariableName) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "VariableName", existingItem.VariableName.ToString(), item.VariableName.ToString())); }
                        if (model.Speciation != model.Speciation) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "Speciation", existingItem.Speciation.ToString(), item.Speciation.ToString())); }
                        if (model.VariableUnitsID != model.VariableUnitsID) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "VariableUnitsID", existingItem.Unit1.UnitsName.ToString(), item.VariableUnitsName.ToString())); }
                        if (model.SampleMedium != model.SampleMedium) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "SampleMedium", existingItem.SampleMedium.ToString(), item.SampleMedium.ToString())); }
                        if (model.ValueType != model.ValueType) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "ValueType", existingItem.ValueType.ToString(), item.ValueType.ToString())); }
                        if (model.IsRegular != model.IsRegular) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "IsRegular", existingItem.IsRegular.ToString(), item.IsRegular.ToString())); }
                        if (model.TimeSupport != model.TimeSupport) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "TimeSupport", existingItem.TimeSupport.ToString(), item.TimeSupport.ToString())); }
                        if (model.TimeUnitsID != model.TimeUnitsID) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "TimeUnitsID", existingItem.Unit1.UnitsName, item.TimeUnitsName.ToString())); }
                        if (model.DataType != model.DataType) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "DataType", existingItem.DataType.ToString(), item.DataType.ToString())); }
                        if (model.GeneralCategory != model.GeneralCategory) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "GeneralCategory", existingItem.GeneralCategory.ToString(), item.GeneralCategory.ToString())); }
                        if (model.NoDataValue != model.NoDataValue) { listOfUpdates.Add(new UpdateFieldsModel("Variables", "NoDataValue", existingItem.NoDataValue.ToString(), item.NoDataValue.ToString())); }

                        if (listOfUpdates.Count() > 0)
                        {
                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(VariablesModel).Name, 0, 1, 0, 0);
                            }
                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var erMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(erMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(VariablesModel).Name, erMessage);
                                }
                            }
                            item.Errors = sb.ToString();

                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(VariablesModel).Name, 0, 0, 0, 1);
                            }
                        }

                    }
                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(VariablesModel).Name, 0, 0, 1, 0);
                    }
                }
            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(VariablesModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Variables
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.Variables.Remove(row);
            //}
            try
            {
                context.Variables.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }

    //  OffsetTypes
    public class OffsetTypesRepository : IOffsetTypesRepository
    {
        public const string CacheName = "default";

        public List<OffsetTypesModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.OffsetTypes
                        select obj;

            var Units = context.Units.ToDictionary(p => p.UnitsID, p => p.UnitsName.Trim());

            var modelList = new List<OffsetTypesModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<OffsetType, OffsetTypesModel>(item);

                if (Units.ContainsKey(item.OffsetUnitsID))
                {
                    var offsetUnitName = Units[item.OffsetUnitsID];
                    //update model
                    model.OffsetUnitsName = offsetUnitName;
                }

                modelList.Add(model);
            }
            return modelList;
        }

        public List<OffsetTypesModel> GetOffsetTypes(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<OffsetTypesModel>();

            
            totalRecordCount = context.OffsetTypes.Count();
            searchRecordCount = totalRecordCount;            
            
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.OffsetTypes.ToList();
                var rst = allItems.
                    Where(c =>
                             //   c.OffsetTypeID.ToString().ToLower().Contains(searchString.ToLower())
                              c.OffsetTypeCode != null && c.OffsetTypeCode.ToLower().Contains(searchString.ToLower())
                             || c.Unit != null && c.Unit.UnitsName.ToLower().Contains(searchString.ToLower())
                             || c.OffsetDescription != null && c.OffsetDescription.ToLower().Contains(searchString.ToLower())
                             );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<OffsetType, OffsetTypesModel>(item);

                    model.OffsetUnitsName = context.Units
                     .Where(a => a.UnitsID == item.OffsetUnitsID)
                     .Select(a => a.UnitsName)
                     .FirstOrDefault();

                    result.Add(model);
                }
            }

            else
            {
                List<OffsetType> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.OffsetTypes.OrderBy(a => a.OffsetTypeCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.OffsetTypes.OrderByDescending(a => a.OffsetTypeCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.OffsetTypes.OrderBy(a => a.Unit.UnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.OffsetTypes.OrderByDescending(a => a.Unit.UnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.OffsetTypes.OrderBy(a => a.OffsetDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.OffsetTypes.OrderByDescending(a => a.OffsetDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            break;


                    }
                }

                if (sortedItems == null) sortedItems = context.OffsetTypes.OrderByDescending(a => a.OffsetTypeCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<OffsetType, OffsetTypesModel>(item);
                    //model.LatLongDatumSRSName = from r in context.SpatialReferences
                    //             where r.SpatialReferenceID == item.LatLongDatumID
                    //             select r.SRSName.ToString()
                    //             .FirstOrDefault();

                    model.OffsetUnitsName = context.Units
                       .Where(a => a.UnitsID == item.OffsetUnitsID)
                       .Select(a => a.UnitsName)
                       .FirstOrDefault();

                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddOffsetTypes(List<OffsetTypesModel> itemList, string entityConnectionString, string instanceIdentifier, List<OffsetTypesModel> listOfIncorrectRecords, List<OffsetTypesModel> listOfCorrectRecords, List<OffsetTypesModel> listOfDuplicateRecords, List<OffsetTypesModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("OffsetTypesRepository.AddOffsetTypes(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //prefetch Units for quick lookup
            var units = context.Units.ToDictionary(p => p.UnitsName.Trim(), p => p.UnitsID);

            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "OffsetTypes");
            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (OffsetTypesModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(OffsetTypesModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {

                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(OffsetTypesModel).Name, statusMessage);
                    }

                    count++;
                    
                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();
                    bool isRejected = false;
                    var model = new OffsetType(); 

                    //OffsetUnitsID
                    if (!string.IsNullOrWhiteSpace(item.OffsetUnitsName))
                                
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.OffsetUnitsName))
                        {
                            var err = new ErrorModel("AddOffsetTypes", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "OffsetUnitsName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {

                            int result;
                            bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.OffsetUnitsName, out result);
                            if (canConvert)//user used id
                            {

                                if (result != 0) model.OffsetUnitsID = result;
                                else
                                {
                                    var err = new ErrorModel("AddOffsetTypes", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "OffsetUnitsName")); listOfErrors.Add(err); isRejected = true;
                                }
                            }
                            else
                            {
                                var unitsID = units
                                    .Where(a => a.Key == item.OffsetUnitsName)
                                    .Select(a => a.Value)
                                    .SingleOrDefault();
                                if (unitsID != 0)
                                {
                                    model.OffsetUnitsID = unitsID;
                                    item.OffsetUnitsID = unitsID.ToString();
                                }
                                else
                                {
                                    var err = new ErrorModel("AddOffsetTypes", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "OffsetUnitsName")); listOfErrors.Add(err); isRejected = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddOffsetTypes", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "OffsetUnitsName")); listOfErrors.Add(err); isRejected = true;
                    }

                    //OffsetDescription
                    if (!string.IsNullOrWhiteSpace(item.OffsetDescription))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.OffsetDescription))
                        {
                            var err = new ErrorModel("AddOffsetTypes", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "OffsetDescription")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.OffsetDescription = item.OffsetDescription;
                        }
                    }
                    else
                    {
                        model.OffsetDescription = null;
                    }

                    //OffsetTypeCode
                    if (!string.IsNullOrWhiteSpace(item.OffsetTypeCode))
                    {
                        if (RepositoryUtils.containsNotOnlyAllowedCharacters(item.OffsetTypeCode))
                        {
                            var err = new ErrorModel("AddOffsetType", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "OffsetTypeCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.OffsetTypeCode = item.OffsetTypeCode;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddOffsetType", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "OffsetTypeCode")); listOfErrors.Add(err); isRejected = true;
                    }

                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(OffsetTypesModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(OffsetTypesModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }

                    //lookup duplicates
                    //check if item with this variablecode exists in the database
                    var existingItem = context.OffsetTypes.Where(a => a.OffsetTypeCode == item.OffsetTypeCode).FirstOrDefault();

                    if (existingItem == null)
                    {
                        var existInUpload = listOfCorrectRecords.Exists(a => a.OffsetTypeCode == item.OffsetTypeCode);
                        if (!existInUpload)
                        {
                            //context.Sites.Add(model);
                            //context.SaveChanges();
                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(OffsetTypesModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddOffsetType", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE, "OffsetTypeCode")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(OffsetTypesModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(OffsetTypesModel).Name, statMsg);
                            }
                        }
                    }
                    else
                    {

                        if (model.OffsetDescription != null && existingItem.OffsetDescription != model.OffsetTypeCode) { listOfUpdates.Add(new UpdateFieldsModel("OffsetType", "OffsetDescription", existingItem.OffsetDescription.ToString(), item.OffsetDescription.ToString())); }
                        if (existingItem.OffsetUnitsID != model.OffsetUnitsID) { listOfUpdates.Add(new UpdateFieldsModel("OffsetType", "OffsetUnitsID", existingItem.OffsetUnitsID.ToString(), item.OffsetUnitsName.ToString())); }

                        if (listOfUpdates.Count() > 0)
                        {
                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(OffsetTypesModel).Name, 0, 1, 0, 0);
                            }

                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var erMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(erMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(OffsetTypesModel).Name, erMessage);
                                }
                            }
                            item.Errors = sb.ToString();

                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(OffsetTypesModel).Name, 0, 0, 0, 1);
                            }
                        }
                    }

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(OffsetTypesModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(OffsetTypesModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.OffsetTypes
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.OffsetTypes.Remove(row);
            //}
            try
            {
                context.OffsetTypes.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }
    ////  ISOMetadata
    //  public class ISOMetadataRepository : IISOMetadataRepository
    //  {

    //      public List<ISOMetadataModel> GetAll(string connectionString)
    //      {
    //          // Create an EntityConnection.         
    //          var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

    //          var items = from obj in context.ISOMetadatas
    //                      select obj;
    //          var modelList = new List<ISOMetadataModel>();
    //          foreach (var item in items)
    //          {

    //              var model = Mapper.Map<ODM_1_1_1EFModel.ISOMetadata, HydroserverToolsBusinessObjects.Models.ISOMetadataModel>(item);

    //              modelList.Add(model);
    //          }
    //          return modelList;
    //      }

    //      public void AddISOMetadata(List<ISOMetadataModel> itemList, string entityConnectionString, Guid instanceGuid, out List<ISOMetadataModel> listOfIncorrectRecords, out List<ISOMetadataModel> listOfCorrectRecords, out List<ISOMetadataModel> listOfDuplicateRecords, out List<ISOMetadataModel> listOfEditedRecords)
    //      {
    //          listOfIncorrectRecords = new List<ISOMetadataModel>();
    //          listOfCorrectRecords = new List<ISOMetadataModel>();
    //          listOfDuplicateRecords = new List<ISOMetadataModel>();
    //          listOfEditedRecords = new List<ISOMetadataModel>();

    //          var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
    //          var objContext = ((IObjectContextAdapter)context).ObjectContext;

    //          foreach (var item in itemList)
    //          {

    //              var model = Mapper.Map<ISOMetadataModel, ISOMetadata>(item);

    //              try
    //              {

    //                  var objectSet = objContext.CreateObjectSet<ISOMetadata>().EntitySet;//.EntitySet;

    //                  ////check if entry with this key exists
    //                  object value;

    //                  var key = Utils.GetEntityKey(objectSet, model);

    //                  if (!objContext.TryGetObjectByKey(key, out value))
    //                  {
    //                      try
    //                      {
    //                          // var objContext = ((IObjectContextAdapter)context).ObjectContext;
    //                          objContext.Connection.Open();
    //                          objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[ISOMetaData] ON");
    //                          objContext.AddObject(objectSet.Name, model);
    //                          //context.Sites.Add(d);
    //                          objContext.SaveChanges();
    //                          listOfCorrectRecords.Add(item);
    //                          objContext.Connection.Close();
    //                      }
    //                      catch (Exception ex)
    //                      {
    //                          throw;
    //                      }
    //                  }
    //                  else
    //                  {
    //                      listOfDuplicateRecords.Add(item);
    //                  }

    //              }
    //              catch (Exception ex)
    //              {
    //                  listOfIncorrectRecords.Add(item);


    //              }

    //          }

    //          return;
    //      }

    //  }

    //  Sources
    public class SourcesRepository : ISourcesRepository
    {
        public const string CacheName = "default";

        public List<SourcesModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.Sources
                        select obj;
            var modelList = new List<SourcesModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.Source, HydroserverToolsBusinessObjects.Models.SourcesModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<SourcesModel> GetSources(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<SourcesModel>();

            //if (context.Sources.Count() != null)
            if (0 < context.Sources.Count())
            {
                totalRecordCount = context.Sources.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.Sources.ToList();
                var rst = allItems.
                    Where(c =>
                                     c.SourceCode != null && c.SourceCode.ToString().ToLower().Contains(searchString.ToLower())
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
                                 || c.ISOMetadata.TopicCategory != null && c.ISOMetadata.TopicCategory.ToLower().Contains(searchString.ToLower())
                                 || c.ISOMetadata.Title != null && c.ISOMetadata.Title.ToLower().Contains(searchString.ToLower())
                                 || c.ISOMetadata.Abstract != null && c.ISOMetadata.Abstract.ToLower().Contains(searchString.ToLower())
                                 || c.ISOMetadata.ProfileVersion != null && c.ISOMetadata.ProfileVersion.ToLower().Contains(searchString.ToLower())
                                 || c.ISOMetadata.MetadataLink != null && c.ISOMetadata.MetadataLink.ToLower().Contains(searchString.ToLower())
                            );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<Source, SourcesModel>(item);
                    model.TopicCategory = item.ISOMetadata.TopicCategory;
                    model.Title = item.ISOMetadata.Title;
                    model.Abstract = item.ISOMetadata.Abstract;
                    model.ProfileVersion = item.ISOMetadata.ProfileVersion;
                    model.MetadataLink = item.ISOMetadata.MetadataLink;
                    result.Add(model);
                }
            }

            else
            {
                List<Source> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.SourceCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.SourceCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.Organization).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.Organization).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.SourceDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.SourceDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.SourceLink).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.SourceLink).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "4":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.ContactName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.ContactName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "5":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.Phone).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.Phone).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "6":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.Email).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.Email).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "7":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.Address).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.Address).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "8":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.City).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.City).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "9":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.State).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "10":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.ZipCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.ZipCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "11":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.Citation).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.Citation).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "12":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.ISOMetadata.TopicCategory).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.ISOMetadata.TopicCategory).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "13":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.ISOMetadata.Title).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.ISOMetadata.Title).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "14":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.ISOMetadata.Abstract).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.ISOMetadata.Abstract).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "15":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.ISOMetadata.ProfileVersion).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.ISOMetadata.ProfileVersion).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "16":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Sources.OrderBy(a => a.ISOMetadata.MetadataLink).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.ISOMetadata.MetadataLink).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                    }
                }

                if (sortedItems == null) sortedItems = context.Sources.OrderByDescending(a => a.SourceCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<Source, SourcesModel>(item);
                    model.TopicCategory = item.ISOMetadata.TopicCategory;
                    model.Title = item.ISOMetadata.Title;
                    model.Abstract = item.ISOMetadata.Abstract;
                    model.ProfileVersion = item.ISOMetadata.ProfileVersion;
                    model.MetadataLink = item.ISOMetadata.MetadataLink;

                    //model.LatLongDatumSRSName = context.SpatialReferences
                    //                     .Where(a => a.SpatialReferenceID == item.LatLongDatumID)
                    //                     .Select(a => a.SRSName)
                    //                     .FirstOrDefault();


                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddSources(List<SourcesModel> itemList, string entityConnectionString, string instanceIdentifier, List<SourcesModel> listOfIncorrectRecords, List<SourcesModel> listOfCorrectRecords, List<SourcesModel> listOfDuplicateRecords, List<SourcesModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("SourcesRepository.AddSources(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           // var objContext = ((IObjectContextAdapter)context).ObjectContext;

            //read CV in to list for fster searching
            var topicCategoryCV = context.TopicCategoryCVs.ToList();

            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "Sources");

            if ( null == statusContext)
            BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);    
            else
            {
                await statusContext.AddStatusMessage(typeof (SourcesModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(SourcesModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {
                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(SourcesModel).Name, statusMessage);
                    }

                    count++;
 
                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();
                    bool isRejected = false;
                    var source = new Source();

                    //set default values
                    string unk = "Unknown";
                    source.ContactName = unk;
                    source.Phone = unk;
                    source.Email = unk;
                    source.Phone = unk;
                    source.Address = unk;
                    source.City = unk;
                    source.State = unk;
                    source.ZipCode = unk;
                    source.Citation = unk;
                    source.MetadataID = 0;

                    var isometadata = new ISOMetadata();
                    isometadata.TopicCategory = unk;
                    isometadata.Title = unk;
                    isometadata.Abstract = unk;
                    isometadata.ProfileVersion = unk;
                    isometadata.MetadataLink = null;

                    //SourceCode
                    if (!string.IsNullOrWhiteSpace(item.SourceCode))
                    {
                        if (RepositoryUtils.containsNotOnlyAllowedCharacters(item.SourceCode))
                        {
                            var err = new ErrorModel("AddSource", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "SourceCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.SourceCode = item.SourceCode;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddSource", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SourceCode")); listOfErrors.Add(err); isRejected = true;
                    }	
                    //Organization
                    if (!string.IsNullOrWhiteSpace(item.Organization))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Organization))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Organization")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.Organization = item.Organization;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Organization")); listOfErrors.Add(err); isRejected = true;
                    }
                    //SourceDescription
                    if (!string.IsNullOrWhiteSpace(item.SourceDescription))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.SourceDescription))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "SourceDescription")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.SourceDescription = item.SourceDescription;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SourceDescription")); listOfErrors.Add(err); isRejected = true;
                    }
                    //SourceLink
                    if (!string.IsNullOrWhiteSpace(item.SourceLink))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.SourceLink))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "SourceLink")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.SourceLink = item.SourceLink;
                        }
                    }

                    //ContactName
                    if (!string.IsNullOrWhiteSpace(item.ContactName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.ContactName))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "ContactName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.ContactName = item.ContactName;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "ContactName")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Phone
                    if (!string.IsNullOrWhiteSpace(item.Phone))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Phone))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Phone")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.Phone = item.Phone;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Phone")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Email
                    if (!string.IsNullOrWhiteSpace(item.Email))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Email))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Email")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.Email = item.Email;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Email")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Address
                    if (!string.IsNullOrWhiteSpace(item.Address))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Address))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Address")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.Address = item.Address;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Address")); listOfErrors.Add(err); isRejected = true;
                    }
                    //City
                    if (!string.IsNullOrWhiteSpace(item.City))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.City))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "City")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.City = item.City;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "City")); listOfErrors.Add(err); isRejected = true;
                    }
                    //State
                    if (!string.IsNullOrWhiteSpace(item.State))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.State))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "State")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.State = item.State;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "State")); listOfErrors.Add(err); isRejected = true;
                    }
                    //ZipCode
                    if (!string.IsNullOrWhiteSpace(item.ZipCode))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.ZipCode))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "ZipCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.ZipCode = item.ZipCode;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "ZipCode")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Citation
                    if (!string.IsNullOrWhiteSpace(item.Citation))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Citation))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Citation")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            source.Citation = item.Citation;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Citation")); listOfErrors.Add(err); isRejected = true;
                    }


                    //TopicCategory
                    if (!string.IsNullOrWhiteSpace(item.TopicCategory))
                    {
                        var topicCategory = topicCategoryCV
                               .Exists(a => a.Term.ToString() == item.TopicCategory);
                        if (!topicCategory)
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.TopicCategory, "TopicCategory"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            isometadata.TopicCategory = item.TopicCategory;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "TopicCategory")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Title
                    if (!string.IsNullOrWhiteSpace(item.Title))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Title))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Title")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            isometadata.Title = item.Title;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Title")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Abstract
                    if (!string.IsNullOrWhiteSpace(item.Abstract))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Abstract))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Abstract")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            isometadata.Abstract = item.Abstract;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Abstract")); listOfErrors.Add(err); isRejected = true;
                    }
                    //ProfileVersion
                    if (!string.IsNullOrWhiteSpace(item.ProfileVersion))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.ProfileVersion))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "ProfileVersion")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            isometadata.ProfileVersion = item.ProfileVersion;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "ProfileVersion")); listOfErrors.Add(err); isRejected = true;
                    }
                    //MetadataLink
                    if (!string.IsNullOrWhiteSpace(item.MetadataLink))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.MetadataLink))
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "MetadataLink")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            isometadata.MetadataLink = item.MetadataLink;
                        }
                    }

                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(SourcesModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SourcesModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }

                    //Source contains info about metadata 
                    //need to be added first so I can get the ID
                    var existingIsometadataItem = context.ISOMetadatas
                        .Where(a => 
                            a.TopicCategory == item.TopicCategory &&
                            a.Title == item.Title &&
                            a.Abstract == item.Abstract &&
                            a.ProfileVersion == item.ProfileVersion &&
                            a.MetadataLink == item.MetadataLink)
                        .FirstOrDefault();

                    if (existingIsometadataItem == null)
                    {

                        context.ISOMetadatas.Add(isometadata);
                        //context.SaveChanges();
                    }
                    else
                    {
                        //set source metatdata id to id of existing item
                        source.MetadataID = existingIsometadataItem.MetadataID;                     
                  
                    }
                    //var metadataId = context.ISOMetadatas
                    //    .Where(a => a.MetadataLink == item.MetadataLink)
                    //    .Select(a => a.MetadataID)
                    //    .FirstOrDefault();
                    //var existingSourcesItem = context.Sources
                    //            .Where(a =>
                    //                a.SourceDescription == item.SourceDescription &&
                    //                a.SourceLink == item.SourceLink &&
                    //                a.ContactName == item.ContactName &&
                    //                a.Phone == item.Phone &&
                    //                a.Email == item.Email &&
                    //                a.Address == item.Address &&
                    //                a.City == item.City &&
                    //                a.State == item.State &&
                    //                a.ZipCode == item.ZipCode &&
                    //                a.Citation == item.Citation &&
                    //                a.ISOMetadata.TopicCategory == item.TopicCategory &&
                    //                a.ISOMetadata.Title == item.Title &&
                    //                a.ISOMetadata.Abstract == item.Abstract &&
                    //                a.ISOMetadata.ProfileVersion == item.ProfileVersion &&
                    //                a.ISOMetadata.MetadataLink == item.MetadataLink                                    
                    //                ).FirstOrDefault();

                    var existingSourcesItem = context.Sources
                               .Where(a => a.SourceCode == source.SourceCode).FirstOrDefault();

                    if (existingSourcesItem == null)
                    {

                        var existInUpload = listOfCorrectRecords.Exists(a => a.SourceCode == item.SourceCode);
                        if (!existInUpload)
                        {
                            //update model
                            source.MetadataID = isometadata.MetadataID;

                            context.Sources.Add(source);
                            //context.SaveChanges();                     

                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SourcesModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddSources", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE, "SourceCode")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SourcesModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(SourcesModel).Name, statMsg);
                            }
                        }


                    }
                    else
                    {
                        if (existingSourcesItem.SourceCode != source.SourceCode) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "SourcesCode", existingSourcesItem.SourceCode.ToString(), item.SourceCode.ToString())); }
                        if (source.Organization != null && existingSourcesItem.Organization != source.Organization) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "Organization", existingSourcesItem.Organization.ToString(), item.Organization.ToString())); }
                        if (source.SourceDescription != null && existingSourcesItem.SourceDescription != source.SourceDescription) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "SourceDescription", existingSourcesItem.SourceDescription.ToString(), item.SourceDescription.ToString())); }
                        if (source.SourceLink != null && existingSourcesItem.SourceLink != source.SourceLink) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "SourceLink", existingSourcesItem.SourceLink.ToString(), item.SourceLink.ToString())); }
                        if (source.ContactName != null && existingSourcesItem.ContactName != source.ContactName) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "ContactName", existingSourcesItem.ContactName.ToString(), item.ContactName.ToString())); }
                        if (source.Phone != null && existingSourcesItem.Phone != source.Phone) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "Phone", existingSourcesItem.Phone.ToString(), item.Phone.ToString())); }
                        if (source.Email != null && existingSourcesItem.Email != source.Email) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "Email", existingSourcesItem.Email.ToString(), item.Email.ToString())); }
                        if (source.Address != null && existingSourcesItem.Address != source.Address) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "Address", existingSourcesItem.Address.ToString(), item.Address.ToString())); }
                        if (source.City != null && existingSourcesItem.City != source.City) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "City", existingSourcesItem.City.ToString(), item.City.ToString())); }
                        if (source.State != null && existingSourcesItem.State != source.State) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "State", existingSourcesItem.State.ToString(), item.State.ToString())); }
                        if (source.ZipCode != null && existingSourcesItem.ZipCode != source.ZipCode) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "ZipCode", existingSourcesItem.ZipCode.ToString(), item.ZipCode.ToString())); }
                        if (source.Citation != null && existingSourcesItem.Citation != source.Citation) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "Citation", existingSourcesItem.Citation.ToString(), item.Citation.ToString())); }
                        if (isometadata.TopicCategory != null && existingSourcesItem.ISOMetadata.TopicCategory != isometadata.TopicCategory) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "TopicCategory", existingSourcesItem.ISOMetadata.TopicCategory.ToString(), item.TopicCategory.ToString())); }
                        if (isometadata.Title != null && existingSourcesItem.ISOMetadata.Title != isometadata.Title) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "Title", existingSourcesItem.ISOMetadata.Title.ToString(), item.Title.ToString())); }
                        if (isometadata.Abstract != null && existingSourcesItem.ISOMetadata.Abstract != isometadata.Abstract) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "Abstract", existingSourcesItem.ISOMetadata.Abstract.ToString(), item.Abstract.ToString())); }
                        if (isometadata.ProfileVersion != null && existingSourcesItem.ISOMetadata.ProfileVersion != isometadata.ProfileVersion) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "ProfileVersion", existingSourcesItem.ISOMetadata.ProfileVersion.ToString(), item.ProfileVersion.ToString())); }
                        if (isometadata.MetadataLink != null && existingSourcesItem.ISOMetadata.MetadataLink != isometadata.MetadataLink) { listOfUpdates.Add(new UpdateFieldsModel("Sources", "MetadataLink", existingSourcesItem.ISOMetadata.MetadataLink.ToString(), item.MetadataLink.ToString())); }

                        if (listOfUpdates.Count() > 0)
                        {
                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SourcesModel).Name, 0, 1, 0, 0);
                            }

                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var erMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(erMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(SourcesModel).Name, erMessage);
                                }
                            }
                            item.Errors += sb.ToString();

                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SourcesModel).Name, 0, 0, 0, 1);
                            }
                        }
                    }
                   

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SourcesModel).Name, 0, 0, 1, 0);
                    }
                }
            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(SourcesModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var ISOMetadataRows = from o in context.ISOMetadatas
                                  where o.MetadataID != 0
                       select o;

            var sourcesRows = from o in context.Sources                              
                       select o;

            if (ISOMetadataRows.Count() == 0 && sourcesRows.Count() == 0) return;
           
            try
            {
                context.ISOMetadatas.RemoveRange(ISOMetadataRows);
                context.Sources.RemoveRange(sourcesRows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }
           
            

        }
    }

    //  Methods
    public class MethodsRepository : IMethodsRepository
    {
        public const string CacheName = "default";

        public List<MethodModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.Methods
                        select obj;
            var modelList = new List<MethodModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.Method, HydroserverToolsBusinessObjects.Models.MethodModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<MethodModel> GetMethods(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<MethodModel>();

            //BC - Test - 02-Dec-2017 - Test EntityFramework.metadata package...
            var methodData = context.Db<Method>();

            var tableName = methodData.TableName;

            if (0 < context.Methods.Count())
            {
                totalRecordCount = context.Methods.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.Methods.ToList();
                var rst = allItems.
                    Where(c =>
                                   c.MethodCode != null && c.MethodCode.ToString().ToLower().Contains(searchString.ToLower())
                                || c.MethodDescription != null && c.MethodDescription.ToLower().Contains(searchString.ToLower())
                                || c.MethodLink != null && c.MethodLink.ToLower().Contains(searchString.ToLower())
                                );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<Method, MethodModel>(item);

                    result.Add(model);
                }
            }

            else
            {
                List<Method> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Methods.OrderBy(a => a.MethodCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Methods.OrderByDescending(a => a.MethodCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Methods.OrderBy(a => a.MethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Methods.OrderByDescending(a => a.MethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Methods.OrderBy(a => a.MethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Methods.OrderByDescending(a => a.MethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                            break;

                    }
                }

                if (sortedItems == null) sortedItems = context.Methods.OrderByDescending(a => a.MethodCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<Method, MethodModel>(item);

                    //model.LatLongDatumSRSName = context.SpatialReferences
                    //                     .Where(a => a.SpatialReferenceID == item.LatLongDatumID)
                    //                     .Select(a => a.SRSName)
                    //                     .FirstOrDefault();


                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddMethods(List<MethodModel> itemList, string entityConnectionString, string instanceIdentifier, List<MethodModel> listOfIncorrectRecords, List<MethodModel> listOfCorrectRecords, List<MethodModel> listOfDuplicateRecords, List<MethodModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("MethodsRepository.AddMethods(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //var objContext = ((IObjectContextAdapter)context).ObjectContext;
            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "Method");

            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (MethodModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(MethodModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {
                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof (MethodModel).Name, statusMessage);
                    }

                    count++;

                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();

                    bool isRejected = false;
                    var model = new Method();
                    model.MethodLink = null;

                    //MethodCode
                    if (!string.IsNullOrWhiteSpace(item.MethodCode))
                    {
                        if (RepositoryUtils.containsNotOnlyAllowedCharacters(item.MethodCode))
                        {
                            var err = new ErrorModel("AddMethod", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "MethodCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.MethodCode = item.MethodCode;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddMethod", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "MethodCode")); listOfErrors.Add(err); isRejected = true;
                    }

                    //MethodDescription
                    if (!string.IsNullOrWhiteSpace(item.MethodDescription))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.MethodDescription))
                        {
                            var err = new ErrorModel("AddMethods", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "MethodDescription")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.MethodDescription = item.MethodDescription;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddMethods", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "MethodDescription")); listOfErrors.Add(err); isRejected = true;
                    }
                    //MethodLink
                    if (!string.IsNullOrWhiteSpace(item.MethodLink))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.MethodLink))
                        {
                            var err = new ErrorModel("AddMethods", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "MethodLink")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.MethodLink = item.MethodLink;
                        }
                    }

                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof (MethodModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(MethodModel).Name, 0, 0, 1, 0);

                        }

                        continue;
                    }

                    //var existingItem = context.Methods.Where(a => a.MethodDescription == model.MethodDescription &&
                    //                                                a.MethodLink == model.MethodLink
                    //                                                ).FirstOrDefault();

                    var existingItem = context.Methods.Where(a => a.MethodCode == model.MethodCode).FirstOrDefault();

                    if (existingItem == null)
                    {
                        var existInUpload = listOfCorrectRecords.Exists(a => a.MethodCode == item.MethodCode);
                        if (!existInUpload)
                        {
                            //context.Sites.Add(model);
                            //context.SaveChanges();
                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(MethodModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddMethod", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE, "MethodCode")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(MethodModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(MethodModel).Name, statMsg);
                            }
                        }
                    }
                    else
                    {
                        //if (existingItem.MethodCode != model.MethodCode) { listOfUpdates.Add(new UpdateFieldsModel("Method", "MethodCode", existingItem.MethodCode.ToString(), item.MethodCode.ToString())); }
                        if (model.MethodDescription != null && existingItem.MethodDescription != model.MethodDescription) { listOfUpdates.Add(new UpdateFieldsModel("Method", "MethodDescription", existingItem.MethodDescription.ToString(), item.MethodDescription.ToString())); }
                        if (model.MethodLink != null && existingItem.MethodLink != model.MethodLink) { listOfUpdates.Add(new UpdateFieldsModel("Method", "MethodLink", existingItem.MethodLink.ToString(), item.MethodLink.ToString())); }

                        if (listOfUpdates.Count() > 0)
                        {
                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(MethodModel).Name, 0, 1, 0, 0);
                            }

                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var errorMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(errorMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(MethodModel).Name, errorMessage);
                                }
                            }
                            item.Errors += sb.ToString();

                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(MethodModel).Name, 0, 0, 0, 1);
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(MethodModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(MethodModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Methods
                       where o.MethodID != 0
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.Methods.Remove(row);
            //}
            try
            {
                context.Methods.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }

    //  LabMethods
    public class LabMethodsRepository : ILabMethodsRepository
    {
        public const string CacheName = "default";

        public List<LabMethodModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.LabMethods
                        select obj;
            var modelList = new List<LabMethodModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.LabMethod, HydroserverToolsBusinessObjects.Models.LabMethodModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<LabMethodModel> GetLabMethods(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<LabMethodModel>();

            //if (context.LabMethods.Count() != null)
            if ( 0 < context.LabMethods.Count())
            {
                    totalRecordCount = context.LabMethods.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.LabMethods.ToList();
                var rst = allItems.
                    Where(c =>
                                    c.LabName != null && c.LabName.ToLower().Contains(searchString.ToLower())
                                 || c.LabOrganization != null && c.LabOrganization.ToLower().Contains(searchString.ToLower())
                                 || c.LabMethodName != null && c.LabMethodName.ToLower().Contains(searchString.ToLower())
                                 || c.LabMethodDescription != null && c.LabMethodDescription.ToLower().Contains(searchString.ToLower())
                                 || c.LabMethodLink != null && c.LabMethodLink.ToLower().Contains(searchString.ToLower())
                                );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<LabMethod, LabMethodModel>(item);

                    result.Add(model);
                }
            }

            else
            {
                List<LabMethod> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabOrganization).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabOrganization).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabMethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "4":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabMethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                            break;

                    }
                }

                if (sortedItems == null) sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<LabMethod, LabMethodModel>(item);



                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddLabMethods(List<LabMethodModel> itemList, string entityConnectionString, string instanceIdentifier, List<LabMethodModel> listOfIncorrectRecords, List<LabMethodModel> listOfCorrectRecords, List<LabMethodModel> listOfDuplicateRecords, List<LabMethodModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("LabMethodsRepository.AddLabMethods(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //prefetch Units for quick lookup
            var offsetUnits = context.Units.ToDictionary(p => p.UnitsName.Trim(), p => p.UnitsID);

            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "LabMethods");

            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (LabMethodModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(LabMethodModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {

                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(LabMethodModel).Name, statusMessage);
                    }

                    count++;

                    var model = new LabMethod();
                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();

                    bool isRejected = false;
                    //set default values
                    string unk = "Unknown";
                    model.LabName = unk;
                    model.LabOrganization = unk;
                    model.LabMethodName = unk;
                    model.LabMethodDescription = unk;

                    ////LabMethodCode
                    //if (!string.IsNullOrWhiteSpace(item.LabMethodCode))
                    //{
                    //    if (RepositoryUtils.containsNotOnlyAllowedCaracters(item.LabMethodCode))
                    //    {
                    //        var err = new ErrorModel("AddLabMethod", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LabMethodCode")); listOfErrors.Add(err); isRejected = true;
                    //    }
                    //    else
                    //    {
                    //        model.LabMethodCode = item.LabMethodCode;
                    //    }

                    //}
                    //else
                    //{
                    //    var err = new ErrorModel("AddLabMethod", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "LabMethodCode")); listOfErrors.Add(err); isRejected = true;
                    //}	

                    //LabName
                    if (!string.IsNullOrWhiteSpace(item.LabName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LabName))
                        {
                            var err = new ErrorModel("AddLabMethods", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LabName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.LabName = item.LabName;
                        }
                    }
                    else
                    {
                        item.LabName = model.LabName;
                    }
                    //LabOrganization
                    if (!string.IsNullOrWhiteSpace(item.LabOrganization))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LabOrganization))
                        {
                            var err = new ErrorModel("AddLabMethods", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LabOrganization")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.LabOrganization = item.LabOrganization;
                        }
                    }
                    else
                    {
                        item.LabOrganization = model.LabOrganization;
                    }
                    //LabMethodName
                    if (!string.IsNullOrWhiteSpace(item.LabMethodName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LabMethodName))
                        {
                            var err = new ErrorModel("AddLabMethods", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LabMethodName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.LabMethodName = item.LabMethodName;
                        }
                    }
                    else
                    {
                        item.LabMethodName = model.LabMethodName;
                    }
                    //LabMethodDescription
                    if (!string.IsNullOrWhiteSpace(item.LabMethodDescription))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LabMethodDescription))
                        {
                            var err = new ErrorModel("AddLabMethods", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LabMethodDescription")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.LabMethodDescription = item.LabMethodDescription;
                        }
                    }
                    else
                    {
                        item.LabMethodDescription = model.LabMethodDescription;
                    }
                    //LabMethodLink
                    if (!string.IsNullOrWhiteSpace(item.LabMethodLink))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LabMethodLink))
                        {
                            var err = new ErrorModel("AddLabMethods", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LabMethodLink")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.LabMethodLink = item.LabMethodLink;
                        }
                    }

                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(LabMethodModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(LabMethodModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }

                    //lookup duplicates
                    var existingItem = context.LabMethods
                                                 .Where(
                                                     a => a.LabMethodName == model.LabMethodName)
                                                          .FirstOrDefault();

                    if (existingItem == null)
                    {
                        var existInUpload = listOfCorrectRecords.Exists(a => a.LabMethodName == item.LabMethodName);
                        if (!existInUpload)
                        {
                            //context.Sites.Add(model);
                            //context.SaveChanges();
                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(LabMethodModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddLabMethods", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE, "LabMethodName")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(LabMethodModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(LabMethodModel).Name, statMsg);
                            }
                        }
                    }
                    else
                    {
                        if (existingItem.LabMethodName != model.LabMethodName) { listOfUpdates.Add(new UpdateFieldsModel("LabMethod", "LabMethodName", existingItem.LabMethodName.ToString(), item.LabMethodName.ToString())); }
                        if (model.LabName != null && existingItem.LabName != model.LabName) { listOfUpdates.Add(new UpdateFieldsModel("LabMethod", "LabName", existingItem.LabName.ToString(), item.LabName.ToString())); }
                        if (model.LabOrganization != null && existingItem.LabOrganization != model.LabOrganization) { listOfUpdates.Add(new UpdateFieldsModel("LabMethod", "LabOrganization", existingItem.LabOrganization.ToString(), item.LabOrganization.ToString())); }
                        //if (model.LabMethodName != null && existingItem.LabMethodName != model.LabMethodName) { listOfUpdates.Add(new UpdateFieldsModel("LabMethod", "LabMethodName", existingItem.LabMethodCode.ToString(), item.LabMethodName.ToString())); }
                        if (model.LabMethodDescription != null && existingItem.LabMethodDescription != model.LabMethodDescription) { listOfUpdates.Add(new UpdateFieldsModel("LabMethod", "LabMethodDescription", existingItem.LabMethodDescription.ToString(), item.LabMethodDescription.ToString())); }
                        if (model.LabMethodLink != null && existingItem.LabMethodLink != model.LabMethodLink) { listOfUpdates.Add(new UpdateFieldsModel("LabMethod", "LabMethodLink", existingItem.LabMethodLink.ToString(), item.LabMethodLink.ToString())); }
                      
                        
                        if (listOfUpdates.Count() > 0)
                        {
                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(LabMethodModel).Name, 0, 1, 0, 0);
                            }

                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var erMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(erMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(LabMethodModel).Name, erMessage);
                                }
                            }
                            item.Errors = sb.ToString();

                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(LabMethodModel).Name, 0, 0, 0, 1);
                            }
                        }
                    }

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(LabMethodModel).Name, 0, 0, 1, 0);
                    }
                }
            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(LabMethodModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.LabMethods
                       where o.LabMethodID != 0
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.LabMethods.Remove(row);
            //}
            try
            {
                context.LabMethods.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }

    //  Samples
    public class SamplesRepository : ISamplesRepository
    {
        public const string CacheName = "default";

        public List<SampleModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.Samples
                        select obj;

            var LabMethods = context.LabMethods.ToDictionary(p => p.LabMethodID, p => p.LabMethodName.Trim());
            var modelList = new List<SampleModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.Sample, HydroserverToolsBusinessObjects.Models.SampleModel>(item);
                if (LabMethods.ContainsKey(item.LabMethodID))
                {
                    var labMethodsName = LabMethods[item.LabMethodID];
                    //update model
                    model.LabMethodName = labMethodsName;
                }
                modelList.Add(model);
            }
            return modelList;
        }

        public List<SampleModel> GetSamples(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<SampleModel>();

            //if (context.Samples.Count() != null)
            if (0 < context.Samples.Count())
            {
                totalRecordCount = context.Samples.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.Samples.ToList();
                var rst = allItems.
                    Where(c =>      c.SampleType != null && c.SampleType.ToLower().Contains(searchString.ToLower())
                                 || c.LabSampleCode != null && c.LabSampleCode.ToLower().Contains(searchString.ToLower())
                                 || c.LabMethod != null && c.LabMethod.LabMethodName.Contains(searchString.ToLower())
                                 );
                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<Sample, SampleModel>(item);

                    //model.LabSampleCode = context.Samples
                    //                .Where(a => a.SampleID == item.SampleID)
                    //                .Select(a => a.LabSampleCode)
                    //                .FirstOrDefault();

                    model.LabMethodName = context.LabMethods
                                     .Where(a => a.LabMethodID == item.LabMethodID)
                                     .Select(a => a.LabMethodName)
                                     .FirstOrDefault();

                    result.Add(model);
                }
            }

            else
            {
                List<Sample> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        //case "0":
                        //    if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                        //    { sortedItems = context.Samples.OrderBy(a => a.SampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                        //    else
                        //    { sortedItems = context.Samples.OrderByDescending(a => a.SampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                        //    break;
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Samples.OrderBy(a => a.SampleType).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Samples.OrderByDescending(a => a.SampleType).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Samples.OrderBy(a => a.LabSampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Samples.OrderByDescending(a => a.LabSampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Samples.OrderBy(a => a.LabMethod.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Samples.OrderByDescending(a => a.LabMethod.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Samples.OrderBy(a => a.LabMethod.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Samples.OrderByDescending(a => a.LabMethod.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;

                    }
                }

                if (sortedItems == null) sortedItems = context.Samples.OrderByDescending(a => a.LabSampleCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<Sample, SampleModel>(item);

                    //model.LabSampleCode = context.Samples
                    //             .Where(a => a.SampleID == item.SampleID)
                    //             .Select(a => a.LabSampleCode)
                    //             .FirstOrDefault();

                    model.LabMethodName = context.LabMethods
                                     .Where(a => a.LabMethodID == item.LabMethodID)
                                     .Select(a => a.LabMethodName)
                                     .FirstOrDefault();




                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddSamples(List<SampleModel> itemList, string entityConnectionString, string instanceIdentifier, List<SampleModel> listOfIncorrectRecords, List<SampleModel> listOfCorrectRecords, List<SampleModel> listOfDuplicateRecords, List<SampleModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("SamplesRepository.AddSamples(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //prefetch Units for quick lookup
            //var labMethods = context.LabMethods.ToList();
            var sampleTypeCV = context.SampleTypeCVs.ToList();
            var labMethods = context.LabMethods.ToDictionary(p => p.LabMethodName, p => p.LabMethodID);
            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "Samples");
            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof(SampleModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(SampleModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {

                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(SampleModel).Name, statusMessage);
                    }

                    count++;

                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();

                    bool isRejected = false;
                    var model = new Sample();

                    //set default values
                    string unk = "Unknown";
                    model.SampleType = unk;
                    model.LabMethodID = 0;

                    
                    //SampleType
                    if (!string.IsNullOrWhiteSpace(item.SampleType))
                    {
                        var sampleType = sampleTypeCV
                               .Exists(a => a.Term.ToString() == item.SampleType);
                        if (!sampleType)
                        {
                            var err = new ErrorModel("AddSamples", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.SampleType, "SampleType"));
                            listOfErrors.Add(err);
                            isRejected = true;
                        }
                        else
                        {
                            model.SampleType = item.SampleType;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSamples", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SampleType")); listOfErrors.Add(err); isRejected = true;
                    }

                    //LabSampleCode
                    if (!string.IsNullOrWhiteSpace(item.LabSampleCode))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LabSampleCode))
                        {
                            var err = new ErrorModel("AddSamples", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LabSampleCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {                          
                           model.LabSampleCode = item.LabSampleCode;                            
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddSamples", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "LabSampleCode")); listOfErrors.Add(err); isRejected = true;
                    }

                    //LabMethodID
                    if (!string.IsNullOrWhiteSpace(item.LabMethodName))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.LabMethodName))
                        {
                            var err = new ErrorModel("AddSamples", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "LabMethodName")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            if (labMethods.ContainsKey(item.LabMethodName))
                               {
                                   var labMethodId = labMethods[item.LabMethodName];
                                        //update model
                                        model.LabMethodID = labMethodId;
                                        item.LabMethodID = labMethodId.ToString();
                                    }
                                    else
                                    {
                                        var err = new ErrorModel("AddSamples", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.LabMethodName, "LabMethodName")); listOfErrors.Add(err); isRejected = true;
                                    }
                                }                               
                                                           
                        
                    }
                    else
                    {
                        var err = new ErrorModel("AddSamples", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "LabMethodName")); listOfErrors.Add(err); isRejected = true;
                    }                    
                    
                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(SampleModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SampleModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }
                    //need to look up Id's for LabMethodId
                    //User has no concept of ID's
                    //lookup LabmethodId
                    //if (item.LabMethodName != null)
                    //{
                    //    if (labMethods.ContainsKey(item.LabMethodName))
                    //    {
                    //        var labMethodsId = labMethods[item.LabMethodName];
                    //        //update model
                    //        model.LabMethodID = labMethodsId;
                    //    }
                    //    else
                    //    {
                    //        //if CSV has LabMethidId specified convert and process
                    //        int labMethodId;
                    //        bool res = int.TryParse(item.LabMethodName, out labMethodId);
                    //        if (res)
                    //        {
                    //            //update model
                    //            model.LabMethodID = labMethodId;
                    //        }
                    //        else
                    //        {
                    //            listOfIncorrectRecords.Add(item);
                    //            continue;

                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    listOfIncorrectRecords.Add(item);
                    //    continue;
                    //}

                    //lookup duplicates
                    //var existingItem = context.Samples.Where(a => a.SampleType == model.SampleType &&
                    //                                              a.LabSampleCode == model.LabSampleCode &&
                    //                                              a.LabMethodID == a.LabMethodID
                    //                                              ).FirstOrDefault();

                    var existingItem = context.Samples.Where(a => a.LabSampleCode == model.LabSampleCode).FirstOrDefault();

                    if (existingItem == null)
                    {
                        var existInUpload = listOfCorrectRecords.Exists(a => a.LabSampleCode == item.LabSampleCode);
                        if (!existInUpload)
                        {
                            //context.Sites.Add(model);
                            //context.SaveChanges();
                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SampleModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddSample", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE, "LabSampleCode")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SampleModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(SampleModel).Name, statMsg);
                            }
                        }
                    }
                    else
                    {
                        //if (existingItem.LabSampleCode != model.LabSampleCode) { listOfUpdates.Add(new UpdateFieldsModel("Sample", "LabSampleCode", existingItem.LabSampleCode.ToString(), item.LabSampleCode.ToString())); }
                        if (model.SampleType != null && existingItem.SampleType != model.SampleType) { listOfUpdates.Add(new UpdateFieldsModel("Sample", "SampleType", existingItem.SampleType.ToString(), item.SampleType.ToString())); }
                        if (existingItem.LabMethodID != model.LabMethodID) { listOfUpdates.Add(new UpdateFieldsModel("Sample", "LabMethodID", existingItem.LabMethodID.ToString(), item.LabMethodID.ToString())); }

                        if (listOfUpdates.Count() > 0)
                        {
                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SampleModel).Name, 0, 1, 0, 0);
                            }

                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var erMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(erMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(SampleModel).Name, erMessage);
                                }
                            }
                            item.Errors = sb.ToString();

                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SampleModel).Name, 0, 0, 0, 1);
                            }
                        }
                    }

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(SampleModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(SampleModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Samples
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.Samples.Remove(row);
            //}
            try
            {
                context.Samples.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }

    //  Qualifiers
    public class QualifiersRepository : IQualifiersRepository
    {
        public const string CacheName = "default";

        public List<QualifiersModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.Qualifiers
                        select obj;
            var modelList = new List<QualifiersModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.Qualifier, HydroserverToolsBusinessObjects.Models.QualifiersModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<QualifiersModel> GetQualifiers(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<QualifiersModel>();

            //if (context.Qualifiers.Count() != null)
            if ( 0 < context.Qualifiers.Count())
            {
                totalRecordCount = context.Qualifiers.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.Qualifiers.ToList();
                var rst = allItems.
                 Where(c =>
                                c.QualifierCode != null && c.QualifierCode.ToLower().Contains(searchString.ToLower())
                             || c.QualifierDescription != null && c.QualifierDescription.ToLower().Contains(searchString.ToLower())
                      );
                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<Qualifier, QualifiersModel>(item);

                    result.Add(model);
                }
            }

            else
            {
                List<Qualifier> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                       
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Qualifiers.OrderBy(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Qualifiers.OrderByDescending(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Qualifiers.OrderBy(a => a.QualifierDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Qualifiers.OrderByDescending(a => a.QualifierDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        default:
                            sortedItems = context.Qualifiers.OrderByDescending(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList();
                            break;
                    }
                }

                if (sortedItems == null) sortedItems = context.Qualifiers.OrderByDescending(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<Qualifier, QualifiersModel>(item);



                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddQualifiers(List<QualifiersModel> itemList, string entityConnectionString, string instanceIdentifier, List<QualifiersModel> listOfIncorrectRecords, List<QualifiersModel> listOfCorrectRecords, List<QualifiersModel> listOfDuplicateRecords, List<QualifiersModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("QualifiersRepository.AddQualifiers(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "Qualifiers");
            if ( null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (QualifiersModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(QualifiersModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {

                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(QualifiersModel).Name, statusMessage);
                    }

                    count++;

                    var model = new Qualifier();
                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();

                    bool isRejected = false;

                    model.QualifierCode = null;
                    model.QualifierDescription = null;

                    //QualifierCode
                    if (!string.IsNullOrWhiteSpace(item.QualifierCode))
                    {
                        if (RepositoryUtils.containsNotOnlyAllowedCharacters(item.QualifierCode))
                        {
                            var err = new ErrorModel("AddQualifiers", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "QualifierCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.QualifierCode = item.QualifierCode;
                        }

                    }
                    
                    //QualifierDescription
                    if (!string.IsNullOrWhiteSpace(item.QualifierDescription))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.QualifierDescription))
                        {
                            var err = new ErrorModel("AddQualifiers", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "QualifierDescription")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.QualifierDescription = item.QualifierDescription;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddQualifiers", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "QualifierDescription")); listOfErrors.Add(err); isRejected = true;
                    }


                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(QualifiersModel).Name, er.ErrorMessage);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualifiersModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }

                    //lookup duplicates
                    var existingItem = context.Qualifiers.Where(a => a.QualifierCode == model.QualifierCode).FirstOrDefault();

                    if (existingItem == null)
                    {
                        var existInUpload = listOfCorrectRecords.Exists(a => a.QualifierCode == item.QualifierCode);
                        if (!existInUpload)
                        {
                            context.Qualifiers.Add(model);
                            //context.SaveChanges();
                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualifiersModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddQualifiers", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE, "QualifierCode")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualifiersModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(QualifiersModel).Name, statMsg);
                            }
                        }


                    }
                    else
                    {
                        //if (existingItem.QualifierCode != model.QualifierCode) { listOfUpdates.Add(new UpdateFieldsModel("Qualifiers", "QualifierCode", existingItem.QualifierCode.ToString(), item.QualifierCode.ToString())); }
                        if (model.QualifierDescription != null && existingItem.QualifierDescription != model.QualifierDescription) { listOfUpdates.Add(new UpdateFieldsModel("Qualifiers", "QualifierDescription", existingItem.QualifierDescription.ToString(), item.QualifierDescription.ToString())); }

                        if (listOfUpdates.Count() > 0)
                        {
                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualifiersModel).Name, 0, 1, 0, 0);
                            }

                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var erMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(erMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(QualifiersModel).Name, erMessage);
                                }
                            }
                            item.Errors += sb.ToString();

                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualifiersModel).Name, 0, 0, 0, 1);
                            }
                        }
                    }

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualifiersModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(QualifiersModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Qualifiers
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.Qualifiers.Remove(row);
            //}
            try
            {
                context.Qualifiers.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }
    //  QualityControlLevels
    public class QualityControlLevelsRepository : IQualityControlLevelsRepository
    {
        public const string CacheName = "default";

        public List<QualityControlLevelModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.QualityControlLevels
                        select obj;
            var modelList = new List<QualityControlLevelModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.QualityControlLevel, HydroserverToolsBusinessObjects.Models.QualityControlLevelModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<QualityControlLevelModel> GetQualityControlLevels(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<QualityControlLevelModel>();

            //if (context.QualityControlLevels.Count() != null)
            if (0 < context.QualityControlLevels.Count())
            {
                totalRecordCount = context.QualityControlLevels.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.QualityControlLevels.ToList();
                var rst = allItems.
                    Where(c =>   //c.QualityControlLevelID.ToLower().Contains(searchString.ToLower())
                                    c.QualityControlLevelCode != null && c.QualityControlLevelCode.ToLower().Contains(searchString.ToLower())
                                 || c.Definition != null && c.Definition.ToLower().Contains(searchString.ToLower())
                                 || c.Explanation != null && c.Explanation.ToLower().Contains(searchString.ToLower())
                          );
                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<QualityControlLevel, QualityControlLevelModel>(item);

                    result.Add(model);
                }
            }
            else
            {
                List<QualityControlLevel> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.QualityControlLevels.OrderBy(a => a.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.QualityControlLevels.OrderByDescending(a => a.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.QualityControlLevels.OrderBy(a => a.Definition).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.QualityControlLevels.OrderByDescending(a => a.Definition).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.QualityControlLevels.OrderBy(a => a.Explanation).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.QualityControlLevels.OrderByDescending(a => a.Explanation).Skip(startIndex).Take(pageSize).ToList(); }
                            break;

                    }
                }

                if (sortedItems == null) sortedItems = context.QualityControlLevels.OrderByDescending(a => a.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<QualityControlLevel, QualityControlLevelModel>(item);

                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddQualityControlLevels(List<QualityControlLevelModel> itemList, string entityConnectionString, string instanceIdentifier, List<QualityControlLevelModel> listOfIncorrectRecords, List<QualityControlLevelModel> listOfCorrectRecords, List<QualityControlLevelModel> listOfDuplicateRecords, List<QualityControlLevelModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("QualityControlLevelsRepository.AddQualityControlLevels(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "QualityControlLevels");

            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (QualityControlLevelModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(QualityControlLevelModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {
                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if ( null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof (QualityControlLevelModel).Name, statusMessage);
                    }

                    count++;
                    
                    var model = new QualityControlLevel();
                    var listOfErrors = new List<ErrorModel>();
                    var listOfUpdates = new List<UpdateFieldsModel>();

                    bool isRejected = false;
                    count++;

                    //QualityControlLevelCode
                    if (!string.IsNullOrWhiteSpace(item.QualityControlLevelCode))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.QualityControlLevelCode))
                        {
                            var err = new ErrorModel("AddQualityControlLevel", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "QualityControlLevelCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.QualityControlLevelCode = item.QualityControlLevelCode;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddQualityControlLevel", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "QualityControlLevelCode")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Definition
                    if (!string.IsNullOrWhiteSpace(item.Definition))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Definition))
                        {
                            var err = new ErrorModel("AddQualityControlLevel", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Definition")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.Definition = item.Definition;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddQualityControlLevel", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Definition")); listOfErrors.Add(err); isRejected = true;
                    }
                    //Explanation
                    if (!string.IsNullOrWhiteSpace(item.Explanation))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.Explanation))
                        {
                            var err = new ErrorModel("AddQualityControlLevel", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "Explanation")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.Explanation = item.Explanation;
                        }

                    }
                    else
                    {
                        var err = new ErrorModel("AddQualityControlLevel", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "Explanation")); listOfErrors.Add(err); isRejected = true;
                    }


                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(QualityControlLevelModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualityControlLevelModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }

                    //lookup duplicates
                    var existingItem = context.QualityControlLevels.Where(a => a.QualityControlLevelCode == model.QualityControlLevelCode
                                                                               //&& a.Definition == model.Definition &&
                                                                               //a.Explanation == model.Explanation
                                                                               ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        var existInUpload = listOfCorrectRecords.Exists(a => a.QualityControlLevelCode == item.QualityControlLevelCode);
                        if (!existInUpload)
                        {
                            context.QualityControlLevels.Add(model);
                            //context.SaveChanges();
                            listOfCorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualityControlLevelModel).Name, 1, 0, 0, 0);
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddQualityControlLevel", string.Format(Resources.IMPORT_VALUE_ISDUPLICATE, "QualityControlLevelCode")); listOfErrors.Add(err); isRejected = true;
                            listOfIncorrectRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualityControlLevelModel).Name, 0, 0, 1, 0);
                            }

                            item.Errors += err.ErrorMessage + ";";
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count - 1;
                                StatusMessage statMsg = new StatusMessage(err.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(QualityControlLevelModel).Name, statMsg);
                            }
                        }
                    }
                    else
                    {
                        //if (existingItem.QualityControlLevelCode != model.QualityControlLevelCode) { listOfUpdates.Add(new UpdateFieldsModel("QualityControlLevels", "QualityControlLevelCode", existingItem.QualityControlLevelCode.ToString(), item.QualityControlLevelCode.ToString())); }
                        if (model.Definition != null && existingItem.Definition != model.Definition) { listOfUpdates.Add(new UpdateFieldsModel("QualityControlLevels", "Definition", existingItem.Definition.ToString(), item.Definition.ToString())); }
                        if (model.Explanation != null && existingItem.Explanation != model.Explanation) { listOfUpdates.Add(new UpdateFieldsModel("QualityControlLevels", "Explanation", existingItem.Explanation.ToString(), item.Explanation.ToString())); }
                       
                        if (listOfUpdates.Count() > 0)
                        {
                            listOfEditedRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualityControlLevelModel).Name, 0, 1, 0, 0);
                            }

                            var sb = new StringBuilder();
                            foreach (var u in listOfUpdates)
                            {
                                var erMessage = string.Format(Resources.IMPORT_VALUE_UPDATED, u.ColumnName, u.CurrentValue, u.UpdatedValue);
                                sb.Append(erMessage + ";");
                                if (null != statusContext)
                                {
                                    await statusContext.AddStatusMessage(typeof(QualityControlLevelModel).Name, erMessage);
                                }
                            }
                            item.Errors = sb.ToString();

                            continue;
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                            if (null != statusContext)
                            {
                                await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualityControlLevelModel).Name, 0, 0, 0, 1);
                            }
                        }
                    }

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(QualityControlLevelModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(QualityControlLevelModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            var rows = from o in context.QualityControlLevels
                       where o.QualityControlLevelCode != "-9999"
                             && o.QualityControlLevelCode != "0"
                             && o.QualityControlLevelCode != "1"
                             && o.QualityControlLevelCode != "2"
                             && o.QualityControlLevelCode != "3"
                             && o.QualityControlLevelCode != "4"

                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.QualityControlLevels.Remove(row);
            //}
            try
            {
                context.QualityControlLevels.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }
    //  DataValues
    public class DataValuesRepository : IDataValuesRepository
    {
        public const string CacheName = "default";

        
        public List<DataValuesModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.DataValues
                        select obj;
            var modelList = new List<DataValuesModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.DataValue, HydroserverToolsBusinessObjects.Models.DataValuesModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<DataValuesModel> GetDatavalues(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<DataValuesModel>();

            //if (context.DataValues.Count() != null)
            if (0 < context.DataValues.Count())
            {
                totalRecordCount = context.DataValues.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                //var allItems = context.DataValues.ToList();
                var rst = context.DataValues.
                    Where(c =>
                                c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.DataValue1.ToString().ToLower().Contains(searchString.ToLower())
                             || c.Site.SiteCode != null && c.Site.SiteName.ToLower().Contains(searchString.ToLower())
                             || c.Variable.VariableCode != null && c.Variable.VariableName.ToString().ToLower().Contains(searchString.ToLower())
                             || c.ValueAccuracy != null && c.ValueAccuracy.ToString().ToLower().Contains(searchString.ToLower())
                             || c.LocalDateTime != null && c.LocalDateTime.ToString().Contains(searchString.ToLower())
                             || c.UTCOffset.ToString().ToLower().Contains(searchString.ToLower())
                             || c.DateTimeUTC != null && c.DateTimeUTC.ToString().ToLower().Contains(searchString.ToLower())
                             || c.Site.SiteCode != null && c.Site.SiteCode.ToLower().Contains(searchString.ToLower())
                             || c.Variable.VariableCode != null && c.Variable.VariableCode.ToString().ToLower().Contains(searchString.ToLower())
                             || c.OffsetValue != null && c.OffsetValue.ToString().ToLower().Contains(searchString.ToLower())
                             || c.OffsetType.OffsetTypeCode != null && c.OffsetType.OffsetTypeCode.ToLower().Contains(searchString.ToLower())
                             || c.CensorCode != null && c.CensorCode.ToLower().Contains(searchString.ToLower())
                             || c.Qualifier.QualifierCode != null && c.Qualifier.QualifierCode.ToLower().Contains(searchString.ToLower())
                             || c.Method.MethodCode != null && c.Method.MethodCode.ToLower().Contains(searchString.ToLower())
                             || c.Source.SourceCode != null && c.Source.SourceCode.ToString().ToLower().Contains(searchString.ToLower())
                             || c.Sample.LabSampleCode != null && c.Sample.LabSampleCode.ToLower().Contains(searchString.ToLower())
                             || c.DerivedFromID != null && c.DerivedFromID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.QualityControlLevel.QualityControlLevelCode != null && c.QualityControlLevel.QualityControlLevelCode.ToLower().Contains(searchString.ToLower())
                          );
                if (rst == null) return result;
                //count
                //searchRecordCount = context.DataValues.
                //    Where(c =>
                //                c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.DataValue1 != null && c.DataValue1.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.ValueAccuracy != null && c.ValueAccuracy.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.LocalDateTime != null && c.LocalDateTime.ToString().Contains(searchString.ToLower())
                //             || c.UTCOffset != null && c.UTCOffset.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.DateTimeUTC != null && c.DateTimeUTC.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.Site.SiteCode != null && c.Site.SiteCode.ToLower().Contains(searchString.ToLower())
                //             || c.Variable.VariableCode != null && c.Variable.VariableCode.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.OffsetValue != null && c.OffsetValue.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.OffsetType.OffsetTypeCode != null && c.OffsetType.OffsetTypeCode.ToLower().Contains(searchString.ToLower())
                //             || c.CensorCode != null && c.CensorCode.ToLower().Contains(searchString.ToLower())
                //             || c.Qualifier.QualifierCode != null && c.Qualifier.QualifierCode.ToLower().Contains(searchString.ToLower())
                //             || c.Method.MethodCode != null && c.Method.MethodCode.ToLower().Contains(searchString.ToLower())
                //             || c.Source.SourceCode != null && c.Source.SourceCode.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.Sample.LabSampleCode != null && c.Sample.LabSampleCode.ToLower().Contains(searchString.ToLower())
                //             || c.DerivedFromID != null && c.DerivedFromID.ToString().ToLower().Contains(searchString.ToLower())
                //             || c.QualityControlLevel.QualityControlLevelCode != null && c.QualityControlLevel.QualityControlLevelCode.ToLower().Contains(searchString.ToLower())
                //          ).Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<DataValue, DataValuesModel>(item);

                    model.VariableCode = context.Variables
                                         .Where(a => a.VariableID == item.VariableID)
                                         .Select(a => a.VariableCode)
                                         .FirstOrDefault();

                    model.SiteCode = context.Sites
                                        .Where(a => a.SiteID == item.SiteID)
                                        .Select(a => a.SiteCode)
                                        .FirstOrDefault();
                    model.LabSampleCode = context.Samples
                                       .Where(a => a.SampleID == item.SampleID)
                                       .Select(a => a.LabSampleCode)
                                       .FirstOrDefault();

                    model.QualityControlLevelCode = context.QualityControlLevels
                                        .Where(a => a.QualityControlLevelID == item.QualityControlLevelID)
                                        .Select(a => a.QualityControlLevelCode)
                                        .FirstOrDefault();
                    model.MethodCode = context.Methods
                                         .Where(a => a.MethodID == item.MethodID)
                                         .Select(a => a.MethodCode)
                                         .FirstOrDefault();
                    model.SourceCode = context.Sources
                                         .Where(a => a.SourceID == item.SourceID)
                                         .Select(a => a.SourceCode)
                                         .FirstOrDefault();

                    
                    result.Add(model);
                }
            }

            else
            {
                List<DataValue> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.DataValue1).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.DataValue1).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.ValueAccuracy).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.ValueAccuracy).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.LocalDateTime).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.LocalDateTime).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "4":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.UTCOffset).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.UTCOffset).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "5":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.DateTimeUTC).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.DateTimeUTC).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "6":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.Site.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.Site.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "7":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.Variable.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.Variable.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "8":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.OffsetValue).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.OffsetValue).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "9":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.OffsetType.OffsetTypeCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.OffsetType.OffsetTypeCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "10":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.CensorCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.CensorCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "11":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.Qualifier.QualifierCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.Qualifier.QualifierCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "12":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.Method.MethodCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.Method.MethodCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "13":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.Source.SourceCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.Source.SourceCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "14":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.Sample.LabSampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.Sample.LabSampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "15":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.DerivedFromID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.DerivedFromID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "16":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.QualityControlLevel.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.QualityControlLevel.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                    }
                }

                if (sortedItems == null) sortedItems = context.DataValues.OrderByDescending(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<DataValue, DataValuesModel>(item);

                    model.VariableCode = context.Variables
                                         .Where(a => a.VariableID == item.VariableID)
                                         .Select(a => a.VariableCode)
                                         .FirstOrDefault();

                    model.SiteCode = context.Sites
                                        .Where(a => a.SiteID == item.SiteID)
                                        .Select(a => a.SiteCode)
                                        .FirstOrDefault();
                    model.LabSampleCode = context.Samples
                                       .Where(a => a.SampleID == item.SampleID)
                                       .Select(a => a.LabSampleCode)
                                       .FirstOrDefault();

                    model.QualityControlLevelCode = context.QualityControlLevels
                                        .Where(a => a.QualityControlLevelID == item.QualityControlLevelID)
                                        .Select(a => a.QualityControlLevelCode)
                                        .FirstOrDefault();
                    model.MethodCode = context.Methods
                                         .Where(a => a.MethodID == item.MethodID)
                                         .Select(a => a.MethodCode)
                                         .FirstOrDefault();
                    model.SourceCode = context.Sources
                                         .Where(a => a.SourceID == item.SourceID)
                                         .Select(a => a.SourceCode)
                                         .FirstOrDefault();

                    result.Add(model);
                }
            }
            return result;
        }

        public class SiteCodeVariableCode
        {
            public string SiteCode { get; set; }

            public string VariableCode { get; set; }
        }

        public async Task AddDataValues(List<DataValuesModel> itemList, string entityConnectionString, string instanceIdentifier, List<DataValuesModel> listOfIncorrectRecords, List<DataValuesModel> listOfCorrectRecords, List<DataValuesModel> listOfDuplicateRecords, List<DataValuesModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("DataValuesRepository.AddDataValues(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            //debug
            var timeTocomplete = new TimeSpan();
            var timeToRetrieveVars = new TimeSpan();
            var timeToFindDatavalues = new TimeSpan();
            var timeExistInUpload = new TimeSpan();
            var timeToFindDuplicates = new TimeSpan();

            int maxAllowedDuplicates = int.Parse(System.Configuration.ConfigurationManager.AppSettings["maxAllowedDuplicates"]);


            var recordsToInsert = new List<DataValue>();

            var startTime = DateTime.Now;

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var siteCodes = context.Sites.ToDictionary(p => p.SiteCode, p => p.SiteID);
            var variableCodes = context.Variables.ToDictionary(p => p.VariableCode, p => p.VariableID);
            var offsetTypeCodes = context.OffsetTypes.ToDictionary(p => p.OffsetTypeCode, p => p.OffsetTypeID);
            var censorCodeCV = context.CensorCodeCVs.ToList();
            var qualifierCodes = context.Qualifiers.ToDictionary(p => p.QualifierCode, p => p.QualifierID);
            var methodCodes = context.Methods.ToDictionary(p => p.MethodCode, p => p.MethodID);
            var sourceCodes = context.Sources.ToDictionary(p => p.SourceCode, p => p.SourceID);
            var sampleCodes = context.Samples.ToDictionary(p => p.LabSampleCode, p => p.SampleID);
            var qualityControlLevelIds = context.QualityControlLevels.ToDictionary(p => p.QualityControlLevelCode, p => p.QualityControlLevelID);

            //BC - Traverse the itemList once to build the collections... 
            var siteCodeVarCodePermutations = new Dictionary<string, SiteCodeVariableCode>();
            var filteredLists = new Dictionary<string, List<DataValuesModel>>();

            foreach (var item in itemList)
            {
                var key = item.SiteCode + "_" + item.VariableCode;
                if (!siteCodeVarCodePermutations.Keys.Contains(key))
                {
                    var siteCodeVariableCode = new SiteCodeVariableCode { SiteCode = item.SiteCode, VariableCode = item.VariableCode };
                    siteCodeVarCodePermutations[key] = siteCodeVariableCode;

                    filteredLists[key] = new List<DataValuesModel>();
                }

                filteredLists[key].Add(item);
            }

            //var siteCodeVarCodePermutations = (from s in itemList
            //                                 group s by new { s.SiteCode, s.VariableCode } into d
            //                              select
            //                              new
            //                              {
            //                                  d.Key.SiteCode,                                              
            //                                  d.Key.VariableCode
            //                              }).ToList();


            
            //get unique sitecodes to speed up search
            //var uniqueSitecodes = itemList.GroupBy(g => g.SiteCode).Select(grp => new { SiteCode = grp.Key }).ToList();   //BC - never referenced

            ////Retrieve Minimum Date
            //DateTime MinDate;   //BCC - 14-May-2018 - Never referenced...
            //var d1 = (from d in itemList select d.DateTimeUTC).Min();
            //bool isConvertable = UniversalTypeConverter.TryConvertTo<DateTime>(d1, out MinDate);
            //if (!isConvertable) MinDate = DateTime.MinValue;

            ////Retrieve Maximum Date
            //DateTime MaxDate;   //BCC - 14-May-2018 - Never referenced...
            //var d2 =(from d in itemList select d.DateTimeUTC).Max();
            //isConvertable = UniversalTypeConverter.TryConvertTo<DateTime>(d2, out MaxDate);
            //if (!isConvertable) MaxDate = DateTime.MaxValue;
            bool ignoreDuplicateTest = false;
            try
            {
                ignoreDuplicateTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ignoreDuplicateTest"]);
            }
            catch
            {

            }

            var maxCount = itemList.Count;
            var count = 0;

            var a_end = DateTime.Now;

            timeToRetrieveVars = a_end - startTime;
            //var resultCollection = new System.Collections.Concurrent.ConcurrentBag<string>();
            //System.Threading.Tasks.Parallel.ForEach(siteCodeVarCodePermutations, sv =>
            //{

            //});

           Debug.WriteLine("timeToRetrieveVars:" + timeToRetrieveVars);
            try
            {
                foreach (var kvp in siteCodeVarCodePermutations)
                {
                    ////Use Task yield here to yield time back to the caller...
                    ////Source: https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                    //await Task.Yield();

                    var sv = kvp.Value;

                    //siteid for sitecode
                    int currentSiteId = 0;
                    if (siteCodes.ContainsKey(sv.SiteCode))
                    {
                        currentSiteId = siteCodes[sv.SiteCode];
                    }
                    //variableId for variablecode
                    int currentVariableId = 0;
                    if (variableCodes.ContainsKey(sv.VariableCode))
                    {
                        currentVariableId = variableCodes[sv.VariableCode];
                    }

                    var a_start = DateTime.Now;
                    //get min/max dates in upload

                    HashSet<DateTime> setDatetime = null;
                    HashSet<double> setDataValue = null;                    
                    HashSet<int> setVariableId = null;
                    HashSet<int> setMethodId = null;
                    HashSet<DataValue> allValues = null;

                    if (!ignoreDuplicateTest)
                    {
                        setDatetime = new HashSet<DateTime>(from d in context.DataValues.AsNoTracking()
                                                            where d.SiteID == currentSiteId && d.VariableID == currentVariableId
                                                            //where d.SiteID == sv.SiteCode && d.VariableID == sv.VariableID
                                                            select d.DateTimeUTC);

                        setDataValue = new HashSet<double>(from d in context.DataValues.AsNoTracking()
                                                           where d.SiteID == currentSiteId && d.VariableID == currentVariableId
                                                           select d.DataValue1);
                        setVariableId = new HashSet<int>(from d in context.DataValues.AsNoTracking()
                                                         where d.SiteID == currentSiteId && d.VariableID == currentVariableId
                                                         select d.VariableID);
                        setMethodId = new HashSet<int>(from d in context.DataValues.AsNoTracking()
                                                       where d.SiteID == currentSiteId && d.VariableID == currentVariableId
                                                       select d.MethodID);

                        a_end = DateTime.Now;

                        allValues = new HashSet<DataValue>((from d in context.DataValues.AsNoTracking()
                                                            where d.SiteID == currentSiteId && d.VariableID == currentVariableId
                                                            //&& d.DateTimeUTC >= MinDate && d.DateTimeUTC <= MaxDate
                                                            select d).ToList());

                    }
                    else
                    {
                        a_end = DateTime.Now;
                    }

                    var span = a_end - a_start;
                    timeToFindDatavalues.Add(span);
                    Debug.WriteLine("timeToRetrieve " + currentSiteId + ": " + span);

                    var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_DATAVALUES, count, maxCount, listOfCorrectRecords.Count(), listOfIncorrectRecords.Count(), listOfDuplicateRecords.Count());
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof (DataValuesModel).Name, statusMessage);
                        await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(DataValuesModel).Name, maxCount);
                    }

                    BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, String.Format(Resources.IMPORT_STATUS_PROCESSING, count, maxCount, listOfCorrectRecords.Count(), listOfIncorrectRecords.Count(), listOfDuplicateRecords.Count()));
                    #region loop through series
                    //var filteredList = (from i in itemList
                    //                    where i.SiteCode == sv.SiteCode && i.VariableCode == sv.VariableCode
                    //                    select i).ToList();
                    var filteredList = filteredLists[kvp.Key];

                    foreach (var item in filteredList)
                        {
                            ////Use Task yield here to yield time back to the caller...
                            ////Source: https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                            //await Task.Yield();

                            try
                            {
                                statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_DATAVALUES, count, maxCount, listOfCorrectRecords.Count(), listOfIncorrectRecords.Count(), listOfDuplicateRecords.Count());
                                if (null == statusContext)
                                {
                                    BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                                }
                                else
                                {
                                    await statusContext.AddStatusMessage(typeof(DataValuesModel).Name, statusMessage);
                                }

                                count++;
                                #region data matching
                                bool isRejected = false;
                                var model = new DataValue();
                                var listOfErrors = new List<ErrorModel>();

                                //set default values
                                string unk = "Unknown";

                                model.ValueAccuracy = null;
                                model.OffsetValue = null;
                                model.OffsetTypeID = null;
                                model.CensorCode = "nc";
                                model.QualifierID = null;
                                model.MethodID = 0;
                                model.SampleID = null;
                                model.DerivedFromID = null;
                                model.QualityControlLevelID = -9999;

                                //DataValue
                                if (!string.IsNullOrWhiteSpace(item.DataValue))
                                {
                                    double result;
                                    bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.DataValue, out result);
                                    //NaN can be converted properly ino a double sql field is a float and will not accept this so we need to test separately for NaN and reject
                                    if (!canConvert || Double.IsNaN(result))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "DataValue")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        model.DataValue1 = result;
                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "DataValue")); listOfErrors.Add(err); isRejected = true;
                                }
                                //ValueAccuracy
                                if (!string.IsNullOrWhiteSpace(item.ValueAccuracy))
                                {
                                    double result;
                                    bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.ValueAccuracy, out result);

                                    if (!canConvert || Double.IsNaN(result))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "ValueAccuracy")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        model.ValueAccuracy = result;
                                    }
                                }

                                //LocalDateTime
                                if (!string.IsNullOrWhiteSpace(item.LocalDateTime))
                                {
                                    DateTime result;
                                    bool canConvert = UniversalTypeConverter.TryConvertTo<DateTime>(item.LocalDateTime, out result);

                                    if (!canConvert)
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "LocalDateTime")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        model.LocalDateTime = result;
                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "LocalDateTime")); listOfErrors.Add(err); isRejected = true;
                                }
                                //UTCOffset
                                if (!string.IsNullOrWhiteSpace(item.UTCOffset))
                                {
                                    double result;
                                    bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.UTCOffset, out result);

                                    if (!canConvert)
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "UTCOffset")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        model.UTCOffset = result;
                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "UTCOffset")); listOfErrors.Add(err); isRejected = true;
                                }
                                //DateTimeUTC
                                if (!string.IsNullOrWhiteSpace(item.DateTimeUTC))
                                {
                                    DateTime result;
                                    bool canConvert = UniversalTypeConverter.TryConvertTo<DateTime>(item.DateTimeUTC, out result);

                                    if (!canConvert)
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "DateTimeUTC")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        model.DateTimeUTC = result;
                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "DateTimeUTC")); listOfErrors.Add(err); isRejected = true;
                                }
                                //SiteCode
                                if (!string.IsNullOrWhiteSpace(item.SiteCode))
                                {
                                    if (RepositoryUtils.containsSpecialCharacters(item.SiteCode))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "SiteCode")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        if (siteCodes.ContainsKey(item.SiteCode))
                                        {
                                            var siteId = siteCodes[item.SiteCode];
                                            //update model
                                            model.SiteID = siteId;
                                            item.SiteID = siteId.ToString();
                                        }
                                        else
                                        {
                                            var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.SiteCode, "SiteCode")); listOfErrors.Add(err); isRejected = true;
                                        }

                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SiteCode")); listOfErrors.Add(err); isRejected = true;
                                }
                                //VariableID
                                //VariableCode
                                if (!string.IsNullOrWhiteSpace(item.VariableCode))
                                {
                                    if (RepositoryUtils.containsSpecialCharacters(item.VariableCode))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "VariableCode")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        if (variableCodes.ContainsKey(item.VariableCode))
                                        {
                                            var variableId = variableCodes[item.VariableCode];
                                            //update model
                                            model.VariableID = variableId;
                                            item.VariableID = variableId.ToString();
                                        }
                                        else
                                        {
                                            var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.VariableCode, "VariableCode")); listOfErrors.Add(err); isRejected = true;
                                        }

                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "VariableCode")); listOfErrors.Add(err); isRejected = true;
                                }
                                //OffsetValue
                                if (!string.IsNullOrWhiteSpace(item.OffsetValue))
                                {
                                    double result;
                                    bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.OffsetValue, out result);

                                    if (!canConvert || Double.IsNaN(result))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "OffsetValue")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        model.OffsetValue = result;
                                    }
                                }
                                //OffsetTypeID
                                if (!string.IsNullOrWhiteSpace(item.OffsetTypeCode))
                                {


                                    if (RepositoryUtils.containsSpecialCharacters(item.OffsetTypeCode))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "OffsetTypeCode")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        if (offsetTypeCodes.ContainsKey(item.OffsetTypeCode))
                                        {
                                            var offsetTypeId = offsetTypeCodes[item.OffsetTypeCode];
                                            //update model
                                            model.OffsetTypeID = offsetTypeId;
                                            item.OffsetTypeID = offsetTypeId.ToString();
                                        }
                                        else
                                        {
                                            var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.OffsetTypeID, "OffsetType")); listOfErrors.Add(err); isRejected = true;
                                        }

                                    }
                                }
                                //CensorCode 
                                //Added to allow files without censorcode to be processed MS 8/21/18
                                if (!string.IsNullOrWhiteSpace(item.CensorCode))
                                {
                                    if (RepositoryUtils.containsSpecialCharacters(item.CensorCode))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "CensorCode")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        var censorCode = censorCodeCV
                                                            .Where(a => a.Term.ToLower() == item.CensorCode.ToLower()).FirstOrDefault();
                                        if (censorCode != null)
                                        {
                                            model.CensorCode = item.CensorCode;
                                            item.CensorCode = censorCode.Term;
                                        }
                                        else
                                        {
                                            var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.CensorCode, "CensorCode")); listOfErrors.Add(err); isRejected = true;
                                        }
                                    }
                                }
                                else
                                {
                                    item.CensorCode = "Unknown";
                                //var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "CensorCode")); listOfErrors.Add(err); isRejected = true;
                                }

                                //QualifierID
                                if (!string.IsNullOrWhiteSpace(item.QualifierCode))
                                {

                                    if (RepositoryUtils.containsSpecialCharacters(item.QualifierCode))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "QualifierCode")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        if (qualifierCodes.ContainsKey(item.QualifierCode))
                                        {
                                            var qualifierId = qualifierCodes[item.QualifierCode];
                                            //update model
                                            model.QualifierID = qualifierId;
                                            item.QualifierID = qualifierId.ToString();
                                        }
                                        else
                                        {
                                            var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.QualifierID, "QualifierCode")); listOfErrors.Add(err); isRejected = true;
                                        }

                                    }
                                }

                                //MethodID
                                if (!string.IsNullOrWhiteSpace(item.MethodCode))
                                {
                                    if (RepositoryUtils.containsSpecialCharacters(item.MethodCode))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "MethodCode")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        if (methodCodes.ContainsKey(item.MethodCode))
                                        {
                                            var methodId = methodCodes[item.MethodCode];
                                            //update model
                                            model.MethodID = methodId;
                                            item.MethodID = methodId.ToString();
                                        }
                                        else
                                        {
                                            var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.MethodID, "MethodCode")); listOfErrors.Add(err); isRejected = true;
                                        }

                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "MethodCode")); listOfErrors.Add(err); isRejected = true;
                                }
                                //SourceID
                                if (!string.IsNullOrWhiteSpace(item.SourceCode))
                                {
                                    if (RepositoryUtils.containsSpecialCharacters(item.SourceCode))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "SourceCode")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        if (sourceCodes.ContainsKey(item.SourceCode))
                                        {
                                            var sourceId = sourceCodes[item.SourceCode];
                                            //update model
                                            model.SourceID = sourceId;
                                            item.SourceID = sourceId.ToString();
                                        }
                                        else
                                        {
                                            var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.SourceID, "SourceCode")); listOfErrors.Add(err); isRejected = true;
                                        }
                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "SourceCode")); listOfErrors.Add(err); isRejected = true;
                                }
                                //SampleID- labsamplecode is unique identifier
                                if (!string.IsNullOrWhiteSpace(item.LabSampleCode) || !string.IsNullOrWhiteSpace(item.SampleCode))
                                {

                                    //setting Sample code to Labcsample code to correct a bug where template contained samplecode instead of Labsamplecode MS 11/25/2016                                
                                    if (string.IsNullOrWhiteSpace(item.LabSampleCode)) { item.LabSampleCode = item.SampleCode; };

                                    if (RepositoryUtils.containsSpecialCharacters(item.LabSampleCode))
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "LabSampleCode")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        if (sampleCodes.ContainsKey(item.LabSampleCode))
                                        {
                                            var sampleId = sampleCodes[item.LabSampleCode];
                                            //update model
                                            model.SampleID = sampleId;
                                            item.SampleID = sampleId.ToString();
                                        }
                                        else
                                        {
                                            var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.SampleID, "LabSampleCode")); listOfErrors.Add(err); isRejected = true;
                                        }

                                    }
                                }

                                //DerivedFromID
                                if (!string.IsNullOrWhiteSpace(item.DerivedFromID))
                                {
                                    int result;
                                    bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.DerivedFromID, out result);

                                    if (!canConvert)
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "DerivedFromID")); listOfErrors.Add(err); isRejected = true;
                                    }
                                    else
                                    {
                                        model.DerivedFromID = result;
                                    }
                                }
                                //QualityControlLevelID
                                if (!string.IsNullOrWhiteSpace(item.QualityControlLevelCode))
                                {
                                    if (qualityControlLevelIds.ContainsKey(item.QualityControlLevelCode))
                                    {
                                        var qualityControlLevelId = qualityControlLevelIds[item.QualityControlLevelCode];
                                        //update model
                                        model.QualityControlLevelID = qualityControlLevelId;
                                        item.QualityControlLevelID = qualityControlLevelId.ToString();
                                    }
                                    else
                                    {
                                        var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.QualityControlLevelCode, "QualityControlLevelCode")); listOfErrors.Add(err); isRejected = true;

                                    }
                                }
                                else
                                {
                                    var err = new ErrorModel("AddDataValues", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "QualityControlLevelCode")); listOfErrors.Add(err); isRejected = true;
                                }
                                #endregion data

                                if (isRejected)
                                {
                                    var sb = new StringBuilder();
                                    foreach (var er in listOfErrors)
                                    {
                                        sb.Append(er.ErrorMessage + ";");
                                        if (null != statusContext)
                                        {
                                            var errorIndex = listOfIncorrectRecords.Count;
                                            StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                            statMsg.IsError = true;
                                            await statusContext.AddStatusMessage(typeof(DataValuesModel).Name, statMsg);
                                        }
                                    }
                                    item.Errors = sb.ToString();
                                    listOfIncorrectRecords.Add(item);
                                    if (null != statusContext)
                                    {
                                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(DataValuesModel).Name, 0, 0, 1, 0);
                                    }

                                    continue;
                                }

                                a_start = DateTime.Now;
                                bool doesExist = false;
                                //pretest with date and datavalue if 
                                var possibleInSet = false;
                                if (!ignoreDuplicateTest && setDatetime != null)
                                {
                                    possibleInSet = setDatetime.Contains(model.DateTimeUTC) && setDataValue.Contains(model.DataValue1) && setVariableId.Contains(model.VariableID) && setMethodId.Contains(model.MethodID);
                                }

                                if (possibleInSet)
                                {

                                    doesExist = allValues.Where(a =>
                                                                a.DataValue1.Equals(model.DataValue1) &&
                                                                a.ValueAccuracy.Equals(model.ValueAccuracy) &&
                                                                a.LocalDateTime.Equals(model.LocalDateTime) &&
                                                                a.UTCOffset.Equals(model.UTCOffset) &&
                                                                a.DateTimeUTC.Ticks.Equals(model.DateTimeUTC.Ticks) &&
                                                                a.SiteID.Equals(model.SiteID) &&
                                                                a.VariableID.Equals(model.VariableID) &&
                                                                a.OffsetValue.Equals(model.OffsetValue) &&
                                                                a.OffsetTypeID.Equals(model.OffsetTypeID) &&
                                                                a.CensorCode.Equals(model.CensorCode) &&
                                                                a.QualifierID.Equals(model.QualifierID) &&
                                                                a.MethodID.Equals(model.MethodID) &&
                                                                a.SourceID.Equals(model.SourceID) &&
                                                                a.SampleID.Equals(model.SampleID) &&
                                                                a.DerivedFromID.Equals(model.DerivedFromID) &&
                                                                a.QualityControlLevelID.Equals(model.QualityControlLevelID)
                                                                ).FirstOrDefault() != null;

                                    a_end = DateTime.Now;

                                    span = a_end - a_start;
                                    timeToFindDuplicates = timeToFindDuplicates.Add(span);
                                    //Debug.WriteLine("timeToFindDuplicates: " + span);


                                }
                                if (!doesExist)
                                {
                                    if (count % 100 == 0) Debug.WriteLine(count);
                                    listOfCorrectRecords.Add(item);
                                    if (null != statusContext)
                                    {
                                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(DataValuesModel).Name, 1, 0, 0, 0);
                                    }
                                }
                                else
                                {

                                    //no editing possible no unique field in upload
                                    if (listOfDuplicateRecords.Count() > maxAllowedDuplicates)
                                    {
                                        throw new System.OperationCanceledException("The upload was cancelled due to a large number of duplicates (" + maxAllowedDuplicates + ") in upload. Please review data.");
                                    }

                                    listOfDuplicateRecords.Add(item);
                                    if (null != statusContext)
                                    {
                                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(DataValuesModel).Name, 0, 0, 0, 1);
                                    }
                                }

                            }
                            catch (OperationCanceledException ex)
                            {
                                throw ex;
                            }
                            catch (Exception)
                            {
                                listOfIncorrectRecords.Add(item);
                                if (null != statusContext)
                                {
                                    await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(DataValuesModel).Name, 0, 0, 1, 0);
                                }
                            }


                        }
                    #endregion
                }

                //Finalize status context...
                if (null != statusContext)
                {
                    await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(DataValuesModel).Name);
                }
            }
            catch (OperationCanceledException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Debug.WriteLine("timeToRetrieveVars:" + timeToRetrieveVars);
            Debug.WriteLine("timeToFindDatavalues: " + timeToFindDatavalues);
            Debug.WriteLine("timeToFindDuplicates: " + timeToFindDuplicates);
            Debug.WriteLine("timeExistInUpload: " + timeExistInUpload);           
            timeTocomplete = DateTime.Now - startTime;
            Debug.WriteLine("timeTocomplete: " + timeTocomplete);

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            try
            {
                //context.Configuration.AutoDetectChangesEnabled = false;
                int rowsCount = (from o in context.DataValues
                                 select o).Count(); 

                //var rows = from o in context.DataValues
                //           select o;

                if (rowsCount > 0)
                {
                    int count = rowsCount / 10000 ;
                    for (int i = 0; i <= count; i++)
                    {
                        //context.Database.CommandTimeout = 60;
                        context.Database.ExecuteSqlCommand("Delete top (10000) from DataValues");
                    }
                    //context.Database.SaveChanges();
                }
 
                var seriescatalogrows = from o in context.SeriesCatalogs
                                        select o;

                if (seriescatalogrows.Count() != 0)
                {
                    context.SeriesCatalogs.RemoveRange(seriescatalogrows);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }
            return;          
        }
    }



    //  GroupDescriptions
    public class GroupDescriptionsRepository : IGroupDescriptionsRepository
    {
        public const string CacheName = "default";

        public List<GroupDescriptionModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.GroupDescriptions
                        select obj;
            var modelList = new List<GroupDescriptionModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.GroupDescription, HydroserverToolsBusinessObjects.Models.GroupDescriptionModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<GroupDescriptionModel> GetGroupDescriptions(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<GroupDescriptionModel>();

            //if (context.GroupDescriptions.Count() != null)
            if (0 < context.GroupDescriptions.Count())
            {
                totalRecordCount = context.GroupDescriptions.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.GroupDescriptions.ToList();
                var rst = allItems.
                    Where(c =>
                                 c.GroupID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.GroupDescription1 != null && c.GroupDescription1.ToLower().Contains(searchString.ToLower())
                          );
                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<GroupDescription, GroupDescriptionModel>(item);

                    result.Add(model);
                }
            }

            else
            {
                List<GroupDescription> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.GroupDescriptions.OrderBy(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.GroupDescriptions.OrderByDescending(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.GroupDescriptions.OrderBy(a => a.GroupDescription1).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.GroupDescriptions.OrderByDescending(a => a.GroupDescription1).Skip(startIndex).Take(pageSize).ToList(); }
                            break;


                    }
                }

                if (sortedItems == null) sortedItems = context.GroupDescriptions.OrderByDescending(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<GroupDescription, GroupDescriptionModel>(item);

                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddGroupDescriptions(List<GroupDescriptionModel> itemList, string entityConnectionString, string instanceIdentifier, List<GroupDescriptionModel> listOfIncorrectRecords, List<GroupDescriptionModel> listOfCorrectRecords, List<GroupDescriptionModel> listOfDuplicateRecords, List<GroupDescriptionModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("GroupDescriptionsRepository.AddGroupDescriptions(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "GroupDescriptions");

            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (GroupDescriptionModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(GroupDescriptionModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {
                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage );
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(GroupDescriptionModel).Name, statusMessage);
                    }

                    count++;
          
                    var model = new GroupDescription();
                    var listOfErrors = new List<ErrorModel>();
                    bool isRejected = false;
                    //GroupDescription
                    if (!string.IsNullOrWhiteSpace(item.GroupDescription))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.GroupDescription))
                        {
                            var err = new ErrorModel("AddGroupDescriptions", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "GroupDescription")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.GroupDescription1 = item.GroupDescription;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddGroupDescriptions", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "GroupDescription")); listOfErrors.Add(err); isRejected = true;
                    }

                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(GroupDescriptionModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(GroupDescriptionModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }

                    //lookup duplicates
                    var existingItem = context.GroupDescriptions.Where(a => a.GroupDescription1 == model.GroupDescription1
                                                                               ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.GroupDescriptions.Add(model);
                        //context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(GroupDescriptionModel).Name, 1, 0, 0, 0);
                        }
                    }
                    else
                    {
                        //no editing possible no unique field in upload
                        listOfDuplicateRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(GroupDescriptionModel).Name, 0, 0, 0, 1);
                        }
                    }

                }
                //catch (Exception ex)
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(GroupDescriptionModel).Name, 0, 0, 1, 0);
                    }
                }
            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(GroupDescriptionModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.GroupDescriptions
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.GroupDescriptions.Remove(row);
            //}
            try
            {
                context.GroupDescriptions.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }
    //  Groups
    public class GroupsRepository : IGroupsRepository
    {
        public const string CacheName = "default";

        public List<GroupsModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.Groups
                        select obj;
            var modelList = new List<GroupsModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.Group, HydroserverToolsBusinessObjects.Models.GroupsModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<GroupsModel> GetGroups(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<GroupsModel>();

            //if (context.Groups.Count() != null)
            if (0 < context.Groups.Count())
            {
                totalRecordCount = context.Groups.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {

                var allItems = context.Groups.ToList();
                var rst = allItems.
                    Where(c =>
                                    c.GroupID.ToString().ToLower().Contains(searchString.ToLower())
                                 || c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
                         );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<Group, GroupsModel>(item);

                    result.Add(model);
                }
            }

            else
            {
                List<Group> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Groups.OrderBy(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Groups.OrderByDescending(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Groups.OrderBy(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Groups.OrderByDescending(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;


                    }
                }

                if (sortedItems == null) sortedItems = context.Groups.OrderByDescending(a => a.GroupID).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<Group, GroupsModel>(item);

                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddGroups(List<GroupsModel> itemList, string entityConnectionString, string instanceIdentifier, List<GroupsModel> listOfIncorrectRecords, List<GroupsModel> listOfCorrectRecords, List<GroupsModel> listOfDuplicateRecords, List<GroupsModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("GroupsRepository.AddGroups(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            var groupDescriptions = context.GroupDescriptions.ToList();

            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "Groups");

            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof (GroupsModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof( GroupsModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {

                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);

                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(GroupsModel).Name, statusMessage);
                    }

                    count++;

                    var model = new Group();
                    var listOfErrors = new List<ErrorModel>();
                    bool isRejected = false;

                    //GroupID
                    if (!string.IsNullOrWhiteSpace(item.GroupID))
                    {
                         int result;
                         bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.GroupID, out result);
                         if (canConvert)//user used id
                         {
                             var groupId = groupDescriptions
                                         .Exists(a => a.GroupID == result);

                             if (!groupId)
                             {
                                 var err = new ErrorModel("AddGroups", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.GroupID, "Groups"));
                                 listOfErrors.Add(err);
                                 isRejected = true;
                             }
                             else
                             {
                                 model.GroupID = result;
                             }
                         }
                         else
                         {
                             var err = new ErrorModel("AddGroups", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "GroupID")); listOfErrors.Add(err); isRejected = true;
                         }  
                    }
                    else
                    {
                        var err = new ErrorModel("AddGroups", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "GroupID")); listOfErrors.Add(err); isRejected = true;
                    }

                    //ValuerID                    
                    if (!string.IsNullOrWhiteSpace(item.ValueID))
                    {
                        int result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.ValueID, out result);
                        if (canConvert)//user used id
                        {
                            var valueID = context.DataValues
                                        .Where(a => a.ValueID == result).FirstOrDefault() ;

                            if (valueID != null)
                            {
                                model.ValueID = result;
                            }
                            else
                            {                                
                                var err = new ErrorModel("AddGroups", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.ValueID, "ValueID"));
                                listOfErrors.Add(err);
                                isRejected = true;
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddGroups", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "ValueID")); listOfErrors.Add(err); isRejected = true;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddGroups", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "ValueID")); listOfErrors.Add(err); isRejected = true;
                    }
                    
                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(GroupsModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(GroupsModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }


                    //lookup duplicates
                    var existingItem = context.Groups.Where(a => a.GroupID == model.GroupID &&
                                                                               a.ValueID == model.ValueID
                                                                               ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.Groups.Add(model);
                        //context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(GroupsModel).Name, 1, 0, 0, 0);
                        }
                    }
                    else
                    {
                        //no editing possible no unique field in upload
                        listOfDuplicateRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(GroupsModel).Name, 0, 0, 0, 1);
                        }
                    }

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(GroupsModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(GroupsModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Groups
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.Groups.Remove(row);
            //}
            try
            {
                context.Groups.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }

    //  DerivedFrom
    public class DerivedFromRepository : IDerivedFromRepository
    {
        public const string CacheName = "default";

        public List<DerivedFromModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.DerivedFroms
                        select obj;
            var modelList = new List<DerivedFromModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.DerivedFrom, HydroserverToolsBusinessObjects.Models.DerivedFromModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<DerivedFromModel> GetDerivedFrom(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<DerivedFromModel>();

            if (context.DerivedFroms.Count() != 0)
            {
                totalRecordCount = context.DerivedFroms.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.DerivedFroms.ToList();
                var rst = allItems.
                    Where(c =>
                                     c.DerivedFromID.ToString().ToLower().Contains(searchString.ToLower())
                                 ||  c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
                         );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<DerivedFrom, DerivedFromModel>(item);

                    result.Add(model);
                }
            }

            else
            {
                List<DerivedFrom> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DerivedFroms.OrderBy(a => a.DerivedFromID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DerivedFroms.OrderByDescending(a => a.DerivedFromID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DerivedFroms.OrderBy(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DerivedFroms.OrderByDescending(a => a.ValueID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;


                    }
                }

                if (sortedItems == null) sortedItems = context.DerivedFroms.OrderByDescending(a => a.DerivedFromID).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<DerivedFrom, DerivedFromModel>(item);

                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddDerivedFrom(List<DerivedFromModel> itemList, string entityConnectionString, string instanceIdentifier, List<DerivedFromModel> listOfIncorrectRecords, List<DerivedFromModel> listOfCorrectRecords, List<DerivedFromModel> listOfDuplicateRecords, List<DerivedFromModel> listOfEditedRecords, StatusContext statusContext)
        {
            listOfIncorrectRecords = new List<DerivedFromModel>();
            listOfCorrectRecords = new List<DerivedFromModel>();
            listOfDuplicateRecords = new List<DerivedFromModel>();
            listOfEditedRecords = new List<DerivedFromModel>();
            
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            var derivedFroms = context.DerivedFroms.ToList();

            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "DerivedFrom");

            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof(DerivedFromModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(DerivedFromModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {

                try
                {
                    statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING, (count + 1), maxCount);
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(DerivedFromModel).Name, statusMessage);
                    }

                    count++;

                    var model = new DerivedFrom();
                    var listOfErrors = new List<ErrorModel>();
                    bool isRejected = false;

                    //DerivedFromID
                    if (!string.IsNullOrWhiteSpace(item.DerivedFromId))
                    {
                        int result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.DerivedFromId, out result);
                        if (canConvert)//user used id
                        {
                            var derivedFromID = context.DataValues
                                        .Where(a => a.DerivedFromID == result) != null;

                            if (!derivedFromID)
                            {
                                var err = new ErrorModel("AddDerivedFrom", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.DerivedFromId, "Datavalues"));
                                listOfErrors.Add(err);
                                isRejected = true;
                            }
                            else
                            {
                                model.DerivedFromID = result;
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddDerivedFrom", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "DerivedFromID")); listOfErrors.Add(err); isRejected = true;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddDerivedFrom", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "DerivedFromID")); listOfErrors.Add(err); isRejected = true;
                    }

                    //ValueID                    
                    if (!string.IsNullOrWhiteSpace(item.ValueID))
                    {
                        int result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<int>(item.ValueID, out result);
                        if (canConvert)//user used id
                        {
                            var valueID = context.DataValues
                                        .Where(a => a.ValueID == result).FirstOrDefault();

                            if (valueID == null)
                            {
                                var err = new ErrorModel("AddDerivedFrom", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.ValueID, "ValueID"));
                                listOfErrors.Add(err);
                                isRejected = true;
                            }
                            else
                            {
                                model.ValueID = result;
                            }
                        }
                        else
                        {
                            var err = new ErrorModel("AddDerivedFrom", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "ValueID")); listOfErrors.Add(err); isRejected = true;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddDerivedFrom", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "ValueID")); listOfErrors.Add(err); isRejected = true;
                    }

                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(DerivedFromModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(DerivedFromModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }


                    //lookup duplicates
                    var existingItem = context.DerivedFroms.Where(a => a.DerivedFromID == model.DerivedFromID &&
                                                                               a.ValueID == model.ValueID
                                                                               ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.DerivedFroms.Add(model);
                        //context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(DerivedFromModel).Name, 1, 0, 0, 0);
                        }
                    }
                    else
                    {
                        //no editing possible no unique field in upload
                        listOfDuplicateRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(DerivedFromModel).Name, 0, 0, 0, 1);
                        }
                    }

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(DerivedFromModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(DerivedFromModel).Name);
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.DerivedFroms
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.DerivedFroms.Remove(row);
            //}
            try
            {
                context.DerivedFroms.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }

    //  Categories
    public class CategoriesRepository : ICategoriesRepository
    {
        public const string CacheName = "default";

        public List<CategoriesModel> GetAll(string connectionString)
        {
            // Create an EntityConnection.         
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var items = from obj in context.Categories
                        select obj;
            var modelList = new List<CategoriesModel>();
            foreach (var item in items)
            {

                var model = Mapper.Map<ODM_1_1_1EFModel.Category, HydroserverToolsBusinessObjects.Models.CategoriesModel>(item);

                modelList.Add(model);
            }
            return modelList;
        }

        public List<CategoriesModel> GetCategories(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<CategoriesModel>();

            //if (context.Categories.Count() != null)
            if (0 < context.Categories.Count())
            {
                totalRecordCount = context.Categories.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.Categories.ToList();
                var rst = allItems.
                    Where(c =>
                                    c.VariableID.ToString().ToLower().Contains(searchString.ToLower())
                                 || c.DataValue.ToString().ToLower().Contains(searchString.ToLower())
                                 || c.CategoryDescription.ToLower().Contains(searchString.ToLower())
                          );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<Category, CategoriesModel>(item);

                    result.Add(model);
                }
            }

            else
            {
                List<Category> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Categories.OrderBy(a => a.VariableID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Categories.OrderByDescending(a => a.VariableID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Categories.OrderBy(a => a.DataValue).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Categories.OrderByDescending(a => a.DataValue).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Categories.OrderBy(a => a.CategoryDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Categories.OrderByDescending(a => a.CategoryDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            break;

                    }
                }

                if (sortedItems == null) sortedItems = context.Categories.OrderByDescending(a => a.VariableID).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<Category, CategoriesModel>(item);

                    result.Add(model);
                }
            }
            return result;
        }

        public async Task AddCategories(List<CategoriesModel> itemList, string entityConnectionString, string instanceIdentifier, List<CategoriesModel> listOfIncorrectRecords, List<CategoriesModel> listOfCorrectRecords, List<CategoriesModel> listOfDuplicateRecords, List<CategoriesModel> listOfEditedRecords, StatusContext statusContext)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == itemList ||
                String.IsNullOrWhiteSpace(entityConnectionString) ||
                String.IsNullOrWhiteSpace(instanceIdentifier) ||
                null == listOfIncorrectRecords ||
                null == listOfCorrectRecords ||
                null == listOfDuplicateRecords ||
                null == listOfEditedRecords)
            {
                ArgumentNullException ex = new ArgumentNullException("CategoriesRepository.AddCategories(...) invalid parameter...");
                throw ex;
            }
#endif
            //Reset input lists...
            listOfIncorrectRecords.Clear();
            listOfCorrectRecords.Clear();
            listOfDuplicateRecords.Clear();
            listOfEditedRecords.Clear();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //var objContext = ((IObjectContextAdapter)context).ObjectContext;

            var variables = context.Variables.ToDictionary(p => p.VariableCode, p => p.VariableID);
          
            var maxCount = itemList.Count;
            var count = 0;
            var statusMessage = String.Format(Resources.IMPORT_STATUS_PROCESSING_RECORDS, maxCount, "Categories");

            if (null == statusContext)
            {
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, statusMessage);
            }
            else
            {
                await statusContext.AddStatusMessage(typeof(CategoriesModel).Name, statusMessage);
                await statusContext.SetRecordCount(StatusContext.enumCountType.ct_DbProcess, typeof(CategoriesModel).Name, itemList.Count);
            }

            foreach (var item in itemList)
            {

                try
                {
                    //Updating status
                    statusMessage = Resources.IMPORT_STATUS_EXTRACTNG;
                    if (null == statusContext)
                    {
                        BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, "default", statusMessage);
                    }
                    else
                    {
                        await statusContext.AddStatusMessage(typeof(CategoriesModel).Name, statusMessage);
                    }

                    var model = new Category();
                    var listOfErrors = new List<ErrorModel>();
                    bool isRejected = false;

                    //Categories                    
                    if (!string.IsNullOrWhiteSpace(item.VariableCode))
                    {
                        if (RepositoryUtils.containsNotOnlyAllowedCharacters(item.VariableCode))
                        {
                            var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "VariableCode")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            if (variables.ContainsKey(item.VariableCode))
                            {
                                var variableId = variables[item.VariableCode];
                                //update model
                                model.VariableID = variableId;
                                item.VariableID = variableId.ToString();
                            }
                            else
                            {
                                var err = new ErrorModel("AddVariables", string.Format(Resources.IMPORT_VALUE_NOT_IN_DATABASE, item.VariableCode, "Sites")); listOfErrors.Add(err); isRejected = true;

                                //continue;

                            }
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddCategories", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "VariableCode")); listOfErrors.Add(err); isRejected = true;
                    }
                    //DataValue
                    if (!string.IsNullOrWhiteSpace(item.DataValue))
                    {
                        double result;
                        bool canConvert = UniversalTypeConverter.TryConvertTo<double>(item.DataValue, out result);

                        if (!canConvert || Double.IsNaN(result))
                        {
                            var err = new ErrorModel("AddCategories", string.Format(Resources.IMPORT_VALUE_INVALIDVALUE, "DataValue")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.DataValue = result;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddCategories", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "DataValue")); listOfErrors.Add(err); isRejected = true;
                    }
                    //CategoryDescription
                    if (!string.IsNullOrWhiteSpace(item.CategoryDescription))
                    {
                        if (RepositoryUtils.containsSpecialCharacters(item.CategoryDescription))
                        {
                            var err = new ErrorModel("AddCategories", string.Format(Resources.IMPORT_VALUE_INVALIDCHARACTERS, "CategoryDescription")); listOfErrors.Add(err); isRejected = true;
                        }
                        else
                        {
                            model.CategoryDescription = item.CategoryDescription;
                        }
                    }
                    else
                    {
                        var err = new ErrorModel("AddCategories", string.Format(Resources.IMPORT_VALUE_CANNOTBEEMPTY, "CategoryDescription")); listOfErrors.Add(err); isRejected = true;
                    }

                    //var variablesID = variables
                    //                  .Where(a => a.Key == item.VariableCode)
                    //                  .Select(a => a.Value)
                    //                  .SingleOrDefault();
                    //if (variablesID == 0)
                    //{
                    //    var err = new ErrorModel("AddCategories", string.Format(Resources.IMPORT_VALUE_NOT_IN_CV, item.VariableCode, "Variables")); listOfErrors.Add(err); isRejected = true;
                    //}
                    //else
                    //{
                    //    model.VariableID = variablesID;
                    //}
                    if (isRejected)
                    {
                        var sb = new StringBuilder();
                        foreach (var er in listOfErrors)
                        {
                            sb.Append(er.ErrorMessage + ";");
                            if (null != statusContext)
                            {
                                var errorIndex = listOfIncorrectRecords.Count;
                                StatusMessage statMsg = new StatusMessage(er.ErrorMessage, errorIndex);
                                statMsg.IsError = true;
                                await statusContext.AddStatusMessage(typeof(CategoriesModel).Name, statMsg);
                            }
                        }
                        item.Errors = sb.ToString();
                        listOfIncorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(CategoriesModel).Name, 0, 0, 1, 0);
                        }

                        continue;
                    }
                    //lookup duplicates
                    var existingItem = context.Categories.Where(a => a.VariableID == model.VariableID &&
                                                                                a.DataValue == model.DataValue &&
                                                                                a.CategoryDescription == model.CategoryDescription
                                                                                ).FirstOrDefault();


                    if (existingItem == null)
                    {
                        context.Categories.Add(model);
                        //context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(CategoriesModel).Name, 1, 0, 0, 0);
                        }
                    }
                    else
                    {
                        //no editing possible no unique field in upload
                        listOfDuplicateRecords.Add(item);
                        if (null != statusContext)
                        {
                            await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(CategoriesModel).Name, 0, 0, 0, 1);
                        }
                    }

                }
                //catch (Exception ex)
                catch (Exception)
                {
                    listOfIncorrectRecords.Add(item);
                    if (null != statusContext)
                    {
                        await statusContext.AddToCounts(StatusContext.enumCountType.ct_DbProcess, typeof(CategoriesModel).Name, 0, 0, 1, 0);
                    }
                }

            }

            //Finalize status context...
            if (null != statusContext)
            {
                await statusContext.Finalize(StatusContext.enumCountType.ct_DbProcess, typeof(CategoriesModel).Name);
            }


            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Categories
                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.Categories.Remove(row);
            //}
            try
            {
                context.Categories.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }
    }

    // Seriescatalog
    public class SeriesCatalogRepository : ISeriesCatalogRepository
    {
        public const string CacheName = "default";
        
        public List<SeriesCatalogModel> GetSeriesCatalog(string connectionString, int startIndex, int pageSize, System.Collections.ObjectModel.ReadOnlyCollection<jQuery.DataTables.Mvc.SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount, string searchString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
            var result = new List<SeriesCatalogModel>();

            totalRecordCount = context.SeriesCatalogs
                        .Count();
            searchRecordCount = totalRecordCount;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.SeriesCatalogs.ToList();
                var rst = allItems.
                    Where(c =>
                        //c.SeriesID != null && c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
                        //   c.SiteID != null && c.SiteID.ToString().ToLower().Contains(searchString.ToLower())
                                c.SiteCode != null && c.SiteCode.ToString().ToLower().Contains(searchString.ToLower())
                             || c.SiteName != null && c.SiteName.ToString().ToLower().Contains(searchString.ToLower())
                             || c.SiteType != null && c.SiteType.ToString().ToLower().Contains(searchString.ToLower())
                                    //|| c.VariableID != null && c.VariableID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.VariableCode != null && c.VariableCode.ToString().ToLower().Contains(searchString.ToLower())
                             || c.VariableName != null && c.VariableName.ToString().ToLower().Contains(searchString.ToLower())
                             || c.Speciation != null && c.Speciation.ToString().ToLower().Contains(searchString.ToLower())
                                    //|| c.VariableUnitsID != null && c.VariableUnitsID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.VariableUnitsName != null && c.VariableUnitsName.ToString().ToLower().Contains(searchString.ToLower())
                             || c.SampleMedium != null && c.SampleMedium.ToString().ToLower().Contains(searchString.ToLower())
                             || c.ValueType != null && c.ValueType.ToString().ToLower().Contains(searchString.ToLower())
                             || c.TimeSupport != null && c.TimeSupport.ToString().ToLower().Contains(searchString.ToLower())
                                    //|| c.TimeUnitsID != null && c.TimeUnitsID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.TimeUnitsName != null && c.TimeUnitsName.ToString().ToLower().Contains(searchString.ToLower())
                             || c.DataType != null && c.DataType.ToString().ToLower().Contains(searchString.ToLower())
                             || c.GeneralCategory != null && c.GeneralCategory.ToString().ToLower().Contains(searchString.ToLower())
                                    //|| c.MethodID != null && c.MethodID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.MethodDescription != null && c.MethodDescription.ToString().ToLower().Contains(searchString.ToLower())
                                    //|| c.SourceID != null && c.SourceID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.Organization != null && c.Organization.ToString().ToLower().Contains(searchString.ToLower())
                             || c.SourceDescription != null && c.SourceDescription.ToString().ToLower().Contains(searchString.ToLower())
                             || c.Citation != null && c.Citation.ToString().ToLower().Contains(searchString.ToLower())
                                    //|| c.QualityControlLevelID != null && c.QualityControlLevelID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.QualityControlLevelCode != null && c.QualityControlLevelCode.ToString().ToLower().Contains(searchString.ToLower())
                             || c.BeginDateTime != null && c.BeginDateTime.ToString().ToLower().Contains(searchString.ToLower())
                             || c.EndDateTime != null && c.EndDateTime.ToString().ToLower().Contains(searchString.ToLower())
                                    //|| c.BeginDateTimeUTC != null && c.BeginDateTimeUTC.ToString().ToLower().Contains(searchString.ToLower())
                                    //|| c.EndDateTimeUTC != null && c.EndDateTimeUTC.ToString().ToLower().Contains(searchString.ToLower())
                             || c.ValueCount != null && c.ValueCount.ToString().ToLower().Contains(searchString.ToLower())
                         );

                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<SeriesCatalog, SeriesCatalogModel>(item);

                    result.Add(model);
                }
            }

            else
            {
                List<SeriesCatalog> sortedItems = null;

                foreach (var sortedColumn in sortedColumns)
                {
                    switch (sortedColumn.PropertyName.ToLower())
                    {
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.SiteName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.SiteName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.SiteType).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.SiteType).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.VariableCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "4":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.VariableName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.VariableName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "5":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.Speciation).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.Speciation).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "6":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.VariableUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.VariableUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "7":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.SampleMedium).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.SampleMedium).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "8":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.ValueType).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.ValueType).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "9":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.TimeSupport).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.TimeSupport).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "10":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.TimeUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.TimeUnitsName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "11":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.DataType).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.DataType).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "12":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.GeneralCategory).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.GeneralCategory).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "13":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "14":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.Organization).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.Organization).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "15":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.SourceDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.SourceDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "16":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.Citation).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.Citation).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "17":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.QualityControlLevelCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "18":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.BeginDateTime).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.BeginDateTime).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "19":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.EndDateTime).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.EndDateTime).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "20":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.SeriesCatalogs.OrderBy(a => a.ValueCount).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.ValueCount).Skip(startIndex).Take(pageSize).ToList(); }
                            break;



                    }
                }

                if (sortedItems == null) sortedItems = context.SeriesCatalogs.OrderByDescending(a => a.SiteCode).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<SeriesCatalog, SeriesCatalogModel>(item);

                    result.Add(model);
                }
            }
            return result;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.SeriesCatalogs

                       select o;
            if (rows.Count() == 0) return;
            //foreach (var row in rows)
            //{
            //    context.SeriesCatalogs.Remove(row);
            //}
            try
            {
                context.SeriesCatalogs.RemoveRange(rows);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }

        public void UpdateSeriesCatalog(string instanceIdentifier, string entityConnectionstring)
        {
            //var s = Resources.EFMODELDEF_IN_CONNECTIONSTRING;
            //var constring = "metadata=res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl;provider=System.Data.SqlClient;provider connection string=&quot; Data Source=tcp:bhi5g2ajst.database.windows.net,1433;Initial Catalog=HydroServertest2;User ID=HisCentralAdmin@bhi5g2ajst; Password=f3deratedResearch; Persist Security Info=true; MultipleActiveResultSets=True;App=EntityFramework";
            //var constring = "metadata=res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=mseul-cuahsi;initial catalog=HydroSample2;integrated security=true; MultipleActiveResultSets=True;App=EntityFramework";
            //var cleanedConnectionString = entityConnectionstring.Replace(s, string.Empty);
            BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, Resources.IMPORT_STATUS_TIMESERIES);    

            var ec = new EntityConnectionStringBuilder(entityConnectionstring);
            //clear SeriesCatolog table
            var repo = new SeriesCatalogRepository();
            repo.deleteAll(entityConnectionstring);

            SqlConnection sqlConnection1 = new SqlConnection(ec.ProviderConnectionString);

            var adp = new SqlDataAdapter();
            var cmd = new SqlCommand();
            var dataTable = new DataTable();
            var sb = new StringBuilder();
            sb.Append("SELECT dv.SiteID, s.SiteCode, s.SiteName, s.SiteType, dv.VariableID, v.VariableCode, v.VariableName, ");
            sb.Append("v.Speciation, v.VariableUnitsID, u.UnitsName AS VariableUnitsName, v.SampleMedium, ");
            sb.Append("v.ValueType, v.TimeSupport, v.TimeUnitsID, u1.UnitsName AS TimeUnitsName, v.DataType, ");
            sb.Append(" v.GeneralCategory, dv.MethodID, m.MethodDescription, dv.SourceID, so.Organization, ");
            sb.Append("so.SourceDescription, so.Citation, dv.QualityControlLevelID, qc.QualityControlLevelCode, dv.BeginDateTime, ");
            sb.Append("dv.EndDateTime, dv.BeginDateTimeUTC, dv.EndDateTimeUTC, dv.ValueCount ");
            sb.Append("FROM  (");
            sb.Append("SELECT SiteID, VariableID, MethodID, QualityControlLevelID, SourceID, MIN(LocalDateTime) AS BeginDateTime,");
            sb.Append("MAX(LocalDateTime) AS EndDateTime, MIN(DateTimeUTC) AS BeginDateTimeUTC, MAX(DateTimeUTC) AS EndDateTimeUTC, ");
            sb.Append("COUNT(DataValue) AS ValueCount ");
            sb.Append("FROM DataValues ");
            sb.Append("GROUP BY SiteID, VariableID, MethodID, QualityControlLevelID, SourceID) dv ");
            sb.Append("INNER JOIN dbo.Sites s ON dv.SiteID = s.SiteID ");
            sb.Append("INNER JOIN dbo.Variables v ON dv.VariableID = v.VariableID ");
            sb.Append("INNER JOIN dbo.Units u ON v.VariableUnitsID = u.UnitsID ");
            sb.Append("INNER JOIN dbo.Methods m ON dv.MethodID = m.MethodID ");
            sb.Append("INNER JOIN dbo.Units u1 ON v.TimeUnitsID = u1.UnitsID ");
            sb.Append("INNER JOIN dbo.Sources so ON dv.SourceID = so.SourceID ");
            sb.Append("INNER JOIN dbo.QualityControlLevels qc ON dv.QualityControlLevelID = qc.QualityControlLevelID ");
            sb.Append("GROUP BY   dv.SiteID, s.SiteCode, s.SiteName, s.SiteType, dv.VariableID, v.VariableCode, v.VariableName, v.Speciation, ");
            sb.Append("v.VariableUnitsID, u.UnitsName, v.SampleMedium, v.ValueType, v.TimeSupport, v.TimeUnitsID, u1.UnitsName, ");
            sb.Append("v.DataType, v.GeneralCategory, dv.MethodID, m.MethodDescription, dv.SourceID, so.Organization, ");
            sb.Append("so.SourceDescription, so.Citation, dv.QualityControlLevelID, qc.QualityControlLevelCode, dv.BeginDateTime, ");
            sb.Append("dv.EndDateTime, dv.BeginDateTimeUTC, dv.EndDateTimeUTC, dv.ValueCount ");
            sb.Append("ORDER BY   dv.SiteID, dv.VariableID, v.VariableUnitsID");

            cmd.CommandText = sb.ToString();

            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            adp.SelectCommand = cmd;
            adp.Fill(dataTable);

            //List<SeriesCatalogDBModel> list = AutoMapper.Mapper.DynamicMap<IDataReader, List<SeriesCatalogDBModel>>(dataTable.CreateDataReader());

            //List<SeriesCatalogDBModel> list = dataTable.AsEnumerable().ToList();
            //                                   .Select(row=> new SeriesCatalogDBModel)
            // open the destination data
            using (SqlConnection destinationConnection =
                            new SqlConnection(ec.ProviderConnectionString))
            {
                // open the connection
                destinationConnection.Open();

                using (SqlBulkCopy bulkCopy =
                            new SqlBulkCopy(destinationConnection.ConnectionString, SqlBulkCopyOptions.KeepNulls )) //| SqlBulkCopyOptions.CheckConstraints
                {
                    //bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsTransfer);
                    //bulkCopy.NotifyAfter = 100;
                    bulkCopy.BatchSize = 10000;

                    foreach (DataColumn dc in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                    }


                    bulkCopy.DestinationTableName = "SeriesCatalog";
                    bulkCopy.WriteToServer(dataTable);
                }
            }

        }

        public void UpdateSeriesCatalog2(string instanceIdentifier, List<DataValuesModel> listOfRecords, string entityConnectionstring)
        {
            BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, Resources.IMPORT_STATUS_TIMESERIES);
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionstring);

           IEnumerable<string> uniqueSiteIds = listOfRecords.GroupBy(item => item.SiteID).Select(i => i.Key).ToList();

            int count = 0;

            foreach (var uniqueSiteId in uniqueSiteIds) 
            {

               
                int maxCount = uniqueSiteIds.Count();
                int siteId;
                if (Int32.TryParse(uniqueSiteId, out siteId))
                {
                    count++;
                    BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, String.Format(Resources.IMPORT_STATUS_PROCESSING_TIMESERIES, count, maxCount));

                    getTimeseries(siteId, listOfRecords, context, instanceIdentifier, entityConnectionstring);
                }
            }
        }

        public void UpdateSeriesCatalog3(string instanceIdentifier, List<DataValuesModel> listOfRecords, string entityConnectionstring)
        {
            //var s = Resources.EFMODELDEF_IN_CONNECTIONSTRING;
            //var constring = "metadata=res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl;provider=System.Data.SqlClient;provider connection string=&quot; Data Source=tcp:bhi5g2ajst.database.windows.net,1433;Initial Catalog=HydroServertest2;User ID=HisCentralAdmin@bhi5g2ajst; Password=f3deratedResearch; Persist Security Info=true; MultipleActiveResultSets=True;App=EntityFramework";
            //var constring = "metadata=res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=mseul-cuahsi;initial catalog=HydroSample2;integrated security=true; MultipleActiveResultSets=True;App=EntityFramework";
            //var cleanedConnectionString = entityConnectionstring.Replace(s, string.Empty);
            BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, Resources.IMPORT_STATUS_TIMESERIES);
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionstring);

            var uniqueSiteCodes = listOfRecords.GroupBy(item => item.SiteID).Select(i => i.Key).ToList();
           
            //var rows2 = context.SeriesCatalogs.Where(p => uniqueSiteCodes.Intersect().Any()).ToList();


            //var rows = context.SeriesCatalogs.ToList();
                       //listOfRecords.GroupBy(p => p.SiteID, p=>p.VariableID, p=>p.MethodID, p=>p.SourceID);
            // get unique occurences
            var list = (from p in listOfRecords select new {p.SiteID, p.VariableID, p.SourceID, p.MethodID, p.QualityControlLevelID}).
                                                            GroupBy(y => new  { y.SiteID, y.VariableID, y.SourceID, y.MethodID, y.QualityControlLevelID }).ToList();
            
            //var list = (from p in listOfRecords
            //            select new[] { p.SiteID, p.VariableID, p.SourceID, p.MethodID, p.QualityControlLevelID }).Distinct().ToList();
            int count = 0;
            int maxCount = list.Count;
            //loop and compare
            foreach (var item in list)
            {
                count++;
                BusinessObjectsUtils.UpdateCachedprocessStatusMessage(instanceIdentifier, CacheName, String.Format(Resources.IMPORT_STATUS_PROCESSING, count, maxCount));

                var siteID = Convert.ToInt32(item.ElementAt(0).SiteID);
                var variableID = Convert.ToInt32(item.ElementAt(0).VariableID);
                var sourceID = Convert.ToInt32(item.ElementAt(0).SourceID);
                var methodID = Convert.ToInt32(item.ElementAt(0).MethodID);
                var qualityControlLevelID = Convert.ToInt32(item.ElementAt(0).QualityControlLevelID);

                //var uniqueTimeseriesCombination = new UniqueTimeseriesCombination();
                // var siteid = Convert.ToInt32(item.SiteID);
                //var variableID = Convert.ToInt32(item.VariableID);
                //var sourceID = Convert.ToInt32(item.SourceID);
                //var methodID = Convert.ToInt32(item.MethodID);
                //var qualityControlLevelID = Convert.ToInt32(item.QualityControlLevelID);


                var timeseriesExists = context.SeriesCatalogs.Where(p => p.SiteID == siteID &&
                                                  p.VariableID == variableID &&
                                                  p.SourceID == sourceID &&
                                                  p.MethodID == methodID &&
                                                  p.QualityControlLevelID == qualityControlLevelID
                ).FirstOrDefault();
                
                //get timeseries data
                var tsd = RepositoryUtils.getTimeseriesData(siteID, variableID, sourceID, methodID, qualityControlLevelID, entityConnectionstring);
                
                if (timeseriesExists == null)//new
                {
                    var timeseries = new SeriesCatalog();
                    //Siteinformation
                    var site = context.Sites.Where(p => p.SiteID == siteID).FirstOrDefault();
                    if (site != null)
                    {
                        timeseries.SiteID = siteID;                        
                        timeseries.SiteCode = site.SiteCode;
                        timeseries.SiteName = site.SiteName;
                    }
                    else
                    {
                        throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG,"Site"));
                    }
                    //Variable information
                    var variable = context.Variables.Where(p=> p.VariableID == variableID).FirstOrDefault();
                    if (variable != null)
                    {
                        timeseries.VariableID = variableID;
                        timeseries.VariableCode = variable.VariableCode;
                        timeseries.VariableName = variable.VariableName;
                        timeseries.Speciation = variable.Speciation;
                        timeseries.VariableUnitsID = variable.VariableUnitsID;
                        timeseries.VariableUnitsName = variable.Unit.UnitsName;
                        timeseries.SampleMedium = variable.SampleMedium;
                        timeseries.ValueType = variable.ValueType;
                        timeseries.TimeSupport = variable.TimeSupport;
                        timeseries.TimeUnitsID = variable.TimeUnitsID;
                        timeseries.TimeUnitsName = variable.Unit1.UnitsName;
                        timeseries.DataType = variable.DataType;
                        timeseries.GeneralCategory = variable.GeneralCategory;
                    }
                    else
                    {                        
                        throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG,"Variable"));                        
                    }
                    //Method information
                    var method = context.Methods.Where(p=> p.MethodID == methodID).FirstOrDefault();
                    if (method != null)
                    {
                        timeseries.MethodID = methodID;
                        timeseries.MethodDescription = method.MethodDescription;                       
                    }
                    else
                    {                        
                         throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG,"Method"));                        
                    }
                    //Sources information
                    var source = context.Sources.Where(p=> p.SourceID == sourceID).FirstOrDefault();
                    if (source != null)
                    {
                        timeseries.SourceID = sourceID;
                        timeseries.Organization = source.Organization; 
                        timeseries.SourceDescription = source.SourceDescription;
                        timeseries.Citation = source.Citation;
                    }
                    else
                    {                        
                         throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG,"Source"));                        
                    }
                    //Qualitycontrollevel information
                    var qualitycontrollevel = context.QualityControlLevels.Where(p=> p.QualityControlLevelID == qualityControlLevelID).FirstOrDefault();
                    if (qualitycontrollevel != null)
                    {
                        timeseries.QualityControlLevelID = qualityControlLevelID;
                        timeseries.QualityControlLevelCode = qualitycontrollevel.QualityControlLevelCode;                         
                    }
                    else
                    {                        
                         throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG,"Qualitycontrollevel"));                        
                    }
                    //cummulative information
                    timeseries.BeginDateTime = tsd.BeginDateTime;
                    timeseries.EndDateTime = tsd.EndDateTime;
                    timeseries.BeginDateTimeUTC = tsd.BeginDateTimeUTC;
                    timeseries.EndDateTimeUTC = tsd.EndDateTimeUTC;
                    timeseries.ValueCount = tsd.ValueCount;
                    context.SeriesCatalogs.Add(timeseries);
                }
                else//update
                {
                    timeseriesExists.BeginDateTime = tsd.BeginDateTime;
                    timeseriesExists.EndDateTime = tsd.EndDateTime;
                    timeseriesExists.BeginDateTimeUTC = tsd.BeginDateTimeUTC;
                    timeseriesExists.EndDateTimeUTC = tsd.EndDateTimeUTC;
                    timeseriesExists.ValueCount = tsd.ValueCount;
                    context.SeriesCatalogs.Add(timeseriesExists);
                }
                context.SaveChanges();
            }
            

        }

        private void getTimeseries(int id, List<DataValuesModel> listOfRecords, ODM_1_1_1EFModel.ODM_1_1_1Entities context, string instanceIdentifier, string entityConnectionstring)
        {
            var list = (from p in listOfRecords where p.SiteID == id.ToString() select new { p.SiteID, p.VariableID, p.SourceID, p.MethodID, p.QualityControlLevelID }).                                                            
                                                            GroupBy(y => new { y.SiteID, y.VariableID, y.SourceID, y.MethodID, y.QualityControlLevelID }).ToList();

            var listOfTimeseries = context.SeriesCatalogs.Where(p => p.SiteID == id);
            
            //loop and compare
            foreach (var item in list)
            {
               
                var siteID = Convert.ToInt32(item.ElementAt(0).SiteID);
                var variableID = Convert.ToInt32(item.ElementAt(0).VariableID);
                var sourceID = Convert.ToInt32(item.ElementAt(0).SourceID);
                var methodID = Convert.ToInt32(item.ElementAt(0).MethodID);
                var qualityControlLevelID = Convert.ToInt32(item.ElementAt(0).QualityControlLevelID);

                //var uniqueTimeseriesCombination = new UniqueTimeseriesCombination();
                // var siteid = Convert.ToInt32(item.SiteID);
                //var variableID = Convert.ToInt32(item.VariableID);
                //var sourceID = Convert.ToInt32(item.SourceID);
                //var methodID = Convert.ToInt32(item.MethodID);
                //var qualityControlLevelID = Convert.ToInt32(item.QualityControlLevelID);


                var timeseriesExists = listOfTimeseries.Where(p => p.SiteID == siteID &&
                                                  p.VariableID == variableID &&
                                                  p.SourceID == sourceID &&
                                                  p.MethodID == methodID &&
                                                  p.QualityControlLevelID == qualityControlLevelID
                ).FirstOrDefault();

                //get timeseries data
                var tsd = RepositoryUtils.getTimeseriesData(siteID, variableID, sourceID, methodID, qualityControlLevelID, entityConnectionstring);

                if (timeseriesExists == null)//new
                {
                    var timeseries = new SeriesCatalog();
                    //Siteinformation
                    var site = context.Sites.Where(p => p.SiteID == siteID).FirstOrDefault();
                    if (site != null)
                    {
                        timeseries.SiteID = siteID;
                        timeseries.SiteCode = site.SiteCode;
                        timeseries.SiteName = site.SiteName;
                    }
                    else
                    {
                        throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG, "Site"));
                    }
                    //Variable information
                    var variable = context.Variables.Where(p => p.VariableID == variableID).FirstOrDefault();
                    if (variable != null)
                    {
                        timeseries.VariableID = variableID;
                        timeseries.VariableCode = variable.VariableCode;
                        timeseries.VariableName = variable.VariableName;
                        timeseries.Speciation = variable.Speciation;
                        timeseries.VariableUnitsID = variable.VariableUnitsID;
                        timeseries.VariableUnitsName = variable.Unit.UnitsName;
                        timeseries.SampleMedium = variable.SampleMedium;
                        timeseries.ValueType = variable.ValueType;
                        timeseries.TimeSupport = variable.TimeSupport;
                        timeseries.TimeUnitsID = variable.TimeUnitsID;
                        timeseries.TimeUnitsName = variable.Unit1.UnitsName;
                        timeseries.DataType = variable.DataType;
                        timeseries.GeneralCategory = variable.GeneralCategory;
                    }
                    else
                    {
                        throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG, "Variable"));
                    }
                    //Method information
                    var method = context.Methods.Where(p => p.MethodID == methodID).FirstOrDefault();
                    if (method != null)
                    {
                        timeseries.MethodID = methodID;
                        timeseries.MethodDescription = method.MethodDescription;
                    }
                    else
                    {
                        throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG, "Method"));
                    }
                    //Sources information
                    var source = context.Sources.Where(p => p.SourceID == sourceID).FirstOrDefault();
                    if (source != null)
                    {
                        timeseries.SourceID = sourceID;
                        timeseries.Organization = source.Organization;
                        timeseries.SourceDescription = source.SourceDescription;
                        timeseries.Citation = source.Citation;
                    }
                    else
                    {
                        throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG, "Source"));
                    }
                    //Qualitycontrollevel information
                    var qualitycontrollevel = context.QualityControlLevels.Where(p => p.QualityControlLevelID == qualityControlLevelID).FirstOrDefault();
                    if (qualitycontrollevel != null)
                    {
                        timeseries.QualityControlLevelID = qualityControlLevelID;
                        timeseries.QualityControlLevelCode = qualitycontrollevel.QualityControlLevelCode;
                    }
                    else
                    {
                        throw new Exception(String.Format(Resources.IMPORT_CREATE_SERIESCATALOG, "Qualitycontrollevel"));
                    }
                    //cummulative information
                    timeseries.BeginDateTime = tsd.BeginDateTime;
                    timeseries.EndDateTime = tsd.EndDateTime;
                    timeseries.BeginDateTimeUTC = tsd.BeginDateTimeUTC;
                    timeseries.EndDateTimeUTC = tsd.EndDateTimeUTC;
                    timeseries.ValueCount = tsd.ValueCount;
                    context.SeriesCatalogs.Add(timeseries);
                }
                else//update
                {
                    timeseriesExists.BeginDateTime = tsd.BeginDateTime;
                    timeseriesExists.EndDateTime = tsd.EndDateTime;
                    timeseriesExists.BeginDateTimeUTC = tsd.BeginDateTimeUTC;
                    timeseriesExists.EndDateTimeUTC = tsd.EndDateTimeUTC;
                    timeseriesExists.ValueCount = tsd.ValueCount;
                    context.SeriesCatalogs.Add(timeseriesExists);
                }
                
            }
            context.SaveChanges();
        }
   
        
    }


    public class DatabaseRepository : IDatabaseRepository
    {
        public class TableValueCount
        {
            //string servername { get; set; }
            //string databasename { get; set; }
            //string schemaname { get; set; }
            public string tablename { get; set; }
            public Int64 rowcounts { get; set; }
            //DateTime captureddatetime { get; set; }
        }

        public DatabaseTableValueCountModel GetDatabaseTableValueCount(string connectionString)
        {
            var databaseTableValueCountModel = new DatabaseTableValueCountModel();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

            var sb = new StringBuilder();
            //sb.Append("SELECT @@servername as servername, db_name() as databasename, s.name AS schemaname, t.name AS tablename,  p.rows AS rowcounts, getdate() as captureddatetime ");
            sb.Append("SELECT t.name AS tablename, p.rows  AS rowcounts ");

            sb.Append("FROM sys.tables t ");
            sb.Append("INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id ");
            sb.Append("INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id ");
            sb.Append("LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id ");
            sb.Append("WHERE t.NAME NOT LIKE 'dt%' ");
            sb.Append("AND t.is_ms_shipped = 0 ");
            sb.Append("AND i.OBJECT_ID > 255 ");
            sb.Append("--and t.name = ''XXXX''---- replace the XXXX with table name ");
            sb.Append("GROUP BY ");
            sb.Append("t.name, s.name, p.Rows");

            var t = context.Database.SqlQuery<TableValueCount>(sb.ToString()).ToList();

            for (int i = 0; i < t.Count; i++)
            {
                if (t[i].tablename.ToLower() == "sites") { databaseTableValueCountModel.SiteCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "variables") { databaseTableValueCountModel.VariablesCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "offsettypes") { databaseTableValueCountModel.OffsetTypesCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "sources") { databaseTableValueCountModel.SourcesCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "methods") { databaseTableValueCountModel.MethodsCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "labmethods") { databaseTableValueCountModel.LabMethodsCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "samples") { databaseTableValueCountModel.SamplesCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "qualifiers") { databaseTableValueCountModel.QualifiersCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "qualitycontrollevels") { databaseTableValueCountModel.QualityControlLevelsCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "datavalues") { databaseTableValueCountModel.DataValuesCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "groupdescriptions") { databaseTableValueCountModel.GroupDescriptionsCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "groups") { databaseTableValueCountModel.GroupsCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "derivedfrom") { databaseTableValueCountModel.DerivedFromCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "categories") { databaseTableValueCountModel.CategoriesCount = t[i].rowcounts; };
                if (t[i].tablename.ToLower() == "seriescatalog") { databaseTableValueCountModel.SeriesCatalog = t[i].rowcounts; };
            }


            //databaseTableValueCountModel.SiteCount = context.Sites.Count();
            //databaseTableValueCountModel.VariablesCount = context.Variables.Count();
            //databaseTableValueCountModel.OffsetTypesCount = context.OffsetTypes.Count();
            //databaseTableValueCountModel.SourcesCount = context.Sources.Count();
            //databaseTableValueCountModel.MethodsCount = context.Methods.Count();
            //databaseTableValueCountModel.LabMethodsCount = context.LabMethods.Count();
            //databaseTableValueCountModel.SamplesCount = context.Samples.Count();
            //databaseTableValueCountModel.QualifiersCount = context.Qualifiers.Count();
            //databaseTableValueCountModel.QualityControlLevelsCount = context.QualityControlLevels.Count();
            //databaseTableValueCountModel.DataValuesCount = context.DataValues.Count();
            //databaseTableValueCountModel.GroupDescriptionsCount = context.GroupDescriptions.Count();
            //databaseTableValueCountModel.GroupsCount = context.Groups.Count();
            //databaseTableValueCountModel.DerivedFromCount = context.DerivedFroms.Count();
            //databaseTableValueCountModel.CategoriesCount = context.Categories.Count();
            //databaseTableValueCountModel.SeriesCatalog = context.SeriesCatalogs.Count();

            return databaseTableValueCountModel;
        }

        
    }



    //public static class myExtentions
    // {
    //     public static bool EntityExists<T>(this ObjectContext context, T entity)
    //     where T : EntityObject
    //     {
    //         object value;
    //         var entityKeyValues = new List<KeyValuePair<string, object>>();
    //         var objectSet = context.CreateObjectSet<T>().EntitySet;
    //         foreach (var member in objectSet.ElementType.KeyMembers)
    //         {
    //             var info = entity.GetType().GetProperty(member.Name);
    //             var tempValue = info.GetValue(entity, null);
    //             var pair = new KeyValuePair<string, object>(member.Name, tempValue);
    //             entityKeyValues.Add(pair);
    //         }
    //         var key = new EntityKey(objectSet.EntityContainer.Name + "." + objectSet.Name, entityKeyValues);
    //         if (context.TryGetObjectByKey(key, out value))
    //         {
    //             return value != null;
    //         }
    //         return false;
    //     }
    // }


}