using AutoMapper;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using ODM_1_1_1EFModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
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

            public void AddSites(List<SiteModel> sites, string entityConnectionString, out List<SiteModel> listOfIncorrectRecords, out List<SiteModel> listOfCorrectRecords, out List<SiteModel> listOfDuplicateRecords, out List<SiteModel> listOfEditedRecords)
            {
                listOfIncorrectRecords = new List<SiteModel>();
                listOfCorrectRecords = new List<SiteModel>();
                listOfDuplicateRecords = new List<SiteModel>();
                listOfEditedRecords = new List<SiteModel>();


                var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
                var objContext = ((IObjectContextAdapter)context).ObjectContext;

                foreach (var s in sites)
                {
                    var item = new ODM_1_1_1EFModel.Site();

                    try
                    {
                        var d = Mapper.Map<SiteModel, Site>(s);
                        
                        var LatLongDatumID = context.SpatialReferences
                                             .Where (a => a.SRSName == s.LatLongDatumSRSName)
                                             .Select (a => a.SpatialReferenceID)
                                             .FirstOrDefault();
                        d.LatLongDatumID = LatLongDatumID;

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

                        var existingItem = context.Sites.Where(a => a.SiteCode == s.SiteCode).FirstOrDefault();
                        //var j = context.Sites.Find(s.SiteCode);

                        if (existingItem == null)
                        { 
                            context.Sites.Add(d);
                            context.SaveChanges();
                            listOfCorrectRecords.Add(s);
                        }
                        else
                        {
                            var editedFields = new List<string>();
                            if (existingItem.SiteName != d.SiteName) { existingItem.SiteName = d.SiteName; editedFields.Add("SiteName"); }
                            if (existingItem.Latitude != d.Latitude) { existingItem.Latitude = d.Latitude; editedFields.Add("Latitude"); }
                            if (existingItem.Longitude != d.Longitude) { existingItem.Longitude = d.Longitude; editedFields.Add("Longitude"); }
                            if (existingItem.LatLongDatumID != d.LatLongDatumID) { existingItem.LatLongDatumID = d.LatLongDatumID; editedFields.Add("LatLongDatumID"); }
                            if (existingItem.Elevation_m != d.Elevation_m) {existingItem.Elevation_m = d.Elevation_m; editedFields.Add("Elevation_m");}
                            if (existingItem.VerticalDatum != d.VerticalDatum) {existingItem.VerticalDatum = d.VerticalDatum; editedFields.Add("VerticalDatum");}
                            if (existingItem.LocalX != d.LocalX) {existingItem.LocalX = d.LocalX; editedFields.Add("LocalX");}
                            if (existingItem.LocalY != d.LocalY) {existingItem.LocalY = d.LocalY; editedFields.Add("LocalY");}
                            if (existingItem.LocalProjectionID != d.LocalProjectionID) {existingItem.LocalProjectionID = d.LocalProjectionID; editedFields.Add("LocalProjectionID");}
                            if (existingItem.PosAccuracy_m != d.PosAccuracy_m) {existingItem.PosAccuracy_m = d.PosAccuracy_m; editedFields.Add("PosAccuracy_m");}
                            if (existingItem.State != d.State) {existingItem.State = d.State; editedFields.Add("State");}
                            if (existingItem.County != d.County) {existingItem.County = d.County; editedFields.Add("County");}
                            if (existingItem.Comments != d.Comments) {existingItem.Comments = d.Comments; editedFields.Add("Comments");}
                            if (existingItem.SiteType != d.SiteType) {existingItem.SiteType = d.SiteType; editedFields.Add("SiteType");}


                            if (editedFields.Count() > 0)
                            {
                                context.SaveChanges();
                                listOfEditedRecords.Add(s);
                            }
                            else
                            {
                                listOfDuplicateRecords.Add(s);
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
                    catch (Exception ex)
                    {
                        listOfIncorrectRecords.Add(s);


                    }

                }


                return;
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

        public void AddVariables(List<VariablesModel> itemList, string entityConnectionString, out List<VariablesModel> listOfIncorrectRecords, out List<VariablesModel> listOfCorrectRecords, out List<VariablesModel> listOfDuplicateRecords, out List<VariablesModel> listOfEditedRecords)
        {
            listOfIncorrectRecords = new List<VariablesModel>();
            listOfCorrectRecords = new List<VariablesModel>();
            listOfDuplicateRecords = new List<VariablesModel>();
            listOfEditedRecords = new List<VariablesModel>();

            
                var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
                var objContext = ((IObjectContextAdapter)context).ObjectContext;
                   
                foreach (var item in itemList)
                {
                    try
                    {
                    var model = Mapper.Map<VariablesModel, Variable>(item);

                    //need to look up Id's for VariableUnitsName,TimeUnitsName
                    //User has no concept of ID's

                    var variableUnitsID = context.Units
                        .Where (a => a.UnitsName == item.VariableUnitsName)
                        .Select (a => a.UnitsID)
                        .FirstOrDefault();

                    var timeUnitsID = context.Units
                       .Where(a => a.UnitsName == item.TimeUnitsName)
                       .Select(a => a.UnitsID)
                       .FirstOrDefault();
                    //update model
                    model.VariableUnitsID = variableUnitsID;
                    model.TimeUnitsID = timeUnitsID;

                    //lookup duplicates
                    var objectSet = objContext.CreateObjectSet<ODM_1_1_1EFModel.Variable>().EntitySet;//.EntitySet;
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
           var modelList = new List<OffsetTypesModel>();
           foreach (var item in items)
           {

               var model = Mapper.Map<OffsetType, OffsetTypesModel>(item);

               modelList.Add(model);
           }
           return modelList;
       }

       public void AddOffsetTypes(List<OffsetTypesModel> itemList, string entityConnectionString, out List<OffsetTypesModel> listOfIncorrectRecords, out List<OffsetTypesModel> listOfCorrectRecords, out List<OffsetTypesModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<OffsetTypesModel>();
           listOfCorrectRecords = new List<OffsetTypesModel>();
           listOfDuplicateRecords = new List<OffsetTypesModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<OffsetTypesModel, OffsetType>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<OffsetType>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[OffsetTypes] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
       }

   }

 //  ISOMetadata
   public class ISOMetadataRepository : IISOMetadataRepository
   {

       public List<ISOMetadataModel> GetAll(string connectionString)
       {
           // Create an EntityConnection.         
           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);

           var items = from obj in context.ISOMetadatas
                       select obj;
           var modelList = new List<ISOMetadataModel>();
           foreach (var item in items)
           {

               var model = Mapper.Map<ODM_1_1_1EFModel.ISOMetadata, HydroserverToolsBusinessObjects.Models.ISOMetadataModel>(item);
            
               modelList.Add(model);
           }
           return modelList;
       }

       public void AddISOMetadata(List<ISOMetadataModel> itemList, string entityConnectionString, out List<ISOMetadataModel> listOfIncorrectRecords, out List<ISOMetadataModel> listOfCorrectRecords, out List<ISOMetadataModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<ISOMetadataModel>();
           listOfCorrectRecords = new List<ISOMetadataModel>();
           listOfDuplicateRecords = new List<ISOMetadataModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<ISOMetadataModel, ISOMetadata>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<ISOMetadata>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[ISOMetaData] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
       }

   }
   
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

       public void AddSources(List<SourcesModel> itemList, string entityConnectionString, out List<SourcesModel> listOfIncorrectRecords, out List<SourcesModel> listOfCorrectRecords, out List<SourcesModel> listOfDuplicateRecords, out List<SourcesModel> listOfEditedRecords)
       {
           listOfIncorrectRecords = new List<SourcesModel>();
           listOfCorrectRecords = new List<SourcesModel>();
           listOfDuplicateRecords = new List<SourcesModel>();
           listOfEditedRecords = new List<SourcesModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {
               try
               {
                   var model = Mapper.Map<SourcesModel, Source>(item);
                   var ism = new ODM_1_1_1EFModel.ISOMetadata();
                   //need to look up Id's for VariableUnitsName,TimeUnitsName
                   //User has no concept of ID's

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

       public void AddMethods(List<MethodModel> itemList, string entityConnectionString, out List<MethodModel> listOfIncorrectRecords, out List<MethodModel> listOfCorrectRecords, out List<MethodModel> listOfDuplicateRecords, out List<MethodModel> listOfEditedRecords)
       {
           listOfIncorrectRecords = new List<MethodModel>();
           listOfCorrectRecords = new List<MethodModel>();
           listOfDuplicateRecords = new List<MethodModel>();
           listOfEditedRecords = new List<MethodModel>();


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

       public void AddLabMethods(List<LabMethodModel> itemList, string entityConnectionString, out List<LabMethodModel> listOfIncorrectRecords, out List<LabMethodModel> listOfCorrectRecords, out List<LabMethodModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<LabMethodModel>();
           listOfCorrectRecords = new List<LabMethodModel>();
           listOfDuplicateRecords = new List<LabMethodModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<LabMethodModel, LabMethod>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<LabMethod>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[LabMethods] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
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
           var modelList = new List<SampleModel>();
           foreach (var item in items)
           {

               var model = Mapper.Map<ODM_1_1_1EFModel.Sample, HydroserverToolsBusinessObjects.Models.SampleModel>(item);

               modelList.Add(model);
           }
           return modelList;
       }

       public void AddSamples(List<SampleModel> itemList, string entityConnectionString, out List<SampleModel> listOfIncorrectRecords, out List<SampleModel> listOfCorrectRecords, out List<SampleModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<SampleModel>();
           listOfCorrectRecords = new List<SampleModel>();
           listOfDuplicateRecords = new List<SampleModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<SampleModel, Sample>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<Sample>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[Samples] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
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

       public void AddQualifiers(List<QualifiersModel> itemList, string entityConnectionString, out List<QualifiersModel> listOfIncorrectRecords, out List<QualifiersModel> listOfCorrectRecords, out List<QualifiersModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<QualifiersModel>();
           listOfCorrectRecords = new List<QualifiersModel>();
           listOfDuplicateRecords = new List<QualifiersModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<QualifiersModel, Qualifier>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<Qualifier>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[Qualifiers] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
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

       public void AddQualityControlLevel(List<QualityControlLevelModel> itemList, string entityConnectionString, out List<QualityControlLevelModel> listOfIncorrectRecords, out List<QualityControlLevelModel> listOfCorrectRecords, out List<QualityControlLevelModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<QualityControlLevelModel>();
           listOfCorrectRecords = new List<QualityControlLevelModel>();
           listOfDuplicateRecords = new List<QualityControlLevelModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<QualityControlLevelModel, QualityControlLevel>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<QualityControlLevel>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[QualityControlLevels] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
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

       public void AddDataValues(List<DataValuesModel> itemList, string entityConnectionString, out List<DataValuesModel> listOfIncorrectRecords, out List<DataValuesModel> listOfCorrectRecords, out List<DataValuesModel> listOfDuplicateRecords, out List<DataValuesModel> listOfEditedRecords)
       {
           listOfIncorrectRecords = new List<DataValuesModel>();
           listOfCorrectRecords = new List<DataValuesModel>();
           listOfDuplicateRecords = new List<DataValuesModel>();
           listOfEditedRecords = new List<DataValuesModel>();

           var recordsToInsert = new List<DataValue>();

           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           //var objContext = ((IObjectContextAdapter)context).ObjectContext;

           var sites = context.Sites.ToDictionary(p => p.SiteCode, p => p.SiteID);               
                    
           var variables = context.Variables.ToDictionary(p => p.VariableCode, p=>p.VariableID);  

           foreach (var item in itemList)
           {
               try
               {
                   var model = Mapper.Map<DataValuesModel, DataValue>(item);

                   //lookup siteid
                   if (sites.ContainsKey(item.SiteCode))
                   {
                       var siteId = sites[item.SiteCode];
                       //update model
                       model.SiteID = siteId;
                   }
                   else
                   {
                       listOfIncorrectRecords.Add(item);
                       continue;
                       
                   }
                   
                  
                   if (item.VariableCode != null)
                   {
                      
                       if (variables.ContainsKey(item.VariableCode))
                       {
                            var variableId = variables[item.VariableCode];
                            //update model
                            model.VariableID = variableId;
                       }
                      
                   }
                   else
                   {
                       int variableId;
                       bool res = int.TryParse(item.VariableID, out variableId);
                       if (res)
                       {
                           //update model
                           model.VariableID = variableId;
                       }
                       else
                       {
                           listOfIncorrectRecords.Add(item);
                           continue;

                       }
                   }
                   //Validate foreign keys
                   //var methodId = context.Methods
                   //                       .Where(a => a.MethodID == model.MethodID)
                   //                       .Select(a => a);
                    

                   //lookup duplicates
                   //check if item with this item exists in the database
                   //object existingItem = null;
                   var existingItem = context.DataValues.Where(a => a.DateTimeUTC == model.DateTimeUTC &&
                                                                    a.DataValue1 == model.DataValue1 &&
                                                                    a.SiteID == model.SiteID &&
                                                                    a.VariableID == model.VariableID
                   //                                                 //&&
                   //                                                 //a.ValueAccuracy == model.ValueAccuracy &&
                   //                                                 //a.LocalDateTime == model.LocalDateTime &&
                   //                                                 //a.UTCOffset == model.UTCOffset &&
                                                                    
                                                                   
                                                                   
                   //                                                 //a.OffsetValue == model.OffsetValue &&
                   //                                                 //a.CensorCode == model.CensorCode &&
                   //                                                 //a.QualifierID == model.QualifierID &&
                   //                                                 //a.MethodID == model.MethodID &&
                   //                                                 //a.SourceID == model.SourceID &&
                   //                                                 //a.SampleID == model.SampleID &&
                   //                                                 //a.DerivedFromID == model.DerivedFromID &&
                   //                                                 //a.QualityControlLevelID == model.QualityControlLevelID
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
                           //context.SaveChanges();
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
           myHelper.BulkInsert(context.Database.Connection.ConnectionString, "Datavalues", recordsToInsert);

           return;
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

       public void AddGroupDescriptions(List<GroupDescriptionModel> itemList, string entityConnectionString, out List<GroupDescriptionModel> listOfIncorrectRecords, out List<GroupDescriptionModel> listOfCorrectRecords, out List<GroupDescriptionModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<GroupDescriptionModel>();
           listOfCorrectRecords = new List<GroupDescriptionModel>();
           listOfDuplicateRecords = new List<GroupDescriptionModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<GroupDescriptionModel, GroupDescription>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<GroupDescription>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[GroupDescriptions] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
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

       public void AddGroups(List<GroupsModel> itemList, string entityConnectionString, out List<GroupsModel> listOfIncorrectRecords, out List<GroupsModel> listOfCorrectRecords, out List<GroupsModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<GroupsModel>();
           listOfCorrectRecords = new List<GroupsModel>();
           listOfDuplicateRecords = new List<GroupsModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<GroupsModel, Group>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<Group>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[Groups] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
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

       public void AddDerivedFrom(List<DerivedFromModel> itemList, string entityConnectionString, out List<DerivedFromModel> listOfIncorrectRecords, out List<DerivedFromModel> listOfCorrectRecords, out List<DerivedFromModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<DerivedFromModel>();
           listOfCorrectRecords = new List<DerivedFromModel>();
           listOfDuplicateRecords = new List<DerivedFromModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<DerivedFromModel, DerivedFrom>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<Group>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[DerivedFrom] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
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

       public void AddCategories(List<CategoriesModel> itemList, string entityConnectionString, out List<CategoriesModel> listOfIncorrectRecords, out List<CategoriesModel> listOfCorrectRecords, out List<CategoriesModel> listOfDuplicateRecords)
       {
           listOfIncorrectRecords = new List<CategoriesModel>();
           listOfCorrectRecords = new List<CategoriesModel>();
           listOfDuplicateRecords = new List<CategoriesModel>();


           var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
           var objContext = ((IObjectContextAdapter)context).ObjectContext;

           foreach (var item in itemList)
           {

               var model = Mapper.Map<CategoriesModel, Category>(item);

               try
               {

                   var objectSet = objContext.CreateObjectSet<Group>().EntitySet;//.EntitySet;

                   ////check if entry with this key exists
                   object value;

                   var key = Utils.GetEntityKey(objectSet, model);

                   if (!objContext.TryGetObjectByKey(key, out value))
                   {
                       try
                       {
                           // var objContext = ((IObjectContextAdapter)context).ObjectContext;
                           objContext.Connection.Open();
                           objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[Categories] ON");
                           objContext.AddObject(objectSet.Name, model);
                           //context.Sites.Add(d);
                           objContext.SaveChanges();
                           listOfCorrectRecords.Add(item);
                           objContext.Connection.Close();
                       }
                       catch (Exception ex)
                       {
                           throw;
                       }
                   }
                   else
                   {
                       listOfDuplicateRecords.Add(item);
                   }

               }
               catch (Exception ex)
               {
                   listOfIncorrectRecords.Add(item);


               }

           }

           return;
       }

   }

   public static class myExtentions
    {
        public static bool EntityExists<T>(this ObjectContext context, T entity)
        where T : EntityObject
        {
            object value;
            var entityKeyValues = new List<KeyValuePair<string, object>>();
            var objectSet = context.CreateObjectSet<T>().EntitySet;
            foreach (var member in objectSet.ElementType.KeyMembers)
            {
                var info = entity.GetType().GetProperty(member.Name);
                var tempValue = info.GetValue(entity, null);
                var pair = new KeyValuePair<string, object>(member.Name, tempValue);
                entityKeyValues.Add(pair);
            }
            var key = new EntityKey(objectSet.EntityContainer.Name + "." + objectSet.Name, entityKeyValues);
            if (context.TryGetObjectByKey(key, out value))
            {
                return value != null;
            }
            return false;
        }
    }

   
}