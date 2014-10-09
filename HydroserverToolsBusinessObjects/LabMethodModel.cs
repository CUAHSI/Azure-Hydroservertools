using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class LabMethodModel
    {
        //public string LabMethodID { get; set; }
        //[Required]
        //public string LabMethodCode { get; set; }
        [Required]
        public string LabName { get; set; }
        [Required]
        public string LabOrganization { get; set; }
        [Required]
        public string LabMethodName { get; set; }
        [Required]
        public string LabMethodDescription { get; set; }
        
        public string LabMethodLink { get; set; }
        public string Errors { get; set; }
    }
}
