using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class LabMethodsViewModel
    {
        public List<LabMethodModel> listOfIncorrectRecords { get; set; }
        public List<LabMethodModel> listOfCorrectRecords { get; set; }
        public List<LabMethodModel> listOfDuplicateRecords { get; set; }
        public List<LabMethodModel> listOfEditedRecords { get; set; }
         
    }
}