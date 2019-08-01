using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features.Farmasi
{
    public class FarmasiValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_FARMASI_ITEM";        
        private const string EDIT_PRIVILEGE_NAME = "EDIT_FARMASI_ITEM";        

        public FarmasiValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public void Validate(FarmasiRequest request, out FarmasiResponse response)
        {
            bool isHavePrivilege = true;
            response = new FarmasiResponse();
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
                response = new FarmasiHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }
    }
}
