using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper.Configuration;

using HydroserverToolsBusinessObjects.Models;

namespace HydroserverToolsBusinessObjects.ModelMaps
{
    //A CsvHelper mapping class for the CategoriesModel
    public class CategoriesModelMap : ClassMap<CategoriesModel>
    {
        //Default constructor...
        public CategoriesModelMap()
        {
            //For now, call Automap...
            AutoMap();

            //Ignored class members...
            Map(m => m.Errors).Ignore();
        }
    }
}
