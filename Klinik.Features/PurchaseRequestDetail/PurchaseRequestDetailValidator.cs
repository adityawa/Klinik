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
    public class PurchaseRequestDetailValidator : BaseFeatures
    {
        private const string ADD_M_PURCHASEREQUEST = "ADD_M_PURCHASEREQUEST";
        private const string EDIT_M_PURCHASEREQUEST = "EDIT_M_PURCHASEREQUEST";
        private const string DELETE_M_PURCHASEREQUEST = "DELETE_M_PURCHASEREQUEST";

        public PurchaseRequestDetailValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PurchaseRequestDetailRequest request, out PurchaseRequestDetailResponse response)
        {
            response = new PurchaseRequestDetailResponse();

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

                    isHavePrivilege = IsHaveAuthorization(ADD_M_PURCHASEREQUEST, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEREQUEST, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new PurchaseRequestDetailHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(PurchaseRequestDetailRequest request, out PurchaseRequestDetailResponse response)
        {
            response = new PurchaseRequestDetailResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_M_PURCHASEREQUEST, request.Data.Account.Privileges.PrivilegeIDs);
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
