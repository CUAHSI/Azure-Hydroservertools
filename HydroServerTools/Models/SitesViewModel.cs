using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class VariablesViewModel
    {

        public List<VariablesModel> listOfIncorrectRecords { get; set; }
        public List<VariablesModel> listOfCorrectRecords { get; set; }
        public List<VariablesModel> listOfDuplicateRecords { get; set; }
        public List<VariablesModel> listOfEditedRecords { get; set; }
         
    }
}