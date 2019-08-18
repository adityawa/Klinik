using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;

namespace Klinik.Features.Pharmacy
{
	public class PharmacyValidator : BaseFeatures
    {        
        private const string EDIT_PRIVILEGE_NAME = "EDIT_FORM_EXAMINE_MEDICINE";        

        public PharmacyValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public PharmacyResponse Validate(PharmacyRequest request)
        {
            bool isHavePrivilege = true;
			PharmacyResponse response = new PharmacyResponse();
            try
            {
                isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            if (response.Status)
            {
                response = new PharmacyHandler(_unitOfWork, _context).CreateOrEdit(request);
            }

			return response;
        }
    }
}
