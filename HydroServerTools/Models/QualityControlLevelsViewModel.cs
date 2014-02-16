using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class QualityControlLevelsViewModel
    {
        public List<QualityControlLevelModel> listOfIncorrectRecords { get; set; }
        public List<QualityControlLevelModel> listOfCorrectRecords { get; set; }
        public List<QualityControlLevelModel> listOfDuplicateRecords { get; set; }
        public List<QualityControlLevelModel> listOfEditedRecords { get; set; }
         
    }
}