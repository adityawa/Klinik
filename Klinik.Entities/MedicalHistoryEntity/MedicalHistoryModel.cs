using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.MedicalHistoryEntity
{
    public class MedicalHistoryModel : BaseModel
    {
        public string Tanggal { get; set; }
        public string ClinicName { get; set; }
        public string PoliName { get; set; }
        public string PatientName { get; set; }
        public string Relationship { get; set; }
        public string Keperluan { get; set; }

        public long FormMedicalId { get; set; }
        public long FormExamineID { get; set; }
        public long EmployeeID { get; set; }
        public long IDPatient { get; set; }
        public EmployeeModel EmployeeData { get; set; }
    }

    public class MedicalHistoryForDoctorModel : MedicalHistoryModel
    {
        public string DoctorName { get; set; }
        public string Anamnesa { get; set; }
        public string Diagnosa { get; set; }
        public string Therapy { get; set; }
        public string ICDDescription { get; set; }

        public string Result { get; set; }
    }
}
