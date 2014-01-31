using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class DataValuesModel
    {
        //public string ValueID { get; set; }
        public string DataValue { get; set; }
        public string ValueAccuracy { get; set; }
        public string LocalDateTime { get; set; }
        public string UTCOffset { get; set; }
        public string DateTimeUTC { get; set; }
        //public string SiteID { get; set; }
        public string SiteCode { get; set; }
        public string VariableID { get; set; }
        public string VariableCode { get; set; }        
        public string OffsetValue { get; set; }
        public string OffsetTypeID { get; set; }
        public string CensorCode { get; set; }
        public string QualifierID { get; set; }
        public string MethodID { get; set; }
        public string SourceID { get; set; }
        public string SampleID { get; set; }
        public string DerivedFromID { get; set; }
        public string QualityControlLevelID { get; set; }
    }
}
