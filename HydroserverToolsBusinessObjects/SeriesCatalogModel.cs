using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class SeriesCatalogModel
    {
        //public string SeriesID { get; set; } 
        //public string SiteID { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string SiteType { get; set; }
        //public string VariableID { get; set; }
        public string VariableCode { get; set; }
        public string VariableName { get; set; }
        public string Speciation { get; set; }
        //public string VariableUnitsID { get; set; }
        public string VariableUnitsName { get; set; }
        public string SampleMedium { get; set; }
        public string ValueType { get; set; }
        public string TimeSupport { get; set; }
        //public string TimeUnitsID { get; set; }
        public string TimeUnitsName { get; set; }
        public string DataType { get; set; }
        public string GeneralCategory { get; set; }
        //public string MethodID { get; set; }
        public string MethodDescription { get; set; }
        //public string SourceID { get; set; }
        public string Organization { get; set; }
        public string SourceDescription { get; set; }
        public string Citation { get; set; }
        //public string QualityControlLevelID { get; set; }
        public string QualityControlLevelCode { get; set; }
        public string BeginDateTime { get; set; }
        public string EndDateTime { get; set; }
        //public string BeginDateTimeUTC { get; set; }
        //public string EndDateTimeUTC { get; set; }
        public string ValueCount { get; set; }
    }
}
