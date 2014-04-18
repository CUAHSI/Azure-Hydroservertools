using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class ErrorModel
    {
        
        public string ErrorArea;
        public string ErrorMessage;

        public ErrorModel(string errorArea, string errorMessage)
        {
            ErrorArea = errorArea;
            ErrorMessage = errorMessage;
        }
       
    }
}
