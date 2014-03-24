using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class CategoriesModel
    {
        //[Required]
        public string VariableID { get; set; }
        [Required]
        public string VariableCode { get; set; }
        [Required]
        public string DataValue { get; set; }
        [Required]
        public string CategoryDescription { get; set; }
    }
}
