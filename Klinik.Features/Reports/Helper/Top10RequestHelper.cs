using DotNet.Highcharts;
using Klinik.Entities.Reports;
using System;

namespace Klinik.Features.Reports.Helper
{
    public class Top10RequestHelper : ReportHelperOptions<Top10RequestLogParam, Top10RequestChartModel>
    {
        public override Highcharts DrawChart(Top10RequestChartModel chartParam)
        {
            throw new NotImplementedException();
        }

        public override long GenerateExcel(Top10RequestLogParam reportLogParam)
        {
            throw new NotImplementedException();
        }
    }
}
