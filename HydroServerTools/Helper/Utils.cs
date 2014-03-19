using HydroServerTools.Models;
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
using System.Web;
using System.Xml.Linq;

namespace HydroServerTools.Helper
{
    public class Utils
    {
        
        const string EFMODEL = @"res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl";

        public static string BuildEFConnnectionString (ConnectionModel model)
        {
            // Specify the provider name, server and database.
                string providerName = "System.Data.SqlClient";
                string serverName = model.ServerName;
                string databaseName = model.DataSourceName;
                string userName = model.Username;
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
                string path = HttpContext.Current.Server.MapPath( "~/XML/users.xml");
                XElement doc = XElement.Load(path); 
                IEnumerable<XElement> users = doc.Elements();

                var e = from u in doc.Elements("user")
                        where (string)u.Element("username") == userName
                        select u;
                result = e.FirstOrDefault().Element("connectionname").Value.ToString();
            }
            catch (DirectoryNotFoundException ex) 
            {
                throw;// return ex.Message;
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
                var userEmail = Db.Users.First(u => u.UserName == userName).UserEmail; 
            
                
                string path = HttpContext.Current.Server.MapPath("~/XML/users.xml");
                XElement doc = XElement.Load(path);
                IEnumerable<XElement> users = doc.Elements();

                var e = from u in doc.Elements("user")
                        where (string)u.Element("useremail") == userEmail
                        select u;
                result = e.FirstOrDefault().Element("connectionname").Value.ToString();
            }
            catch (DirectoryNotFoundException ex)
            {
                throw;// return ex.Message;
            }
            catch (NullReferenceException ex)
            {
                return string.Empty;
            }
            catch (Exception ex)
            { 
                throw;
            }

            return result;
        }

        public static string GetDBConnectionStringByName(string name)
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

        public static string GetUserEmailForCurrentUser(string userName)
        {
            string userEmail = string.Empty;
            var Db = new ApplicationDbContext();
            userEmail = Db.Users.First(u => u.UserName == userName).UserEmail;
            return userEmail;
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

        public static List<T> GetRecordsFromCache<T>(int id, string dataCacheName)
        {
            var listOfRecords = new List<T>();

            
            if (Utils.IsLocalHostServer())
            {
                var httpContext = new HttpContextWrapper(System.Web.HttpContext.Current);

                switch (id)
                {
                    case 0:
                        if (System.Web.HttpContext.Current.Session["listOfCorrectRecords"] != null) listOfRecords = (List<T>)System.Web.HttpContext.Current.Session["listOfCorrectRecords"];
                        break;
                    case 1:
                        if (System.Web.HttpContext.Current.Session["listOfIncorrectRecords"] != null) listOfRecords = (List<T>)System.Web.HttpContext.Current.Session["listOfIncorrectRecords"];
                        break;
                    case 2:
                        if (System.Web.HttpContext.Current.Session["listOfEditedRecords"] != null) listOfRecords = (List<T>)System.Web.HttpContext.Current.Session["listOfEditedRecords"];
                        break;
                    case 3:
                        if (System.Web.HttpContext.Current.Session["listOfDuplicateRecords"] != null) listOfRecords = (List<T>)System.Web.HttpContext.Current.Session["listOfDuplicateRecords"];
                        break;
                }
            }
            else
            {
                DataCache cache = new DataCache(dataCacheName);
                var httpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
                //hack to provide unique id, work around the problem with the session and google ID
                var identifier = HttpContext.Current.User.Identity.Name;
               
              
                switch (id)
                {
                    case 0:
                        if (cache.Get(identifier + "listOfCorrectRecords") != null) listOfRecords = (List<T>)cache.Get(identifier + "listOfCorrectRecords");
                        break;
                    case 1:
                        if (cache.Get(identifier + "listOfIncorrectRecords") != null) listOfRecords = (List<T>)cache.Get(identifier + "listOfIncorrectRecords");
                        break;
                    case 2:
                        if (cache.Get(identifier + "listOfEditedRecords") != null) listOfRecords = (List<T>)cache.Get(identifier + "listOfEditedRecords");
                        break;
                    case 3:
                        if (cache.Get(identifier + "listOfDuplicateRecords") != null) listOfRecords = (List<T>)cache.Get(identifier + "listOfDuplicateRecords");
                        break;
                }
            }






            return listOfRecords;
        }

        public static String stripNonValidXMLCharacters(string textIn)
        {
            StringBuilder textOut = new StringBuilder(); // Used to hold the output.
            char current; // Used to reference the current character.


            if (textIn == null || textIn == string.Empty) return string.Empty; // vacancy test.
            for (int i = 0; i < textIn.Length; i++) {
                current = textIn[i];


                if ((current == 0x9 || current == 0xA || current == 0xD) ||
                    ((current >= 0x20) && (current <= 0xD7FF)) ||
                    ((current >= 0xE000) && (current <= 0xFFFD))
                    )
                {
                    textOut.Append(current);
                }
            }
            return textOut.ToString();
        }
       

    }
}