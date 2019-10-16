using DotNet.Highcharts;
using Klinik.Entities.Reports.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class Top10CostReportModel:BaseReportModel
    {
        public List<CostReportDataModel> CostReportData { get; set; }

        public List<Highcharts> Charts { get; set; }

        public string Category { get; set; }
    }
}
