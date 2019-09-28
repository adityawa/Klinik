using Klinik.Entities.MasterData;
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
      
       

        public SuratRujukanKeluarModel SuratRujukanLabKeluar { get; set; }

        public List<LabItemModel> LabItems { get; set; }
    }

    public class SuratRujukanKeluarModel :BaseModel
    { 
        public string NoSurat { get; set; }
        public long FormMedicalID { get; set; }
        public string DokterPengirim { get; set; }
        public List<int> ListOfLabItemId { get; set; }

    }
    
}
