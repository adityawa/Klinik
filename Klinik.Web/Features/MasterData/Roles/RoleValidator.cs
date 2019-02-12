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
        public RoleValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(RoleRequest request, out RoleResponse response)
        {
            response = new RoleResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.RequestRoleData.RoleName == null || String.IsNullOrWhiteSpace( request.RequestRoleData.RoleName))
            {
                errorFields.Add("Role Name");
            }

            if (request.RequestRoleData.OrgID == 0 )
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

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
                response = new RoleHandler(_unitOfWork).CreateOrEdit(request);
        }
    }
}