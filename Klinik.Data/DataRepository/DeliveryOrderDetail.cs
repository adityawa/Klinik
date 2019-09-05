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
    
    public partial class DeliveryOrderDetail
    {
        public int id { get; set; }
        public int DeliveryOderId { get; set; }
        public int ProductId { get; set; }
        public string namabarang { get; set; }
        public Nullable<double> qty_request { get; set; }
        public string nama_by_ho { get; set; }
        public Nullable<double> qty_by_HP { get; set; }
        public string remark_by_ho { get; set; }
        public Nullable<double> qty_adj { get; set; }
        public string remark_adj { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<short> RowStatus { get; set; }
        public Nullable<bool> Recived { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual DeliveryOrder DeliveryOrder { get; set; }
    }
}
