using Klinik.Entities.Form;
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Letter
{ 
    public class SuratIzinSakitModel
    {
        public string NoSurat { get; set; }
        public PatientModel patientData { get; set; }
        public FormExamineModel ExamineData { get; set; }

        public string strStartIstirahat { get; set; }
        public string strSelesaiIstirahat { get; set; }

        public string NamaDokter { get; set; }

        public SuratIzinSakitModel()
        {
            patientData = new PatientModel();
            ExamineData = new FormExamineModel();
        }
    }
}
