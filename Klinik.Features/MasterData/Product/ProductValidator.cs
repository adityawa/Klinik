using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class ProductValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_PRODUCT";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_PRODUCT";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_PRODUCT";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ProductValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(ProductRequest request, out ProductResponse response)
        {
            response = new ProductResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.Name == null || String.IsNullOrWhiteSpace(request.Data.Name))
                {
                    errorFields.Add("Product Name");
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
                    response = new ProductHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(ProductRequest request, out ProductResponse response)
        {
            response = new ProductResponse();

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
                response = new ProductHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}