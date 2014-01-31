using HydroServerManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace HydroServerManage.Controllers
{
    public class UserController : ApiController
    {
        // GET api/user
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/user/5
        public string Get(int id)
        {
            return "value";
        }
        // GET api/user/5
        public string Get(string userName)
        {
            string connectionName = string.Empty;
            string path = "http://" + this.Request.RequestUri.Authority + "/XML/users.xml";
            
            XElement doc = XElement.Load(path);
            IEnumerable<XElement> users = doc.Elements();
            
            var e = from u in doc.Elements("user")
                                 where (string) u.Element("username") == userName
                                 select u;
            connectionName = e.FirstOrDefault().Element("connectionname").Value.ToString();
            return connectionName;
        }

        // POST api/user
        public void Post([FromBody]string value)
        {
        }

        // PUT api/user/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/user/5
        public void Delete(int id)
        {
        }
    }
}
