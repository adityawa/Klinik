using Klinik.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.Interfaces;
using Klinik.Common;
using Klinik.Resources;

namespace Klinik.Features.Patients.Pasien
{
    public class PatientValidator : BaseFeatures, IValidator<PatientResponse, PatientRequest>
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_POLISCHEDULE_M";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_POLISCHEDULE_M";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_POLISCHEDULE_M";

        public PatientValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                        if(clinicId.KlinikID==0 ||clinicId.KlinikID==null)
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
