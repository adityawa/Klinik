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
    
    public partial class QueuePoli
    {
        public long ID { get; set; }
        public Nullable<long> ClinicID { get; set; }
        public Nullable<long> PatientID { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<short> Type { get; set; }
        public Nullable<long> AppointmentID { get; set; }
        public Nullable<int> SortNumber { get; set; }
        public Nullable<int> PoliFrom { get; set; }
        public Nullable<int> PoliTo { get; set; }
        public string Remark { get; set; }
        public Nullable<long> ReffID { get; set; }
        public Nullable<int> Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual Patient Patient { get; set; }
        public virtual Poli Poli { get; set; }
        public virtual Poli Poli1 { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}
