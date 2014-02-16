using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerTools.Models
{
    public class GroupsViewModel
    {
        public List<GroupsModel> listOfIncorrectRecords { get; set; }
        public List<GroupsModel> listOfCorrectRecords { get; set; }
        public List<GroupsModel> listOfDuplicateRecords { get; set; }
        public List<GroupsModel> listOfEditedRecords { get; set; }
         
    }
}