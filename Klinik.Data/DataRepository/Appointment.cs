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
    
    public partial class Appointment
    {
        public long ID { get; set; }
        public Nullable<long> EmployeeID { get; set; }
        public long ClinicID { get; set; }
        public Nullable<int> DoctorID { get; set; }
        public Nullable<int> RequirementID { get; set; }
        public System.DateTime AppointmentDate { get; set; }
        public Nullable<int> MCUPackageID { get; set; }
        public Nullable<int> PoliID { get; set; }
        public Nullable<short> Status { get; set; }
        public short RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual Clinic Clinic { get; set; }
        public virtual MCUPackage MCUPackage { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
