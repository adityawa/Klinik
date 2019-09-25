using Klinik.Entities.Form;
using Klinik.Entities.MasterData;
using Klinik.Entities.PreExamine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Letter
{
    public class RujukanBerobatModel : LetterModel
    {
        public FormExamineModel FormExamineData { get; set; }
        public PreExamineModel PreExamineData { get; set; }
        public string OtherInfo { get; set; }
        public string Perusahaan { get; set; }
        public string strPemFisik { get; set; }
        public string NoSurat { get; set; }
        public InfoRujukan InfoRujukanData { get; set; }
    }

    public class InfoRujukan
    {
        public string RSRujukan { get; set; }
        public string Phone { get; set; }
        public string NamaDokter { get; set; }
        public string HariPraktek { get; set; }
    }
}
