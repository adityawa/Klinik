using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Klinik.Data.DataRepository;
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


        public EmployeeValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public EmployeeResponse Validate(EmployeeRequest request, bool isFromApi = false)
        {
            bool isHavePrivilege = true;

            var response = new EmployeeResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                if (request.Data.EmpID == null || String.IsNullOrEmpty(request.Data.EmpID) || String.IsNullOrWhiteSpace(request.Data.EmpID))
                {
                    var cekEmpType = _unitOfWork.FamilyRelationshipRepository.GetById(request.Data.EmpType) == null ? "" : _unitOfWork.FamilyRelationshipRepository.GetById(request.Data.EmpType).Code;
                    if (cekEmpType.ToString().Trim() == "E")
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

                if (request.Data.StartDate == null)
                {
                    errorFields.Add("Join Date");
                }

                if (!String.IsNullOrEmpty(request.Data.Email))
                {
                    if (!Regex.IsMatch(request.Data.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}"))
                        errorFields.Add("Email");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                // skip the authorization for temporary
                if (!isFromApi)
                {
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
                        response.Message = Messages.UnauthorizedAccess;
                    }
                }

                if (response.Status)
                {
                    response = new EmployeeHandler(_unitOfWork, _context).CreateOrEdit(request);
                }
            }

            return response;
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
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new EmployeeHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}