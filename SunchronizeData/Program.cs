using HydroServerTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SynchronizeData
{
    class Program
    {
        static void Main(string userName)
        {
            //DeleteDuplicatesDatavalues(userName);
            //recreateSeriescatalog("");
            //Harvest().GetAwaiter().GetResult();
        }
        public static void recreateSeriescatalog(string entityConnectionstring)
        {
            string providerConnectionString = "";// new EntityConnectionStringBuilder(entityConnectionstring).ProviderConnectionString;

           // var seriesCatalogRepository = new SeriesCatalogRepository();
            //seriesCatalogRepository.deleteAll(connectionString);
            using (var conn = new SqlConnection(providerConnectionString))
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

        public static void DeleteDuplicatesDatavalues(string userName)
        {
            try
            {
                string entityConnectionString = HydroServerToolsUtils.BuildConnectionStringForUserName(userName);
                //string providerConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;



                using (var conn = new SqlConnection(entityConnectionString))
                using (var command = new SqlCommand("spDeleteDuplicatesDatavalues", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                }

                //ALTER TABLE[dbo].[DataValues] DROP CONSTRAINT[UNIQUE_DataValues]

                //HttpContext.Response.ContentType = "text/plain";
                //HttpContext.Response.StatusCode = 200;

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
    }
}
