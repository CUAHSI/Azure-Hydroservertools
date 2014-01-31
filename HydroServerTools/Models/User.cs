using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class HydroServerUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string ConnectionName { get; set; }
        public string Role { get; set; } 
    }
}