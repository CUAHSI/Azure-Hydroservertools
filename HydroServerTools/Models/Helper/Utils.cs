using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using Microsoft.ApplicationServer.Caching;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace HydroServerTools
{
    public class HydroServerToolsUtils
    {

        const string EFMODEL = @"res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl";

        public static string BuildEFConnnectionString(ConnectionParameters model)
        {
            // Specify the provider name, server and database.
            string providerName = "System.Data.SqlClient";
            string serverName = model.DataSource;
            string databaseName = model.InitialCatalog;
            string userName = model.UserId;
            string password = model.Password;

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder =
            new SqlConnectionStringBuilder();
            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.UserID = userName;
            sqlBuilder.Password = password;
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.PersistSecurityInfo = true;

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
            new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = EFMODEL;


            return entityBuilder.ToString();
        }


        public static string GetConnectionNameByUserName(string userName)
        {
            string result = string.Empty;

            //string path = "http://" + this.Request.RequestUri.Authority + "XML/users.xml";
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/XML/users.xml");
                XElement doc = XElement.Load(path);
                IEnumerable<XElement> users = doc.Elements();

                var e = from u in doc.Elements("user")
                        where (string)u.Element("username") == userName
                        select u;
                result = e.FirstOrDefault().Element("connectionname").Value.ToString();
            }
            catch (DirectoryNotFoundException ex)
            {
                throw ex;// return ex.Message;
            }


            return result;
        }

        public static string GetConnectionNameByUserEmail(string userName)
        {
            //string userName = context.User.Identity.Name;//  "martin.seul@yahoo.com";
            var Db = new ApplicationDbContext();
            string result = string.Empty;

            //string path = "http://" + this.Request.RequestUri.Authority + "XML/users.xml";
            try
            {
                var userEmail = Db.Users.FirstOrDefault(u => u.UserName == userName).UserEmail;


                string path = HttpContext.Current.Server.MapPath("~/XML/users.xml");
                XElement doc = XElement.Load(path);
                IEnumerable<XElement> users = doc.Elements();

                var e = from u in doc.Elements("user")
                        where (String)u.Element("useremail") == userEmail
                        select u;
                result = e.FirstOrDefault().Element("connectionname").Value.ToString().ToLower();
            }
            catch (DirectoryNotFoundException ex)
            {
                throw ex;// return ex.Message;
            }
            //catch (NullReferenceException ex)
            catch (NullReferenceException)
            {
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static string GetDBEntityConnectionStringByName(string name)
        {
            string connectionString = string.Empty;
            System.Configuration.Configuration rootWebConfig =
                    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            System.Configuration.ConnectionStringSettings connString;
            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
            {
                connString =
                    rootWebConfig.ConnectionStrings.ConnectionStrings[name];
                if (connString != null)
                {
                    connectionString = connString.ConnectionString;
                }
            }
            return connectionString;

        }

        public static string GetProviderConnectionStringByName(string name)
        {
            string connectionString = string.Empty;
            System.Configuration.Configuration rootWebConfig =
                    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            System.Configuration.ConnectionStringSettings connString;
            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
            {
                connString =
                    rootWebConfig.ConnectionStrings.ConnectionStrings[name];
                if (connString != null)
                {
                    connectionString = connString.ConnectionString;
                }
            }
            string providerConnectionString = new EntityConnectionStringBuilder(connectionString).ProviderConnectionString;
            return providerConnectionString;

        }

        public static string GetUserEmailForCurrentUser(string userName)
        {
            string userEmail = string.Empty;
            var Db = new ApplicationDbContext();
            userEmail = Db.Users.First(u => u.UserName == userName).UserEmail;
            return userEmail;
        }

        public static string GetConnectionName(string userName)
        {
            string connectionName = Resources.NOT_LINKED_TO_DATABASE;
            ApplicationDbContext context = new ApplicationDbContext();

            var p = (from c in context.ConnectionParametersUser
                     where c.User.UserName == userName
                     select new
                     {
                         c.ConnectionParameters.Name
                     }).FirstOrDefault();
            if (p != null)
            {
                connectionName = p.Name;
            }



            return connectionName;
        }

        public static int GetConnectionId(string userName)
        {
            int connectionId = 0;
            ApplicationDbContext context = new ApplicationDbContext();

            var p = (from c in context.ConnectionParametersUser
                     where c.User.UserName == userName
                     select new
                     {
                         c.ConnectionParameters.Id
                     }).FirstOrDefault();
            if (p != null)
            {
                connectionId = p.Id;
            }



            return connectionId;
        }

        public static string BuildConnectionStringForUserName(string userName)
        {
            string connectionString = string.Empty;

            ApplicationDbContext context = new ApplicationDbContext();
            context.Database.CommandTimeout = 10000;
            // var entityConnectionstringParameters = context.ConnectionParameters.Where(r => r.Name.Equals(userName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            //var entityConnectionstringParameters = context.ConnectionParametersUser.Where(r => r.Name.Equals(userName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var p = (from c in context.ConnectionParametersUser
                     where c.User.UserName == userName
                     select new
                     {
                         c.ConnectionParameters.Name,
                         c.ConnectionParameters.DataSource,
                         c.ConnectionParameters.InitialCatalog,
                         c.ConnectionParameters.UserId,
                         c.ConnectionParameters.Password
                     }).FirstOrDefault();


            var entityConnectionstringParameters = new ConnectionParameters();

            if (p != null)
            {
                entityConnectionstringParameters.DataSource = p.DataSource;
                entityConnectionstringParameters.InitialCatalog = p.InitialCatalog;
                entityConnectionstringParameters.UserId = p.UserId;
                entityConnectionstringParameters.Password = p.Password;
                if (entityConnectionstringParameters != null)
                {
                    connectionString = BuildEFConnnectionString(entityConnectionstringParameters);
                }
            }
            return connectionString;
        }

        public static string GetUserIdFromUserName(string userName)
        {
            string userId = null;
            ApplicationDbContext context = new ApplicationDbContext();

            var p = (from c in context.Users
                     where c.UserName == userName
                     select new
                     {
                         c.Id
                     }).FirstOrDefault();
            if (p != null)
            {
                userId = p.Id;
            }



            return userId;
        }

        public static bool GetSyncStatusFromUserId(string userId)
        {
            bool syncStatus = true;//assume is synchronized
            ApplicationDbContext context = new ApplicationDbContext();

            var p = (from c in context.TrackUpdates
                     where c.UserId == userId
                     select new
                     {
                         c.IsSynchronized
                     }).FirstOrDefault();
            if (p != null)
            {
                syncStatus = p.IsSynchronized;
            }



            return syncStatus;
        }
        /// <summary>
        //insert data to track updates to the Datavalues tabel indicating a synchronization is required 
        /// </summary>
        /// <param name="userName"></param>

        public static void InsertTrackUpdates(string userName)
        {
            var db = new ApplicationDbContext();
            var trackUpdates = new TrackUpdates();
            trackUpdates.ConnectionId = HydroServerToolsUtils.GetConnectionId(userName);
            trackUpdates.UserId = HydroServerToolsUtils.GetUserIdFromUserName(userName);
            trackUpdates.IsUpdated = true;
            trackUpdates.UpdateDateTime = DateTime.Now;
            trackUpdates.IsSynchronized = false;
            trackUpdates.SynchronizedDateTime = DateTime.MinValue;
            db.SaveChanges();
        }
        public static List<string> ValidateFields<T>(List<string> columnHeaders)
        {
            //Sites
            //get mandatory fields
            var missingMandatoryFields = new List<string>();
            var model = typeof(T);
            //check attribute
            var properties = model.GetProperties();
            foreach (var p in properties)
            {
                Attribute a = Attribute.GetCustomAttribute(p, typeof(RequiredAttribute));
                if (a != null)
                {
                    if (!columnHeaders.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                        missingMandatoryFields.Add(p.Name.ToLower());
                }
            }
            //compare 

            return missingMandatoryFields;
        }

        public static bool IsLocalHostServer()
        {
            string host = HttpContext.Current.Request.Url.Host.ToLower();
            return (host == "localhost");
        }

        //public static List<T> GetRecordsFromSession<T>(int id)
        //{
        //    var listOfRecords = new List<T>();

            
        //        var httpContext = new HttpContextWrapper(System.Web.HttpContext.Current);

        //        switch (id)
        //        {
        //            case 0:
        //                if (System.Web.HttpContext.Current.Session["listOfCorrectRecords"] != null) listOfRecords = (List<T>)System.Web.HttpContext.Current.Session["listOfCorrectRecords"];
        //                break;
        //            case 1:
        //                if (System.Web.HttpContext.Current.Session["listOfIncorrectRecords"] != null) listOfRecords = (List<T>)System.Web.HttpContext.Current.Session["listOfIncorrectRecords"];
        //                break;
        //            case 2:
        //                if (System.Web.HttpContext.Current.Session["listOfEditedRecords"] != null) listOfRecords = (List<T>)System.Web.HttpContext.Current.Session["listOfEditedRecords"];
        //                break;
        //            case 3:
        //                if (System.Web.HttpContext.Current.Session["listOfDuplicateRecords"] != null) listOfRecords = (List<T>)System.Web.HttpContext.Current.Session["listOfDuplicateRecords"];
        //                break;
        //        }
            
           
        //    return listOfRecords;
        //}

        //public static void UpdateCachedprocessStatusMessage(string dataCacheName, string message)
        //{
        //    DataCache cache = new DataCache(dataCacheName);
        //    //needed to uniquely identify 
        //    var identifier = MvcApplication.InstanceGuid;

        //    if (cache.Get(identifier + "processStatus") == null) cache.Add(identifier + "processStatus", message); else cache.Put(identifier + "processStatus", message);

        //}

        //public static void RemoveItemFromCache(string dataCacheName, string itemName)
        //{
        //    DataCache cache = new DataCache(dataCacheName);
        //    //needed to uniquely identify 
        //    var identifier = MvcApplication.InstanceGuid;

        //    cache.Remove(identifier + itemName);
        //}
        public static String stripNonValidXMLCharacters(string strIn)
        {
            // Replace invalid characters with empty strings. 
            string re = "\v";

            try
            {
                return Regex.Replace(strIn, re, " ", RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,  
            // we should return Empty. 
            catch (RegexMatchTimeoutException)
            {
                return strIn;
            }
        }
       

    }
}