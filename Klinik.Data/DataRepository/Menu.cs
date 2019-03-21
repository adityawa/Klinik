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
    
    public partial class Menu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Menu()
        {
            this.Privileges = new HashSet<Privilege>();
        }
    
        public long ID { get; set; }
        public string Description { get; set; }
        public Nullable<long> ParentMenuId { get; set; }
        public string PageLink { get; set; }
        public int SortIndex { get; set; }
        public Nullable<bool> HasChild { get; set; }
        public Nullable<bool> IsMenu { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public Nullable<int> Level { get; set; }
        public string Icon { get; set; }
        public short RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Privilege> Privileges { get; set; }
    }
}
