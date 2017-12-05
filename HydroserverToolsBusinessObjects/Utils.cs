using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics;
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

        public static List<T> GetRecordsFromCache<T>(string identifier, int id)
        {
            //DataCache cache = new DataCache(dataCacheName);



            var listOfRecords = new List<T>();

            switch (id)
            {
                case 0:
                    if (HttpRuntime.Cache.Get(identifier + "listOfCorrectRecords") != null) listOfRecords = (List<T>)HttpRuntime.Cache.Get(identifier + "listOfCorrectRecords");
                    else listOfRecords = null;
                    break;
                case 1:
                    if (HttpRuntime.Cache.Get(identifier + "listOfIncorrectRecords") != null) listOfRecords = (List<T>)HttpRuntime.Cache.Get(identifier + "listOfIncorrectRecords");
                    else listOfRecords = null;
                    break;
                case 2:
                    if (HttpRuntime.Cache.Get(identifier + "listOfEditedRecords") != null) listOfRecords = (List<T>)HttpRuntime.Cache.Get(identifier + "listOfEditedRecords");
                    else listOfRecords = null;
                    break;
                case 3:
                    if (HttpRuntime.Cache.Get(identifier + "listOfDuplicateRecords") != null) listOfRecords = (List<T>)HttpRuntime.Cache.Get(identifier + "listOfDuplicateRecords");
                    else listOfRecords = null;
                    break;
            }


            return listOfRecords;
        }

        public static List<T> GetRecordsFromSession<T>(string identifier, int id)
        {
            
            var session = System.Web.HttpContext.Current.Session;

             //    if (httpContext.Session["listOfEditedRecords"] == null) httpContext.Session["listOfEditedRecords"] = listOfEditedRecords; else httpContext.Session["listOfEditedRecords"] = listOfEditedRecords;
            //    if (httpContext.Session["listOfDuplicateRecords"] == null) httpContext.Session["listOfDuplicateRecords"] = listOfDuplicateRecords; else httpContext.Session["listOfDuplicateRecords"] = listOfDuplicateRecords;
  
            var listOfRecords = new List<T>();

            switch (id)
            {
                case 0:
                    if (session["listOfCorrectRecords"] != null) listOfRecords = (List<T>)session["listOfCorrectRecords"]; else listOfRecords = null;
                    break;
                case 1:
                    if (session["listOfIncorrectRecords"] != null) listOfRecords = (List<T>)session["listOfIncorrectRecords"];
                    else listOfRecords = null;
                    break;
                case 2:
                    if (session["listOfEditedRecords"] != null) listOfRecords = (List<T>)session["listOfEditedRecords"];
                    else listOfRecords = null;
                    break;
                case 3:
                    if (session["listOfDuplicateRecords"] != null) listOfRecords = (List<T>)session["listOfDuplicateRecords"];
                    else listOfRecords = null;
                    break;
            }


            return listOfRecords;
        }


        public static void UpdateCachedprocessStatusMessage(string instanceIdentifier, string dataCacheName, string message)
        {
            //DataCache cache = new DataCache(dataCacheName);
            //needed to uniquely identify 

            HttpRuntime.Cache.Insert(instanceIdentifier + "_processStatus", message);
            //var session = System.Web.HttpContext.Current.Session;

            //if (session["processStatus"] == null)
            //{
            //    session.Add("processStatus", message);
            //}
            //else
            //{
            //    session["processStatus"] = message;
            //}
            // Debug.WriteLine(message);
        }



        public static void RemoveItemFromCache(string identifier, string itemName)
        {
            //DataCache cache = new DataCache(dataCacheName);
            //needed to uniquely identify 


            HttpRuntime.Cache.Remove(identifier + itemName);
        }

        public static void RemoveItemFromSession(string identifier, string itemName)
        {
            var session = System.Web.HttpContext.Current.Session;

            //needed to uniquely identify 


            session.Remove(itemName);
        }

 



        public static UploadStatisticsModel GetUploadStatsFromCache<T>(string identifier)
        {
            //DataCache cache = new DataCache(dataCacheName);
            var uploadStatisticsModel = new UploadStatisticsModel();
            if (HttpRuntime.Cache.Get(identifier + "listOfCorrectRecords") != null) 
            {
               var l  = (List<T>)HttpRuntime.Cache.Get(identifier + "listOfCorrectRecords");
                uploadStatisticsModel.NewRecordCount = l.Count();
            }
            if (HttpRuntime.Cache.Get(identifier + "listOfIncorrectRecords") != null)
            {
                var l = (List<T>)HttpRuntime.Cache.Get(identifier + "listOfIncorrectRecords");
                uploadStatisticsModel.RejectedRecordCount = l.Count();
            }
            if (HttpRuntime.Cache.Get(identifier + "listOfEditedRecords") != null)
            {
                var l = (List<T>)HttpRuntime.Cache.Get(identifier + "listOfEditedRecords");
                uploadStatisticsModel.UpdatedRecordCount = l.Count();
            }
            if (HttpRuntime.Cache.Get(identifier + "listOfDuplicateRecords") != null)
            {
                var l = (List<T>)HttpRuntime.Cache.Get(identifier + "listOfDuplicateRecords");
                uploadStatisticsModel.DuplicateRecordCount = l.Count();
            }            

            return uploadStatisticsModel;
        }

        public static UploadStatisticsModel GetUploadStatsFromSession<T>(string identifier)
        {
            var session = System.Web.HttpContext.Current.Session;
            var uploadStatisticsModel = new UploadStatisticsModel();
            if (session["listOfCorrectRecords"] != null)
            {
                var l = (List<T>)session["listOfCorrectRecords"];
                uploadStatisticsModel.NewRecordCount = l.Count();
            }
            if (session["listOfIncorrectRecords"] != null)
            {
                var l = (List<T>)session["listOfIncorrectRecords"];
                uploadStatisticsModel.RejectedRecordCount = l.Count();
            }
            if (session["listOfEditedRecords"] != null)
            {
                var l = (List<T>)session["listOfEditedRecords"];
                uploadStatisticsModel.UpdatedRecordCount = l.Count();
            }
            if (session["listOfDuplicateRecords"] != null)
            {
                var l = (List<T>)session["listOfDuplicateRecords"];
                uploadStatisticsModel.DuplicateRecordCount = l.Count();
            }

            return uploadStatisticsModel;
        }
    
        //Transfer some character checking methods from HydroServerToolsRepository...

        public static bool containsInvalidCharacters(string value)
        {
            bool hasInvalidCharacters;
            hasInvalidCharacters = ((System.Text.RegularExpressions.Regex.Matches(value, @"[\040]").Count != 0) ||
                                        (System.Text.RegularExpressions.Regex.Matches(value, @"[\,\+]").Count != 0) ||
                                        (System.Text.RegularExpressions.Regex.Matches(value, @"[\:\\/\=]").Count != 0) ||
                                        (System.Text.RegularExpressions.Regex.Matches(value, @"[\t\r\v\f\n]").Count != 0));
            return hasInvalidCharacters;
        }

        public static bool containsSpecialCharacters(string value)
        {
            bool hasSpecialCharacters;
            hasSpecialCharacters = (System.Text.RegularExpressions.Regex.Matches(value, "[\t\r\v\f\n]").Count != 0);
            return hasSpecialCharacters;
        }

        public static bool containsNotOnlyAllowedCharacters(string value)
        {
            bool containsNotAllowedCharacters;
            containsNotAllowedCharacters = (System.Text.RegularExpressions.Regex.Matches(value, @"[^0-9a-zA-Z\.\-_]").Count != 0);
            return containsNotAllowedCharacters;
        }

    }
}