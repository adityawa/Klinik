using Klinik.Entities.Form;
using System.Collections.Generic;

namespace Klinik.Entities.Pharmacy
{
    public class PrescriptionModel
    {
        public List<FormExamineMedicineModel> Medicines { get; set; }
        public long FormMedicalID { get; set; }

        public string PatientName { get; set; }
        public string ObatRacikanKomponens { get; set; }

      
       
        public PrescriptionModel()
        {
            Medicines = new List<FormExamineMedicineModel>();
        }
    }
}
