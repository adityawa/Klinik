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
    public class PurchaseOrderPusatValidator : BaseFeatures
    {
        private const string ADD_M_PURCHASEORDERPUSAT = "ADD_M_PURCHASEORDERPUSAT";
        private const string EDIT_M_PURCHASEORDERPUSAT = "EDIT_M_PURCHASEORDERPUSAT";
        private const string DELETE_M_PURCHASEORDERPUSAT = "DELETE_M_PURCHASEORDERPUSAT";

        public PurchaseOrderPusatValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PurchaseOrderPusatRequest request, out PurchaseOrderPusatResponse response)
        {
            response = new PurchaseOrderPusatResponse();

            if ((request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString())) || (request.Action != null && request.Action.Equals(ClinicEnums.Action.APPROVE.ToString())))
            {
                if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
                {
                    ValidateForDelete(request, out response);
                }
                else
                {
                    ValidateForApprove(request, out response);
                }
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.ponumber == null || String.IsNullOrWhiteSpace(request.Data.ponumber))
                {
                    errorFields.Add("Ponumber");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                if (request.Data.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_M_PURCHASEORDERPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEORDERPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new PurchaseOrderPusatHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(PurchaseOrderPusatRequest request, out PurchaseOrderPusatResponse response)
        {
            response = new PurchaseOrderPusatResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_M_PURCHASEORDERPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseOrderPusatHandler(_unitOfWork).RemoveData(request);
            }
        }

        private void ValidateForApprove(PurchaseOrderPusatRequest request, out PurchaseOrderPusatResponse response)
        {
            response = new PurchaseOrderPusatResponse();

            if (request.Action == ClinicEnums.Action.APPROVE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEORDERPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseOrderPusatHandler(_unitOfWork).ApproveData(request);
            }
        }
    }
}
