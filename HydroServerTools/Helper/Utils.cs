using HydroServerTools.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            var userEmail = Db.Users.First(u => u.UserName == userName).UserEmail; 
            string result = string.Empty;

            //string path = "http://" + this.Request.RequestUri.Authority + "XML/users.xml";
            try
            {
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

       public static bool ValidateMandatoryFields(string tableName, List<string> columnHeaders)
        {
           bool isValid = false;

           return isValid;
        }


       

    }
}