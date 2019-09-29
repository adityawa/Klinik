using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class Top10ReferalParamModel:BaseModel
    {
        public int YearStart { get; set; }

        public int MonthStart { get; set; }

        public int YearEnd { get; set; }

        public int MonthEnd { get; set; }

        public string HospitalDest { get; set; }

        public string Diagnose { get; set; }

        public string PatientName { get; set; }
    }
}
