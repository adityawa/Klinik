using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class OrganizationPrivilegeValidator : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public OrganizationPrivilegeValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(OrganizationPrivilegeRequest request, out OrganizationPrivilegeResponse response)
        {
            response = new OrganizationPrivilegeResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.RequestOrgPrivData.OrgID == 0)
            {
                errorFields.Add("Organization");
            }

            if (request.RequestOrgPrivData.PrivilegeIDs.Count == 0)
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