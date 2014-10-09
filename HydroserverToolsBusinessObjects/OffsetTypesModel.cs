using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class OffsetTypesModel
    {
        //public string OffsetTypeID { get; set; }
        public string OffsetTypeCode { get; set; }
        [Display(Name = "NotVisible")]//
        public string OffsetUnitsID { get; set; }
        [Required]
        public string OffsetUnitsName { get; set; }
        [Required]
        public string OffsetDescription { get; set; }
        public string Errors { get; set; }
    }
}
