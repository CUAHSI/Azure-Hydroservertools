using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class SampleModel
    {
        public string SampleID { get; set; }
        [Required]
        public string SampleType { get; set; }
        [Required]
        public string LabSampleCode { get; set; }
        //public string LabMethodID { get; set; }
        [Required]
        public string LabMethodName { get; set; }
    }
}
