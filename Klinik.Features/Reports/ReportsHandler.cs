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

        public IQueryable<LookupCategory> GetLookUpByName(string name)
        {
            return _unitOfWork.LookUpCategoryRepository.Query(x => x.TypeName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IQueryable<ReferalReportDataModel> GetAllReferalData()
        {
            var referalsData = new List<ReferalReportDataModel>();

            using (var connection = new SqlConnection(ConnectionStrings.ReportConnectionString))
            {
                try
                {
                    connection.Open();
                    referalsData = connection.Query<ReferalReportDataModel>(Constants.ReportQueries.SQL_TOP_10_REFERAL_REPORT).ToList();
                }
                catch(Exception ex)
                {
                    connection.Close();
                    throw new Exception(ex.Message);
                }
            }

            return referalsData.AsQueryable();
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

            try
            {
                var referalsData = GetAllReferalData();

                if (referalsData.Count() > 0)
                {
                    //if (request.Data.Year != 0) referalsData.Where(x => x.Year == request.Data.Year);
                    //if (request.Data.Month != 0) referalsData.Where(x => x.Month == request.Data.Month);
                    //if (!string.IsNullOrEmpty(request.Data.HospitalDest)) referalsData.Where(x => x.OtherInfo == request.Data.HospitalDest);
                    //if (!string.IsNullOrEmpty(request.Data.Diagnose)) referalsData.Where(x => x.Diagnose == request.Data.Diagnose);
                    //if (!string.IsNullOrEmpty(request.Data.PatientName)) referalsData.Where(x => x.PatientName == request.Data.PatientName);

                    var result = new Top10ReferalReportModel();
                    result.ReferalReportsData = referalsData.ToList();
                    result.ReportHeader = Resources.UIMessages.Top10ReferalReport;
                    result.TotalRecord = referalsData.Count();
                    response.Entity = result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return response;
        }




    }
}
