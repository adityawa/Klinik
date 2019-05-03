using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Radiologi
{
    public class RadiologiValidator: BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_Lab_Item";
        private const string ADD_RESULT_PRIVILEGE_NAME = "ADD_LAB_RESULT";
        private const string EDIT_PRIVILEGE_NAME = "Edit_Lab_Item";
        private const string DELETE_PRIVILEGE_NAME = "Delete_Lab_Item";

        public RadiologiValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public void Validate(RadiologiRequest request, out RadiologiResponse response)
        {
            bool isHavePrivilege = true;
            response = new RadiologiResponse();
            try
            {
                if (request.Data.FormMedicalID == 0)
                {
                    errorFields.Add("Form Medical ID");
                }

                if (request.Data.LabItemsId.Count == 0)
                {
                    errorFields.Add("Lab Item");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                //cek is lab item inside form nedical Id already filled
                var _qryFormExamineLab = _unitOfWork.RegistrationRepository.GetFirstOrDefault(x => x.FormMedicalID == request.Data.FormMedicalID);
                if (_qryFormExamineLab != null)
                {
                    if (_qryFormExamineLab.Status == (int)RegistrationStatusEnum.Finish)
                    {
                        response.Status = false;
                        response.Message = Messages.LabItemCannotChange;
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
                response = new RadiologiHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }

        public void ValidateAddResult(RadiologiRequest request, out RadiologiResponse response)
        {
            bool isHavePrivilege = true;
            response = new RadiologiResponse();
            try
            {
                if (request.Data.FormMedicalID == 0)
                {
                    errorFields.Add("Form Medical ID");
                }

                if (request.Data.LabItemCollsJs.Count == 0)
                {
                    errorFields.Add("Lab Radiologi Result");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }



                isHavePrivilege = IsHaveAuthorization(ADD_RESULT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
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
                response = new RadiologiHandler(_unitOfWork, _context).CreateLabResult(request);
            }
        }
    }
}
