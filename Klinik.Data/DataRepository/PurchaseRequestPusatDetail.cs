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
    
    public partial class PurchaseRequestPusatDetail
    {
        public int id { get; set; }
        public int PurchaseRequestPusatId { get; set; }
        public int ProductId { get; set; }
        public string namabarang { get; set; }
        public int VendorId { get; set; }
        public string namavendor { get; set; }
        public Nullable<double> satuan { get; set; }
        public Nullable<double> harga { get; set; }
        public Nullable<double> stok_prev { get; set; }
        public Nullable<double> total_req { get; set; }
        public Nullable<double> total_dist { get; set; }
        public Nullable<double> sisa_stok { get; set; }
        public Nullable<double> qty { get; set; }
        public Nullable<double> qty_add { get; set; }
        public string reason_add { get; set; }
        public Nullable<double> qty_final { get; set; }
        public string remark { get; set; }
        public Nullable<double> total { get; set; }
        public Nullable<double> qty_unit { get; set; }
        public Nullable<double> qty_box { get; set; }
        public Nullable<int> RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<int> statusop { get; set; }
    
        public virtual PurchaseRequestPusat PurchaseRequestPusat { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual Product Product { get; set; }
    }
}
