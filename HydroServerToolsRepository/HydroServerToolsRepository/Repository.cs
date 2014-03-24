using AutoMapper;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
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
using System.Linq;
using System.Text;
using System.Web;

namespace HydroServerToolsRepository.Repository
{

    //  Sites
    public class SitesRepository : ISitesRepository
    {

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

            if (context.Sites.Count() != null)
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
                            || c.Latitude != null && c.Latitude.ToString().ToLower().Contains(searchString.ToLower())
                            || c.Longitude != null && c.Longitude.ToString().ToLower().Contains(searchString.ToLower())
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

        public void AddSites(List<SiteModel> sites, string entityConnectionString, out List<SiteModel> listOfIncorrectRecords, out List<SiteModel> listOfCorrectRecords, out List<SiteModel> listOfDuplicateRecords, out List<SiteModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<SiteModel>();
            listOfCorrectRecords = new List<SiteModel>();
            listOfDuplicateRecords = new List<SiteModel>();
            listOfEditedRecords = new List<SiteModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            var LatLongDatum = context.SpatialReferences.ToDictionary(p => p.SRSName.Trim(), p => p.SpatialReferenceID);
            var VerticalDatumCV = context.VerticalDatumCVs.ToList();
            var SiteTypeCV = context.SiteTypeCVs.ToList();


            foreach (var item in sites)
            {
                //var item = new ODM_1_1_1EFModel.Site();

                try
                {
                    var model = Mapper.Map<SiteModel, Site>(item);
                    //set default values
                    string unk = "Unknown";
                    if (String.IsNullOrEmpty(model.VerticalDatum)) model.VerticalDatum = unk;

                    bool isRejected = false;

                    //var LatLongDatumID = context.SpatialReferences
                    //                     .Where (a => a.SRSName == s.LatLongDatumSRSName)
                    //                     .Select (a => a.SpatialReferenceID)
                    //                     .FirstOrDefault();
                    //var LatLongDatumID = LatLongDatum[item.LatLongDatumSRSName];

                    var LatLongDatumID = LatLongDatum.
                                                Where(a => a.Key == item.LatLongDatumSRSName)
                                                .Select(a => a.Value)
                                                .Single();

                    model.LatLongDatumID = LatLongDatumID;


                    var LocalProjectionID = LatLongDatum.
                                               Where(a => a.Key == item.LocalProjectionSRSName)
                                               .Select(a => a.Value)
                                               .SingleOrDefault();

                    model.LocalProjectionID = LocalProjectionID;

                    if (item.VerticalDatum != null && item.VerticalDatum.Length > 0)
                    {
                        var VerticalDatum = VerticalDatumCV
                                       .Exists(a => a.Term.ToString() == item.VerticalDatum);

                        if (!VerticalDatum) { var err = new ErrorModel("AddSites", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VerticalDatum, "VerticalDatum")); listOfErrors.Add(err); isRejected = true; };


                    }

                    if (item.SiteType != null && item.SiteType.Length > 0)
                    {
                        var SiteType = SiteTypeCV
                                       .Exists(a => a.Term.ToString() == item.SiteType);

                        if (!SiteType) { var err = new ErrorModel("AddSites", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.SiteType, "SiteType")); listOfErrors.Add(err); isRejected = true; };


                    }

                    if (isRejected)
                    {
                        listOfIncorrectRecords.Add(item); continue;
                    }
                    ////d.SiteID = s.SiteID;
                    //d.SiteCode = s.SiteCode;
                    //d.SiteName = s.SiteName;
                    //d.Latitude = float.Parse(s.Latitude);
                    //d.Longitude = float.Parse(s.Longitude);


                    ////var LatLongDatumID = from r in context.SpatialReferences
                    ////                     where r.SRSName == "NAD83"
                    ////                     select (int)r.SpatialReferenceID;





                    ////d.LatLongDatumID = int.Parse(s.LatLongDatumID);
                    //if (!string.IsNullOrEmpty(s.Elevation_m) && (s.Elevation_m.ToLower() != "null")) { d.Elevation_m = (float.Parse(s.Elevation_m)); } else d.Elevation_m = null;
                    //d.VerticalDatum = s.VerticalDatum;
                    //if (!string.IsNullOrEmpty(s.LocalX) && (s.LocalX.ToLower() != "null")) { d.LocalX = (float.Parse(s.LocalX)); } else d.LocalX = null;
                    //if (!string.IsNullOrEmpty(s.LocalY) && (s.LocalY.ToLower() != "null")) { d.LocalY = (float.Parse(s.LocalY)); } else d.LocalY = null;
                    //if (!string.IsNullOrEmpty(s.LocalProjectionID) && (s.LocalProjectionID.ToLower() != "null")) { d.LocalProjectionID = (int.Parse(s.LocalProjectionID)); } else d.LocalProjectionID = null;
                    //if (!string.IsNullOrEmpty(s.PosAccuracy_m) && (s.PosAccuracy_m.ToLower() != "null")) { d.PosAccuracy_m = (float.Parse(s.PosAccuracy_m)); } else d.PosAccuracy_m = null;
                    //d.State = s.State;
                    //d.County = s.County;
                    //d.Comments = s.Comments;
                    //d.SiteType = s.SiteType;


                    //var objectSet = objContext.CreateObjectSet<Site>().EntitySet;//.EntitySet;

                    var existingItem = context.Sites.Where(a => a.SiteCode == item.SiteCode).FirstOrDefault();
                    //var j = context.Sites.Find(s.SiteCode);

                    if (existingItem == null)
                    {
                        context.Sites.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (existingItem.SiteName != model.SiteName) { existingItem.SiteName = model.SiteName; editedFields.Add("SiteName"); }
                        if (existingItem.Latitude != model.Latitude) { existingItem.Latitude = model.Latitude; editedFields.Add("Latitude"); }
                        if (existingItem.Longitude != model.Longitude) { existingItem.Longitude = model.Longitude; editedFields.Add("Longitude"); }
                        if (existingItem.LatLongDatumID != model.LatLongDatumID) { existingItem.LatLongDatumID = model.LatLongDatumID; editedFields.Add("LatLongDatumID"); }
                        if (existingItem.Elevation_m != model.Elevation_m) { existingItem.Elevation_m = model.Elevation_m; editedFields.Add("Elevation_m"); }
                        if (existingItem.VerticalDatum != model.VerticalDatum) { existingItem.VerticalDatum = model.VerticalDatum; editedFields.Add("VerticalDatum"); }
                        if (existingItem.LocalX != model.LocalX) { existingItem.LocalX = model.LocalX; editedFields.Add("LocalX"); }
                        if (existingItem.LocalY != model.LocalY) { existingItem.LocalY = model.LocalY; editedFields.Add("LocalY"); }
                        if (existingItem.LocalProjectionID != model.LocalProjectionID) { existingItem.LocalProjectionID = model.LocalProjectionID; editedFields.Add("LocalProjectionID"); }
                        if (existingItem.PosAccuracy_m != model.PosAccuracy_m) { existingItem.PosAccuracy_m = model.PosAccuracy_m; editedFields.Add("PosAccuracy_m"); }
                        if (existingItem.State != model.State) { existingItem.State = model.State; editedFields.Add("State"); }
                        if (existingItem.County != model.County) { existingItem.County = model.County; editedFields.Add("County"); }
                        if (existingItem.Comments != model.Comments) { existingItem.Comments = model.Comments; editedFields.Add("Comments"); }
                        if (existingItem.SiteType != model.SiteType) { existingItem.SiteType = model.SiteType; editedFields.Add("SiteType"); }


                        if (editedFields.Count() > 0)
                        {

                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
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
                catch (KeyNotFoundException ex)
                {

                    listOfIncorrectRecords.Add(item);
                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Sites
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.Sites.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    //  Variables
    public class VariablesRepository : IVariablesRepository
    {

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

            if (context.Variables.Count() != null)
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
                                   c.VariableID != null && c.VariableID.ToString().Contains(searchString.ToLower())
                                || c.VariableCode != null && c.VariableCode.Contains(searchString.ToLower())
                                || c.VariableName != null && c.VariableName.ToLower().Contains(searchString.ToLower())
                                || c.Speciation != null && c.Speciation.Contains(searchString.ToLower())
                                || c.Unit != null && c.VariableUnitsID.ToString().Contains(searchString.ToLower())
                                || c.SampleMedium != null && c.SampleMedium.ToLower().Contains(searchString.ToLower())
                                || c.ValueType != null && c.ValueType.ToLower().Contains(searchString.ToLower())
                                || c.IsRegular != null && c.IsRegular.ToString().ToLower().Contains(searchString.ToLower())
                                || c.TimeSupport != null && c.TimeSupport.ToString().ToLower().Contains(searchString.ToLower())
                                || c.Unit != null && c.Unit.UnitsName.ToLower().Contains(searchString.ToLower())
                                || c.DataType != null && c.DataType.ToLower().Contains(searchString.ToLower())
                                || c.GeneralCategory != null && c.GeneralCategory.ToLower().Contains(searchString.ToLower())
                                || c.NoDataValue != null && c.NoDataValue.ToString().ToLower().Contains(searchString.ToLower())
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


        public void AddVariables(List<VariablesModel> itemList, string entityConnectionString, out List<VariablesModel> listOfIncorrectRecords, out List<VariablesModel> listOfCorrectRecords, out List<VariablesModel> listOfDuplicateRecords, out List<VariablesModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<VariablesModel>();
            listOfCorrectRecords = new List<VariablesModel>();
            listOfDuplicateRecords = new List<VariablesModel>();
            listOfEditedRecords = new List<VariablesModel>();
            listOfErrors = new List<ErrorModel>();
            //var errorModel = new ErrorModel(); 


            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            //read CV in to list for fster searching
            var variableCV = context.VariableNameCVs.ToList();
            var speciationCV = context.SpeciationCVs.ToList();
            var units = context.Units.ToDictionary(p => p.UnitsName.Trim(), p => p.UnitsID);
            var sampleMediumCV = context.SampleMediumCVs.ToList();
            var valueTypeCV = context.ValueTypeCVs.ToList();
            var dataTypeCV = context.DataTypeCVs.ToList();
            var generalCategoryCV = context.GeneralCategoryCVs.ToList();

            foreach (var item in itemList)
            {
                try
                {
                    var model = Mapper.Map<VariablesModel, Variable>(item);

                    //need to look up Id's for VariableName, Speciation, VariableUnitsName, SampleMedium, ValueType, DataType, GeneralCategory, TimeUnitsName
                    //User has no concept of ID's
                    bool isRejected = false;
                    var VariableName = variableCV
                                           .Exists(a => a.Term.ToString() == item.VariableName);
                    if (!VariableName) { var err = new ErrorModel("AddVariable", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableName, "VariableName")); listOfErrors.Add(err); isRejected = true; };

                    var speciationName = speciationCV
                                           .Exists(a => a.Term.ToString() == item.Speciation);
                    if (!speciationName) { var err = new ErrorModel("AddVariable", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.Speciation, "SpeciationName")); listOfErrors.Add(err); isRejected = true; };

                    var variableUnitsID = units
                                            .Where(a => a.Key == item.VariableUnitsName)
                                            .Select(a => a.Value)
                                            .SingleOrDefault();
                    if (variableUnitsID == 0) { var err = new ErrorModel("AddVariable", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableUnitsName, "VariableUnitsName")); listOfErrors.Add(err); isRejected = true; };

                    var sampleMedium = sampleMediumCV
                                            .Exists(a => a.Term.ToString() == item.SampleMedium);
                    if (!sampleMedium) { var err = new ErrorModel("AddVariable", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableName, "SampleMedium")); listOfErrors.Add(err); isRejected = true; };

                    var valueType = valueTypeCV
                                            .Exists(a => a.Term.ToString() == item.ValueType);
                    if (!valueType) { var err = new ErrorModel("AddVariable", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableName, "ValueType")); listOfErrors.Add(err); isRejected = true; };

                    var dataType = dataTypeCV
                                            .Exists(a => a.Term.ToString() == item.DataType);
                    if (!dataType) { var err = new ErrorModel("AddVariable", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableName, "DataType")); listOfErrors.Add(err); isRejected = true; };

                    var generalCategory = generalCategoryCV
                                            .Exists(a => a.Term.ToString() == item.GeneralCategory);
                    if (!generalCategory) { var err = new ErrorModel("AddVariable", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableName, "generalCategory")); listOfErrors.Add(err); isRejected = true; };

                    var timeUnitsID = units
                                    .Where(a => a.Key == item.TimeUnitsName)
                                    .Select(a => a.Value)
                                    .SingleOrDefault();
                    if (timeUnitsID == 0) { var err = new ErrorModel("AddVariable", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableName, "TimeUnitsID")); listOfErrors.Add(err); isRejected = true; };

                    if (isRejected)
                    {
                        listOfIncorrectRecords.Add(item); continue;
                    }


                    //update model
                    model.VariableUnitsID = variableUnitsID;
                    model.TimeUnitsID = timeUnitsID;

                    //lookup duplicates
                    //var objectSet = objContext.CreateObjectSet<ODM_1_1_1EFModel.Variable>().EntitySet;//.EntitySet;
                    //check if item with this variablecode exists in the database
                    var existingItem = context.Variables.Where(a => a.VariableCode == item.VariableCode).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.Variables.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (existingItem.VariableCode != model.VariableCode) { existingItem.VariableCode = model.VariableCode; editedFields.Add("VariableCode"); }
                        if (existingItem.VariableName != model.VariableName) { existingItem.VariableName = model.VariableName; editedFields.Add("VariableName"); }
                        if (existingItem.Speciation != model.Speciation) { existingItem.Speciation = model.Speciation; editedFields.Add("Speciation"); }
                        if (existingItem.VariableUnitsID != model.VariableUnitsID) { existingItem.VariableUnitsID = model.VariableUnitsID; editedFields.Add("VariableUnitsID"); }
                        if (existingItem.SampleMedium != model.SampleMedium) { existingItem.SampleMedium = model.SampleMedium; editedFields.Add("SampleMedium"); }
                        if (existingItem.ValueType != model.ValueType) { existingItem.ValueType = model.ValueType; editedFields.Add("ValueType"); }
                        if (existingItem.IsRegular != model.IsRegular) { existingItem.IsRegular = model.IsRegular; editedFields.Add("IsRegular"); }
                        if (existingItem.TimeSupport != model.TimeSupport) { existingItem.TimeSupport = model.TimeSupport; editedFields.Add("TimeSupport"); }
                        if (existingItem.TimeUnitsID != model.TimeUnitsID) { existingItem.TimeUnitsID = model.TimeUnitsID; editedFields.Add("TimeUnitsID"); }
                        if (existingItem.DataType != model.DataType) { existingItem.DataType = model.DataType; editedFields.Add("DataType"); }
                        if (existingItem.GeneralCategory != model.GeneralCategory) { existingItem.GeneralCategory = model.GeneralCategory; editedFields.Add("GeneralCategory"); }
                        if (existingItem.NoDataValue != model.NoDataValue) { existingItem.NoDataValue = model.NoDataValue; editedFields.Add("NoDataValue"); }

                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }

                    }
                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Variables
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.Variables.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    //  OffsetTypes
    public class OffsetTypesRepository : IOffsetTypesRepository
    {

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

            if (context.OffsetTypes.Count() != null)
            {
                totalRecordCount = context.OffsetTypes.Count();
                searchRecordCount = totalRecordCount;
            }
            else
            {
                totalRecordCount = searchRecordCount = 0;
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var allItems = context.OffsetTypes.ToList();
                var rst = allItems.
                    Where(c =>
                                c.OffsetTypeID != null && c.OffsetTypeID.ToString().ToLower().Contains(searchString.ToLower())
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
                            { sortedItems = context.OffsetTypes.OrderBy(a => a.OffsetTypeID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.OffsetTypes.OrderByDescending(a => a.OffsetTypeID).Skip(startIndex).Take(pageSize).ToList(); }
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

                if (sortedItems == null) sortedItems = context.OffsetTypes.OrderByDescending(a => a.OffsetTypeID).Skip(startIndex).Take(pageSize).ToList();

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

        public void AddOffsetTypes(List<OffsetTypesModel> itemList, string entityConnectionString, out List<OffsetTypesModel> listOfIncorrectRecords, out List<OffsetTypesModel> listOfCorrectRecords, out List<OffsetTypesModel> listOfDuplicateRecords, out List<OffsetTypesModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<OffsetTypesModel>();
            listOfCorrectRecords = new List<OffsetTypesModel>();
            listOfDuplicateRecords = new List<OffsetTypesModel>();
            listOfEditedRecords = new List<OffsetTypesModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //prefetch Units for quick lookup
            var offsetUnits = context.Units.ToDictionary(p => p.UnitsName.Trim(), p => p.UnitsID);

            foreach (var item in itemList)
            {

                try
                {
                    var model = Mapper.Map<OffsetTypesModel, OffsetType>(item);

                    //need to look up Id's for OffsetUnitsID
                    //User has no concept of ID's
                    //lookup OffsetUnitsID

                    if (offsetUnits.ContainsKey(item.OffsetUnitsName))
                    {
                        var offsetUnitId = offsetUnits[item.OffsetUnitsName];
                        //update model
                        model.OffsetUnitsID = offsetUnitId;
                    }
                    else
                    {
                        listOfIncorrectRecords.Add(item);
                        continue;

                    }

                    //lookup duplicates
                    var existingItem = context.OffsetTypes.Where(a => a.OffsetUnitsID == model.OffsetUnitsID && a.OffsetDescription == model.OffsetDescription).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.OffsetTypes.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        //if (existingItem.OffsetUnitsID != model.OffsetUnitsID) { existingItem.OffsetUnitsID = model.OffsetUnitsID; editedFields.Add("OffsetUnitsID"); }
                        //if (existingItem.OffsetDescription != model.OffsetDescription) { existingItem.OffsetDescription = model.OffsetDescription; editedFields.Add("OffsetDescription"); }

                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

            }
            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.OffsetTypes
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.OffsetTypes.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
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

    //      public void AddISOMetadata(List<ISOMetadataModel> itemList, string entityConnectionString, out List<ISOMetadataModel> listOfIncorrectRecords, out List<ISOMetadataModel> listOfCorrectRecords, out List<ISOMetadataModel> listOfDuplicateRecords, out List<ISOMetadataModel> listOfEditedRecords)
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

            if (context.Sources.Count() != null)
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
                            { sortedItems = context.Sources.OrderBy(a => a.SourceID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Sources.OrderByDescending(a => a.SourceID).Skip(startIndex).Take(pageSize).ToList(); }
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

                if (sortedItems == null) sortedItems = context.Sources.OrderByDescending(a => a.SourceID).Skip(startIndex).Take(pageSize).ToList();

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

        public void AddSources(List<SourcesModel> itemList, string entityConnectionString, out List<SourcesModel> listOfIncorrectRecords, out List<SourcesModel> listOfCorrectRecords, out List<SourcesModel> listOfDuplicateRecords, out List<SourcesModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<SourcesModel>();
            listOfCorrectRecords = new List<SourcesModel>();
            listOfDuplicateRecords = new List<SourcesModel>();
            listOfEditedRecords = new List<SourcesModel>();
            listOfErrors = new List<ErrorModel>();


            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            //read CV in to list for fster searching
            var topicCategoryCV = context.TopicCategoryCVs.ToList();

            foreach (var item in itemList)
            {
                try
                {
                    var model = Mapper.Map<SourcesModel, Source>(item);

                    //set default values
                    string unk = "Unknown";
                    if (String.IsNullOrEmpty(model.ContactName)) model.ContactName = unk;
                    if (String.IsNullOrEmpty(model.Phone)) model.Phone = unk;
                    if (String.IsNullOrEmpty(model.Email)) model.Email = unk;
                    if (String.IsNullOrEmpty(model.Phone)) model.Phone = unk;
                    if (String.IsNullOrEmpty(model.Address)) model.Address = unk;
                    if (String.IsNullOrEmpty(model.City)) model.City = unk;
                    if (String.IsNullOrEmpty(model.State)) model.State = unk;
                    if (String.IsNullOrEmpty(model.ZipCode)) model.ZipCode = unk;
                    if (String.IsNullOrEmpty(model.Citation)) model.Citation = unk;




                    var ism = new ODM_1_1_1EFModel.ISOMetadata();
                    //if (model.MetadataID == null) model.MetadataID = 0;
                    if (ism.TopicCategory == null) ism.TopicCategory = unk;
                    if (ism.Title == null) ism.Title = unk;
                    if (ism.Abstract == null) ism.Abstract = unk;
                    if (ism.ProfileVersion == null) ism.ProfileVersion = unk;
                    //need to look up Id's for VariableUnitsName,TimeUnitsName
                    //User has no concept of ID's
                    bool isRejected = false;
                    var topicCategory = topicCategoryCV
                                           .Exists(a => a.Term.ToString() == item.TopicCategory);
                    if (!topicCategory) { var err = new ErrorModel("AddSources", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.TopicCategory, "topicCategory")); listOfErrors.Add(err); isRejected = true; };

                    if (isRejected)
                    {
                        listOfIncorrectRecords.Add(item); continue;
                    }

                    //Source contains info about metadata 
                    //need to be added first so I can get the ID
                    var existingItem = context.ISOMetadatas
                        .Where(a => a.MetadataLink == item.MetadataLink)
                        .FirstOrDefault();

                    if (existingItem == null)
                    {

                        ism.MetadataLink = item.MetadataLink;
                        ism.Title = item.Title;
                        ism.Abstract = item.Abstract;
                        ism.ProfileVersion = item.ProfileVersion;
                        ism.TopicCategory = item.TopicCategory;

                        context.ISOMetadatas.Add(ism);
                        context.SaveChanges();
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        // TopicCategory	Title	Abstract	ProfileVersion	MetadataLink

                        if (existingItem.TopicCategory != item.TopicCategory) { existingItem.TopicCategory = item.TopicCategory; editedFields.Add("TopicCategory"); }
                        if (existingItem.Title != item.Title) { existingItem.Title = item.Title; editedFields.Add("Title"); }
                        if (existingItem.Abstract != item.Abstract) { existingItem.Abstract = item.Abstract; editedFields.Add("Abstract"); }
                        if (existingItem.ProfileVersion != item.ProfileVersion) { existingItem.ProfileVersion = item.ProfileVersion; editedFields.Add("ProfileVersion"); }
                        if (existingItem.MetadataLink != item.MetadataLink) { existingItem.MetadataLink = item.MetadataLink; editedFields.Add("MetadataLink"); }

                        if (editedFields.Count() > 0)
                        {
                            context.ISOMetadatas.Add(ism);
                            context.SaveChanges();
                            //    listOfEditedRecords.Add(item);
                        }
                    }
                    var metadataId = context.ISOMetadatas
                        .Where(a => a.MetadataLink == item.MetadataLink)
                        .Select(a => a.MetadataID)
                        .FirstOrDefault();

                    //update model
                    model.MetadataID = metadataId;

                    context.Sources.Add(model);
                    context.SaveChanges();


                    listOfCorrectRecords.Add(item);

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }
            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Sources
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.Sources.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    //  Methods
    public class MethodsRepository : IMethodsRepository
    {

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

            if (context.Methods.Count() != null)
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
                                   c.MethodID != null && c.MethodID.ToString().ToLower().Contains(searchString.ToLower())
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
                            { sortedItems = context.Methods.OrderBy(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Methods.OrderByDescending(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList(); }
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

                if (sortedItems == null) sortedItems = context.Methods.OrderByDescending(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList();

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


        public void AddMethods(List<MethodModel> itemList, string entityConnectionString, out List<MethodModel> listOfIncorrectRecords, out List<MethodModel> listOfCorrectRecords, out List<MethodModel> listOfDuplicateRecords, out List<MethodModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<MethodModel>();
            listOfCorrectRecords = new List<MethodModel>();
            listOfDuplicateRecords = new List<MethodModel>();
            listOfEditedRecords = new List<MethodModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            foreach (var item in itemList)
            {

                var model = Mapper.Map<MethodModel, Method>(item);

                try
                {

                    var existingItem = context.Methods.Where(a => a.MethodDescription == model.MethodDescription &&
                                                                    a.MethodLink == model.MethodLink
                                                                    ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.Methods.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        //if (existingItem.VariableCode != model.VariableCode) { existingItem.VariableCode = model.VariableCode; editedFields.Add("VariableCode"); }
                        //if (existingItem.VariableName != model.VariableName) { existingItem.VariableName = model.VariableName; editedFields.Add("VariableName"); }
                        //if (existingItem.Speciation != model.Speciation) { existingItem.Speciation = model.Speciation; editedFields.Add("Speciation"); }
                        //if (existingItem.VariableUnitsID != model.VariableUnitsID) { existingItem.VariableUnitsID = model.VariableUnitsID; editedFields.Add("VariableUnitsID"); }
                        //if (existingItem.SampleMedium != model.SampleMedium) { existingItem.SampleMedium = model.SampleMedium; editedFields.Add("SampleMedium"); }
                        //if (existingItem.ValueType != model.ValueType) { existingItem.ValueType = model.ValueType; editedFields.Add("ValueType"); }
                        //if (existingItem.IsRegular != model.IsRegular) { existingItem.IsRegular = model.IsRegular; editedFields.Add("IsRegular"); }
                        //if (existingItem.TimeSupport != model.TimeSupport) { existingItem.TimeSupport = model.TimeSupport; editedFields.Add("TimeSupport"); }
                        //if (existingItem.TimeUnitsID != model.TimeUnitsID) { existingItem.TimeUnitsID = model.TimeUnitsID; editedFields.Add("TimeUnitsID"); }
                        //if (existingItem.DataType != model.DataType) { existingItem.DataType = model.DataType; editedFields.Add("DataType"); }
                        //if (existingItem.GeneralCategory != model.GeneralCategory) { existingItem.GeneralCategory = model.GeneralCategory; editedFields.Add("GeneralCategory"); }
                        //if (existingItem.NoDataValue != model.NoDataValue) { existingItem.NoDataValue = model.NoDataValue; editedFields.Add("NoDataValue"); }

                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);


                }

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
            foreach (var row in rows)
            {
                context.Methods.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    //  LabMethods
    public class LabMethodsRepository : ILabMethodsRepository
    {

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

            if (context.LabMethods.Count() != null)
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
                                    c.LabMethodID != null && c.LabMethodID.ToString().ToLower().Contains(searchString.ToLower())
                                 || c.LabName != null && c.LabName.ToLower().Contains(searchString.ToLower())
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
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabMethodID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabOrganization).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabOrganization).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "4":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabMethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "5":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.LabMethods.OrderBy(a => a.LabMethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodLink).Skip(startIndex).Take(pageSize).ToList(); }
                            break;

                    }
                }

                if (sortedItems == null) sortedItems = context.LabMethods.OrderByDescending(a => a.LabMethodID).Skip(startIndex).Take(pageSize).ToList();

                //map models
                foreach (var item in sortedItems)
                {

                    var model = Mapper.Map<LabMethod, LabMethodModel>(item);



                    result.Add(model);
                }
            }
            return result;
        }

        public void AddLabMethods(List<LabMethodModel> itemList, string entityConnectionString, out List<LabMethodModel> listOfIncorrectRecords, out List<LabMethodModel> listOfCorrectRecords, out List<LabMethodModel> listOfDuplicateRecords, out List<LabMethodModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<LabMethodModel>();
            listOfCorrectRecords = new List<LabMethodModel>();
            listOfDuplicateRecords = new List<LabMethodModel>();
            listOfEditedRecords = new List<LabMethodModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //prefetch Units for quick lookup
            var offsetUnits = context.Units.ToDictionary(p => p.UnitsName.Trim(), p => p.UnitsID);

            foreach (var item in itemList)
            {

                try
                {
                    var model = Mapper.Map<LabMethodModel, LabMethod>(item);
                    //set default values
                    string unk = "Unknown";
                    if (String.IsNullOrEmpty(model.LabName)) model.LabName = unk;
                    if (model.LabOrganization == null) model.LabOrganization = unk;
                    if (String.IsNullOrEmpty(model.LabMethodName)) model.LabMethodName = unk;
                    if (String.IsNullOrEmpty(model.LabMethodDescription)) model.LabMethodDescription = unk;

                    //lookup duplicates
                    var existingItem = context.LabMethods
                                                 .Where(
                                                     a => a.LabMethodName == model.LabMethodName &&
                                                          a.LabOrganization == model.LabOrganization &&
                                                          a.LabMethodName == model.LabMethodName &&
                                                          a.LabMethodDescription == a.LabMethodDescription &&
                                                          a.LabMethodLink == a.LabMethodLink)
                                                          .FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.LabMethods.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

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
            foreach (var row in rows)
            {
                context.LabMethods.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    //  Samples
    public class SamplesRepository : ISamplesRepository
    {

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

            if (context.Samples.Count() != null)
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
                    Where(c => c.SampleID != null && c.SampleID.ToString().ToLower().Contains(searchString.ToLower())
                                 || c.SampleType != null && c.SampleType.ToLower().Contains(searchString.ToLower())
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
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Samples.OrderBy(a => a.SampleID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Samples.OrderByDescending(a => a.SampleID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Samples.OrderBy(a => a.SampleType).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Samples.OrderByDescending(a => a.SampleType).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "2":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Samples.OrderBy(a => a.LabSampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Samples.OrderByDescending(a => a.LabSampleCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "3":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Samples.OrderBy(a => a.LabMethod.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Samples.OrderByDescending(a => a.LabMethod.LabMethodName).Skip(startIndex).Take(pageSize).ToList(); }
                            break;

                    }
                }

                if (sortedItems == null) sortedItems = context.Samples.OrderByDescending(a => a.SampleID).Skip(startIndex).Take(pageSize).ToList();

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


        public void AddSamples(List<SampleModel> itemList, string entityConnectionString, out List<SampleModel> listOfIncorrectRecords, out List<SampleModel> listOfCorrectRecords, out List<SampleModel> listOfDuplicateRecords, out List<SampleModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<SampleModel>();
            listOfCorrectRecords = new List<SampleModel>();
            listOfDuplicateRecords = new List<SampleModel>();
            listOfEditedRecords = new List<SampleModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //prefetch Units for quick lookup
            var labMethods = context.LabMethods.ToDictionary(p => p.LabMethodName.Trim(), p => p.LabMethodID);

            foreach (var item in itemList)
            {

                try
                {
                    var model = Mapper.Map<SampleModel, Sample>(item);
                    //set default values
                    string unk = "Unknown";
                    if (String.IsNullOrEmpty(model.SampleType)) model.SampleType = unk;


                    //need to look up Id's for LabMethodId
                    //User has no concept of ID's
                    //lookup LabmethodId
                    if (item.LabMethodName != null)
                    {
                        if (labMethods.ContainsKey(item.LabMethodName))
                        {
                            var labMethodsId = labMethods[item.LabMethodName];
                            //update model
                            model.LabMethodID = labMethodsId;
                        }
                        else
                        {
                            //if CSV has LabMethidId specified convert and process
                            int labMethodId;
                            bool res = int.TryParse(item.LabMethodName, out labMethodId);
                            if (res)
                            {
                                //update model
                                model.LabMethodID = labMethodId;
                            }
                            else
                            {
                                listOfIncorrectRecords.Add(item);
                                continue;

                            }
                        }
                    }
                    else
                    {
                        listOfIncorrectRecords.Add(item);
                        continue;
                    }

                    //lookup duplicates
                    var existingItem = context.Samples.Where(a => a.SampleType == model.SampleType &&
                                                                  a.LabSampleCode == model.LabSampleCode &&
                                                                  a.LabMethodID == a.LabMethodID
                                                                  ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.Samples.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

            }
            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Samples
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.Samples.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    //  Qualifiers
    public class QualifiersRepository : IQualifiersRepository
    {

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

            if (context.Qualifiers.Count() != null)
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
                                c.QualifierID != null && c.QualifierID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.QualifierCode != null && c.QualifierCode.ToLower().Contains(searchString.ToLower())
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
                        case "0":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Qualifiers.OrderBy(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Qualifiers.OrderByDescending(a => a.QualifierCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "1":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.Qualifiers.OrderBy(a => a.QualifierDescription).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.Qualifiers.OrderByDescending(a => a.QualifierDescription).Skip(startIndex).Take(pageSize).ToList(); }
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

        public void AddQualifiers(List<QualifiersModel> itemList, string entityConnectionString, out List<QualifiersModel> listOfIncorrectRecords, out List<QualifiersModel> listOfCorrectRecords, out List<QualifiersModel> listOfDuplicateRecords, out List<QualifiersModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<QualifiersModel>();
            listOfCorrectRecords = new List<QualifiersModel>();
            listOfDuplicateRecords = new List<QualifiersModel>();
            listOfEditedRecords = new List<QualifiersModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            foreach (var item in itemList)
            {

                try
                {
                    var model = Mapper.Map<QualifiersModel, Qualifier>(item);



                    //lookup duplicates
                    var existingItem = context.Qualifiers.Where(a => a.QualifierCode == model.QualifierCode && a.QualifierDescription == model.QualifierDescription).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.Qualifiers.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

            }
            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Qualifiers
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.Qualifiers.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }
    //  QualityControlLevels
    public class QualityControlLevelsRepository : IQualityControlLevelRepository
    {

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

            if (context.QualityControlLevels.Count() != null)
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

        public void AddQualityControlLevel(List<QualityControlLevelModel> itemList, string entityConnectionString, out List<QualityControlLevelModel> listOfIncorrectRecords, out List<QualityControlLevelModel> listOfCorrectRecords, out List<QualityControlLevelModel> listOfDuplicateRecords, out List<QualityControlLevelModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<QualityControlLevelModel>();
            listOfCorrectRecords = new List<QualityControlLevelModel>();
            listOfDuplicateRecords = new List<QualityControlLevelModel>();
            listOfEditedRecords = new List<QualityControlLevelModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            foreach (var item in itemList)
            {

                try
                {
                    var model = Mapper.Map<QualityControlLevelModel, QualityControlLevel>(item);



                    //lookup duplicates
                    var existingItem = context.QualityControlLevels.Where(a => a.QualityControlLevelCode == model.QualityControlLevelCode &&
                                                                               a.Definition == model.Definition &&
                                                                               a.Explanation == model.Explanation
                                                                               ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.QualityControlLevels.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

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
            foreach (var row in rows)
            {
                context.QualityControlLevels.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }
    //  DataValues
    public class DataValuesRepository : IDataValuesRepository
    {

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

            if (context.DataValues.Count() != null)
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
                var allItems = context.DataValues.ToList();
                var rst = allItems.
                    Where(c =>
                        c.ValueID != null && c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.DataValue1 != null && c.DataValue1.ToString().ToLower().Contains(searchString.ToLower())
                             || c.ValueAccuracy != null && c.ValueAccuracy.ToString().ToLower().Contains(searchString.ToLower())
                             || c.LocalDateTime != null && c.LocalDateTime.ToString().Contains(searchString.ToLower())
                             || c.UTCOffset != null && c.UTCOffset.ToString().ToLower().Contains(searchString.ToLower())
                             || c.DateTimeUTC != null && c.DateTimeUTC.ToString().ToLower().Contains(searchString.ToLower())
                             || c.Site != null && c.Site.SiteCode.ToLower().Contains(searchString.ToLower())
                             || c.Variable != null && c.Variable.VariableCode.ToLower().Contains(searchString.ToLower())
                             || c.OffsetValue != null && c.OffsetValue.ToString().ToLower().Contains(searchString.ToLower())
                             || c.OffsetTypeID != null && c.OffsetTypeID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.CensorCode != null && c.CensorCode.ToLower().Contains(searchString.ToLower())
                             || c.QualifierID != null && c.QualifierID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.MethodID != null && c.MethodID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.SourceID != null && c.SourceID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.SampleID != null && c.SampleID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.DerivedFromID != null && c.DerivedFromID.ToString().ToLower().Contains(searchString.ToLower())
                             || c.QualityControlLevelID != null && c.QualityControlLevelID.ToString().ToLower().Contains(searchString.ToLower())
                          );
                if (rst == null) return result;
                //count
                searchRecordCount = rst.Count();
                //take only top x
                var finalrst = rst.Take(pageSize).ToList();

                foreach (var item in finalrst)
                {

                    var model = Mapper.Map<DataValue, DataValuesModel>(item);

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
                            { sortedItems = context.DataValues.OrderBy(a => a.OffsetTypeID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.OffsetTypeID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "10":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.CensorCode).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.CensorCode).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "11":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.QualifierID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.QualifierID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "12":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.MethodID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "13":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.SourceID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.SourceID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "14":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.SampleID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.SampleID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "15":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.DerivedFromID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.DerivedFromID).Skip(startIndex).Take(pageSize).ToList(); }
                            break;
                        case "16":
                            if (sortedColumn.Direction.ToString().ToLower() == "ascending")
                            { sortedItems = context.DataValues.OrderBy(a => a.QualityControlLevelID).Skip(startIndex).Take(pageSize).ToList(); }
                            else
                            { sortedItems = context.DataValues.OrderByDescending(a => a.QualityControlLevelID).Skip(startIndex).Take(pageSize).ToList(); }
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
                                         .Select(a => a.VariableName)
                                         .FirstOrDefault();

                    model.SiteCode = context.Sites
                                        .Where(a => a.SiteID == item.SiteID)
                                        .Select(a => a.SiteCode)
                                        .FirstOrDefault();

                    result.Add(model);
                }
            }
            return result;
        }

        public void AddDataValues(List<DataValuesModel> itemList, string entityConnectionString, out List<DataValuesModel> listOfIncorrectRecords, out List<DataValuesModel> listOfCorrectRecords, out List<DataValuesModel> listOfDuplicateRecords, out List<DataValuesModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<DataValuesModel>();
            listOfCorrectRecords = new List<DataValuesModel>();
            listOfDuplicateRecords = new List<DataValuesModel>();
            listOfEditedRecords = new List<DataValuesModel>();
            listOfErrors = new List<ErrorModel>();

            var recordsToInsert = new List<DataValue>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            //var objContext = ((IObjectContextAdapter)context).ObjectContext;
            //get data to lookup values
            var sites = context.Sites.ToDictionary(p => p.SiteCode, p => p.SiteID);
            var variables = context.Variables.ToDictionary(p => p.VariableCode, p => p.VariableID);
            var OffsetTypeIds = context.OffsetTypes.Select(p => p.OffsetTypeID).ToList();
            var censorCodeCV = context.CensorCodeCVs.ToList();
            var QualifierId = context.Qualifiers.Select(p => p.QualifierID);
            var methodIds = context.Methods.ToList();
            var sourceIds = context.Sources.ToList();
            var sampleIds = context.Samples.ToList();
            //var derivedFromIds = context.DerivedFroms.Select(p => p.DerivedFromID);
            var qualityControlLevelIds = context.QualityControlLevels.ToDictionary(p => p.QualityControlLevelCode, p => p.QualityControlLevelID);


            foreach (var item in itemList)
            {
                try
                {
                    bool isRejected = false;
                    var model = Mapper.Map<DataValuesModel, DataValue>(item);
                    //set default values
                    string unk = "Unknown";
                    if (String.IsNullOrEmpty(model.CensorCode)) model.CensorCode = "nc";
                    if (model.QualityControlLevelID == 0) model.QualityControlLevelID = -9999;

                    //lookup siteid
                    if (sites.ContainsKey(item.SiteCode))
                    {
                        var siteId = sites[item.SiteCode];
                        //update model
                        model.SiteID = siteId;
                    }
                    else
                    {
                        var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.SiteCode, "AddDataValues")); listOfErrors.Add(err); isRejected = true;

                        //continue;

                    }

                    if (variables.ContainsKey(item.VariableCode))
                    {
                        var variableId = variables[item.VariableCode];
                        //update model
                        model.VariableID = variableId;
                    }
                    else
                    {
                        var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableCode, "AddDataValues")); listOfErrors.Add(err); isRejected = true;

                    }
                    if (model.OffsetTypeID != null)
                    {
                        var offsetTyperID = OffsetTypeIds
                                                .Exists(a => a == model.OffsetTypeID);
                        var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_DATABASE, item.OffsetTypeID.ToString(), "OffsetTypes:")); listOfErrors.Add(err); isRejected = true;

                    }

                    if (model.CensorCode != "nc")
                    {
                        var censorCode = censorCodeCV
                                                .Exists(a => a.Term == item.CensorCode);
                        if (!censorCode) { var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.OffsetTypeID, "AddDataValues")); listOfErrors.Add(err); isRejected = true; };

                    }

                    if (model.QualifierID != null)
                    {
                        var censorCode = censorCodeCV
                                                .Exists(a => a.Term == item.CensorCode);
                        if (!censorCode) { var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_DATABASE, item.QualifierID, "Qualifiers")); listOfErrors.Add(err); isRejected = true; };

                    }

                    if (model.MethodID != null)
                    {
                        var methodId = methodIds
                                                .Exists(a => a.MethodID == model.MethodID);
                        if (!methodId) { var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_DATABASE, item.MethodID, "Methods")); listOfErrors.Add(err); isRejected = true; };

                    }

                    if (model.SourceID != null)
                    {
                        var sourceId = sourceIds
                                                .Exists(a => a.SourceID == model.SourceID);
                        if (!sourceId) { var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_DATABASE, item.SourceID, "Sources")); listOfErrors.Add(err); isRejected = true; };

                    }

                    if (model.SampleID != null)
                    {
                        var sampleId = sampleIds
                                                .Exists(a => a.SampleID == model.SampleID);
                        if (!sampleId) { var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_DATABASE, item.SampleID, "Samples")); listOfErrors.Add(err); isRejected = true; };

                    }

                    if (qualityControlLevelIds.ContainsKey(item.QualityControlLevelCode))
                    {
                        var qualityControlLevelId = qualityControlLevelIds[item.QualityControlLevelCode];
                        //update model
                        model.QualityControlLevelID = qualityControlLevelId;
                    }
                    else
                    {
                        var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableCode, "AddDataValues")); listOfErrors.Add(err); isRejected = true;

                    }
                    // var dataType = dataTypeCV
                    //                         .Exists(a => a.Term.ToString() == item.DataType);

                    if (isRejected)
                    {
                        listOfIncorrectRecords.Add(item); continue;
                    }

                    //else
                    //{
                    //    int variableId;
                    //    bool res = int.TryParse(item.VariableID, out variableId);
                    //    if (res)
                    //    {
                    //        //update model
                    //        model.VariableID = variableId;
                    //    }
                    //    else
                    //    {
                    //        listOfIncorrectRecords.Add(item);
                    //        continue;

                    //    }
                    //}
                    //Validate foreign keys
                    //var methodId = context.Methods
                    //                       .Where(a => a.MethodID == model.MethodID)
                    //                       .Select(a => a);


                    //lookup duplicates
                    //check if item with this item exists in the database
                    //object existingItem = null;
                    var existingItem = context.DataValues.Where(a => a.DataValue1 == model.DataValue1 &&
                                                                     a.ValueAccuracy == model.ValueAccuracy &&
                                                                     a.LocalDateTime == model.LocalDateTime &&
                                                                     a.UTCOffset == model.UTCOffset &&
                                                                     a.DateTimeUTC == model.DateTimeUTC &&
                                                                     a.SiteID == model.SiteID &&
                                                                     a.VariableID == model.VariableID &&
                                                                     a.OffsetValue == model.OffsetValue &&
                                                                     a.OffsetTypeID == model.OffsetTypeID &&
                                                                     a.CensorCode == model.CensorCode &&
                                                                     a.QualifierID == model.QualifierID &&
                                                                     a.MethodID == model.MethodID &&
                                                                     a.SourceID == model.SourceID &&
                                                                     a.SampleID == model.SampleID &&
                                                                     a.DerivedFromID == model.DerivedFromID &&
                                                                     a.QualityControlLevelID == model.QualityControlLevelID
                                                                     ).FirstOrDefault();


                    if (existingItem == null)
                    {
                        //context.DataValues.Add(model);
                        recordsToInsert.Add(model);
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (existingItem.DataValue1 != model.DataValue1) { existingItem.DataValue1 = model.DataValue1; editedFields.Add("DataValue1"); }
                        if (existingItem.ValueAccuracy != model.ValueAccuracy) { existingItem.ValueAccuracy = model.ValueAccuracy; editedFields.Add("ValueAccuracy"); }
                        if (existingItem.LocalDateTime != model.LocalDateTime) { existingItem.LocalDateTime = model.LocalDateTime; editedFields.Add("LocalDateTime"); }
                        if (existingItem.UTCOffset != model.UTCOffset) { existingItem.UTCOffset = model.UTCOffset; editedFields.Add("UTCOffset"); }
                        if (existingItem.SiteID != model.SiteID) { existingItem.SiteID = model.SiteID; editedFields.Add("SiteID"); }
                        if (existingItem.VariableID != model.VariableID) { existingItem.VariableID = model.VariableID; editedFields.Add("VariableID"); }
                        if (existingItem.OffsetValue != model.OffsetValue) { existingItem.OffsetValue = model.OffsetValue; editedFields.Add("OffsetValue"); }
                        if (existingItem.OffsetTypeID != model.OffsetTypeID) { existingItem.OffsetTypeID = model.OffsetTypeID; editedFields.Add("OffsetTypeID"); }
                        if (existingItem.CensorCode != model.CensorCode) { existingItem.CensorCode = model.CensorCode; editedFields.Add("CensorCode"); }
                        if (existingItem.QualifierID != model.QualifierID) { existingItem.QualifierID = model.QualifierID; editedFields.Add("QualifierID"); }
                        if (existingItem.MethodID != model.MethodID) { existingItem.MethodID = model.MethodID; editedFields.Add("MethodID"); }
                        if (existingItem.SourceID != model.SourceID) { existingItem.SourceID = model.SourceID; editedFields.Add("SourceID"); }
                        if (existingItem.SampleID != model.SampleID) { existingItem.SampleID = model.SampleID; editedFields.Add("SampleID"); }
                        if (existingItem.DerivedFromID != model.DerivedFromID) { existingItem.DerivedFromID = model.DerivedFromID; editedFields.Add("DerivedFromID"); }
                        if (existingItem.QualityControlLevelID != model.QualityControlLevelID) { existingItem.QualityControlLevelID = model.QualityControlLevelID; editedFields.Add("QualityControlLevelID"); }


                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }


            }
            //context.SaveChanges();
            //Pass in cnx, tablename, and list of imports
            Utils.BulkInsert(context.Database.Connection.ConnectionString, "Datavalues", recordsToInsert);

            //Utils.UpdateSeriesCatalog(context.Database.Connection.ConnectionString);


            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.DataValues

                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.DataValues.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }



    //  GroupDescriptions
    public class GroupDescriptionsRepository : IGroupDescriptionsRepository
    {

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

            if (context.GroupDescriptions.Count() != null)
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

        public void AddGroupDescriptions(List<GroupDescriptionModel> itemList, string entityConnectionString, out List<GroupDescriptionModel> listOfIncorrectRecords, out List<GroupDescriptionModel> listOfCorrectRecords, out List<GroupDescriptionModel> listOfDuplicateRecords, out List<GroupDescriptionModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<GroupDescriptionModel>();
            listOfCorrectRecords = new List<GroupDescriptionModel>();
            listOfDuplicateRecords = new List<GroupDescriptionModel>();
            listOfEditedRecords = new List<GroupDescriptionModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            foreach (var item in itemList)
            {
                try
                {
                    var model = Mapper.Map<GroupDescriptionModel, GroupDescription>(item);

                    //lookup duplicates
                    var existingItem = context.GroupDescriptions.Where(a => a.GroupDescription1 == model.GroupDescription1
                                                                               ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.GroupDescriptions.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

            }
            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.GroupDescriptions
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.GroupDescriptions.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }
    //  Groups
    public class GroupsRepository : IGroupsRepository
    {

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

            if (context.Groups.Count() != null)
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
                                    c.GroupID != null && c.GroupID.ToString().ToLower().Contains(searchString.ToLower())
                                 || c.ValueID != null && c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
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

        public void AddGroups(List<GroupsModel> itemList, string entityConnectionString, out List<GroupsModel> listOfIncorrectRecords, out List<GroupsModel> listOfCorrectRecords, out List<GroupsModel> listOfDuplicateRecords, out List<GroupsModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<GroupsModel>();
            listOfCorrectRecords = new List<GroupsModel>();
            listOfDuplicateRecords = new List<GroupsModel>();
            listOfEditedRecords = new List<GroupsModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            var groupDescriptions = context.GroupDescriptions.ToList();


            foreach (var item in itemList)
            {

                try
                {
                    var model = Mapper.Map<GroupsModel, Group>(item);
                    bool isRejected = false;



                    //lookup siteid
                    if (model.GroupID != 0)
                    {
                        var groupId = groupDescriptions
                                                .Exists(a => a.GroupID == model.GroupID);
                        if (!groupId) { var err = new ErrorModel("AddDataValues", string.Format(Ressources.IMPORT_VALUE_NOT_IN_DATABASE, item.GroupID, "GroupDescriptions")); listOfErrors.Add(err); isRejected = true; };

                    }
                    if (isRejected)
                    {
                        listOfIncorrectRecords.Add(item); continue;
                    }


                    //lookup duplicates
                    var existingItem = context.Groups.Where(a => a.GroupID == model.GroupID &&
                                                                               a.ValueID == model.ValueID
                                                                               ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.Groups.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

            }
            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Groups
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.Groups.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    //  DerivedFrom
    public class DerivedFromRepository : IDerivedFromRepository
    {

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

            if (context.DerivedFroms.Count() != null)
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
                                    c.DerivedFromID != null && c.DerivedFromID.ToString().ToLower().Contains(searchString.ToLower())
                                 || c.ValueID != null && c.ValueID.ToString().ToLower().Contains(searchString.ToLower())
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

        public void AddDerivedFrom(List<DerivedFromModel> itemList, string entityConnectionString, out List<DerivedFromModel> listOfIncorrectRecords, out List<DerivedFromModel> listOfCorrectRecords, out List<DerivedFromModel> listOfDuplicateRecords, out List<DerivedFromModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<DerivedFromModel>();
            listOfCorrectRecords = new List<DerivedFromModel>();
            listOfDuplicateRecords = new List<DerivedFromModel>();
            listOfEditedRecords = new List<DerivedFromModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

            var derivedFroms = context.DerivedFroms.ToList();

            foreach (var item in itemList)
            {

                try
                {
                    var model = Mapper.Map<DerivedFromModel, DerivedFrom>(item);

                    bool isRejected = false;

                    //lookup Value Id in datavalues
                    if (model.DerivedFromID != 0)
                    {
                        var derivedFrom = context.DataValues
                                                .FirstOrDefault(a => a.ValueID == model.ValueID);
                        if (derivedFrom == null) { var err = new ErrorModel("AddDerivedFrom", string.Format(Ressources.IMPORT_VALUE_NOT_IN_DATABASE, item.ValueID, "DerivedFroms")); listOfErrors.Add(err); isRejected = true; };

                    }
                    if (isRejected)
                    {
                        listOfIncorrectRecords.Add(item); continue;
                    }


                    //lookup duplicates
                    var existingItem = context.DerivedFroms.Where(a => a.DerivedFromID == model.DerivedFromID &&
                                                                               a.ValueID == model.ValueID
                                                                               ).FirstOrDefault();

                    if (existingItem == null)
                    {
                        context.DerivedFroms.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.DerivedFroms
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.DerivedFroms.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    //  Categories
    public class CategoriesRepository : ICategoriesRepository
    {

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

            if (context.Categories.Count() != null)
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

        public void AddCategories(List<CategoriesModel> itemList, string entityConnectionString, out List<CategoriesModel> listOfIncorrectRecords, out List<CategoriesModel> listOfCorrectRecords, out List<CategoriesModel> listOfDuplicateRecords, out List<CategoriesModel> listOfEditedRecords, out List<ErrorModel> listOfErrors)
        {
            listOfIncorrectRecords = new List<CategoriesModel>();
            listOfCorrectRecords = new List<CategoriesModel>();
            listOfDuplicateRecords = new List<CategoriesModel>();
            listOfEditedRecords = new List<CategoriesModel>();
            listOfErrors = new List<ErrorModel>();

            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            var variables = context.Variables.ToDictionary(p => p.VariableCode, p => p.VariableID);

            foreach (var item in itemList)
            {

                try
                {
                    var model = Mapper.Map<CategoriesModel, Category>(item);
                    bool isRejected = false;

                    var variablesID = variables
                                      .Where(a => a.Key == item.VariableCode)
                                      .Select(a => a.Value)
                                      .SingleOrDefault();
                    if (variablesID == 0)
                    {
                        var err = new ErrorModel("AddCategories", string.Format(Ressources.IMPORT_VALUE_NOT_IN_CV, item.VariableCode, "Variables")); listOfErrors.Add(err); isRejected = true;
                    }
                    else
                    {
                        model.VariableID = variablesID;
                    }
                    if (isRejected)
                    {
                        listOfIncorrectRecords.Add(item); continue;
                    }
                    //lookup duplicates
                    var existingItem = context.Categories.Where(a => a.VariableID == model.VariableID &&
                                                                                a.DataValue == model.DataValue &&
                                                                                a.CategoryDescription == model.CategoryDescription
                                                                                ).FirstOrDefault();


                    if (existingItem == null)
                    {
                        context.Categories.Add(model);
                        context.SaveChanges();
                        listOfCorrectRecords.Add(item);
                    }
                    else
                    {
                        var editedFields = new List<string>();
                        if (editedFields.Count() > 0)
                        {
                            context.SaveChanges();
                            listOfEditedRecords.Add(item);
                        }
                        else
                        {
                            listOfDuplicateRecords.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    listOfIncorrectRecords.Add(item);
                }

            }

            return;
        }

        public void deleteAll(string entityConnectionString)
        {
            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
            var rows = from o in context.Categories
                       select o;
            if (rows.Count() == 0) return;
            foreach (var row in rows)
            {
                context.Categories.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }
    }

    // Seriescatalog
    public class SeriesCatalogRepository : ISeriesCatalogRepository
    {
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
            foreach (var row in rows)
            {
                context.SeriesCatalogs.Remove(row);
            }
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

        }

        public void UpdateSeriesCatalog(string entityConnectionstring)
        {
            //var s = Ressources.EFMODELDEF_IN_CONNECTIONSTRING;
            //var constring = "metadata=res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl;provider=System.Data.SqlClient;provider connection string=&quot; Data Source=tcp:bhi5g2ajst.database.windows.net,1433;Initial Catalog=HydroServertest2;User ID=HisCentralAdmin@bhi5g2ajst; Password=f3deratedResearch; Persist Security Info=true; MultipleActiveResultSets=True;App=EntityFramework";
            //var constring = "metadata=res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=mseul-cuahsi;initial catalog=HydroSample2;integrated security=true; MultipleActiveResultSets=True;App=EntityFramework";
            //var cleanedConnectionString = entityConnectionstring.Replace(s, string.Empty);
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
                            new SqlBulkCopy(destinationConnection.ConnectionString, SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.CheckConstraints))
                {
                    //bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsTransfer);
                    //bulkCopy.NotifyAfter = 100;
                    bulkCopy.BatchSize = 50;

                    foreach (DataColumn dc in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                    }


                    bulkCopy.DestinationTableName = "SeriesCatalog";
                    bulkCopy.WriteToServer(dataTable);
                }
            }

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