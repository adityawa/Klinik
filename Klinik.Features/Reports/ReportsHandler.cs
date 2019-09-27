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

        public List<DiseaseReportDataModel> GetAllDiseaseData()
        {
            var diseasesData = new List<DiseaseReportDataModel>();

            using (var connection = new SqlConnection(ConnectionStrings.ReportConnectionString))
            {

                var command = new SqlCommand(Constants.ReportQueries.SQL_TOP_10_DISEASE_REPORT, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        diseasesData.Add(new DiseaseReportDataModel {
                            FormExamineId = Convert.ToInt32(reader[0]),
                            ICDId = Convert.ToInt32(reader[1]),
                            ClinicId = Convert.ToInt32(reader[2]),
                            ClinicName = Convert.ToString(reader[3]),
                            PatientName = Convert.ToString(reader[4]),
                            EmpName = Convert.ToString(reader[5]),
                            Department = Convert.ToString(reader[6]),
                            BusinessUnit = Convert.ToString(reader[7]),
                            Region = Convert.ToString(reader[8]),
                            StatusName = Convert.ToString(reader[9]),
                            BirthDate = Convert.ToDateTime(reader[10]),
                            Gender = Convert.ToString(reader[11]),
                            BPJSNumber = Convert.ToString(reader[12]),
                            Age = Convert.ToDecimal(reader[13]),
                            AgeCode = Convert.ToString(reader[14]),
                            FamCode = Convert.ToString(reader[15]),
                            FamName = Convert.ToString(reader[16]),
                            TransDate = Convert.ToDateTime(reader[17]),
                            NeedRest = Convert.ToString(reader[18]),
                            IsAccident = Convert.ToString(reader[19]),
                            Diagnose = Convert.ToString(reader[20]),
                            Necessity = Convert.ToString(reader[21]),
                            PaymentType = Convert.ToString(reader[22]),
                            ICDCode = Convert.ToString(reader[23])
                        });
                    }
                }
                catch(Exception ex) 
                {
                    connection.Close();
                    throw new Exception(ex.Message);
                }

                connection.Close();
            }

            return diseasesData;
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

            using (var connection = new SqlConnection())
            {
                try
                {
                    var diseasesData = GetAllDiseaseData(); //Get All

                    if (diseasesData.Count() > 0)
                    {
                        //Filter
                        if (request.Data.Year != 0) diseasesData.Where(x => x.TransDate.Year == request.Data.Year);
                        if (request.Data.Month != 0) diseasesData.Where(x => x.TransDate.Month == request.Data.Month);
                        if (request.Data.ClinicId != 0) diseasesData.Where(x => x.ClinicId == request.Data.ClinicId);
                        if (!string.IsNullOrEmpty(request.Data.DeptName) && request.Data.DeptName != "0")
                            diseasesData.Where(x => x.Department == request.Data.DeptName);
                        if (!string.IsNullOrEmpty(request.Data.BUName) && request.Data.BUName != "0")
                            diseasesData.Where(x => x.BusinessUnit == request.Data.BUName);
                        if (!string.IsNullOrEmpty(request.Data.GenderType) && request.Data.GenderType != "0")
                            diseasesData.Where(x => x.Gender == request.Data.GenderType);
                        if (!string.IsNullOrEmpty(request.Data.AgeCode) && request.Data.AgeCode != "0")
                            diseasesData.Where(x => x.AgeCode == request.Data.AgeCode);
                        if (!string.IsNullOrEmpty(request.Data.PatientCategory) && request.Data.PatientCategory != "0")
                            diseasesData.Where(x => x.StatusName == request.Data.PatientCategory);
                        if (!string.IsNullOrEmpty(request.Data.CategoryClinicStatus) && request.Data.CategoryClinicStatus != "0")
                            diseasesData.Where(x => x.Necessity == request.Data.CategoryClinicStatus);
                        if (!string.IsNullOrEmpty(request.Data.PaymentType) && request.Data.PaymentType != "0")
                            diseasesData.Where(x => x.PaymentType == request.Data.PaymentType);
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
