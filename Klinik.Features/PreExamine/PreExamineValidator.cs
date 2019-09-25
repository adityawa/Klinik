using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class PreExamineValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_PREEXAMINE";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_PREEXAMINE";
        private const string VIEW_PRIVILEGE_NAME = "VIEW_PREEXAMINE";

        public PreExamineValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public PreExamineResponse Validate(PreExamineRequest request)
        {
            var response = new PreExamineResponse();
            bool isHavePrivilege = true;
            if (String.IsNullOrEmpty(request.Data.strTransDate) || String.IsNullOrWhiteSpace(request.Data.strTransDate))
            {
                errorFields.Add("Transaction Date");
            }

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }
            //cek gender
            var _gender = _unitOfWork.RegistrationRepository.GetById(request.Data.LoketData.Id);
            if (_gender != null)
            {
                if (_gender.Patient.Gender.ToLower() == "m")
                {
                    if (request.Data.strMenstrualDate != null || request.Data.strKBDate != null)
                    {
                        response.Status = false;
                        response.Message = Messages.MenstrualDataProhibited;
                    }
                }
            }
            else
            {
                response.Status = false;
                response.Message = Messages.PatientNotRegistered;
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

            if (response.Status == true)
            {
                response = new PreExamineHandler(_unitOfWork).CreateOrEdit(request);
            }
            return response;
        }
    }
}
