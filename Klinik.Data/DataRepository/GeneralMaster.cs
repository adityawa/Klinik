
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
    
public partial class GeneralMaster
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public GeneralMaster()
    {

        this.Employees = new HashSet<Employee>();

    }


    public short ID { get; set; }

    public string Type { get; set; }

    public string Name { get; set; }

    public string Value { get; set; }

    public short RowStatus { get; set; }

    public string CreatedBy { get; set; }

    public System.DateTime CreatedDate { get; set; }

    public string ModifiedBy { get; set; }

    public Nullable<System.DateTime> ModifiedDate { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Employee> Employees { get; set; }

}

}
