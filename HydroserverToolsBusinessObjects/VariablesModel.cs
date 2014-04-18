using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class VariablesModel
    {
        //public string VariableID { get; set; }
        [Required]
        public string VariableCode { get; set; }
        [Required]
        public string VariableName { get; set; }
        [Required]
        public string Speciation { get; set; }
        [Display(Name = "NotVisible")]
        public string VariableUnitsID { get; set; }
        [Required]
        public string VariableUnitsName { get; set; }
        [Required]
        public string SampleMedium { get; set; }
        [Required]
        public string ValueType { get; set; }
        [Required]
        public string IsRegular { get; set; }
        [Required]
        public string TimeSupport { get; set; }
        [Display(Name = "NotVisible")]
        public string TimeUnitsID { get; set; }       
        [Required]
        public string TimeUnitsName { get; set; }
        [Required]
        public string DataType { get; set; }
        [Required]
        public string GeneralCategory { get; set; }
        [Required]
        public string NoDataValue { get; set; }
        public string Errors { get; set; }
    }
}
