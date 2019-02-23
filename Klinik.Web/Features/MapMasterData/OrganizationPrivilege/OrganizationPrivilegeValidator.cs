using Klinik.Web.DataAccess;
using Klinik.Web.DataAccess.DataRepository;
using Klinik.Web.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Features.MapMasterData.OrganizationPrivilege
{
    public class OrganizationPrivilegeValidator : BaseFeatures
    {
        public OrganizationPrivilegeValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public void Validate(OrganizationPrivilegeRequest request, out OrganizationPrivilegeResponse response)
        {
            response = new OrganizationPrivilegeResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.RequestOrgPrivData.OrgID == 0 )
            {
                errorFields.Add("Organization");
            }
            if (request.RequestOrgPrivData.PrivilegeIDs.Count==0)
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
                response = new OrganizationPrivilegeHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }
    }
}