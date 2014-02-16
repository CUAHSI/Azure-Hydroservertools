using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class QualifiersViewModel
    {
        public List<QualifiersModel> listOfIncorrectRecords { get; set; }
        public List<QualifiersModel> listOfCorrectRecords { get; set; }
        public List<QualifiersModel> listOfDuplicateRecords { get; set; }
        public List<QualifiersModel> listOfEditedRecords { get; set; }
         
    }
}