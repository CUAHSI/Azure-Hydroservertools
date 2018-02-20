using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

using HydroserverToolsBusinessObjects.Interfaces;

namespace HydroserverToolsBusinessObjects.Models
{
    //A 'subset' of the SiteModel class - for use with the Basic Template...
    [Serializable()]
    public class SiteModelBasicTemplate : IHydroserverRepositoryProxy<SiteModelBasicTemplate, SiteModel>
    {
        //Required fields...
        [Required]
        public string SiteCode { get; set; }
        [Required]
        public string SiteName { get; set; }
        [Required]
        public string Latitude { get; set; }
        [Required]
        public string Longitude { get; set; }
        [Required]
        public string LatLongDatumSRSName { get; set; }
        [Required]
        public string SiteType { get; set; }

        //Optional fields...
        public string Comments { get; set; }

        //For validation error reporting...
        public string Errors { get; set; }

        //Interface methods...
        public SiteModel InitializeProxy()
        {
            var siteModel = new SiteModel();

            //Assign values from current instance...
            siteModel.SiteCode = SiteCode;
            siteModel.SiteName = SiteName;
            siteModel.Latitude = Latitude;
            siteModel.Longitude = Longitude;
            siteModel.LatLongDatumSRSName = LatLongDatumSRSName;
            siteModel.SiteType = SiteType;
            siteModel.Comments = Comments;

            //Assign default values...
            siteModel.LatLongDatumID = null;
            siteModel.Elevation_m = null;
            siteModel.VerticalDatum = null;
            siteModel.LocalX = null;
            siteModel.LocalY = null;
            siteModel.LocalProjectionID = null;
            siteModel.LocalProjectionSRSName = null;
            siteModel.PosAccuracy_m = null;
            siteModel.State = null;
            siteModel.County = null;

            siteModel.Errors = null;

            //Processing complete - return...
            return siteModel;
        }

        public SiteModelBasicTemplate ValueFromProxy(SiteModel proxy )
        {
            if ( null != proxy )
            {
                //Assign values to current instance...
                SiteCode = proxy.SiteCode;
                SiteName = proxy.SiteName;
                Latitude = proxy.Latitude;
                Longitude = proxy.Longitude;
                LatLongDatumSRSName = proxy.LatLongDatumSRSName;
                SiteType = proxy.SiteType;
                Comments = proxy.Comments;

                Errors = proxy.Errors;
            }

            //Processing complete - return current instance
            return this;
        }

        public bool CompareWithProxy(SiteModel proxy)
        {
            return ((null != proxy) &&
                    //Compare proxy values to current instance...
                    SiteCode == proxy.SiteCode &&
                    SiteName == proxy.SiteName &&
                    Latitude == proxy.Latitude &&
                    Longitude == proxy.Longitude &&
                    LatLongDatumSRSName == proxy.LatLongDatumSRSName &&
                    SiteType == proxy.SiteType /*&&     
                    Comments == proxy.Comments */);     //Omit optional fields from comparison...
        }
    }
}
