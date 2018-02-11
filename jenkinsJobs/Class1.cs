using JenkinsNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jobs
{
    public class jenkinsJobs
    {
        static HttpClient client = new HttpClient();

        public static void Main(int networkId, bool isProduction)
        {
            //RunAsync(networkId, isProduction).GetAwaiter().GetResult();
            Run(networkId, isProduction);
        }

        //static async Task RunAsync(int networkId, bool isProduction)
        public static void Run(int networkId, bool isProduction)
        {
            try
            {
                client.BaseAddress = new Uri("https://ci.cuahsi.org:8888");
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Host = "ci.cuahsi.org";
                client.DefaultRequestHeaders.Add("Authorization", "Basic amVua2luczphYmNAMTIzIQ==");
                var response =  client.GetAsync("/crumbIssuer/api/xml?xpath=concat(//crumbRequestField,\":\",//crumb)").Result;
                

                //var response =  client.GetAsync("/crumbIssuer/api/xml?xpath=concat(//crumbRequestField,\":\",//crumb)").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    var tmp = responseString.ToString().Split(':');
                    client.DefaultRequestHeaders.Add(tmp[0], tmp[1]);

                    StringContent content = null;
                    HttpResponseMessage retvar = null;

                    if (isProduction)
                    {
                         content = new StringContent("json={\"parameter\": [{\"name\":\"ID\", \"value\":\"" + networkId + "\"},{\"name\":\"PRODCORE\", \"value\":\"wof-prod-synonym2\"}]}", Encoding.UTF8, "application/x-www-form-urlencoded");
                         retvar = client.PostAsync("/job/Prod%20-%20addORupdate//build?delay=0sec&token=cuahsi-hybsgtfdnviehs3", content).Result;
                    }
                    else
                    { 
                         content = new StringContent("json={\"parameter\": [{\"name\":\"ID\", \"value\":\"" + networkId + "\"},{\"name\":\"PRODCORE\", \"value\":\"wof-prod-synonym2\"}]}", Encoding.UTF8, "application/x-www-form-urlencoded");
                         retvar = client.PostAsync("/job/QA%20-%20addORupdate//build?delay=0sec&token=abc123", content).Result;
                    }
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
                //TO DO Send email

            }

            
        }
    }

}
