using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HydroserverToolsBusinessObjects.Models
{
    public class DataValuesModel
    {
        public string ValueID { get; set; }
        [Required]
        public string DataValue { get; set; }
        public string ValueAccuracy { get; set; }
        [Required]
        public string LocalDateTime { get; set; }
        [Required]
        public string UTCOffset { get; set; }
        public string DateTimeUTC { get; set; }
        //public string SiteID { get; set; }
        [Required]
        public string SiteCode { get; set; }
        //public string VariableID { get; set; }
        [Required]
        public string VariableCode { get; set; }
        public string OffsetValue { get; set; }
        public string OffsetTypeID { get; set; }
        [Required]
        public string CensorCode { get; set; }
        public string QualifierID { get; set; }
        [Required]
        public string MethodID { get; set; }        
        public string MethodDescription { get; set; }

        [Required]
        public string SourceID { get; set; }
        public string SampleID { get; set; }
        public string DerivedFromID { get; set; }
        [Required]
        public string QualityControlLevelCode { get; set; }
        //public string QualityControlLevelID { get; set; }
    }
}
