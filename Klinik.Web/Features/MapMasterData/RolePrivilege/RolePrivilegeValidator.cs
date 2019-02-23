using Klinik.Web.DataAccess;
using Klinik.Web.DataAccess.DataRepository;
using Klinik.Web.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Features.MapMasterData.RolePrivilege
{
    public class RolePrivilegeValidator: BaseFeatures
    {
        public RolePrivilegeValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public void Validate(RolePrivilegeRequest request, out RolePrivilegeResponse response)
        {
            response = new RolePrivilegeResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.RequestRolePrivData.RoleID == 0)
            {
                errorFields.Add("Role");
            }
            if (request.RequestRolePrivData.PrivilegeIDs.Count == 0)
            {
                errorFields.Add("Privileges");
            }

            if (errorFields.Any())
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
            }



            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new RolePrivilegeHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }
    }
}