using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class LoketValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_REGISTRATION";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_REGISTRATION";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_REGISTRATION";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LoketValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public LoketResponse Validate(LoketRequest request)
        {
            var response = new LoketResponse();

            if (request.Action != null)
            {
                if (request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
                    response = ValidateForDelete(request);
                else if (request.Action.Equals(ClinicEnums.Action.Process.ToString()))
                    response = ValidateForProcess(request);
                else if (request.Action.Equals(ClinicEnums.Action.Hold.ToString()))
                    response = ValidateForHold(request);
                else if (request.Action.Equals(ClinicEnums.Action.Finish.ToString()))
                    response = ValidateForFinish(request);
            }
            else
            {
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

                //Validate only for appointment type
                if (request.Data.Type == 1)
                {
                    var today = DateTime.Now.Date;
                    var isHaveAppointment = _unitOfWork.AppointmentRepository.Get(x => x.PatientID == request.Data.PatientID
                     && x.AppointmentDate == today
                     && x.PoliID == request.Data.PoliToID
                     && x.ClinicID == request.Data.Account.ClinicID
                     && x.DoctorID==request.Data.DoctorID
                     && x.RequirementID==request.Data.NecessityType);

                    if (isHaveAppointment.FirstOrDefault() == null)
                    {
                        response.Status = false;
                        response.Message = $"You do not have an appointment yet before";
                    }
                }

                if (response.Status)
                {
                    response = new LoketHandler(_unitOfWork).CreateOrEdit(request);
                }
            }

            return response;
        }

        /// <summary>
        /// Process validation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private LoketResponse ValidateForProcess(LoketRequest request)
        {
            var response = new LoketResponse();

            bool isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new LoketHandler(_unitOfWork).ProcessRegistration(request);
            }

            return response;
        }

        /// <summary>
        /// Hold validation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private LoketResponse ValidateForHold(LoketRequest request)
        {
            var response = new LoketResponse();

            bool isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new LoketHandler(_unitOfWork).HoldRegistration(request);
            }

            return response;
        }

        /// <summary>
        /// Finish validation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private LoketResponse ValidateForFinish(LoketRequest request)
        {
            var response = new LoketResponse();

            bool isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new LoketHandler(_unitOfWork).FinishRegistration(request);
            }

            return response;
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>        
        private LoketResponse ValidateForDelete(LoketRequest request)
        {
            var response = new LoketResponse();

            bool isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new LoketHandler(_unitOfWork).RemoveData(request);
            }

            return response;
        }

        private LoketResponse ValidateBeforeCall(LoketRequest request)
        {
            var response = new LoketResponse();
            if (request.CallRequest.PoliID <= 0)
            {
                errorFields.Add("Poli ID");
            }
            if (string.IsNullOrEmpty(request.CallRequest.QueueCode))
            {
                errorFields.Add("Queue Code");
            }
            if (request.CallRequest.SortNumber <= 0)
            {
                errorFields.Add("Sort Number");
            }

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }
            else
            {
                //response=
            }
            return response;
        }
    }
}
