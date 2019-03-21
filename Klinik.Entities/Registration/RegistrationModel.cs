using System;

namespace Klinik.Entities.Registration
{
    public class RegistrationModel : BaseModel
    {
        public int ClinicID { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public string MRNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionDateStr { get; set; }
        public int Type { get; set; }
        public string TypeStr { get; set; }
        public int AppointmentID { get; set; }
        public int SortNumber { get; set; }
        public int PoliFromID { get; set; }
        public string PoliFromName { get; set; }
        public int PoliToID { get; set; }
        public string PoliToName { get; set; }
        public string Remark { get; set; }
        public int ReffID { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public int DoctorID { get; set; }
        public string DoctorStr { get; set; }
    }
}
