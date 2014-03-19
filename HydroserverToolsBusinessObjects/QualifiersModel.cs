using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class QualifiersModel
    {
        public string QualifierID { get; set; }
        public string QualifierCode { get; set; }
        [Required]
        public string QualifierDescription { get; set; }
    }
}
