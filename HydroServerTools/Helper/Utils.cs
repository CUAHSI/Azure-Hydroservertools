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

        public static string GetDatatableForModel<T>(T var)
        {
            
            StringBuilder table = new StringBuilder();
            table.Append("<table id='table_id' cellpadding='0' cellspacing='0' border='0' class='display datatable' style='width:100%'>");
            table.Append(" <thead>");
            table.Append("  <tr>");
            table.Append("<th>");
            table.Append("<th>SiteCode</th>");
            table.Append("<th>SiteName</th>");
            table.Append("<th>Latitude</th>");
            table.Append("<th>Longitude</th>");
            table.Append("<th>LatLongDatumID</th>");
            table.Append("<th>Elevation_m</th>");
            table.Append("<th>VerticalDatum</th>");
            table.Append("<th>LocalX</th>");
            table.Append("<th>LocalY</th>");
            table.Append("<th>LocalProjectionID</th>");
            table.Append("<th>PosAccuracy_m</th>");
            table.Append("<th>State</th>");
            table.Append("<th>County</th>");
            table.Append("<th>Comments</th>");
            table.Append("<th>SiteType</th>");
            table.Append("</th>");
            table.Append("</tr>");
            table.Append("</thead>");
            table.Append("<tbody>");
            table.Append("@if (Model != null");
            table.Append("{");
            table.Append("foreach (var item in Model");
            table.Append("{");  
            table.Append(" <tr>");
            table.Append(" <td>");
            table.Append("<td>@item.SiteCode</td>");
            table.Append("<td>@item.SiteName</td>");
            table.Append("<td>@item.Latitude</td>");
            table.Append("<td>@item.Longitude</td>");
            table.Append("<td>@item.LatLongDatumID</td>");
            table.Append("<td>@item.Elevation_m</td>");
            table.Append("<td>@item.VerticalDatum</td>");
            table.Append("<td>@item.LocalX</td>");
            table.Append("<td>@item.LocalY</td>");
            table.Append("<td>@item.LocalProjectionID</td>");
            table.Append("<td>@item.PosAccuracy_m</td>");
            table.Append("<td>@item.State</td>");
            table.Append("<td>@item.County</td>");
            table.Append("<td>@item.Comments</td>");
            table.Append("<td>@item.SiteType</td>");
            table.Append("</td>");
            table.Append("}");
            table.Append("}");
            table.Append("</tbody>");          
            table.Append("</table>");                  
   
            return table.ToString();
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