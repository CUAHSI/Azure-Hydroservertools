using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class MethodsViewModel
    {
        public List<MethodModel> listOfIncorrectRecords { get; set; }
        public List<MethodModel> listOfCorrectRecords { get; set; }
        public List<MethodModel> listOfDuplicateRecords { get; set; }
        public List<MethodModel> listOfEditedRecords { get; set; }
         
    }
}