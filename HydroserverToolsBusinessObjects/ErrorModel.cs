using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class ErrorModel
    {
        
        string ErrorArea;
        string ErrorMessage;

        public ErrorModel(string errorArea, string errorMessage)
        {
            ErrorArea = errorArea;
            ErrorMessage = errorMessage;
        }
       
    }
}
