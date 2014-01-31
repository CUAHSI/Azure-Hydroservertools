using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class SitesViewModel
    {

        public List<SiteModel> listOfIncorrectRecords { get; set; }
        public List<SiteModel> listOfCorrectRecords { get; set; }
        public List<SiteModel> listOfDuplicateRecords { get; set; }
        public List<SiteModel> listOfEditedRecords { get; set; }
         
    }
}