using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.Account;
using Klinik.Entities.MappingMaster;
using System;
using System.Collections.Generic;

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
        public AccountHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var _getByUname = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.RequestAccountModel.UserName && x.Status == true && x.ExpiredDate > DateTime.Now);
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
                }
                else
                {
                    response.Status = ClinicEnums.enumAuthResult.UNRECOGNIZED.ToString();
                    response.Message = "Password Incorrect";
                }
            }
            else
            {
                response.Status = ClinicEnums.enumAuthResult.UNRECOGNIZED.ToString();
                response.Message = "User Name or Password Incorrect";
            }

            return response;
        }
    }
}