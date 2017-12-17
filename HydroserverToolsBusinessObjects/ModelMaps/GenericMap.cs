using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

using CsvHelper.Configuration;

namespace HydroserverToolsBusinessObjects.ModelMaps
{
    public class GenericMap<MappedType> : ClassMap<MappedType>
    {
        //Default constructor...
        public GenericMap()
        {
            //Retrieve <MapType> public properties
            Type mappedType = typeof(MappedType);
            var members = mappedType.GetMembers();
            var errorsName = "Errors";

            //For each member...
            foreach (var memberInfo in members)
            {
                if (System.Reflection.MemberTypes.Property == memberInfo.MemberType)
                {
                    //Member is a property - check name.
                    var propName = memberInfo.Name;
                    var propertyInfo = mappedType.GetProperty(propName);

                    if (errorsName.ToLowerInvariant() != propName.ToLowerInvariant())
                    {
                        //Other than 'Errors' property - map...
                        Map(mappedType, memberInfo).Name(propName);     //For header validation...

                        //Retrieve attributes
                        //NOTE: CsvHelper operation note:
                        //      Returning false from validation code causes CsvHelper to throw a ValidationException
                        //      Can return true from validation code to inhibit the exception throw - thus can keep going through the file to check for more errors... 
                        if (Attribute.IsDefined(propertyInfo, typeof(RequiredAttribute)))
                        {
                            //Required - validate field is not empty
                            //Assumption - fields are all strings...
                            Map(mappedType, memberInfo).Validate(field => !String.IsNullOrWhiteSpace(field));
                        }
                    }
                    else
                    {
                        //Errors property - DO NOT map...
                        Map(mappedType, memberInfo).Ignore();
                    }
                }
            }
        }
    }
}
