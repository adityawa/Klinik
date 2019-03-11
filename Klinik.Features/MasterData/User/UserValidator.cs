using Klinik.Common;
using Klinik.Data;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class UserValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_USER";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_USER";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_USER";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public UserValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(UserRequest request, out UserResponse response)
        {
            bool isHavePrivilege = true;
            response = new UserResponse
            {
                Status = ClinicEnums.Status.SUCCESS.ToString()
            };

            if (request.action != null && request.action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
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
                    response.Status = ClinicEnums.Status.ERROR.ToString();
                    response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
                }
                else if (request.RequestUserData.Id == 0)
                {
                    //validate is username exist
                    var qry = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName.Equals(request.RequestUserData.UserName) && x.Status == true, includes: x => x.Employee);
                    if (qry != null)
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = $"User name already exist";
                    }
                }
                else if (request.RequestUserData.Id == 0)
                {
                    //validate is username exist
                    var qry = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName.Equals(request.RequestUserData.EmployeeID) && x.Status == true, includes: x => x.Employee);
                    if (qry != null)
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = $"one employee cannot have more than one user Id";
                    }
                }

                if (request.RequestUserData.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.RequestUserData.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.RequestUserData.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = ClinicEnums.Status.ERROR.ToString();
                    response.Message = $"Unauthorized Access!";
                }

                if (response.Status == ClinicEnums.Status.SUCCESS.ToString())
                {
                    response = new UserHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(UserRequest request, out UserResponse response)
        {
            response = new UserResponse();
            response.Status = ClinicEnums.Status.SUCCESS.ToString();

            bool isHavePrivilege = true;

            if (request.action == ClinicEnums.Action.DELETE.ToString())
            {
                isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.RequestUserData.Account.Privileges.PrivilegeIDs);
            }

            if (!isHavePrivilege)
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
                response.Message = $"Unauthorized Access!";
            }

            if (response.Status == ClinicEnums.Status.SUCCESS.ToString())
            {
                response = new UserHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}