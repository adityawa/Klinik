using Klinik.Common;
using Klinik.Data;
using Klinik.Features.MasterData.LookupCategory;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class LookUpCategoryValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_LOOKUP_CATEGORY";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_LOOKUP_CATEGORY";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_LOOKUP_CATEGORY";

        /// <summary>
        /// Constructor of LookUp Category
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LookUpCategoryValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(LookUpCategoryRequest request, out LookUpCategoryResponse response)
        {
            response = new LookUpCategoryResponse();

            if (request.Action != null && request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                bool isHavePrivilege = true;

                if (request.Data.TypeName == null || String.IsNullOrWhiteSpace(request.Data.TypeName))
                {
                    errorFields.Add("LookupCategory Type Name");
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
                    response = new LookUpCategoryHandler(_unitOfWork).CreateOrEdit(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(LookUpCategoryRequest request, out LookUpCategoryResponse response)
        {
            response = new LookUpCategoryResponse();

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
                response = new LookUpCategoryHandler(_unitOfWork).RemoveData(request);
            }
        }
    }
}
