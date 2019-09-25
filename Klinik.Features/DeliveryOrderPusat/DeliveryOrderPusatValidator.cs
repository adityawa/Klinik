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
    public class DeliveryOrderPusatValidator : BaseFeatures
    {
        private const string ADD_M_DELIVERYORDERPUSAT = "ADD_M_DELIVERYORDERPUSAT";
        private const string EDIT_M_DELIVERYORDERPUSAT = "EDIT_M_DELIVERYORDERPUSAT";
        private const string DELETE_M_DELIVERYORDERPUSAT = "DELETE_M_DELIVERYORDERPUSAT";

        public DeliveryOrderPusatValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(DeliveryOrderPusatRequest request, out DeliveryOrderPusatResponse response)
        {
            response = new DeliveryOrderPusatResponse();

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

                    isHavePrivilege = IsHaveAuthorization(ADD_M_DELIVERYORDERPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_M_DELIVERYORDERPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new DeliveryOrderPusatHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(DeliveryOrderPusatRequest request, out DeliveryOrderPusatResponse response)
        {
            response = new DeliveryOrderPusatResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_M_DELIVERYORDERPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new DeliveryOrderPusatHandler(_unitOfWork).RemoveData(request);
            }
        }

        private void ValidateForApprove(DeliveryOrderPusatRequest request, out DeliveryOrderPusatResponse response)
        {
            response = new DeliveryOrderPusatResponse();

            if (request.Action == ClinicEnums.Action.APPROVE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(EDIT_M_DELIVERYORDERPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new DeliveryOrderPusatHandler(_unitOfWork).ApproveData(request);
            }
        }
    }
}
