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
    public class PurchaseOrderValidator : BaseFeatures
    {
        private const string ADD_M_PURCHASEORDER = "ADD_M_PURCHASEORDER";
        private const string EDIT_M_PURCHASEORDER = "EDIT_M_PURCHASEORDER";
        private const string DELETE_M_PURCHASEORDER = "DELETE_M_PURCHASEORDER";

        public PurchaseOrderValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PurchaseOrderRequest request, out PurchaseOrderResponse response)
        {
            response = new PurchaseOrderResponse();

            if ((request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString())) || (request.Action != null && request.Action.Equals(ClinicEnums.Action.APPROVE.ToString())) || (request.Action != null && request.Action.Equals(ClinicEnums.Action.VALIDASI.ToString())))
            {
                if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
                {
                    ValidateForDelete(request, out response);
                }
                else if (request.Action.Equals(ClinicEnums.Action.APPROVE.ToString()))
                {
                    ValidateForApprove(request, out response);
                }
                else
                {
                    ValidateForValidasi(request, out response);
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

                    isHavePrivilege = IsHaveAuthorization(ADD_M_PURCHASEORDER, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEORDER, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new PurchaseOrderHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(PurchaseOrderRequest request, out PurchaseOrderResponse response)
        {
            response = new PurchaseOrderResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_M_PURCHASEORDER, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseOrderHandler(_unitOfWork).RemoveData(request);
            }
        }

        private void ValidateForApprove(PurchaseOrderRequest request, out PurchaseOrderResponse response)
        {
            response = new PurchaseOrderResponse();

            if (request.Action == ClinicEnums.Action.APPROVE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEORDER, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseOrderHandler(_unitOfWork).ApproveData(request);
            }
        }

        private void ValidateForValidasi(PurchaseOrderRequest request, out PurchaseOrderResponse response)
        {
            response = new PurchaseOrderResponse();

            if (request.Action == ClinicEnums.Action.VALIDASI.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEORDER, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseOrderHandler(_unitOfWork).ValidasiData(request);
            }
        }
    }
}
