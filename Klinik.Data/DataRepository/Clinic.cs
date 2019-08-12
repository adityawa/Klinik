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
    
    public partial class Clinic
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clinic()
        {
            this.Appointments = new HashSet<Appointment>();
            this.DoctorClinics = new HashSet<DoctorClinic>();
            this.FormMedicals = new HashSet<FormMedical>();
            this.Organizations = new HashSet<Organization>();
            this.PatientClinics = new HashSet<PatientClinic>();
            this.PoliClinics = new HashSet<PoliClinic>();
            this.PoliSchedules = new HashSet<PoliSchedule>();
            this.PoliScheduleMasters = new HashSet<PoliScheduleMaster>();
            this.PoliServices = new HashSet<PoliService>();
            this.QueuePolis = new HashSet<QueuePoli>();
            this.Gudangs = new HashSet<Gudang>();
            this.DeliveryOrderDetails = new HashSet<DeliveryOrderDetail>();
        }
    
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string LegalNumber { get; set; }
        public Nullable<System.DateTime> LegalDate { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Long { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<short> ClinicType { get; set; }
        public Nullable<int> ReffID { get; set; }
        public short RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual City City { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormMedical> FormMedicals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Organization> Organizations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientClinic> PatientClinics { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PoliClinic> PoliClinics { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PoliSchedule> PoliSchedules { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PoliScheduleMaster> PoliScheduleMasters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PoliService> PoliServices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QueuePoli> QueuePolis { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gudang> Gudangs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }
    }
}
