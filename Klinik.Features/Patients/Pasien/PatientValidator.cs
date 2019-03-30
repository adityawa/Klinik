using Klinik.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.Interfaces;
using Klinik.Common;
using Klinik.Resources;
using Klinik.Data.DataRepository;

namespace Klinik.Features.Patients.Pasien
{
    public class PatientValidator : BaseFeatures, IValidator<PatientResponse, PatientRequest>
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_PATIENT";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_PATIENT";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_PATIENT";

        public PatientValidator(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public PatientResponse Validate(PatientRequest request)
        {
            var response = new PatientResponse();
            if (request.Action != null)
            {
                if (request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
                    response = ValidateForDelete(request);
            }
            else
            {
                bool isHavePrivilege = true;

                //field validation
                if (String.IsNullOrEmpty(request.Data.Name) || String.IsNullOrWhiteSpace(request.Data.Name))
                {
                    errorFields.Add("Name");
                }
                if (String.IsNullOrEmpty(request.Data.BirthDateStr) || String.IsNullOrWhiteSpace(request.Data.BirthDateStr))
                {
                    errorFields.Add("Birthdate");
                }

                if (String.IsNullOrEmpty(request.Data.Address) || String.IsNullOrWhiteSpace(request.Data.Address))
                {
                    errorFields.Add("Address");
                }

                var _typedesc = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Type == Constants.MasterType.PATIENT_TYPE && x.Value == request.Data.Type.ToString());
                if (_typedesc != null)
                {
                    if (_typedesc.Name.ToLower() == "company")
                    {
                        if (request.Data.EmployeeID==0)
                        {
                            errorFields.Add("Employee");
                        }
                        if (request.Data.familyRelationshipID == 0)
                        {
                            errorFields.Add("Employee Relation");
                        }
                    }
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                if (request.Data.Account == null)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }
                else
                {
                    var clinicId = _context.Organizations.FirstOrDefault(x => x.OrgCode == request.Data.Account.Organization);
                    if (clinicId != null)
                    {
                        if (clinicId.KlinikID == 0 || clinicId.KlinikID == null)
                        {
                            response.Status = false;
                            response.Message = Messages.UserDoesNotHaveClinic;
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = Messages.UnauthorizedAccess;
                    }

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

                #region ::VALIDASI PHOTO::
                if (request.Data.file != null)
                {
                    var validImageTypes = new string[]
                   {
                        "image/gif",
                        "image/jpeg",
                        "image/pjpeg",
                        "image/png"
                   };

                    if (!validImageTypes.Contains(request.Data.file.ContentType))
                    {
                        response.Status = false;
                        response.Message = Messages.InvalidImage;
                    }
                }
                #endregion

                

                if (response.Status)
                {
                    response = new PatientHandler(_unitOfWork, _context).CreateOrEdit(request);
                }
            }

            return response;
        }

        private PatientResponse ValidateForDelete(PatientRequest request)
        {
            var response = new PatientResponse();

            bool isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new PatientHandler(_unitOfWork, _context).RemoveData(request);
            }

            return response;
        }
    }
}
