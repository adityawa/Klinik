using DotNet.Highcharts;
using Klinik.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Reports.Helper
{
    public class Top10CostHelper : ReportHelperOptions<Top10CostLogParam, Top10CostChartModel>
    {
        public override Highcharts DrawChart(Top10CostChartModel chartParam)
        {
            throw new NotImplementedException();
        }

        public override long GenerateExcel(Top10CostLogParam reportLogParam)
        {
            throw new NotImplementedException();
        }
    }
}
