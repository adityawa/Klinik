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
    public class DeliveryOrderValidator : BaseFeatures
    {
        private const string ADD_M_DELIVERYORDER = "ADD_M_DELIVERYORDER";
        private const string EDIT_M_DELIVERYORDER = "EDIT_M_DELIVERYORDER";
        private const string DELETE_M_DELIVERYORDER = "DELETE_M_DELIVERYORDER";
        private const string APPROVE_M_DELIVERYORDER = "APPROVE_M_DELIVERYORDER";
        private const string RECIVED_M_DELIVERYORDER = "RECIVED_M_DELIVERYORDER";

        public DeliveryOrderValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(DeliveryOrderRequest request, out DeliveryOrderResponse response)
        {
            response = new DeliveryOrderResponse();

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

                if (request.Data.donumber == null || String.IsNullOrWhiteSpace(request.Data.donumber))
                {
                    errorFields.Add("Gudang Name");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                if (request.Data.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_M_DELIVERYORDER, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_M_DELIVERYORDER, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new DeliveryOrderHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(DeliveryOrderRequest request, out DeliveryOrderResponse response)
        {
            response = new DeliveryOrderResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_M_DELIVERYORDER, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new DeliveryOrderHandler(_unitOfWork).RemoveData(request);
            }
        }

        private void ValidateForApprove(DeliveryOrderRequest request, out DeliveryOrderResponse response)
        {
            response = new DeliveryOrderResponse();

            if (request.Action == ClinicEnums.Action.APPROVE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(RECIVED_M_DELIVERYORDER, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new DeliveryOrderHandler(_unitOfWork).ApproveData(request);
            }
        }
    }
}
