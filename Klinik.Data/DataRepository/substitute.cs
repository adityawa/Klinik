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
    
    public partial class substitute
    {
        public int id { get; set; }
        public int ProductId { get; set; }
        public string namabarang { get; set; }
        public Nullable<double> qty { get; set; }
        public Nullable<int> PurchaseOrderDetailId { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual PurchaseOrderDetail PurchaseOrderDetail { get; set; }
    }
}