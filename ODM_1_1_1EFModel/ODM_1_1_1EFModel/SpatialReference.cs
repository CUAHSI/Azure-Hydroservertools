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
    
    public partial class SpatialReference
    {
        public SpatialReference()
        {
            this.Sites = new HashSet<Site>();
            this.Sites1 = new HashSet<Site>();
        }
    
        public int SpatialReferenceID { get; set; }
        public Nullable<int> SRSID { get; set; }
        public string SRSName { get; set; }
        public Nullable<bool> IsGeographic { get; set; }
        public string Notes { get; set; }
    
        public virtual ICollection<Site> Sites { get; set; }
        public virtual ICollection<Site> Sites1 { get; set; }
    }
}
