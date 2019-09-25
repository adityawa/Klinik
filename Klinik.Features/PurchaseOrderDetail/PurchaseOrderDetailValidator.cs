using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class PurchaseOrderDetailValidator : BaseFeatures
    {
        private const string ADD_M_PURCHASEORDER = "ADD_M_PURCHASEORDER";
        private const string EDIT_M_PURCHASEORDER = "EDIT_M_PURCHASEORDER";
        private const string DELETE_M_PURCHASEORDER = "DELETE_M_PURCHASEORDER";
        public PurchaseOrderDetailValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PurchaseOrderDetailRequest request, out PurchaseOrderDetailResponse response)
        {
            response = new PurchaseOrderDetailResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.namabarang == null || String.IsNullOrWhiteSpace(request.Data.namabarang))
                {
                    errorFields.Add("namabarang");
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
                    response = new PurchaseOrderDetailHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(PurchaseOrderDetailRequest request, out PurchaseOrderDetailResponse response)
        {
            response = new PurchaseOrderDetailResponse();

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
                //response = new DeliveryOrderHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}
