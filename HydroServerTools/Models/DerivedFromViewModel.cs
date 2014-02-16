using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class DerivedFromViewModel
    {
        public List<DerivedFromModel> listOfIncorrectRecords { get; set; }
        public List<DerivedFromModel> listOfCorrectRecords { get; set; }
        public List<DerivedFromModel> listOfDuplicateRecords { get; set; }
        public List<DerivedFromModel> listOfEditedRecords { get; set; }
         
    }
}