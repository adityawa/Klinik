using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MappingMaster;
using System;
using System.Linq;
using System.Collections.Generic;
using Klinik.Entities.Administration;

namespace Klinik.Features
{
    /// <summary>
    /// Account handler class
    /// </summary>
    public class AccountHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public AccountHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Clinic enum
        /// </summary>
        public string ClinicEnum { get; private set; }

        /// <summary>
        /// Authenticate user based on request values
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountResponse AuthenticateUser(AccountRequest request)
        {
            AccountResponse response = new AccountResponse();
            long _orgId = 0;
            //get Org ID
            var _getOrganization = _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == request.RequestAccountModel.Organization);
            if (_getOrganization != null)
                _orgId = _getOrganization.ID;
            var _getByUname = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.RequestAccountModel.UserName && x.Status == true && x.ExpiredDate > DateTime.Now && x.OrganizationID==_orgId);
            if (_getByUname != null)
            {
                var _decryptedPassword = CommonUtils.Decryptor(_getByUname.Password, CommonUtils.KeyEncryptor);
                if (_decryptedPassword == request.RequestAccountModel.Password)
                {
                    if (response.Entity == null)
                        response.Entity = new AccountModel();

                    response.Entity.UserName = _getByUname.UserName;
                    response.Entity.UserID = _getByUname.ID;
                    response.Entity.EmployeeID = _getByUname.EmployeeID;

                    var _getRoles = _unitOfWork.UserRoleRepository.Get(x => x.UserID == response.Entity.UserID);

                    response.Entity.Roles = new List<long>();
                    foreach (var role in _getRoles)
                    {
                        response.Entity.Roles.Add(role.RoleID);
                    }

                    var _getRolePrivileges = _unitOfWork.RolePrivRepository.Get(x => response.Entity.Roles.Contains(x.RoleID));
                    if (response.Entity.Privileges == null)
                        response.Entity.Privileges = new RolePrivilegeModel();

                    foreach (var rp in _getRolePrivileges)
                    {
                        if (response.Entity.Privileges.PrivilegeIDs == null)
                            response.Entity.Privileges.PrivilegeIDs = new List<long>();
                        response.Entity.Privileges.PrivilegeIDs.Add(rp.PrivilegeID);
                    }

                    response.Status = ClinicEnums.enumAuthResult.SUCCESS.ToString();
                    response.Message = "";

                    var logging = new LogModel
                    {
                        Start = DateTime.Now,
                        Module = ClinicEnums.enumModule.LOGIN.ToString(),
                        UserName = response.Entity.UserName,
                        Organization=request.RequestAccountModel.Organization,
                        Command = "Login To System",
                        Status = ClinicEnums.enumStatus.SUCCESS.ToString()
                    };

                    CommandLogging(logging);
                }
                else
                {
                    response.Status = ClinicEnums.enumAuthResult.UNRECOGNIZED.ToString();
                    response.Message = "Password Incorrect";
                    var logging = new LogModel
                    {
                        Start = DateTime.Now,
                        Module = ClinicEnums.enumModule.LOGIN.ToString(),
                        UserName = response.Entity.UserName,
                        Organization = request.RequestAccountModel.Organization,
                        Command = "Login To System",
                        Status = ClinicEnums.enumAuthResult.UNRECOGNIZED.ToString()
                    };

                    CommandLogging(logging);
                }
            }
            else
            {
                response.Status = ClinicEnums.enumAuthResult.UNRECOGNIZED.ToString();
                response.Message = "User Name or Password Incorrect";
                var logging = new LogModel
                {
                    Start = DateTime.Now,
                    Module = ClinicEnums.enumModule.LOGIN.ToString(),
                    UserName = response.Entity.UserName,
                    Organization = request.RequestAccountModel.Organization,
                    Command = "Login To System",
                    Status = ClinicEnums.enumAuthResult.UNRECOGNIZED.ToString()
                };

                CommandLogging(logging);
            }

            return response;
        }

        /// <summary>
        /// Set user reset password code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountResponse SetResetPasswordCode(AccountRequest request)
        {
            // init response
            AccountResponse response = new AccountResponse();

            // get employee by its email
            var employee = _unitOfWork.EmployeeRepository.GetFirstOrDefault(x => x.Email == request.RequestAccountModel.Email);
            if (employee != null)
            {
                try
                {
                    // get user based on employee ID
                    var user = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.EmployeeID == employee.id);

                    // set reset password code
                    user.ResetPasswordCode = request.RequestAccountModel.ResetPasswordCode;

                    // update user                    
                    _unitOfWork.UserRepository.Update(user);

                    // save it
                    int resultAffected = _unitOfWork.Save();

                    // update response
                    response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                    response.Message = "Data Successfully Updated";
                }
                catch (Exception ex)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = CommonUtils.GetGeneralErrorMesg();
                }
            }
            else
            {
                response.Status = ClinicEnums.enumAuthResult.UNRECOGNIZED.ToString();
                response.Message = "User Name or Password Incorrect";
            }

            return response;
        }

        /// <summary>
        /// Validate user reset password code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountResponse ValidateResetPasswordCode(AccountRequest request)
        {
            // init response
            AccountResponse response = new AccountResponse();

            // check if user with passed reset password code exist
            var user = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.ResetPasswordCode == request.RequestAccountModel.ResetPasswordCode);
            if (user != null)
            {
                // update response
                response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
            }
            else
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg();
            }

            return response;
        }

        /// <summary>
        /// Update user password 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccountResponse UpdateUserPassword(AccountRequest request)
        {
            // init response
            AccountResponse response = new AccountResponse();

            // get user by its reset password code
            var user = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.ResetPasswordCode == request.RequestAccountModel.ResetPasswordCode);
            if (user != null)
            {
                try
                {
                    // update password
                    user.Password = CommonUtils.Encryptor(request.RequestAccountModel.Password, CommonUtils.KeyEncryptor);

                    // clear the resert password code
                    user.ResetPasswordCode = null;

                    // update user
                    _unitOfWork.UserRepository.Update(user);

                    // save it
                    int resultAffected = _unitOfWork.Save();

                    // update response
                    response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                    response.Message = "User Password Successfully Updated";
                }
                catch
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = CommonUtils.GetGeneralErrorMesg();
                }
            }
            else
            {
                response.Status = ClinicEnums.enumAuthResult.UNRECOGNIZED.ToString();
                response.Message = "Reset Code Incorrect";
            }

            return response;
        }
    }
}