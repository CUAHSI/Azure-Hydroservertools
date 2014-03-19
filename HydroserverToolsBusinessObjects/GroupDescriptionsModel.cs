﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class GroupDescriptionModel
    {
        public string GroupID { get; set; }
        [Required]
        public string GroupDescription { get; set; }//named to match EF model name
    }
}
