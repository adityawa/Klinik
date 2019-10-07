using DotNet.Highcharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class Top10RequestReportModel:BaseReportModel
    {
        public List<ReferalReportDataModel> ReferalDataReports { get; set; }

        public List<Highcharts> Charts { get; set; }

        public string Category { get; set; }
    }
}
