using Klinik.Entities;
using Klinik.Entities.MasterData;
using Klinik.Entities.MedicalHistoryEntity;
using System.Collections.Generic;

namespace Klinik.Features.HistoryMedical
{
    public class MedicalHistoryResponse : BaseResponse<MedicalHistoryModel>
    {
        public List<EmployeeModel> Employees { get; set; }
        public List<MedicalHistoryModel> MedicalHistories { get; set; }

    }
}
