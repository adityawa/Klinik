using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class ReferalReportDataModel
    {
        public int ClinicId { get; set; }

        public string ClinicCode { get; set; }

        public string ClinicName { get; set; }

        public string Category { get; set; }

        public int Total { get; set; }
    }
}
