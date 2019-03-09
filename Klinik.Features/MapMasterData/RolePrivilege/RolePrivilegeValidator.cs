using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class RolePrivilegeValidator : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public RolePrivilegeValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
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