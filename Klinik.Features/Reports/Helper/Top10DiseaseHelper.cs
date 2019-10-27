using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Options;
using Klinik.Common;
using Klinik.Entities.Reports;
using Klinik.Features.Reports.ReportLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features.Reports.Helper
{
    public class Top10DiseaseHelper : ReportHelperOptions<Top10DiseaseLogParam, Top10DiseaseChartModel>
    {
        public override Highcharts DrawChart(Top10DiseaseChartModel chartParam)
        {
            var icds = chartParam.ReportModel.DiseaseDataReports.Select(x => x.ICDCode).Distinct().ToList();
            var categories = chartParam.ReportModel.DiseaseDataReports.Select(x => x.Category).Distinct().ToList();
            var xnames = categories.ToList();

            var series = new List<Series>();


            foreach (var cat in categories)
            {
                var objects = new List<object>();
                foreach (var icd in icds)
                {
                    var result = chartParam.ReportModel.DiseaseDataReports.FindAll(x => x.ICDCode == icd && x.Category == cat);
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

            Highcharts chart = new Highcharts("chart_by_category")
                 .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
                 .SetTitle(new Title { Text = "Total Pasien Berdasarkan tipe ICD" })
                 .SetXAxis(new XAxis { Categories = icds.ToArray() })

                 .SetYAxis(new YAxis
                 {
                     Title = new YAxisTitle { Text = "Total Pasien" },
                     Min = 0
                 })
                 .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.series.name +' : '+ this.y +' </b>'; }" })
                 .SetSeries(series.ToArray());

            return chart;

        }

        public override long GenerateExcel(Top10DiseaseLogParam reportLogParam)
        {
            var handler = new ReportLogHandler(reportLogParam.UnitOfWork);
            var request = new ReportLogRequest();
            var excelReport = ExcelExportHelper.ExportExcel(reportLogParam.ReportModel.DiseaseDataReports, reportLogParam.WorkSheetName, true, reportLogParam.Columns.ToArray());

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
