using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;

namespace Klinik.Features.MasterData.GeneralMaster
{
    public class MasterValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_GENERAL_MASTER";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_GENERAL_MASTER";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_GENERAL_MASTER";

        public MasterValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(MasterRequest request, out MasterResponse response)
        {
            response = new MasterResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.Name == null || String.IsNullOrWhiteSpace(request.Data.Name))
                {
                    errorFields.Add("Master Name");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                if (request.Data.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new MasterHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        public void ValidateForDelete(MasterRequest request, out MasterResponse response)
        {
            response = new MasterResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Resources.Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new MasterHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}
