using Dapper;
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
            return _unitOfWork.LookUpCategoryRepository.Query(x => x.LookUpName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IQueryable<DiseaseReportDataModel> GetAllDiseaseData()
        {
            var diseasesData = new List<DiseaseReportDataModel>();

            using (var connection = new SqlConnection(ConnectionStrings.ReportConnectionString))
            {
                try
                {
                    connection.Open();
                    diseasesData = connection.Query<DiseaseReportDataModel>(Constants.ReportQueries.SQL_TOP_10_DISEASE_REPORT).ToList();
                }
                catch 
                {
                    connection.Close();
                }
            }

            return diseasesData.AsQueryable();
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
                catch
                {
                    connection.Close();
                }
            }

            return referalsData.AsQueryable();
        }
        
        public Top10DiseaseReportResponse GenerateTop10DiseasesReport(Top10DiseaseReportRequest request)
        {
            var response = new Top10DiseaseReportResponse();
            try
            {
                var diseasesData = GetAllDiseaseData(); //Get All

                if (diseasesData.Count() > 0)
                {
                    //Filter
                    if (request.Data.Year != 0) diseasesData.Where(x => x.TransDate.Year == request.Data.Year);
                    if (request.Data.Month != 0) diseasesData.Where(x => x.TransDate.Month == request.Data.Month);
                    if (request.Data.ClinicId != 0) diseasesData.Where(x => x.ClinicId == request.Data.ClinicId);
                    if (!string.IsNullOrEmpty(request.Data.DeptName)) diseasesData.Where(x => x.Department == request.Data.DeptName);
                    if (!string.IsNullOrEmpty(request.Data.BUName)) diseasesData.Where(x => x.BusinessUnit == request.Data.BUName);
                    if (!string.IsNullOrEmpty(request.Data.GenderType)) diseasesData.Where(x => x.Gender == request.Data.GenderType);
                    if (!string.IsNullOrEmpty(request.Data.AgeCode)) diseasesData.Where(x => x.AgeCode == request.Data.AgeCode);
                    if (!string.IsNullOrEmpty(request.Data.PatientCategory)) diseasesData.Where(x => x.StatusName == request.Data.PatientCategory);
                    if (!string.IsNullOrEmpty(request.Data.CategoryClinicStatus)) diseasesData.Where(x => x.Necessity == request.Data.CategoryClinicStatus);
                    if (!string.IsNullOrEmpty(request.Data.PaymentType)) diseasesData.Where(x => x.PaymentType == request.Data.PaymentType);
                    if (!string.IsNullOrEmpty(request.Data.NeedRest)) diseasesData.Where(x => x.NeedRest == request.Data.NeedRest);
                    if (!string.IsNullOrEmpty(request.Data.ExamineType)) diseasesData.Where(x => x.IsAccident == request.Data.ExamineType);

                    var result = new Top10DiseaseReportModel();
                    result.DiseaseDataReports = diseasesData.ToList();
                    result.ReportHeader = Resources.UIMessages.Top10DiseasesReport;
                    result.TotalRecord = diseasesData.Count();
                    response.Entity = result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                    if (request.Data.Year != 0) referalsData.Where(x => x.Year == request.Data.Year);
                    if (request.Data.Month != 0) referalsData.Where(x => x.Month == request.Data.Month);
                    if (!string.IsNullOrEmpty(request.Data.HospitalDest)) referalsData.Where(x => x.OtherInfo == request.Data.HospitalDest);
                    if (!string.IsNullOrEmpty(request.Data.Diagnose)) referalsData.Where(x => x.Diagnose == request.Data.Diagnose);
                    if (!string.IsNullOrEmpty(request.Data.PatientName)) referalsData.Where(x => x.PatientName == request.Data.PatientName);

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
