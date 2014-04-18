using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class DerivedFromModel
    {
        
        
        public string DerivedFromId { get; set; }
        [Required]
        public string ValueID { get; set; }
        public string Errors { get; set; }
    }
}
