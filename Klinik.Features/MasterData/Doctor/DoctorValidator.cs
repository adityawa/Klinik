using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Klinik.Features
{
    public class DoctorValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_DOCTOR";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_DOCTOR";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_DOCTOR";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public DoctorValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(DoctorRequest request, out DoctorResponse response)
        {
            response = new DoctorResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (String.IsNullOrEmpty(request.Data.Code) || String.IsNullOrWhiteSpace(request.Data.Code))
                {
                    errorFields.Add("Doctor Code");
                }

                if (String.IsNullOrEmpty(request.Data.Name) || String.IsNullOrWhiteSpace(request.Data.Name))
                {
                    errorFields.Add("Doctor Name");
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

                if (response.Status)
                {
                    response = new DoctorHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(DoctorRequest request, out DoctorResponse response)
        {
            response = new DoctorResponse();

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
                response = new DoctorHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}
