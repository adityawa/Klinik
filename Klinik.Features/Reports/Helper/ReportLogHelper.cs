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
