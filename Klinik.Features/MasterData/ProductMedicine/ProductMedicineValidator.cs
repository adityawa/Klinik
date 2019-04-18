using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class ProductMedicineValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_PRODUCT_MEDICINE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_PRODUCT_MEDICINE";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_PRODUCT_MEDICINE";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ProductMedicineValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(ProductMedicineRequest request, out ProductMedicineResponse response)
        {
            response = new ProductMedicineResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.Amount < 0)
                {
                    errorFields.Add("Product Medicine Amount");
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
                    response = new ProductMedicineHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(ProductMedicineRequest request, out ProductMedicineResponse response)
        {
            response = new ProductMedicineResponse();

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
                response = new ProductMedicineHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}