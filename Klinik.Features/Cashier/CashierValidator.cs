using Klinik.Data;
using Klinik.Resources;

namespace Klinik.Features.Cashier
{
    public class CashierValidator : BaseFeatures
    {
        private const string EDIT_CHASIER = "EDIT_CHASIER";
        public CashierValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CashierResponse Validate(CashierRequest request)
        {
            var response = new CashierResponse();

            bool isHavePrivilege = true;

            if (request.Data.Id == 0)
            {
                isHavePrivilege = IsHaveAuthorization(EDIT_CHASIER, request.Data.Account.Privileges.PrivilegeIDs);
            }
            else
            {
                isHavePrivilege = IsHaveAuthorization(EDIT_CHASIER, request.Data.Account.Privileges.PrivilegeIDs);
            }

            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            //if (response.Status)
            //{
            //    response = new FormExamineHandler(_unitOfWork).CreateOrEdit(request);
            //}

            return response;
        }
    }
}
