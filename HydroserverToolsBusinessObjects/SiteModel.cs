using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace HydroserverToolsBusinessObjects.Models
{
    public class SiteModel 
    {
        //public string SiteID { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }  
        public string LatLongDatumSRSName { get; set; }
        //public string LatLongDatumID { get; set; }
        public string Elevation_m { get; set; }
        public string VerticalDatum { get; set; }
        public string LocalX { get; set; }
        public string LocalY { get; set; }
        //public string LocalProjectionID { get; set; }
        public string PosAccuracy_m { get; set; }
        public string State { get; set; }
       
        public string County { get; set; }
        public string Comments { get; set; }
        public string SiteType { get; set; }
      

       
    }
}