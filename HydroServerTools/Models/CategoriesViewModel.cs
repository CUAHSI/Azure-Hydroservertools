using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class CategoriesViewModel
    {
        public List<CategoriesModel> listOfIncorrectRecords { get; set; }
        public List<CategoriesModel> listOfCorrectRecords { get; set; }
        public List<CategoriesModel> listOfDuplicateRecords { get; set; }
        public List<CategoriesModel> listOfEditedRecords { get; set; }
         
    }
}