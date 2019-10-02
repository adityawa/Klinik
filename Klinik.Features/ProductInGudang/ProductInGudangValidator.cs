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
    public class ProductInGudangValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_PRODUCTINGUDANG";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_PRODUCTINGUDANG";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_PRODUCTINGUDANG";

        public ProductInGudangValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(ProductInGudangRequest request, out ProductInGudangResponse response)
        {
            response = new ProductInGudangResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.ProductId == null || request.Data.GudangId == null)
                {
                    errorFields.Add("Gudang Id Product Id");
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
                    response = new ProductInGudangHandler(_unitOfWork).CreateOrEditManual(request);
                }
            }
        }

        private void ValidateForDelete(ProductInGudangRequest request, out ProductInGudangResponse response)
        {
            response = new ProductInGudangResponse();

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
                response = new ProductInGudangHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}
