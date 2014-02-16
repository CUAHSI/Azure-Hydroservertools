using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class GroupDescriptionsViewModel
    {
        public List<GroupDescriptionModel> listOfIncorrectRecords { get; set; }
        public List<GroupDescriptionModel> listOfCorrectRecords { get; set; }
        public List<GroupDescriptionModel> listOfDuplicateRecords { get; set; }
        public List<GroupDescriptionModel> listOfEditedRecords { get; set; }
         
    }
}