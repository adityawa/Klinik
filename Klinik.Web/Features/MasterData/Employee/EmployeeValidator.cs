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
        private const string ADD_PRIVILEGE_NAME = "ADD_M_EMPLOYEE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_EMPLOYEE";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_EMPLOYEE";

        public EmployeeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(EmployeeRequest request, out EmployeeResponse response)
        {
            bool isHavePrivilege = true;

            response = new EmployeeResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.action != null && request.action.Equals(ClinicEnums.enumAction.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                if (request.RequestEmployeeData.EmpID == null || String.IsNullOrEmpty(request.RequestEmployeeData.EmpID) || String.IsNullOrWhiteSpace(request.RequestEmployeeData.EmpID))
                {
                    errorFields.Add("Employee ID");
                }

                if (request.RequestEmployeeData.EmpName == null || String.IsNullOrEmpty(request.RequestEmployeeData.EmpName) || String.IsNullOrWhiteSpace(request.RequestEmployeeData.EmpName))
                {
                    errorFields.Add("Employee Name");
                }

                if (request.RequestEmployeeData.Birthdate == null)
                {
                    errorFields.Add("Birhdate");
                }

                if (!String.IsNullOrEmpty(request.RequestEmployeeData.Email))
                {
                    if (!Regex.IsMatch(request.RequestEmployeeData.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}"))
                        errorFields.Add("Email");
                }

                if (errorFields.Any())
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
                }

                if (request.RequestEmployeeData.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.RequestEmployeeData.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.RequestEmployeeData.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Unauthorized Access!";
                }

                if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
                {
                    response = new EmployeeHandler(_unitOfWork).CreateOrEdit(request);
                }
            }

           
        }

        private void ValidateForDelete(EmployeeRequest request, out EmployeeResponse response)
        {
            response = new EmployeeResponse();
            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();

            bool isHavePrivilege = true;

            if (request.action == ClinicEnums.enumAction.DELETE.ToString())
            {
                isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.RequestEmployeeData.Account.Privileges.PrivilegeIDs);
            }

            if (!isHavePrivilege)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Unauthorized Access!";
            }

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new EmployeeHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}