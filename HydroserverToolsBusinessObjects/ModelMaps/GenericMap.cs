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

        //Members...
        private List<string> requiredPropertyNames = new List<string>();

        private List<string> optionalPropertyNames = new List<string>();

        //Default constructor...
        public GenericMap()
        {
            //Retrieve <MapType> public properties
            Type mappedType = typeof(MappedType);
            var members = mappedType.GetMembers();
            var errorsName = "Errors";
            var displayName = "NotVisible";

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
                        //Other than 'Errors' property - check property 'visibility'...
                        //Source: https://stackoverflow.com/questions/32808132/how-to-get-the-value-in-displayname-attribute-in-controller-for-any-prope
                        var displayAttributes = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
                        if (null != displayAttributes && 0 < displayAttributes.Length)
                        {
                            bool bVisible = true;  //Assume visible...
                            foreach (var displayAttribute in displayAttributes)
                            {
                                if (displayName.ToLowerInvariant() == displayAttribute.Name.ToLowerInvariant())
                                {
                                    //Property is not 'visible' - i.e., not shown on app screens (but perhaps referenced in the db) - skip...
                                    bVisible = false;
                                    break;
                                }
                            }

                            if (!bVisible)
                            {
                                continue;
                            }
                        }

                        //Map for header validation...
                        Map(mappedType, memberInfo).Name(propName);

                        //Retrieve attributes
                        //NOTE: CsvHelper operation note:
                        //      Returning false from validation code causes CsvHelper to throw a ValidationException
                        //      Can return true from validation code to inhibit the exception throw - thus can keep going through the file to check for more errors... 
                        if (Attribute.IsDefined(propertyInfo, typeof(RequiredAttribute)))
                        {
                            //Required - validate field is not empty
                            //Assumption - fields are all strings...
                            Map(mappedType, memberInfo).Validate(field => !String.IsNullOrWhiteSpace(field));

                            //Retain required property name...
                            requiredPropertyNames.Add(propName);
                        }
                        else
                        {
                            //Not required - make validation a 'no-op' so field contents, if available, are copied
                            //Assumption - fields are all strings...
                            Map(mappedType, memberInfo).Validate(field => true);

                            //Retain optional property name...
                            optionalPropertyNames.Add(propName);
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

        //Methods

        //Return a COPY of the list of required property names...
        public List<string> GetRequiredPropertyNames()
        {
            var result = new List<string>();

            foreach (var propName in requiredPropertyNames)
            {
                result.Add(propName);
            }

            //Processing complete - return result...
            return result;
        }

        //Return a COPY of the list of optional property names...
        public List<string> GetOptionalPropertyNames()
        {
            var result = new List<string>();

            foreach (var propName in optionalPropertyNames)
            {
                result.Add(propName);
            }

            //Processing complete - return result...
            return result;
        }
    }
}
