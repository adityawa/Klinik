using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class PoliServiceValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_POLI_SERVICE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_POLI_SERVICE";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_POLI_SERVICE";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public PoliServiceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(PoliServiceRequest request, out PoliServiceResponse response)
        {
            response = new PoliServiceResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.ClinicID == null || request.Data.ClinicID <= 0)
                {
                    errorFields.Add("Clinic");
                }

                if (request.Data.PoliID == null || request.Data.PoliID <= 0)
                {
                    errorFields.Add("Poli");
                }

                if (request.Data.ServicesID == null || request.Data.ServicesID <= 0)
                {
                    errorFields.Add("Service");
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
                    response = new PoliServiceHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(PoliServiceRequest request, out PoliServiceResponse response)
        {
            response = new PoliServiceResponse();

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
                response = new PoliServiceHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}