using Dapper;
using DapperExtensions;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Reports;
using Klinik.Features.Reports;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Klinik.Features
{
    public class ReportsHandler:BaseFeatures
    {
        public ReportsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Top10DiseaseReportResponse GenerateTop10DiseasesReport(Top10DiseaseReportRequest request)
        {
            var response = new Top10DiseaseReportResponse();

            using (var connection = new SqlConnection(ConnectionStrings.ReportConnectionString))
            {
                try
                {
                    var sql = "exec dbo.sp_generateTop10DiseaseReport @monthStart, @yearStart, @monthEnd, @yearEnd, @clinicId, @category, @categoryItem";

                    var paramValues = new {
                                            monthStart = request.Data.MonthStart,
                                            yearStart = request.Data.YearStart,
                                            monthEnd = request.Data.MonthEnd,
                                            yearEnd = request.Data.YearEnd,
                                            clinicId = request.Data.ClinicId,
                                            category = request.Data.SelectedCategory,
                                            categoryItem = request.Data.SelectedCategoryItem
                                        };

                    connection.Open();

                    var diseasesData = connection.Query<DiseaseReportDataModel>(sql, paramValues).ToList();

                    var result = new Top10DiseaseReportModel();
                    result.DiseaseDataReports = diseasesData.ToList();
                    result.Category = request.Data.SelectedCategory;
                    result.ReportHeader = Resources.UIMessages.Top10DiseasesReport;
                    result.TotalRecord = diseasesData.Count();
                    response.Entity = result;

                }
                catch (Exception ex)
                {
                    connection.Close();
                    throw new Exception(ex.Message);
                }
            }

            return response;
        }


        public Top10ReferalReportResponse GenerateTop10ReferalReport(Top10ReferalReportRequest request)
        {
            var response = new Top10ReferalReportResponse();

            using (var connection = new SqlConnection(ConnectionStrings.ReportConnectionString))
            {
                try
                {
                    var sql = "exec dbo.sp_generateTop10ReferalReport @monthStart, @yearStart, @monthEnd, @yearEnd, @category, @categoryItem";

                    var paramValues = new
                    {
                        monthStart = request.Data.MonthStart,
                        yearStart = request.Data.YearStart,
                        monthEnd = request.Data.MonthEnd,
                        yearEnd = request.Data.YearEnd,
                        category = request.Data.SelectedCategory,
                        categoryItem = request.Data.SelectedCategoryItem
                    };

                    connection.Open();

                    var referalsData = connection.Query<ReferalReportDataModel>(sql, paramValues).ToList();

                    var result = new Top10ReferalReportModel();
                    result.ReferalReportDataModels = referalsData.ToList();
                    result.Category = request.Data.SelectedCategory;
                    result.ReportHeader = Resources.UIMessages.Top10DiseasesReport;
                    result.TotalRecord = referalsData.Count();
                    response.Entity = result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return response;
        }




    }
}
