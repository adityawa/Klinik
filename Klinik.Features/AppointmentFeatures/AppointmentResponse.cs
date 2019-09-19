using Klinik.Entities;
using Klinik.Entities.AppointmentEntities;
using Klinik.Entities.MasterData;
using Klinik.Entities.PoliSchedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.AppointmentFeatures
{
    public class AppointmentResponse : BaseResponse<AppointmentModel>
    {
        public List<PoliScheduleModel> schedules { get; set; }
    }
}
