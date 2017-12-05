using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper.Configuration;

using HydroserverToolsBusinessObjects.Models;

namespace HydroserverToolsBusinessObjects.ModelMaps
{
    //A CsvHelper mapping class for the SiteModel
    public class SiteModelMap : ClassMap<SiteModel>
    {
        //Default constructor...
        public SiteModelMap()
        {
            //For now, call Automap...
            AutoMap();

            //Ignored class members...
            Map(m => m.Errors).Ignore();
        }
    }
}
