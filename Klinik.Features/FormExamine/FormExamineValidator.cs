using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;

namespace Klinik.Features
{
    public class FormExamineValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_POLI_FORM_EXAMINE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_POLI_FORM_EXAMINE";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public FormExamineValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
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
                response = new FormExamineHandler(_unitOfWork, _context).CreateOrEdit(request);
            }

            return response;
        }
    }
}
