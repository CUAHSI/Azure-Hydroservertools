using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    [Serializable()]
    public class QualifiersModel
    {
        //public string QualifierID { get; set; }
        [Required]
        public string QualifierCode { get; set; }
        public string QualifierDescription { get; set; }
        public string Errors { get; set; }
    }
}
