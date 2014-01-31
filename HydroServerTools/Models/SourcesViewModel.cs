using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class SourcesViewModel
    {

        public List<SourcesModel> listOfIncorrectRecords { get; set; }
        public List<SourcesModel> listOfCorrectRecords { get; set; }
        public List<SourcesModel> listOfDuplicateRecords { get; set; }
        public List<SourcesModel> listOfEditedRecords { get; set; }
         
    }
}