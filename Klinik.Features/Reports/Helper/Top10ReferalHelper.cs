using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Options;
using Klinik.Common;
using Klinik.Entities.Reports;
using Klinik.Features.Reports.ReportLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Reports.Helper
{

    public class Top10ReferalHelper : ReportHelperOptions<Top10ReferalLogParam, Top10ReferalChartModel>
    {
        public override Highcharts DrawChart(Top10ReferalChartModel chartParam)
        {
            var clinics = chartParam.ReportModel.ReferalReportDataModels.Select(x => x.ClinicName).Distinct().ToList();
            var categories = chartParam.ReportModel.ReferalReportDataModels.Select(x => x.Category).Distinct().ToList();

            var xnames = categories.ToList();
            var series = new List<Series>();

            foreach (var cat in categories)
            {
                var objects = new List<object>();
                foreach (var clinic in clinics)
                {
                    var result = chartParam.ReportModel.ReferalReportDataModels.FindAll(x => x.ClinicName == clinic && x.Category == cat);
                    if (result.Count > 0)
                    {
                        var total = 0;
                        foreach (var item in result)
                        {
                            total += item.Total;
                        }
                        objects.Add(total);
                    }
                    else
                    {
                        objects.Add(0);
                    }
                }
                series.Add(new Series
                {
                    Name = cat,
                    Data = new DotNet.Highcharts.Helpers.Data(objects.ToArray())
                });
            }

            Highcharts chart = new Highcharts(chartParam.ChartName)
                 .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
                 .SetTitle(new Title { Text = chartParam.ChartTitle })
                 .SetXAxis(new XAxis { Categories = clinics.ToArray() })

                 .SetYAxis(new YAxis
                 {
                     Title = new YAxisTitle { Text = chartParam.YAxisTitle },
                     Min = 0
                 })
                 .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.series.name +' : '+ this.y +' </b>'; }" })
                 .SetSeries(series.ToArray());

            return chart;
        }

        public override long GenerateExcel(Top10ReferalLogParam reportLogParam)
        {
            var handler = new ReportLogHandler(reportLogParam.UnitOfWork);
            var request = new ReportLogRequest();
            var excelReport = ExcelExportHelper.ExportExcel(reportLogParam.ReportModel.ReferalReportDataModels, reportLogParam.WorkSheetName, true, reportLogParam.Columns.ToArray());

            request.Data = new Entities.ReportLogModel
            {
                ExcelResult = excelReport,
                ChartResult = null,
                Account = reportLogParam.Account,
                CreatedDate = DateTime.Now,
                CreatedBy = reportLogParam.Account.UserName
            };

            return handler.CreateReportLog(request);
        }
    }
}
