using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class ReferalReportDataModel
    {
        public int LetterId { get; set; }

        public int ClinicId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string ClinicName { get; set; }

        public string LetterType { get; set; }

        public string Keperluan { get; set; }

        public int AutoNumber { get; set; }

        public string PatientName { get; set; }

        public string OtherInfo { get; set; }

        public string ICDCode { get; set; }

        public string Diagnose { get; set; }
    }
}
