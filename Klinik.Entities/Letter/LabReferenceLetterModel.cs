using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Letter
{
    public class LabReferenceLetterModel :LetterModel
    {
        public string PatientAge { get; set; }
        public DateTime? Cekdate { get; set; }
        public long FormMedicalID { get; set; }

        public string strCekdate { get; set; }
    }

    
}
