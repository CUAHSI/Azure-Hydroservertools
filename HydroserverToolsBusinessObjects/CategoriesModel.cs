using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    [Serializable()]
    public class CategoriesModel
    {
        [Display(Name = "NotVisible")]//
        public string VariableID { get; set; }
        [Required]
        public string VariableCode { get; set; }
        [Required]
        public string DataValue { get; set; }
        [Required]
        public string CategoryDescription { get; set; }
        public string Errors { get; set; }
    }
}
