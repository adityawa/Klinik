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
    
    public partial class Letter
    {
        public long Id { get; set; }
        public string LetterType { get; set; }
        public long AutoNumber { get; set; }
        public int Year { get; set; }
        public string Keperluan { get; set; }
        public string Decision { get; set; }
        public string Treatment { get; set; }
        public string Action { get; set; }
        public string ReferenceTo { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<long> ForPatient { get; set; }
        public string ResponsiblePerson { get; set; }
        public string PatientAge { get; set; }
        public Nullable<long> FormMedicalID { get; set; }
        public Nullable<System.DateTime> Cekdate { get; set; }
        public string Pekerjaan { get; set; }
        public string OtherInfo { get; set; }
        public Nullable<int> ClinicID { get; set; }
    }
}
