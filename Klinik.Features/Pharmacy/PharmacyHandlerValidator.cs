using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features.Pharmacy
{
    public class PharmacyValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_Pharmacy_ITEM";        
        private const string EDIT_PRIVILEGE_NAME = "EDIT_Pharmacy_ITEM";        

        public PharmacyValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public void Validate(PharmacyRequest request, out PharmacyResponse response)
        {
            bool isHavePrivilege = true;
            response = new PharmacyResponse();
            try
            {
                if (request.Data.FormMedicalID == 0)
                {
                    errorFields.Add("Form Medical ID");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                // check if the medicine are filled
                var _qryFormExamineMedicine = _unitOfWork.RegistrationRepository.GetFirstOrDefault(x => x.FormMedicalID == request.Data.FormMedicalID);
                if (_qryFormExamineMedicine != null)
                {
                    if (_qryFormExamineMedicine.Status == (int)RegistrationStatusEnum.Finish)
                    {
                        response.Status = false;
                        response.Message = "This data could not be changed";
                    }
                }

                isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
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
        }
    }
}
