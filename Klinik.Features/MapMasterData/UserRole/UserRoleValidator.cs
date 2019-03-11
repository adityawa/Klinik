using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class UserRoleValidator : BaseFeatures
    {
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public UserRoleValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(UserRoleRequest request, out UserRoleResponse response)
        {
            response = new UserRoleResponse
            {
                Status = ClinicEnums.Status.SUCCESS.ToString()
            };

            if (request.RequestUserRoleData.UserID == 0)
            {
                errorFields.Add("User Name");
            }
            if (request.RequestUserRoleData.RoleIds.Count == 0)
            {
                errorFields.Add("Role");
            }

            if (errorFields.Any())
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
                response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
            }

            if (response.Status == ClinicEnums.Status.SUCCESS.ToString())
            {
                response = new UserRoleHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }
    }
}