using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper.Configuration;

using HydroserverToolsBusinessObjects.Models;

namespace CsvHelperTest.ModelMaps
{
    //A CsvHelper mapping class for the MethodModel
    class MethodModelMap : ClassMap<MethodModel>
    {
        //Default constructor...
        public MethodModelMap()
        {
            //Mapped class members...
            Map(m => m.MethodCode).Name("MethodCode");
            //NOTE: Can programmatically add Validate(...) calls for all model fields with 'Required' attribute...
            //      Returning false from validation code causes CsvHelper to throw a ValidationException
            //      Can return true from validation code to inhibit the exception throw - thus can keep going through the file to check for more errors... 
            Map(m => m.MethodCode).Validate(field => !String.IsNullOrWhiteSpace(field));

            Map(m => m.MethodDescription).Name("MethodDescription");
            //NOTE: Can programmatically add Validate(...) calls for all model fields with 'Required' attribute...
            Map(m => m.MethodDescription).Validate(field => !String.IsNullOrWhiteSpace(field));

            Map(m => m.MethodLink).Name("MethodLink");

            //Ignored class members...
            Map(m => m.Errors).Ignore();
        }
    }
}
