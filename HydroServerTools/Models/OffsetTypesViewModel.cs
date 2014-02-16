using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class OffsetTypesViewModel
    {
        public List<OffsetTypesModel> listOfIncorrectRecords { get; set; }
        public List<OffsetTypesModel> listOfCorrectRecords { get; set; }
        public List<OffsetTypesModel> listOfDuplicateRecords { get; set; }
        public List<OffsetTypesModel> listOfEditedRecords { get; set; }
         
    }
}