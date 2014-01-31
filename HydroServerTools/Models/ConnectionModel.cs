using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class ConnectionModel
    {
        [Required(ErrorMessage = "The field is required.")]
        [Display(Name = "Name of Server")]
        public string ServerName { get; set; }

         [Required(ErrorMessage = "The field is required.")]
         [Display(Name = "Name of Datasource")]
        public string DataSourceName { get; set; }

         [Required(ErrorMessage = "The field is required.")]
         [Display(Name = "Username")]
        public string Username { get; set; }

         [Required(ErrorMessage = "The field is required.")]        
         [Display(Name = "Password")]
         [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }
    }
}