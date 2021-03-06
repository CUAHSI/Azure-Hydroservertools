//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ODM_1_1_1EFModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Source
    {
        public Source()
        {
            this.DataValues = new HashSet<DataValue>();
        }
    
        public int SourceID { get; set; }
        public string Organization { get; set; }
        public string SourceDescription { get; set; }
        public string SourceLink { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Citation { get; set; }
        public int MetadataID { get; set; }
        public string SourceCode { get; set; }
    
        public virtual ICollection<DataValue> DataValues { get; set; }
        public virtual ISOMetadata ISOMetadata { get; set; }
    }
}
