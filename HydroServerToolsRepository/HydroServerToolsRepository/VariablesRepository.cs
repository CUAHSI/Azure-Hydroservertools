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
    //public class VariablesRepository : IVariablesRepository
    //{

    //    public List<VariablesModel> GetAll(string connectionString)
    //    {
    //        // Create an EntityConnection.
    //        //EntityConnection conn = new EntityConnection(connectionString);


    //        var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(connectionString);
           
    //        var items = from obj in context.Variables
    //                    select obj;
    //        var modelList = new List<VariablesModel>();
    //        foreach (var item in items)
    //        {

    //            var model = Mapper.Map<Variable, VariablesModel>(item);


    //            //var model = new VariablesModel();               
    //            //if (item.VariableID != null) model.VariableID = item.VariableID.ToString(); else model.VariableID = string.Empty;
    //            //if (item.VariableCode != null) model.VariableCode = item.VariableCode.ToString(); else model.VariableCode = string.Empty;
    //            //if (item.VariableName != null) model.VariableName = item.VariableName.ToString(); else model.VariableName = string.Empty;
    //            //if (item.Speciation != null) model.Speciation = item.Speciation.ToString(); else model.Speciation = string.Empty;
    //            //if (item.VariableUnitsID != null) model.VariableUnitsID = item.VariableUnitsID.ToString(); else model.VariableUnitsID = string.Empty;
    //            //if (item.SampleMedium != null) model.SampleMedium = item.SampleMedium.ToString(); else model.SampleMedium = string.Empty;
    //            //if (item.ValueType != null) model.ValueType = item.ValueType.ToString(); else model.ValueType = string.Empty;
    //            //if (item.IsRegular != null) model.IsRegular = item.IsRegular.ToString(); else model.IsRegular = string.Empty;
    //            //if (item.TimeSupport != null) model.TimeSupport = item.TimeSupport.ToString(); else model.TimeSupport = string.Empty;
    //            //if (item.TimeUnitsID != null) model.TimeUnitsID = item.TimeUnitsID.ToString(); else model.TimeUnitsID = string.Empty;
    //            //if (item.DataType != null) model.DataType = item.DataType.ToString(); else model.DataType = string.Empty;
    //            //if (item.GeneralCategory != null) model.GeneralCategory = item.GeneralCategory.ToString(); else model.GeneralCategory = string.Empty;
    //            //if (item.NoDataValue != null) model.NoDataValue = item.NoDataValue.ToString(); else model.NoDataValue = string.Empty;
               
    //            modelList.Add(model);

    //        }
    //        return modelList;
    //    }

    //    public void AddVariables(List<VariablesModel> itemList, string entityConnectionString, out List<VariablesModel> listOfIncorrectRecords, out List<VariablesModel> listOfCorrectRecords, out List<VariablesModel> listOfDuplicateRecords)
    //    {
    //        listOfIncorrectRecords = new List<VariablesModel>();
    //        listOfCorrectRecords = new List<VariablesModel>();
    //        listOfDuplicateRecords = new List<VariablesModel>();


    //        var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
    //        var objContext = ((IObjectContextAdapter)context).ObjectContext;
                   
    //        foreach (var item in itemList)
    //        {
    //           //var model = new ODM_1_1_1EFModel.Variable();
    //           //int intResult; 
    //           //bool boolResult;
    //           //float floatResult;

    //            var model = Mapper.Map<VariablesModel, Variable>(item);

    //            try
    //            {
    //                //if (int.TryParse(item.VariableID, out intResult)) {model.VariableID = intResult;} else { listOfIncorrectRecords.Add(item); continue;}
    //                //if (!string.IsNullOrEmpty(item.VariableCode) && (item.VariableCode.ToLower() != "null")) { model.VariableCode = item.VariableCode; } else { listOfIncorrectRecords.Add(item); continue;}                   
    //                //if (!string.IsNullOrEmpty(item.VariableName) && (item.VariableName.ToLower() != "null")) { model.VariableName = item.VariableName; } else { listOfIncorrectRecords.Add(item); continue;}                   
    //                //if (!string.IsNullOrEmpty(item.Speciation) && (item.Speciation.ToLower() != "null")) { model.Speciation = item.Speciation; } else { listOfIncorrectRecords.Add(item); continue;}                   
    //                //if (int.TryParse(item.VariableUnitsID, out intResult)) {model.VariableUnitsID = intResult;} else { listOfIncorrectRecords.Add(item); continue;}
    //                //if (!string.IsNullOrEmpty(item.SampleMedium) && (item.SampleMedium.ToLower() != "null")) { model.SampleMedium = item.Speciation; } else { listOfIncorrectRecords.Add(item); continue;}                   
    //                //if (!string.IsNullOrEmpty(item.ValueType) && (item.ValueType.ToLower() != "null")) { model.ValueType = item.ValueType; } else { listOfIncorrectRecords.Add(item); continue;}                   
    //                //if (bool.TryParse(item.IsRegular, out boolResult)) {model.IsRegular = boolResult;} else { listOfIncorrectRecords.Add(item); continue;}
    //                //if (float.TryParse(item.TimeSupport, out floatResult)) {model.TimeSupport = floatResult;} else { listOfIncorrectRecords.Add(item); continue;}
    //                //if (int.TryParse(item.TimeUnitsID, out intResult)) {model.TimeUnitsID = intResult;} else { listOfIncorrectRecords.Add(item); continue;}
    //                //if (!string.IsNullOrEmpty(item.DataType) && (item.DataType.ToLower() != "null")) { model.DataType = item.DataType; } else { listOfIncorrectRecords.Add(item); continue;}                   
    //                //if (!string.IsNullOrEmpty(item.GeneralCategory) && (item.GeneralCategory.ToLower() != "null")) { model.GeneralCategory = item.GeneralCategory; } else { listOfIncorrectRecords.Add(item); continue;}                   
    //                //if (float.TryParse(item.NoDataValue, out floatResult)) {model.NoDataValue = floatResult;} else { listOfIncorrectRecords.Add(item); continue;}

                 
    //               var objectSet = objContext.CreateObjectSet<Variable>().EntitySet;//.EntitySet;
                   

    //                ////check if entry with this key exists
    //                object value;
    //                //var entityKeyValues = new List<KeyValuePair<string, object>>();
    //                //foreach (var member in objectSet.ElementType.KeyMembers)
    //                //{
    //                //    var info = d.GetType().GetProperty(member.Name);
    //                //    var tempValue = info.GetValue(d, null);
    //                //    var pair = new KeyValuePair<string, object>(member.Name, tempValue);
    //                //    entityKeyValues.Add(pair);
    //                //}
    //                //var key = new EntityKey(objectSet.EntityContainer.Name + "." + objectSet.Name, entityKeyValues);

    //               var key = Utils.GetEntityKey(objectSet, model);

    //                if (!objContext.TryGetObjectByKey(key, out value))
    //                {
    //                    try
    //                    {
    //                        // var objContext = ((IObjectContextAdapter)context).ObjectContext;
    //                        objContext.Connection.Open();
    //                        objContext.ExecuteStoreCommand("SET IDENTITY_INSERT [dbo].[Variables] ON");
    //                        objContext.AddObject(objectSet.Name, model);
    //                        //context.Sites.Add(d);
    //                        objContext.SaveChanges();
    //                        listOfCorrectRecords.Add(item);
    //                        objContext.Connection.Close();
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        throw;
    //                    }
    //                }
    //                else
    //                {
    //                    listOfDuplicateRecords.Add(item);
    //                }

                      
                   
    //            }
    //            catch (Exception ex)
    //            {
    //                listOfIncorrectRecords.Add(item);
                    

    //            }

    //            //context.Sites.Add(d);
    //            //context.EntityExists<Site>(d);

    //        }

           
           

    //        return; 
    //    }
     
    //}
    //public static class myExtentions
    //{
    //    public static bool EntityExists<T>(this ObjectContext context, T entity)
    //    where T : EntityObject
    //    {
    //        object value;
    //        var entityKeyValues = new List<KeyValuePair<string, object>>();
    //        var objectSet = context.CreateObjectSet<T>().EntitySet;
    //        foreach (var member in objectSet.ElementType.KeyMembers)
    //        {
    //            var info = entity.GetType().GetProperty(member.Name);
    //            var tempValue = info.GetValue(entity, null);
    //            var pair = new KeyValuePair<string, object>(member.Name, tempValue);
    //            entityKeyValues.Add(pair);
    //        }
    //        var key = new EntityKey(objectSet.EntityContainer.Name + "." + objectSet.Name, entityKeyValues);
    //        if (context.TryGetObjectByKey(key, out value))
    //        {
    //            return value != null;
    //        }
    //        return false;
    //    }
    //}
}