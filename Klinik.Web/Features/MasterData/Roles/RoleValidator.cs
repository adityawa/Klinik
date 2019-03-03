using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.Enumerations;

namespace Klinik.Web.Features.MasterData.Roles
{
    public class RoleValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_ROLE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_ROLE";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_ROLE";
        public RoleValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(RoleRequest request, out RoleResponse response)
        {
            bool isHavePrivilege = true;

            response = new RoleResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.action != null && request.action.Equals(ClinicEnums.enumAction.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                if (request.RequestRoleData.RoleName == null || String.IsNullOrWhiteSpace(request.RequestRoleData.RoleName))
                {
                    errorFields.Add("Role Name");
                }

                if (request.RequestRoleData.OrgID == 0)
                {
                    errorFields.Add("Organization");
                }

                if (errorFields.Any())
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
                }
                else if (request.RequestRoleData.RoleName.Length > 30)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Maximum Character for Role Name is 30";
                }

                if (request.RequestRoleData.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.RequestRoleData.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.RequestRoleData.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Unauthorized Access!";
                }

                if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
                    response = new RoleHandler(_unitOfWork).CreateOrEdit(request);
            }

           
        }

        private void ValidateForDelete(RoleRequest request, out RoleResponse response)
        {
            response = new RoleResponse();
            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();

            bool isHavePrivilege = true;

            if (request.action == ClinicEnums.enumAction.DELETE.ToString())
            {
                isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.RequestRoleData.Account.Privileges.PrivilegeIDs);
            }

            if (!isHavePrivilege)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Unauthorized Access!";
            }

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new RoleHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}