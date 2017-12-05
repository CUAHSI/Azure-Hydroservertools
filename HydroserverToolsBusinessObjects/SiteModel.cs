using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace HydroserverToolsBusinessObjects.Models
{
    [Serializable()]
    public class SiteModel
    {
        //public string SiteID { get; set; }
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
        [Display(Name="NotVisible")]//used to udicate that the fiels is not vosible ib the datatables but required to automap the fiels. bit of a hack
        public string LatLongDatumID { get; set; }
        public string Elevation_m { get; set; }
        public string VerticalDatum { get; set; }
        public string LocalX { get; set; }
        public string LocalY { get; set; }
        [Display(Name = "NotVisible")]
        public string LocalProjectionID { get; set; }
        public string LocalProjectionSRSName { get; set; }
        public string PosAccuracy_m { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Comments { get; set; }
        public string SiteType { get; set; }
        public string Errors { get; set; }


    }
}