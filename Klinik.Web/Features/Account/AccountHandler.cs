using Klinik.Web.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.Infrastructure;
using Klinik.Web.Enumerations;
namespace Klinik.Web.Features.Account
{
    public class AccountHandler : BaseFeatures
    {
        public AccountHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string ClinicEnum { get; private set; }

        public AccountResponse AuthenticateUser(AccountRequest request)
        {
            AccountResponse response = new AccountResponse();
            var _getByUname = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.RequestAccountModel.UserName && x.Status==true && x.ExpiredDate>DateTime.Now);
            if (_getByUname != null)
            {
                var _decryptedPassword = Common.Decryptor(_getByUname.Password, Common.KeyEncryptor);
                if (_decryptedPassword == request.RequestAccountModel.Password)
                {
                    if (response.Entity == null)
                        response.Entity = new Models.Account.AccountModel();

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
                        response.Entity.Privileges = new Models.MappingMaster.RolePrivilegeModel();
                    
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