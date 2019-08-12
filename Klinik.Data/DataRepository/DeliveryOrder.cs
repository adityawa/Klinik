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
    
    public partial class DeliveryOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeliveryOrder()
        {
            this.DeliveryOrderDetails = new HashSet<DeliveryOrderDetail>();
        }
    
        public int id { get; set; }
        public int poid { get; set; }
        public string donumber { get; set; }
        public Nullable<System.DateTime> dodate { get; set; }
        public string dodest { get; set; }
        public Nullable<int> approve_by { get; set; }
        public Nullable<int> approve { get; set; }
        public Nullable<short> RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }
    }
}
