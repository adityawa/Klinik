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
    
    public partial class FormExamine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormExamine()
        {
            this.FormExamineAttachments = new HashSet<FormExamineAttachment>();
            this.FormExamineMedicines = new HashSet<FormExamineMedicine>();
            this.FormExamineServices = new HashSet<FormExamineService>();
        }
    
        public long ID { get; set; }
        public Nullable<long> FormMedicalID { get; set; }
        public Nullable<int> PoliID { get; set; }
        public Nullable<System.DateTime> TransDate { get; set; }
        public Nullable<int> DoctorID { get; set; }
        public string Anamnesa { get; set; }
        public string Diagnose { get; set; }
        public string Therapy { get; set; }
        public string Remark { get; set; }
        public string ICDInformation { get; set; }
        public string Result { get; set; }
        public Nullable<short> RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> NeedSuratSakit { get; set; }
        public Nullable<int> JumHari { get; set; }
        public Nullable<System.DateTime> Sampai { get; set; }
        public Nullable<int> Caused { get; set; }
        public Nullable<int> Condition { get; set; }
    
        public virtual Doctor Doctor { get; set; }
        public virtual FormMedical FormMedical { get; set; }
        public virtual Poli Poli { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormExamineAttachment> FormExamineAttachments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormExamineMedicine> FormExamineMedicines { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormExamineService> FormExamineServices { get; set; }
    }
}
