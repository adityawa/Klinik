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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.DeliveryOrderDetails = new HashSet<DeliveryOrderDetail>();
            this.DeliveryOrderDetails1 = new HashSet<DeliveryOrderDetail>();
            this.FormExamineMedicines = new HashSet<FormExamineMedicine>();
            this.ProductInGudangs = new HashSet<ProductInGudang>();
            this.ProductMedicines = new HashSet<ProductMedicine>();
        }
    
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProductCategoryID { get; set; }
        public int ProductUnitID { get; set; }
        public decimal RetailPrice { get; set; }
        public Nullable<short> RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryOrderDetail> DeliveryOrderDetails1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormExamineMedicine> FormExamineMedicines { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductUnit ProductUnit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductInGudang> ProductInGudangs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductMedicine> ProductMedicines { get; set; }
    }
}
