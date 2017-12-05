using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    [Serializable()]
    public class MethodModel
    {
        //public string MethodID { get; set; }
        [Required]
        public string MethodCode { get; set; }
        [Required]
        public string MethodDescription { get; set; }
        public string MethodLink { get; set; }
        public string Errors { get; set; }
    }
}
