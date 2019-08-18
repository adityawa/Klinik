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
    public class PurchaseRequestPusatValidator : BaseFeatures
    {
        private const string ADD_M_PURCHASEREQUESTPUSAT = "ADD_M_PURCHASEREQUESTPUSAT";
        private const string EDIT_M_PURCHASEREQUESTPUSAT = "EDIT_M_PURCHASEREQUESTPUSAT";
        private const string DELETE_M_PURCHASEREQUESTPUSAT = "DELETE_M_PURCHASEREQUESTPUSAT";

        public PurchaseRequestPusatValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PurchaseRequestPusatRequest request, out PurchaseRequestPusatResponse response)
        {
            response = new PurchaseRequestPusatResponse();

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

                if (request.Data.prnumber == null || String.IsNullOrWhiteSpace(request.Data.prnumber))
                {
                    errorFields.Add("Prnumber");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                if (request.Data.Id == 0)
                {

                    isHavePrivilege = IsHaveAuthorization(ADD_M_PURCHASEREQUESTPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEREQUESTPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new PurchaseRequestPusatHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(PurchaseRequestPusatRequest request, out PurchaseRequestPusatResponse response)
        {
            response = new PurchaseRequestPusatResponse();

            if (request.Action == ClinicEnums.Action.DELETE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(DELETE_M_PURCHASEREQUESTPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseRequestPusatHandler(_unitOfWork).RemoveData(request);
            }
        }

        private void ValidateForApprove(PurchaseRequestPusatRequest request, out PurchaseRequestPusatResponse response)
        {
            response = new PurchaseRequestPusatResponse();

            if (request.Action == ClinicEnums.Action.APPROVE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEREQUESTPUSAT, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseRequestPusatHandler(_unitOfWork).ApproveData(request);
            }
        }
    }
}
