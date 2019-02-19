using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.Models;
using Klinik.Web.Models.MasterData;
using Klinik.Web.DataAccess;
using Klinik.Web.Features.MasterData.User;
using Klinik.Web.Enumerations;
using System.Text.RegularExpressions;

namespace Klinik.Web.Features.MasterData.Employee
{
    public class EmployeeValidator : BaseFeatures
    {
        public EmployeeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(EmployeeRequest request, out EmployeeResponse response)
        {
            response = new EmployeeResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.RequestEmployeeData.EmpID == null || String.IsNullOrEmpty(request.RequestEmployeeData.EmpID) || String.IsNullOrWhiteSpace(request.RequestEmployeeData.EmpID))
            {
                errorFields.Add("Employee ID");
            }

            if (request.RequestEmployeeData.EmpName == null || String.IsNullOrEmpty(request.RequestEmployeeData.EmpName) || String.IsNullOrWhiteSpace(request.RequestEmployeeData.EmpName))
            {
                errorFields.Add("Employee Name");
            }

            if (request.RequestEmployeeData.Birthdate == null )
            {
                errorFields.Add("Birhdate");
            }

            if(!String.IsNullOrEmpty(request.RequestEmployeeData.Email))
            {
                if (!Regex.IsMatch(request.RequestEmployeeData.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}"))
                    errorFields.Add("Email");
            }

            if (errorFields.Any())
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
            }
           

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new EmployeeHandler(_unitOfWork).CreateOrEdit(request);
            }
        }
    }
}