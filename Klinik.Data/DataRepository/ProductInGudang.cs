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
    
    public partial class ProductInGudang
    {
        public int id { get; set; }
        public Nullable<int> GudangId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> stock { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> RowStatus { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Gudang Gudang { get; set; }
    }
}
