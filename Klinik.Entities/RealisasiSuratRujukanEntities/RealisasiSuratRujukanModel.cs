using Klinik.Entities.Poli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.RealisasiSuratRujukanEntities
{
    public class RealisasiSuratRujukanModel :BaseModel
    {
        public string NoSurat { get; set; }
        public string PatientName { get; set; }
        public long PatientID { get; set; }

        public long FormMedicalID { get; set; }

        public string RSRujukan { get; set; }
        public string DoctorName { get; set; }

        public PoliExamineModel poliExamineData { get; set; }
    }
}
