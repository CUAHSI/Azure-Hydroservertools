using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class SourcesModel
    {
        public string SourceID { get; set; }
        [Required]
        public string Organization { get; set; }
        [Required]
        public string SourceDescription { get; set; }

        public string SourceLink { get; set; }
        [Required]
        public string ContactName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string Citation { get; set; }
        //public string MetadataID { get; set; }
        [Required]
        public string TopicCategory { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Abstract { get; set; }
        [Required]
        public string ProfileVersion { get; set; }

        public string MetadataLink { get; set; }
        public string Errors { get; set; }

    }
}
