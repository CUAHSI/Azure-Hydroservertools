using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class VariablesModel
    {
        //public string VariableID { get; set; }
        public string VariableCode { get; set; }
        public string VariableName { get; set; }
        public string Speciation { get; set; }
        //public string VariableUnitsID { get; set; }
        public string VariableUnitsName { get; set; }
        public string SampleMedium { get; set; }
        public string ValueType { get; set; }
        public string IsRegular { get; set; }
        public string TimeSupport { get; set; }
        //public string TimeUnitsID { get; set; }
        public string TimeUnitsName { get; set; }       
        public string DataType { get; set; }
        public string GeneralCategory { get; set; }
        public string NoDataValue { get; set; }       
    }
}
