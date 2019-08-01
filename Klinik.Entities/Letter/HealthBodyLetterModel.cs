using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.Entities.MasterData;
using Klinik.Entities.PreExamine;
namespace Klinik.Entities.Letter
{
    public class HealthBodyLetterModel:LetterModel
    {
        public string Pekerjaan { get; set; }
        public string Decision { get; set; }
        public string NoSurat { get; set; }
        public string Keperluan { get; set; }
        public PreExamineModel PreExamineData { get; set; }
    }
}
