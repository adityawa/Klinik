﻿using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.PurchaseRequest
{
    public class PurchaseRequestValidator : BaseFeatures
    {
        private const string ADD_M_PURCHASEREQUEST = "ADD_M_PURCHASEREQUEST";
        private const string EDIT_M_PURCHASEREQUEST = "EDIT_M_PURCHASEREQUEST";
        private const string DELETE_M_PURCHASEREQUEST = "DELETE_M_PURCHASEREQUEST";

        public PurchaseRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PurchaseRequestRequest request, out PurchaseRequestResponse response)
        {
            response = new PurchaseRequestResponse();

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
                    response = new PurchaseRequestHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        private void ValidateForDelete(PurchaseRequestRequest request, out PurchaseRequestResponse response)
        {
            response = new PurchaseRequestResponse();

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
                response = new PurchaseRequestHandler(_unitOfWork).RemoveData(request);
            }
        }

        private void ValidateForApprove(PurchaseRequestRequest request, out PurchaseRequestResponse response)
        {
            response = new PurchaseRequestResponse();

            if (request.Action == ClinicEnums.Action.APPROVE.ToString())
            {
                bool isHavePrivilege = IsHaveAuthorization(EDIT_M_PURCHASEREQUEST, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }

            if (response.Status)
            {
                response = new PurchaseRequestHandler(_unitOfWork).ApproveData(request);
            }
        }
    }
}