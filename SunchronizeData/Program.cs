
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;

namespace SynchronizeData
{
    class Program
    {
        enum ExitCode : int
        {
            Success = 0,
            InvalidLogin = 1,
            InvalidFilename = 2,
            UnknownError = 10
        }

        static void Main(string[] args)
        {
            try
            {


                //get connection string to user DB
                var userdbConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

                //get List of databases and corrsponding users to synchronize
                List<UpdateDatabasesModel> list = getListofDatabasesToUpdate(userdbConnectionString);

                //get connection details for database to update
                if (list.Count != 0)
                { 
                var connectionParameters = getConnectionParameters(userdbConnectionString, list[0].DatabaseName);

                //build connection string from parameters
                var connection = BuildConnnectionString(connectionParameters);
                    //get name of databse form list
                    foreach (var service in list)
                    {
                        try
                        {
                            SendSupportInfoEmail("DeduplicationStarted", service.UserName, service.DatabaseName, string.Empty);

                            DeleteDuplicatesDatavalues(connection);

                            SendSupportInfoEmail("DeduplicationCompletedSuccess", service.UserName, service.DatabaseName, string.Empty);
                        }
                        catch (Exception e)
                        {
                            SendSupportInfoEmail("DeduplicationCompletedFailure", service.UserName, service.DatabaseName, e.Message);
                            throw;
                        }
                        //Execute deduplication
                        try
                        {
                            SendSupportInfoEmail("SeriescatalogUpdateStarted", service.UserName, service.DatabaseName, string.Empty);
                            recreateSeriescatalog(connection);
                            //update trackUpdates table with success
                            setTrackUpdates(userdbConnectionString, service.DatabaseName, service.ConnectionId);
                            SendSupportInfoEmail("SeriescatalogUpdateCompletedSuccess", service.UserName, service.DatabaseName, string.Empty);
                        }
                        catch (Exception e)
                        {
                            SendSupportInfoEmail("SeriescatalogCompletedFailure", service.UserName, service.DatabaseName, e.Message);
                            throw;
                        }
                        //Execute recrerate seriescataolog
                    }
                }
            }
            catch (Exception ex)
            {
                SendSupportInfoEmail("unknownException", null, null, ex.Message);
            }
            
            //for registered services do harvest
            //Harvest().GetAwaiter().GetResult();
        }
        private static void recreateSeriescatalog(string connectionstring)
        {
           
           // var seriesCatalogRepository = new SeriesCatalogRepository();
            //seriesCatalogRepository.deleteAll(connectionString);
            using (var conn = new SqlConnection(connectionstring))
            using (var command = new SqlCommand("dbo.spUpdateSeriesCatalog", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60000
            })

            {

                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }

        }

        private static void DeleteDuplicatesDatavalues(string connection)
        {
            try
            {
               
                using (var con = new SqlConnection(connection))
                using (var command = new SqlCommand("spDeleteDuplicatesDatavalues", con)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    con.Open();
                    command.ExecuteNonQuery();
                }

                // For compatibility with IE's "done" event we need to return a result as well as setting the context.response
                return;
                //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //HttpContext.Response.ContentType = "text/plain";
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //return Json(new { success = false });
                return;

            }

        }

        static async Task Harvest()
        { 
         var client = new HttpClient();
        //public static void Run()
        
            try
            {
                client.BaseAddress = new Uri("https://ci.cuahsi.org:8888");
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Host = "ci.cuahsi.org";
                client.DefaultRequestHeaders.Add("Authorization", "Basic amVua2luczphYmNAMTIzIQ==");
                var response = await client.GetAsync("/crumbIssuer/api/xml?xpath=concat(//crumbRequestField,\":\",//crumb).Result");


                //var response =  client.GetAsync("/crumbIssuer/api/xml?xpath=concat(//crumbRequestField,\":\",//crumb)").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    var tmp = responseString.ToString().Split(':');
                    client.DefaultRequestHeaders.Add(tmp[0], tmp[1]);
                    var content = new StringContent("json={\"parameter\": [{\"name\":\"ID\", \"value\":\"247\"},{\"name\":\"PRODCORE\", \"value\":\"wof-prod-synonym2\"}]}", Encoding.UTF8, "application/x-www-form-urlencoded");
                    var retvar = client.PostAsync("/job/QA%20-%20addORupdate//build?delay=0sec&token=abc123", content).Result;
                    if (retvar.IsSuccessStatusCode)
                    {
                        var responseContent2 = retvar.Content;

                        // by calling .Result you are synchronously reading the result
                        string responseString2 = responseContent.ReadAsStringAsync().Result;

                        Console.WriteLine(responseString);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        private static List<UpdateDatabasesModel> getListofDatabasesToUpdate(string connectionstring)
        {
            var list = new List<UpdateDatabasesModel>();

            
        using (var con = new SqlConnection(connectionstring))   
        using (var command = new SqlCommand("GetDatabasesNotSynchronized", con)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                con.Open();
                using (var reader = command.ExecuteReader())
                {
                
                    while (reader.Read())
                        list.Add(new UpdateDatabasesModel { DatabaseName = reader.GetString(0), UserName = reader.GetString(1), ConnectionId = reader.GetInt32(2) });
                        list = list.ToList();
                }
                con.Close();
            }
        
            return list;
        }

        private static void setTrackUpdates(string connection, string serviceName, int connectionId)
        {
            var sql = "update TrackUpdates set isSynchronized = 1 where connectionid = " +  connectionId + "; update TrackUpdates set SynchronizedDateTime = GETDATE() where connectionid = " +  connectionId;

            using (var con = new SqlConnection(connection))
            using (var command = new SqlCommand(sql, con)
            {
                CommandType = CommandType.Text,


            })
            {

                con.Open();                

                command.ExecuteNonQuery();
                
                con.Close();
            }
        }

        private static ConnectionParameters getConnectionParameters(string connectionstring, string DatabaseName)
        {
            var connectionParameters = new ConnectionParameters();

            var sql = "SELECT  [Name], [DataSource], [InitialCatalog], [UserId], [Password] FROM[dbo].[ConnectionParameters] where Name ='" + DatabaseName +"'";

            using (var con = new SqlConnection(connectionstring))
                using (var command = new SqlCommand(sql, con)
                {                
                    CommandType = CommandType.Text,
                    
                
                })
                {
                
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                            connectionParameters =   new ConnectionParameters { Name = reader.GetString(0), DataSource = reader.GetString(1), InitialCatalog = reader.GetString(2), UserId = reader.GetString(3), Password = reader.GetString(4)};
                    
                    }
                    con.Close();
                }

            return connectionParameters;
        }

        private static void SendSupportInfoEmail(string action, string userName, string serviceName, string message)
        {
            
            var userEmail = ConfigurationManager.AppSettings["SupportEmailRecipients"];
            var userFromEmail = ConfigurationManager.AppSettings["SupportFromEmail"].ToString();
            var now = DateTime.Now.ToString("s"); 
            using (MailMessage mm = new MailMessage(userFromEmail, userEmail))
            {
                
                
                if (action == "DeduplicationStarted")
                {

                    mm.Subject = "Deduplication has been requested:";
                    string body = "For user " + userName + " and service: " + serviceName;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }

                if (action == "DeduplicationCompletedSuccess")
                {

                    mm.Subject = "Deduplication has been completed:";
                    string body = "For user " + userName + " and service: " + serviceName;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }
                if (action == "DeduplicationCompletedFailure")
                {

                    mm.Subject = "Deduplication has failed:";
                    string body = "For user " + userName + " and service: " + serviceName;
                    body += "<br /> Exception:" + message;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }
                if (action == "SeriescatalogUpdateStarted")
                {

                    mm.Subject = "SeriescatalogUpdate has been requested:";
                    string body = "For user " + userName + " and service: " + serviceName;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }

                if (action == "SeriescatalogUpdateCompletedSuccess")
                {

                    mm.Subject = "SeriescatalogUpdate has been completed:";
                    string body = "For user " + userName + " and service: " + serviceName;
                    body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                }
                if (action == "SeriescatalogUpdateCompletedFailure")
                {

                    mm.Subject = "SeriescatalogUpdate has failed:";
                    string body = "For user " + userName + " and service: " + serviceName;
                    body += "<br /> Exception:" + message;
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

        public static string BuildConnnectionString(ConnectionParameters model)
        {
            // Specify the provider name, server and database.
            //string providerName = "System.Data.SqlClient";
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

            return providerString;
        }


    }
}
