using HydroServerTools.Models;
using HydroserverToolsBusinessObjects;
using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using Microsoft.ApplicationServer.Caching;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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
                var userEmail = Db.Users.FirstOrDefault(u => u.UserName == userName).Id;


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

        public static string GetServiceNameForUserID(string userId)
        {
            string serviceName = null;

            ApplicationDbContext context = new ApplicationDbContext();

            var p = ((from users in context.Users
                     join connectionParametersUsers in context.ConnectionParametersUser on users.Id equals connectionParametersUsers.UserId
                     join connectionParameters in context.ConnectionParameters on connectionParametersUsers.ConnectionParametersId equals connectionParameters.Id
                     where (users.Id == userId)
                     select new
                     { connectionParameters.Name }).FirstOrDefault());

            if (p != null)
            {
                serviceName = p.Name;
            }


            return serviceName;
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

        public static void SendSupportInfoEmail(string action, string userName, string serviceName, string message)
        {

            var userEmail = ConfigurationManager.AppSettings["HelpEmailRecipients"];
            var userFromEmail = ConfigurationManager.AppSettings["SupportFromEmail"].ToString();
            var now = DateTime.Now.ToString("s");
            using (MailMessage mm = new MailMessage(userFromEmail, userEmail))
            {


                if (action == "PublicationRequestedSupport")
                {

                    mm.Subject = "Publication has been requested:";
                    string body = "For user " + userName + " and service: " + serviceName;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }

                
                if (action == "unknownException")
                {

                    mm.Subject = "Unknown Exception has occured:";
                    //string body = "For user " + userName + " and service: " + serviceName;
                    string body = "<br /> Exception:" + message;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }


                try
                {
                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(mm);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //Exception - for now take no action...
                    var errMessage = ex.Message;
                }
            }
        }

        public static void SendInfoEmail(string action, string userName, string serviceName, string message)
        {

            var userEmail = userName;
            var userFromEmail = ConfigurationManager.AppSettings["SupportFromEmail"];
            var helpEmail = ConfigurationManager.AppSettings["HelpEmailRecipients"];

            var now = DateTime.Now.ToString("s");
            using (MailMessage mm = new MailMessage())
            {

                if (action == "PublicationRequestedSupport")
                {
                    mm.From = new MailAddress(userFromEmail);
                    mm.To.Add(helpEmail);
                   
                    mm.Subject = "Publication has been requested:";
                    string body = "For user " + userName + " and service: " + serviceName;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    body += "<br /><br />The CUAHSI Water Data Services Team";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }
                if (action == "PublicationRequestedUser")
                {
                    mm.From = new MailAddress(userFromEmail);
                    mm.To.Add(userEmail);
                    mm.Subject = "Publication has been requested:";
                    string body = "Thank you for requesting publication for your service: " + serviceName ;
                    body += "<br />We are reviewing the request. You will receive an email from help@cuahsi.org on the next steps on how to procced with the final steps before your data becomes available on data.cuahsi.org<br /> ";
                    body += "<br />If you need immediate assistance please contact help@cuahsi.org. <br /> ";
                    body += "<br /><br />Thank you";
                    body += "<br /><br />The CUAHSI Water Data Services Team";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }

                if (action == "unknownException")
                {
                    mm.From = new MailAddress(userFromEmail);
                    mm.To.Add(helpEmail);
                    mm.Subject = "Unknown Exception has occured:";
                    //string body = "For user " + userName + " and service: " + serviceName;
                    string body = "<br /> Exception:" + message;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    body += "<br /><br />The CUAHSI Water Data Services Team";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }


                try
                {
                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(mm);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //Exception - for now take no action...
                    var errMessage = ex.Message;
                }
            }
        }


        public static DatabaseTableValueCountModel getDatabaseTableValueCount(string userName)
        {

            var tableValueCounts = new DatabaseTableValueCountModel();

            string entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(userName);


            if (!String.IsNullOrEmpty(entityConnectionString))
            {
                //var entityConnectionString = HydroServerToolsUtils.GetDBEntityConnectionStringByName(connectionName);

                var databaseRepository = new DatabaseRepository();

                tableValueCounts = databaseRepository.GetDatabaseTableValueCount(entityConnectionString);
            }
                return tableValueCounts;
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
            db.TrackUpdates.Add(trackUpdates);
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

        public static void runSynchronizeJob()
        {
            try
            {
                //App Service Publish Profile Credentials 
                string userName = ConfigurationManager.AppSettings["azurejob-userName"]; //azurejob-userName 
                string userPassword = ConfigurationManager.AppSettings["azurejob-userPassword"]; //userPWD 

                //change webJobName to your WebJob name 
                // string webJobName = "WEBJOBNAME";

                var unEncodedString = String.Format($"{userName}:{userPassword}");
                var encodedString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(unEncodedString));
                //var arg = "arguments= argtest1";
                //Change this URL to your WebApp hosting the  
                //string URL = "https://?.scm.azurewebsites.net/api/triggeredwebjobs/" + webJobName + "/run";
                string URL = ConfigurationManager.AppSettings["azurejob-webhook"]; 
                System.Net.WebRequest request = System.Net.WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentLength = 0;
                request.Headers["Authorization"] = "Basic " + encodedString;
                System.Net.WebResponse response = request.GetResponse();
                System.IO.Stream dataStream = response.GetResponseStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
               

            }
            catch (Exception ex)
            {
                //TODO implement notification
                throw;
                
            }
        }

        public static string GetNetworkIdForUserName(string userName)
        {
            string networkId = null;
            ApplicationDbContext context = new ApplicationDbContext();

            var p = ((from users in context.Users
                      join connectionParametersUsers in context.ConnectionParametersUser on users.Id equals connectionParametersUsers.UserId
                      join connectionParameters in context.ConnectionParameters on connectionParametersUsers.ConnectionParametersId equals connectionParameters.Id
                      where (users.UserName.ToLower() == userName.ToLower())
                      select new
                      { connectionParameters.HIScentralNetworkId }).FirstOrDefault());
            if (p != null)
            {
                networkId = p.HIScentralNetworkId;
            }

            return networkId;
        }

        //HISNETWORKS

        public static HISNetwork getHISNetworksDataForServiceName(int networkId)
        {
            var context = new HiscentralDbContext();
            var hisnetwork = new HISNetwork();

            var p = (from c in context.HISNetwork
                     where ( c.NetworkID == networkId)
                     select new
                     {
                         c.NetworkID,
                         c.NetworkName,
                         c.IsPublic,
                         c.LastHarvested,
                         c.CreatedDate
                         
                     }).FirstOrDefault();
            if (p != null)
            {
                hisnetwork.NetworkID = p.NetworkID;
                hisnetwork.NetworkName = p.NetworkName;
                hisnetwork.IsPublic = p.IsPublic;
                hisnetwork.LastHarvested = p.LastHarvested;
                hisnetwork.CreatedDate = p.CreatedDate;
            }

            return hisnetwork;

        }

    }
}