using Klinik.Entities;
using Klinik.Entities.Letter;
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratLabReferensi
{
    public class RujukanLabResponse : BaseResponse<LabReferenceLetterModel>
    {
        public PatientModel Patient { get; set; }
    }
}
