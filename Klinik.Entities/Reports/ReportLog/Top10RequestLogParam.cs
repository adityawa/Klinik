using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class Top10RequestLogParam:BaseReportLogModel
    {
        public Top10RequestReportModel ReportModel { get; set; }
    }
}
