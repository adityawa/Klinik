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
    public class DeliveryOrderPusatDetailValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_DELIVERYORDERPUSAT";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_DELIVERYORDERPUSAT";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_DELIVERYORDERPUSAT";

        public DeliveryOrderPusatDetailValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(DeliveryOrderPusatDetailRequest request, out DeliveryOrderPusatDetailResponse response)
        {
            response = new DeliveryOrderPusatDetailResponse();

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
                    response = new DeliveryOrderPusatDetailHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(DeliveryOrderPusatDetailRequest request, out DeliveryOrderPusatDetailResponse response)
        {
            response = new DeliveryOrderPusatDetailResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
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
