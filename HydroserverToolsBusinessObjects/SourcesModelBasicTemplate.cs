using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

using HydroserverToolsBusinessObjects.Interfaces;

namespace HydroserverToolsBusinessObjects.Models
{
    //A 'subset' of the SourcesModel class - for use with the Basic Template...
    [Serializable()]
    public class SourcesModelBasicTemplate : IHydroserverRepositoryProxy<SourcesModelBasicTemplate, SourcesModel>
    {
        //Required fields...
        [Required]
        public string SourceCode { get; set; }
        [Required]
        public string Organization { get; set; }
        [Required]
        public string SourceDescription { get; set; }
        [Required]
        public string ContactName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Citation { get; set; }

        //Optional fields...
        public string SourceLink { get; set; }

        //For validation error reporting...
        public string Errors { get; set; }

        //Interface methods...
        public SourcesModel InitializeProxy()
        {
            var sourcesModel = new SourcesModel();

            //Assign values from current instance...
            sourcesModel.SourceCode = SourceCode;
            sourcesModel.Organization = Organization;
            sourcesModel.SourceDescription = SourceDescription;
            sourcesModel.ContactName = ContactName;
            sourcesModel.Email = Email;
            sourcesModel.Citation = Citation;
            sourcesModel.SourceLink = SourceLink;

            //Assign default values...
            sourcesModel.MetadataLink = null;

            var strUnknown = "Unknown";

            sourcesModel.Phone = strUnknown;            //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...
            sourcesModel.Address = strUnknown;          //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...
            sourcesModel.City = strUnknown;             //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...
            sourcesModel.State = strUnknown;            //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...
            sourcesModel.ZipCode = strUnknown;          //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...
            sourcesModel.TopicCategory = strUnknown;    //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...
            sourcesModel.Title = strUnknown;            //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...
            sourcesModel.Abstract = strUnknown;         //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...
            sourcesModel.ProfileVersion = strUnknown;   //Per SourcesRepository.AddSource(...) IMPORT_VALUE_CANNOTBEEMPTY check...

            sourcesModel.Errors = null;

            //Processing complete - return
            return sourcesModel;
        }

        public SourcesModelBasicTemplate ValueFromProxy(SourcesModel proxy)
        {
            if (null != proxy)
            {
                //Assign values to current instance...
                SourceCode = proxy.SourceCode;
                Organization = proxy.Organization;
                SourceDescription = proxy.SourceDescription;
                ContactName = proxy.ContactName;
                Email = proxy.Email;
                Citation = proxy.Citation;
                SourceLink = proxy.SourceLink;

                Errors = proxy.Errors;
            }

            //Processing complete - return current instance
            return this;
        }

        public bool CompareWithProxy(SourcesModel proxy)
        {
            return ((null != proxy) &&
                    //Compare proxy values to current instance...
                    SourceCode == proxy.SourceCode &&
                    Organization == proxy.Organization &&
                    SourceDescription == proxy.SourceDescription &&
                    ContactName == proxy.ContactName &&
                    Email == proxy.Email &&
                    Citation == proxy.Citation &&
                    SourceLink == proxy.SourceLink );
        }
    }
}
