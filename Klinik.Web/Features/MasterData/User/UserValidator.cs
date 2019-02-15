using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.Models;
using Klinik.Web.Models.MasterData;
using Klinik.Web.DataAccess;
using Klinik.Web.Features.MasterData.User;
using Klinik.Web.Enumerations;
namespace Klinik.Web.Features.MasterData.User
{
    public class UserValidator : BaseFeatures
    {
        public UserValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(UserRequest request, out UserResponse response)
        {
            response = new UserResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.RequestUserData.OrgID == 0)
            {
                errorFields.Add("Organization");
            }

            if (String.IsNullOrEmpty(request.RequestUserData.UserName) || String.IsNullOrWhiteSpace(request.RequestUserData.UserName))
            {
                errorFields.Add("UserName");
            }

            if (String.IsNullOrEmpty(request.RequestUserData.Password) || String.IsNullOrWhiteSpace(request.RequestUserData.Password))
            {
                errorFields.Add("Password");
            }

            if (request.RequestUserData.EmployeeID == 0)
            {
                errorFields.Add("Employee");
            }

            if (errorFields.Any())
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
            }
            else if (request.RequestUserData.Id == 0)
            {
                //validate is username exist
                var qry = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName.Equals(request.RequestUserData.UserName) && x.Status==true, includes:x=>x.Employee);
                if(qry!=null)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"User name already exist";
                }
            }
            else if (request.RequestUserData.Id == 0)
            {
                //validate is username exist
                var qry = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName.Equals(request.RequestUserData.EmployeeID) && x.Status == true, includes: x => x.Employee);
                if (qry != null)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"one employee cannot have more than one user Id";
                }
            }

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new UserHandler(_unitOfWork).CreateOrEdit(request);
            }
        }
    }
}