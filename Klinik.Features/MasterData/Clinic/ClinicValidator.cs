using Klinik.Common;
using Klinik.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Klinik.Features.MasterData.Clinic
{
    public class ClinicValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_CLINIC";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_CLINIC";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_CLINIC";
        public ClinicValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(ClinicRequest request, out ClinicResponse response)
        {
            bool isHavePrivilege = true;

            response = new ClinicResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (request.action != null && request.action.Equals(ClinicEnums.enumAction.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                if (String.IsNullOrEmpty(request.RequestClinicModel.Code) || String.IsNullOrWhiteSpace(request.RequestClinicModel.Code) )
                {
                    errorFields.Add("Clinic Code");
                }

                if ( String.IsNullOrEmpty(request.RequestClinicModel.Name) || String.IsNullOrWhiteSpace(request.RequestClinicModel.Name))
                {
                    errorFields.Add("Clinic Name");
                }

               

                if (!String.IsNullOrEmpty(request.RequestClinicModel.Email))
                {
                    if (!Regex.IsMatch(request.RequestClinicModel.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}"))
                        errorFields.Add("Email");
                }

                if (errorFields.Any())
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
                }

                if (request.RequestClinicModel.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.RequestClinicModel.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.RequestClinicModel.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Unauthorized Access!";
                }

                if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
                {
                    response = new ClinicHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(ClinicRequest request, out ClinicResponse response)
        {
            response = new ClinicResponse();
            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();

            bool isHavePrivilege = true;

            if (request.action == ClinicEnums.enumAction.DELETE.ToString())
            {
                isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.RequestClinicModel.Account.Privileges.PrivilegeIDs);
            }

            if (!isHavePrivilege)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Unauthorized Access!";
            }

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new ClinicHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}
