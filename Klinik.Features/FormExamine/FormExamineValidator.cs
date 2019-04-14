using Klinik.Data;
using Klinik.Resources;

namespace Klinik.Features.FormExamines
{
    public class FormExamineValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_POLI_FORM_EXAMINE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_POLI_FORM_EXAMINE";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public FormExamineValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public FormExamineResponse Validate(FormExamineRequest request)
        {
            var response = new FormExamineResponse();

            bool isHavePrivilege = true;

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
                response = new FormExamineHandler(_unitOfWork).CreateOrEdit(request);
            }

            return response;
        }
    }
}
