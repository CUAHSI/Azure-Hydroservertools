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

        public static List<T> GetRecordsFromCache<T>(string identifier, int id, string dataCacheName)
        {
            DataCache cache = new DataCache(dataCacheName);

            

            var listOfRecords = new List<T>();                  
              
                switch (id)
                {
                    case 0:
                        if (cache.Get(identifier + "listOfCorrectRecords") != null) listOfRecords = (List<T>)cache.Get(identifier + "listOfCorrectRecords");
                        else listOfRecords = null;
                        break;
                    case 1:
                        if (cache.Get(identifier + "listOfIncorrectRecords") != null) listOfRecords = (List<T>)cache.Get(identifier + "listOfIncorrectRecords");
                        else listOfRecords = null;
                        break;
                    case 2:
                        if (cache.Get(identifier + "listOfEditedRecords") != null) listOfRecords = (List<T>)cache.Get(identifier + "listOfEditedRecords");
                        else listOfRecords = null;
                        break;
                    case 3:
                        if (cache.Get(identifier + "listOfDuplicateRecords") != null) listOfRecords = (List<T>)cache.Get(identifier + "listOfDuplicateRecords");
                        else listOfRecords = null;
                        break;
                }
            

            return listOfRecords;
        }

        public static void UpdateCachedprocessStatusMessage(string instanceIdentifier, string dataCacheName, string message)
        {
            DataCache cache = new DataCache(dataCacheName);
            //needed to uniquely identify 

            if (cache.Get(instanceIdentifier + "processStatus") == null) cache.Add(instanceIdentifier + "processStatus", message); else cache.Put(instanceIdentifier + "processStatus", message);
             
        }

        public static void RemoveItemFromCache(string identifier, string dataCacheName, string itemName)
        {
            DataCache cache = new DataCache(dataCacheName);
            //needed to uniquely identify 


            cache.Remove(identifier + itemName);
        }

        public static UploadStatisticsModel GetUploadStatsFromCache<T>(string identifier, string dataCacheName)
        {
            DataCache cache = new DataCache(dataCacheName);
            var uploadStatisticsModel = new UploadStatisticsModel();
            if (cache.Get(identifier + "listOfCorrectRecords") != null) 
            {
               var l  = (List<T>)cache.Get(identifier + "listOfCorrectRecords");
                uploadStatisticsModel.NewRecordCount = l.Count();
            }
            if (cache.Get(identifier + "listOfIncorrectRecords") != null)
            {
                var l = (List<T>)cache.Get(identifier + "listOfIncorrectRecords");
                uploadStatisticsModel.RejectedRecordCount = l.Count();
            }
            if (cache.Get(identifier + "listOfEditedRecords") != null)
            {
                var l = (List<T>)cache.Get(identifier + "listOfEditedRecords");
                uploadStatisticsModel.UpdatedRecordCount = l.Count();
            }
            if (cache.Get(identifier + "listOfDuplicateRecords") != null)
            {
                var l = (List<T>)cache.Get(identifier + "listOfDuplicateRecords");
                uploadStatisticsModel.DuplicateRecordCount = l.Count();
            }            

            return uploadStatisticsModel;
        }
    

    }
}