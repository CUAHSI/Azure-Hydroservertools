using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace HydroserverToolsBusinessObjects
{
    public class BusinessObjectsUtils
    {
        
        const string EFMODEL = @"res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl";      

        public static List<T> GetRecordsFromCache<T>(Guid instanceGuid, int id, string dataCacheName)
        {
            DataCache cache = new DataCache(dataCacheName);

            var listOfRecords = new List<T>();                  
              
                switch (id)
                {
                    case 0:
                        if (cache.Get(instanceGuid + "listOfCorrectRecords") != null) listOfRecords = (List<T>)cache.Get(instanceGuid + "listOfCorrectRecords");
                        else listOfRecords = null;
                        break;
                    case 1:
                        if (cache.Get(instanceGuid + "listOfIncorrectRecords") != null) listOfRecords = (List<T>)cache.Get(instanceGuid + "listOfIncorrectRecords");
                        else listOfRecords = null;
                        break;
                    case 2:
                        if (cache.Get(instanceGuid + "listOfEditedRecords") != null) listOfRecords = (List<T>)cache.Get(instanceGuid + "listOfEditedRecords");
                        else listOfRecords = null;
                        break;
                    case 3:
                        if (cache.Get(instanceGuid + "listOfDuplicateRecords") != null) listOfRecords = (List<T>)cache.Get(instanceGuid + "listOfDuplicateRecords");
                        else listOfRecords = null;
                        break;
                }
            

            return listOfRecords;
        }

        public static void UpdateCachedprocessStatusMessage(Guid InstanceGuid, string dataCacheName, string message)
        {
            DataCache cache = new DataCache(dataCacheName);
            //needed to uniquely identify 
          
            if (cache.Get(InstanceGuid + "processStatus") == null) cache.Add(InstanceGuid + "processStatus", message); else cache.Put(InstanceGuid + "processStatus", message);
             
        }

        public static void RemoveItemFromCache(Guid InstanceGuid, string dataCacheName, string itemName)
        {
            DataCache cache = new DataCache(dataCacheName);
            //needed to uniquely identify 


            cache.Remove(InstanceGuid + itemName);
        }

        public static UploadStatisticsModel GetUploadStatsFromCache<T>(Guid instanceGuid, string dataCacheName)
        {
            DataCache cache = new DataCache(dataCacheName);
            var uploadStatisticsModel = new UploadStatisticsModel();
            if (cache.Get(instanceGuid + "listOfCorrectRecords") != null) 
            {
               var l  = (List<T>)cache.Get(instanceGuid + "listOfCorrectRecords");
                uploadStatisticsModel.NewRecordCount = l.Count();
            }
            if (cache.Get(instanceGuid + "listOfIncorrectRecords") != null)
            {
                var l = (List<T>)cache.Get(instanceGuid + "listOfIncorrectRecords");
                uploadStatisticsModel.RejectedRecordCount = l.Count();
            }
            if (cache.Get(instanceGuid + "listOfEditedRecords") != null)
            {
                var l = (List<T>)cache.Get(instanceGuid + "listOfEditedRecords");
                uploadStatisticsModel.UpdatedRecordCount = l.Count();
            }
            if (cache.Get(instanceGuid + "listOfDuplicateRecords") != null)
            {
                var l = (List<T>)cache.Get(instanceGuid + "listOfDuplicateRecords");
                uploadStatisticsModel.DuplicateRecordCount = l.Count();
            }            

            return uploadStatisticsModel;
        }
    

    }
}