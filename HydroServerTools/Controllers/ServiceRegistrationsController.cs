using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HydroServerTools.Models;
using System.Management.Automation;
using System.Text;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using HydroServerTools.Utilities;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using HydroserverToolsBusinessObjects;
using System.Threading.Tasks;

namespace HydroServerTools.Controllers
{
    enum Emailresponse
    {
        ActivationRequestIssued,
        ActivationRequestConfirmed
    }

    [Authorize]
    public class ServiceRegistrationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceRegistrations
        public ActionResult Index()
        {
            return View(db.ServiceRegistrations.ToList());
        }

        // GET: ServiceRegistrations/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRegistration serviceRegistration = db.ServiceRegistrations.Find(id);
            if (serviceRegistration == null)
            {
                return HttpNotFound();
            }
            return View(serviceRegistration);
        }

        // GET: ServiceRegistrations/Create
        public ActionResult Create()
        {
            var userName = User.Identity.GetUserName();
            //check if user has connected database
            var connection = HydroServerToolsUtils.GetConnectionName(userName);
            
            if (connection != Resources.NOT_LINKED_TO_DATABASE)
            {
                return RedirectToAction("AccountIsRegistered", "ServiceRegistrations");
            }
            else
                return View();
        }
        //Get: ActivationInProcess        
        public ActionResult ActivationInProcess()
        {
            
            return View();
        }

        // POST: ServiceRegistrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServiceName,Id,GoogleAccount,ServiceTitle,ServiceDescription,ContactName,ContactEmail,ContactPhone,Organization,OrganizationUrl,Citation,RequestIssued,RequestConfirmed")] ServiceRegistration serviceRegistration)
        {
            //copyAzureDatatabaseTemplate(serviceRegistration.ServiceName); 
            //db.Users.FirstOrDefault( .Identity.Name user 


            // bool databaseConnectionExists = db.ConnectionParametersUser.Any(cp => cp.ToLower() == serviceRegistration.ServiceName.ToLower());
            var user = System.Web.HttpContext.Current.User;
            var serviceRegistrationHelper = new ServiceRegistrationHelper();
            //check if service name is allready used
            var registeredServices = serviceRegistrationHelper.GetWebServices();
            //check in registered services
            if (serviceRegistration.ServiceName != null)
            {
                bool serviceExists = registeredServices.Any(rs => rs.ServiceCode.ToLower() == serviceRegistration.ServiceName.ToLower());

                //check in proposed services
                if (!serviceExists) serviceExists = db.ServiceRegistrations.Any(rs => rs.ServiceName.ToLower() == serviceRegistration.ServiceName.ToLower());
                if (serviceExists)
                {
                    ModelState.AddModelError("ServiceName", "The Service Name is already taken please choose a different name");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //get new Id
                    Guid activationCode = Guid.NewGuid();
                    //set activation id
                    serviceRegistration.ActivationGuid = activationCode;
                    //add registration 
                    db.ServiceRegistrations.Add(serviceRegistration);
                    //commit 
                    db.SaveChanges();
                    //sendCreateDbRequest();
                    //
                    string ODMDBName = "ODM_" + serviceRegistration.ServiceName;
                    setupDatabase(ODMDBName);
                    SendActivationEmail(user.Identity.Name, activationCode);
                    HydroServerToolsUtils.SendInfoEmail("ActivationRequest",  user.Identity.Name, serviceRegistration.ServiceName,string.Empty);
                    // copyAzureDatatabaseTemplate(serviceRegistration.ServiceName);

                    return RedirectToAction("ActivationInProcess");
                }
                catch (Exception ex)
                {
                    HydroServerToolsUtils.SendInfoEmail("unknownException", user.Identity.Name, serviceRegistration.ServiceName,ex.Message);

                    return RedirectToAction("Error","Home");
                }

            }

            return View(serviceRegistration);
        }

        // GET: ServiceRegistrations/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRegistration serviceRegistration = db.ServiceRegistrations.Find(id);
            if (serviceRegistration == null)
            {
                return HttpNotFound();
            }
            return View(serviceRegistration);
        }

        // POST: ServiceRegistrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ServiceName,Id,GoogleAccount,ServiceTitle,ServiceDescription,ContactName,ContactEmail,ContactPhone,Organization,OrganizationUrl,Citation,RequestIssued,RequestConfirmed")] ServiceRegistration serviceRegistration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceRegistration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serviceRegistration);
        }

        // GET: ServiceRegistrations/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRegistration serviceRegistration = db.ServiceRegistrations.Find(id);
            if (serviceRegistration == null)
            {
                return HttpNotFound();
            }
            return View(serviceRegistration);
        }

        // POST: ServiceRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceRegistration serviceRegistration = db.ServiceRegistrations.Find(id);
            db.ServiceRegistrations.Remove(serviceRegistration);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult AccountIsRegistered()
        {
            return View();
        }
        public ActionResult Activation()
        {
            ViewBag.Message = "Invalid Activation code.";
            if (RouteData.Values["id"] != null)
            {
                var activationEmail = RouteData.Values["email"].ToString();
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                var userName = System.Web.HttpContext.Current.User.Identity.GetUserName();

                if (activationEmail.ToLower() != userName.ToLower()) return View("ActivationEmailError");

                var userActivation = db.ServiceRegistrations.Where(p => p.ActivationGuid == activationCode).FirstOrDefault();
                if (userActivation != null)
                {
                    var now = DateTime.Now.ToString("s");
                    userActivation.RequestConfirmed = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        //Add new database to allowed connections
                        var connectionParameters = new ConnectionParameters();
                        connectionParameters.Name = userActivation.ServiceName;
                        connectionParameters.DataSource = ConfigurationManager.AppSettings["DataSource"];
                        connectionParameters.InitialCatalog = "ODM_" + userActivation.ServiceName;
                        connectionParameters.UserId = ConfigurationManager.AppSettings["UserId"];
                        connectionParameters.Password = ConfigurationManager.AppSettings["Password"];
                        connectionParameters.HIScentralNetworkId = null;
                        connectionParameters.HIScentralNetworkName = null;
                        db.ConnectionParameters.Add(connectionParameters);
                         
                        //add user connection 
                        db.SaveChanges();
                        int id = connectionParameters.Id;
                        var connectionParametersUser = new ConnectionParametersUser();
                        connectionParametersUser.ConnectionParametersId = id;
                        //link user and db
                        var user = db.Users.Where(p => p.UserName == userName ).FirstOrDefault();
                        connectionParametersUser.UserId = user.Id;
                        db.ConnectionParametersUser.Add(connectionParametersUser);
                        db.SaveChanges();
                        ViewBag.Message = "Activation successful.";
                        //send info to 
                        HydroServerToolsUtils.SendInfoEmail("ActivationConfirmed", user.UserName, userActivation.ServiceName, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "Activation un-successful. Error message " + ex.Message + ". Please contact <a href='mailto:help@cuahsi.org?Subject=Activation%20failed' target='_top'>help@cuahsi.org</a>";
                    }
                }
            }

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

      

        private void SendActivationEmail(string userEmail, Guid activationCode)
        {

            //UsersEntities usersEntities = new Aspnet();
            //usersEntities.UserActivations.Add(new UserActivation
            //{
            //    UserId = user..UserId,
            //    ActivationCode = activationCode
            //});
            //usersEntities.SaveChanges();

            var userFromEmail = ConfigurationManager.AppSettings["SupportFromEmail"].ToString();

            using (MailMessage mm = new MailMessage(userFromEmail, userEmail))
            {
                mm.Subject = "Account Activation";
                string body = "Hello ";
                body += "<p />Welcome to CUAHSI Data Services. This email address " + userEmail + " was used to request an account on  </p>";
                body += "<br /><br />http://hydroserver.cuahsi.org";
                body += "<p>If you originated the request, please use the link below to verify your email address and activate your account.</p>";
                body += "<p><a href = '" + string.Format("{0}://{1}/ServiceRegistrations/Activation/{2}/{3}", Request.Url.Scheme, Request.Url.Authority, activationCode, Server.UrlEncode(userEmail).Replace(".", "%2E")) + "'>Click here to activate your account.</a></p>";//need to convert to allow routing to pick it up
                //body += "<p><a href = '" + string.Format("{0}://{1}/ServiceRegistrations/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Click here to activate your account.</a></p>";//need to convert to allow routing to pick it up

                body += "<p>Please find attached a guide to formatting data prior to uploading data.</p>"; 
                body += "<p>Please do not hesitate to contact us at help@cuahsi.org if you encounter any problems.</p>";
                body += "<br /><br />Thank you";
                body += "<br /><br />The CUAHSI Water Data Services Team";
                //mm.BodyEncoding = System.Text.Encoding.UTF8;
                mm.Body = body;
                mm.IsBodyHtml = true;

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(Server.MapPath("~/Templates/Standard Formatting Template.xlsx"));
                mm.Attachments.Add(attachment);

                //smtp.Host = "smtp.gmail.com";
                //smtp.EnableSsl = true;
                //NetworkCredential NetworkCred = new NetworkCredential("sender@gmail.com", "<password>");
                //smtp.UseDefaultCredentials = true;
                //smtp.Credentials = NetworkCred;
                //smtp.Port = 587;
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

        //private void SendSupportInfoEmail(string action,string userName, string serviceName)
        //{

        //    var userEmail = ConfigurationManager.AppSettings["SupportEmailRecipients"].ToString();
        //    var userFromEmail = ConfigurationManager.AppSettings["SupportFromEmail"].ToString();
        //    var now = DateTime.Now.ToString("s");
        //    using (MailMessage mm = new MailMessage(userFromEmail, userEmail))
        //    {
        //        if (action == "ActivationRequest")
        //        { 

        //            mm.Subject = "Account Activation has been requested:";
        //            string body = "For user " + userName + " and service: "+ serviceName;
        //            body += "<br />"+ DateTime.Now.ToString("s") +"<br /> ";
        //            body += "<br /><br />Thanks";
        //            mm.Body = body;
        //            mm.IsBodyHtml = true;
        //        }

        //        if (action == "ActivationConfirmed")
        //        {

        //            mm.Subject = "Account Activation has been confirmed:";
        //            string body = "For user " + userName + " and service: " + serviceName;
        //            body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
        //            body += "<br /><br />Thanks";
        //            mm.Body = body;
        //            mm.IsBodyHtml = true;
        //        }
        //        if (action == "unknownException")
        //        {

        //            mm.Subject = "Unknown Exception has occured:";
        //            //string body = "For user " + userName + " and service: " + serviceName;
        //            string body = "<br /> Exception:" + message;
        //            body += "<br />" + DateTime.Now.ToString("s") + "<br /> ";
        //            body += "<br /><br />Thanks";
        //            mm.Body = body;
        //            mm.IsBodyHtml = true;
        //        }
        //        //smtp.Host = "smtp.gmail.com";
        //        //smtp.EnableSsl = true;
        //        //NetworkCredential NetworkCred = new NetworkCredential("sender@gmail.com", "<password>");
        //        //smtp.UseDefaultCredentials = true;
        //        //smtp.Credentials = NetworkCred;
        //        //smtp.Port = 587;
        //        try
        //        {
        //            using (var smtp = new SmtpClient())
        //            {
        //                smtp.Send(mm);
        //                return;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //Exception - for now take no action...
        //            var errMessage = ex.Message;
        //        }
        //    }
        //}

        //private void sendCreateDbRequest()
        //{
        //    // Create a request using a URL that can receive a post.
        //    string Uri = "https://ci.cuahsi.org:8888/view/all/job/createNewSQLDB/build?delay=0sec&token=abc123";

        //    var requestUri = new Uri(Uri);
        //    //var request = (HttpWebRequest)WebRequest.Create("https://ci.cuahsi.org:8888/view/all/job/createNewSQLDB/build?delay=0sec&token=abc123");
        //    // Set the Method property of the request to POST.  


        //    //request.Credentials = new NetworkCredential()
        //    //{
        //    //    UserName = "jenkins",
        //    //    Password = "abc@123!"

        //    //};

        //    string userName = "mseul@cuahsi.org";
        //    string passWord = "";

        //    //byte[] credentialBuffer = new UTF8Encoding().GetBytes(userName + ":" + passWord);
        //    //request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(credentialBuffer);


        //    NetworkCredential nc =
        //    new NetworkCredential(userName, passWord);
        //    CredentialCache cache = new CredentialCache();

        //    cache.Add(requestUri, "Basic", nc);

        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
        //    request.Method = "POST";
        //    //String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("jenkins:abc@123!"));
        //    //request.Headers.Add("Authorization", "Basic " + encoded);
        //    //request.UseDefaultCredentials = true;
        //    // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
        //    ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    // Create POST data and convert it to a byte array.  
        //    string postData = "\"parameter\": [{\"name\":\"copyDatabaseName\", \"value\":\"dbcopy_ms_test_1\"}," +
        //        " {\"name\":\"subscriptionId\", \"value\":\"fbe9ed82-2c21-449e-b0d6-3093522e2459\"}, " +
        //        "{\"name\":\"resourceGroupName\", \"value\":\"Default-SQL-EastUS\"}," +
        //        " {\"name\":\"serverName\", \"value\":\"bhi5g2ajst\"}, " +
        //        "{\"name\":\"copyServerName\", \"value\":\"bhi5g2ajst\"}," +
        //        " {\"name\":\"poolName\", \"value\":\"Hydroportal-Pool-1\"}, " +
        //        "{\"name\":\"databaseName\", \"value\":\"ODM_1_1_1_template\"}, " +
        //        "{\"name\":\"copyResourceGroupName\", \"value\":\"Default-SQL-EastUS\"}]\"";
        //    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        //    // Set the ContentType property of the WebRequest.  
        //    request.ContentType = "application/json";
        //    // Set the ContentLength property of the WebRequest.  
        //    request.ContentLength = byteArray.Length;
        //    // Get the request stream.  
        //    Stream dataStream = request.GetRequestStream();
        //    // Write the data to the request stream.  
        //    dataStream.Write(byteArray, 0, byteArray.Length);
        //    // Close the Stream object.  
        //    dataStream.Close();
        //    // Get the response.  
        //    WebResponse response = request.GetResponse();
        //    // Display the status.  
        //    Console.WriteLine(((HttpWebResponse)response).StatusDescription);
        //    // Get the stream containing content returned by the server.  
        //    dataStream = response.GetResponseStream();
        //    // Open the stream using a StreamReader for easy access.  
        //    StreamReader reader = new StreamReader(dataStream);
        //    // Read the content.  
        //    string responseFromServer = reader.ReadToEnd();
        //    // Display the content.  
        //    Console.WriteLine(responseFromServer);
        //    // Clean up the streams.  
        //    reader.Close();
        //    dataStream.Close();
        //    response.Close();
        //}

        //private void sendCreateDbRequest2()
        //{
        //    string postData = "'parameter': [{'name':'copyDatabaseName', 'value':'dbcopy_ms_test_1'}," +
        //          " {'name':'subscriptionId', 'value':'fbe9ed82-2c21-449e-b0d6-3093522e2459'}, " +
        //          "{'name':'resourceGroupName', 'value':'Default-SQL-EastUS'}," +
        //          " {'name':'serverName', 'value':'bhi5g2ajst'}, " +
        //          "{'name':'copyServerName', 'value':'bhi5g2ajst'}," +
        //          " {'name':'poolName', 'value':'Hydroportal-Pool-1'}, " +
        //          "{'name':'databaseName', 'value':'ODM_1_1_1_template'}, " +
        //          "{'name':'copyResourceGroupName', 'value':'Default-SQL-EastUS'}]'";

        //    var request = (HttpWebRequest)WebRequest.Create(new Uri("https://api.demo.peertransfer.com/v1/transfers"));
        //    request.Method = "POST";
        //    request.AllowAutoRedirect = false;
        //    request.Accept = "*/*";
        //    request.Headers.Add("X-Peertransfer-Digest", "zYUt+Pn0A06wsSbCrrbAZn68Aslq9CbSUAKBrUEwIzI=");
        //    request.ContentType = "application/json";
        //    request.ContentLength = postData.Length;
        //    request.Proxy.Credentials = new NetworkCredential()
        //    {
        //        UserName = "jenkins",
        //        Password = "abc@123!"

        //    };
        //    using (var reqStream = request.GetRequestStream())
        //    using (var writer = new StreamWriter(reqStream))
        //    {
        //        writer.Write(postData);
        //    }

        //    var response = request.GetResponse();
        //    //MessageBox.Show(response.Headers.ToString());


        //}

        private void sendCreateDbPlaceholderRequest()
        {
            try
            {
                //App Service Publish Profile Credentials 
                string userName = ConfigurationManager.AppSettings["azurejob-userName-CopyDBTemplate"]; //userName 
                string userPassword = ConfigurationManager.AppSettings["azurejob-userPassword-CopyDBTemplate"]; //userPWD 

                //change webJobName to your WebJob name 
                // string webJobName = "WEBJOBNAME";

                var unEncodedString = String.Format($"{userName}:{userPassword}");
                var encodedString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(unEncodedString));
                //var arg = "arguments= argtest1";
                //Change this URL to your WebApp hosting the  
                //string URL = "https://?.scm.azurewebsites.net/api/triggeredwebjobs/" + webJobName + "/run";
                string URL = ConfigurationManager.AppSettings["azurejob-webhook-CopyDBTemplate"];
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
                Console.WriteLine("OK");  //no response 

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void setupDatabase(string newName)
        {
            string connectionString = "Data Source=tcp:bhi5g2ajst.database.windows.net,1433;Initial Catalog=master;User ID=HisCentralAdmin@bhi5g2ajst; Password=F@deratedResearch;Persist Security Info=true;";
            string targetDBName = "ODM_1_1_1_placeholder";
            string sourceDBName = "ODM_1_1_1_template";
            
            var targetExists = CheckDatabaseExists(connectionString, targetDBName);
            var sourceExists = CheckDatabaseExists(connectionString, sourceDBName);
            //check if the target db exist to rename if not recreate
            if (targetExists)
            {
                renameDatabase(connectionString, targetDBName, newName);
                //call job to recreate DB
                sendCreateDbPlaceholderRequest();
            }
            else
            {
                if (sourceExists) copyDatabase(connectionString, sourceDBName, newName);
                else
                {
                    throw new FileNotFoundException("Template not found. Please contact support.");
                }
            }

        }

        private bool CheckDatabaseExists(string connectionString, string databaseName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("SELECT [name] FROM [sys].[databases] WHERE [name] = '" + databaseName +"'", connection))
                {
                    connection.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }

        private void renameDatabase(string connectionString, string targetDBName, string newName)
        {
            
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                try
                {
                    using (SqlCommand command = new SqlCommand(
                                                 "ALTER DATABASE "+ targetDBName + "  MODIFY NAME = " + newName, connection))
                    {
                        connection.Open();
                        string result = (string)command.ExecuteScalar();
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                     connection.Close();
                //send email
                }
            }
            
        }

        private void copyDatabase(string connectionString, string sourceDBName, string targetDBName)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(
                                                 "CREATE DATABASE " + targetDBName + " AS COPY OF " + sourceDBName, connection))
                    {
                        connection.Open();
                        command.CommandTimeout = 300;
                        string result = (string)command.ExecuteScalar();
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    //send email
                }
            }

        }

        private async Task ExecuteSomeTask(string sSQL)
        {

            string connectionString = "Data Source=tcp:bhi5g2ajst.database.windows.net,1433;Initial Catalog=master;User ID=HisCentralAdmin@bhi5g2ajst; Password=F@deratedResearch;Persist Security Info=true;";
            string targetDBName = "ODM_1_1_1_placeholder";
            string sourceDBName = "ODM_1_1_1_template";
            //Use Async method to get data
            await ExecuteAsync(connectionString, sourceDBName, targetDBName);
        }

        public async Task<int> ExecuteAsync(string connectionString,
        string sourceDBName, string targetDBName)
        {
            using (var newConnection = new SqlConnection(connectionString))
            using (var newCommand = new SqlCommand("CREATE DATABASE " + sourceDBName + " AS COPY OF " + targetDBName, newConnection))
            {
                newCommand.CommandType = CommandType.Text;                

                await newConnection.OpenAsync().ConfigureAwait(false);
                return await newCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }


      

    }
}
