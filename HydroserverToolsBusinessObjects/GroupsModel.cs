﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    [Serializable()]
    public class GroupsModel
    {
        public string GroupID { get; set; }      
        public string ValueID { get; set; }
        public string Errors { get; set; }
    }
}
