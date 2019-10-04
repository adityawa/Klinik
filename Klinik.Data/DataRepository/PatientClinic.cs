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
    
    public partial class PatientClinic
    {
        public long ID { get; set; }
        public long PatientID { get; set; }
        public long ClinicID { get; set; }
        public string TempAddress { get; set; }
        public Nullable<int> TempCityID { get; set; }
        public string RefferencePerson { get; set; }
        public string RefferenceNumber { get; set; }
        public Nullable<int> RefferenceRelation { get; set; }
        public Nullable<long> PhotoID { get; set; }
        public string OldMRNumber { get; set; }
        public short RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual City City { get; set; }
        public virtual FileArchieve FileArchieve { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}
