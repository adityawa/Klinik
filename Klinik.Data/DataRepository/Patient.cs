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
    
    public partial class Patient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Patient()
        {
            this.FormMedicals = new HashSet<FormMedical>();
            this.PatientClinics = new HashSet<PatientClinic>();
            this.QueuePolis = new HashSet<QueuePoli>();
        }
    
        public long ID { get; set; }
        public Nullable<long> EmployeeID { get; set; }
        public Nullable<short> FamilyRelationshipID { get; set; }
        public string MRNumber { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string KTPNumber { get; set; }
        public string Address { get; set; }
        public Nullable<int> CityID { get; set; }
        public string HPNumber { get; set; }
        public Nullable<short> Type { get; set; }
        public string BPJSNumber { get; set; }
        public string BloodType { get; set; }
        public short RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string PatientKey { get; set; }
    
        public virtual City City { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormMedical> FormMedicals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientClinic> PatientClinics { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QueuePoli> QueuePolis { get; set; }
    }
}
