//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Klinik.Data.DataRepository
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseRequestPusat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseRequestPusat()
        {
            this.PurchaseRequestPusatDetails = new HashSet<PurchaseRequestPusatDetail>();
        }
    
        public int id { get; set; }
        public string prnumber { get; set; }
        public Nullable<System.DateTime> prdate { get; set; }
        public string request_by { get; set; }
        public string approve_by { get; set; }
        public Nullable<int> approve { get; set; }
        public Nullable<int> RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<int> statusop { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseRequestPusatDetail> PurchaseRequestPusatDetails { get; set; }
    }
}
