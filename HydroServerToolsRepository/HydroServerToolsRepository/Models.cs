using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroServerToolsRepository
{
    public class Models
    {
        public class TimeseriesData
        {
            public DateTime BeginDateTime { get; set; }
            public DateTime EndDateTime { get; set; }
            public DateTime BeginDateTimeUTC { get; set; }
            public DateTime EndDateTimeUTC { get; set; }
            public int ValueCount { get; set; }
        }
        
        public class UniqueTimeseriesCombination
        {
            public string SiteID;
            public string VariableID;
            public string SourceID;
            public string MethodID;
            public string QualityControlLevelID;

            public UniqueTimeseriesCombination(string siteID, string variableID, string sourceID, string methodID, string qualityControlLevelID)
            {
                // TODO: Complete member initialization
                this.SiteID = siteID;
                this.VariableID = variableID;
                this.SourceID = sourceID;
                this.MethodID = methodID;
                this.QualityControlLevelID = qualityControlLevelID;
            }
                  
        }
    }
}
