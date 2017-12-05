using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper.Configuration;

using HydroserverToolsBusinessObjects.Models;

namespace CsvHelperTest.ModelMaps
{
    //A CsvHelper mapping class for the SampleModel
    class SampleModelMap : ClassMap<SampleModel>
    {
        //Default constructor...
        public SampleModelMap()
        {
            //For now, call Automap...
            AutoMap();

            //Ignored class members...
            Map(m => m.Errors).Ignore();
        }
    }
}
