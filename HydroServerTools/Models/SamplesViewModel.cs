using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class SamplesViewModel
    {
        public List<SampleModel> listOfIncorrectRecords { get; set; }
        public List<SampleModel> listOfCorrectRecords { get; set; }
        public List<SampleModel> listOfDuplicateRecords { get; set; }
        public List<SampleModel> listOfEditedRecords { get; set; }
         
    }
}