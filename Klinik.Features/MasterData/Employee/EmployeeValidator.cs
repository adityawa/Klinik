using Klinik.Common;
using Klinik.Data;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Klinik.Features
{
    public class EmployeeValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_EMPLOYEE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_EMPLOYEE";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_EMPLOYEE";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public EmployeeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(EmployeeRequest request, out EmployeeResponse response)
        {
            bool isHavePrivilege = true;

            response = new EmployeeResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                if (request.Data.EmpID == null || String.IsNullOrEmpty(request.Data.EmpID) || String.IsNullOrWhiteSpace(request.Data.EmpID))
                {
                    errorFields.Add("Employee ID");
                }

                if (request.Data.EmpName == null || String.IsNullOrEmpty(request.Data.EmpName) || String.IsNullOrWhiteSpace(request.Data.EmpName))
                {
                    errorFields.Add("Employee Name");
                }

                if (request.Data.Birthdate == null)
                {
                    errorFields.Add("Birhdate");
                }

                if (!String.IsNullOrEmpty(request.Data.Email))
                {
                    if (!Regex.IsMatch(request.Data.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}"))
                        errorFields.Add("Email");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
                }

                if (request.Data.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = $"Unauthorized Access!";
                }

                if (response.Status)
                {
                    response = new EmployeeHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(EmployeeRequest request, out EmployeeResponse response)
        {
            response = new EmployeeResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = $"Unauthorized Access!";
                }
            }

            if (response.Status)
            {
                response = new EmployeeHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}