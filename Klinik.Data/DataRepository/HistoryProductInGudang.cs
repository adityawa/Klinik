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
    
    public partial class HistoryProductInGudang
    {
        public int id { get; set; }
        public int ProductId { get; set; }
        public int GudangId { get; set; }
        public int value { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    
        public virtual Gudang Gudang { get; set; }
        public virtual Product Product { get; set; }
    }
}
