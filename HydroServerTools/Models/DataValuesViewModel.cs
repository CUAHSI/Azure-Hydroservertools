using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class DataValuesViewModel
    {

        public List<DataValuesModel> listOfIncorrectRecords { get; set; }
        public List<DataValuesModel> listOfCorrectRecords { get; set; }
        public List<DataValuesModel> listOfDuplicateRecords { get; set; }
        public List<DataValuesModel> listOfEditedRecords { get; set; }
         
    }
}