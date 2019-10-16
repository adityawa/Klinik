using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Options;
using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.Account;
using Klinik.Entities.Reports;
using Klinik.Features.Reports.Helper;
using Klinik.Features.Reports.ReportLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public abstract class ReportHelperOptions<TLogParam, TChartParam>
    { 
        public abstract long GenerateExcel(TLogParam reportLogParam);
        public abstract Highcharts DrawChart(TChartParam chartParam);
    }

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
                 .SetTitle(new Title { Text = chartParam.ChartTitle})
                 .SetXAxis(new XAxis { Categories = clinics.ToArray() })

                 .SetYAxis(new YAxis
                 {
                     Title = new YAxisTitle { Text =  chartParam.YAxisTitle },
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

    public static class ReportLogHelper
    {
        public static long  GenerateExcel(ClinicEnums.ReportType type, object obj, IUnitOfWork unitOfWork, AccountModel accountModel)
        {
            long result = 0;
            switch (type)
            {
                case ClinicEnums.ReportType.Top10DiseaseReport:
                    var diseasehelper = new Top10DiseaseHelper();
                    var diseaseColumns = new List<string> { "ICDId", "ICDCode", "ICDName", "Category", "Total" };
                    result = diseasehelper.GenerateExcel(new Top10DiseaseLogParam
                                                     {
                                                        ReportModel = (Top10DiseaseReportModel)obj,
                                                        UnitOfWork = unitOfWork,
                                                        Columns = diseaseColumns,
                                                        WorkSheetName = "Top10Disease",
                                                        Account = accountModel
                                                     }
                                                 );
                    break;
                case ClinicEnums.ReportType.Top10ReferalReport:
                    var referalhelper = new Top10ReferalHelper();
                    var referalColumns = new List<string> { "ClinicId", "ClinicName", "LetterType", "Category", "Total" };
                    result = referalhelper.GenerateExcel(new Top10ReferalLogParam
                                                        {
                                                            ReportModel = (Top10ReferalReportModel)obj,
                                                            UnitOfWork = unitOfWork,
                                                            Columns = referalColumns,
                                                            WorkSheetName = "Top10Referal",
                                                            Account = accountModel
                                                        });
                    break;
            }

            return result;
        }

        public static Highcharts GenerateChart(ClinicEnums.ReportType type, object obj)
        {
            Highcharts chart = null;
            switch (type)
            {
                case ClinicEnums.ReportType.Top10DiseaseReport:
                    var diseasehelper = new Top10DiseaseHelper();
                    
                    chart = diseasehelper.DrawChart(new Top10DiseaseChartModel
                                                    {
                                                        ReportModel = (Top10DiseaseReportModel)obj,
                                                        ChartTitle = "Total Pasien Berdasarkan tipe ICD",
                                                        YAxisTitle = "Total Perawatan",
                                                        ChartName = "chart_by_category"
                                                     });
                    break;
                case ClinicEnums.ReportType.Top10ReferalReport:
                    var referalhelper = new Top10ReferalHelper();
                    chart = referalhelper.DrawChart(new Top10ReferalChartModel {
                                                        ReportModel = (Top10ReferalReportModel)obj,
                                                        ChartTitle = "Total Pasien Berdasarkan tipe Clinic",
                                                        YAxisTitle = "Total Perawatan",
                                                        ChartName = "chart_by_category"
                                                     });
                    break;
            }

            return chart;
        }
    }
}
