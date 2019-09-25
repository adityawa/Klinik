using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseRequestConfigValidator : BaseFeatures
    {
        private const string ADD_M_PURCHASEREQUESTCONFIG = "ADD_M_PURCHASEREQUESTCONFIG";
        private const string EDIT_M_PURCHASEREQUESTCONFIG = "EDIT_M_PURCHASEREQUESTCONFIG";
        private const string DELETE_M_PURCHASEREQUESTCONFIG = "DELETE_M_PURCHASEREQUESTCONFIG";

        public PurchaseRequestConfigValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PurchaseRequestConfigRequest request, out PurchaseRequestConfigResponse response)
        {
            response = new PurchaseRequestConfigResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
                {
                    ValidateForDelete(request, out response);
                }
            }
            else
            {
                bool isHavePrivilege = true;

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                if (request.Data.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_M_PURCHASEREQUESTCONFIG, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEREQUESTCONFIG, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new PurchaseRequestConfigHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(PurchaseRequestConfigRequest request, out PurchaseRequestConfigResponse response)
        {
            response = new PurchaseRequestConfigResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_M_PURCHASEREQUESTCONFIG, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseRequestConfigHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}
