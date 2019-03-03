using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.Enumerations;

namespace Klinik.Web.Features.MasterData.Privileges
{
    public class PrivilegeValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_PRIVILEGE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_PRIVILEGE";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_PRIVILEGE";

        public PrivilegeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PrivilegeRequest request, out PrivilegeResponse response)
        {
            bool isHavePrivilege = true;
            response = new PrivilegeResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.action != null && request.action.Equals(ClinicEnums.enumAction.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                if (request.RequestPrivilegeData.Privilige_Name == null || String.IsNullOrWhiteSpace(request.RequestPrivilegeData.Privilige_Name))
                {
                    errorFields.Add("Privilege Name");
                }

                if (errorFields.Any())
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
                }
                else if (request.RequestPrivilegeData.Privilige_Name.Length > 150)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Maximum Character for Privilege Name is 150";
                }

                if (request.RequestPrivilegeData.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.RequestPrivilegeData.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.RequestPrivilegeData.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Unauthorized Access!";
                }

                if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
                    response = new PrivilegeHandler(_unitOfWork).CreateOrEdit(request);
            }

           
        }

        private void ValidateForDelete(PrivilegeRequest request, out PrivilegeResponse response)
        {
            response = new PrivilegeResponse();
            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();

            bool isHavePrivilege = true;

            if (request.action == ClinicEnums.enumAction.DELETE.ToString())
            {
                isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.RequestPrivilegeData.Account.Privileges.PrivilegeIDs);
            }

            if (!isHavePrivilege)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Unauthorized Access!";
            }

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new PrivilegeHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}