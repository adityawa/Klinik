using Klinik.Web.DataAccess;
using Klinik.Web.DataAccess.DataRepository;
using Klinik.Web.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Features.MapMasterData.UserRole
{
    public class UserRoleValidator:BaseFeatures
    {
        public UserRoleValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public void Validate(UserRoleRequest request, out UserRoleResponse response)
        {
            response = new UserRoleResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
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
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
            }



            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new UserRoleHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }
    }
}