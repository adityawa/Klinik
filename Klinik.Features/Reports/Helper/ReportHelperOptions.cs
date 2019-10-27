using DotNet.Highcharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Reports.Helper
{
    public abstract class ReportHelperOptions<TLogParam, TChartParam>
    {
        public abstract long GenerateExcel(TLogParam reportLogParam);
        public abstract Highcharts DrawChart(TChartParam chartParam);
    }

}
