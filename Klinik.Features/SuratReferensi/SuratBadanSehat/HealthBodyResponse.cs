using Klinik.Entities;
using Klinik.Entities.Letter;
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratBadanSehat
{
    public class HealthBodyResponse : BaseResponse<HealthBodyLetterModel>
    {
        
        public PatientModel PatientData { get; set; }
    }
}
