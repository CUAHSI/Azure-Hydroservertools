using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class QualityControlLevelModel
    {
        //public string QualityControlLevelID { get; set; }
        [Required]
        public string QualityControlLevelCode { get; set; }
        [Required]
        public string Definition { get; set; }
        [Required]
        public string Explanation { get; set; }
        public string Errors { get; set; }
    }
}
