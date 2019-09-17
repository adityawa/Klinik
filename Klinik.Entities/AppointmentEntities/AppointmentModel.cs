
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;

namespace Klinik.Entities.AppointmentEntities
{
    public class AppointmentModel : BaseModel
    {
        public DateTime AppointmentDate { get; set; }
        public long PoliID { get; set; }
        public long EmployeeID { get; set; }
        public long ClinicID { get; set; }
        public int RequirementID { get; set; }
        public long DoctorID { get; set; }
        public long MCUPakageID { get; set; }
     

        public List<PoliModel> ListPoli { get; set; }

        public AppointmentModel()
        {
            ListPoli = new List<PoliModel>();
        }
    }
}
