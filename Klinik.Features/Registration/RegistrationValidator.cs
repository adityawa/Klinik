using System;
using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;

namespace Klinik.Features.Registration
{
    public class RegistrationValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_REGISTRATION";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_REGISTRATION";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_REGISTRATION";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public RegistrationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public RegistrationResponse Validate(RegistrationRequest request)
        {
            var response = new RegistrationResponse();

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

                if (response.Status)
                {
                    response = new RegistrationHandler(_unitOfWork).CreateOrEdit(request);
                }
            }

            return response;
        }

        /// <summary>
        /// Process validation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RegistrationResponse ValidateForProcess(RegistrationRequest request)
        {
            var response = new RegistrationResponse();

            bool isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new RegistrationHandler(_unitOfWork).ProcessRegistration(request);
            }

            return response;
        }

        /// <summary>
        /// Hold validation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RegistrationResponse ValidateForHold(RegistrationRequest request)
        {
            var response = new RegistrationResponse();

            bool isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new RegistrationHandler(_unitOfWork).HoldRegistration(request);
            }

            return response;
        }

        /// <summary>
        /// Finish validation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RegistrationResponse ValidateForFinish(RegistrationRequest request)
        {
            var response = new RegistrationResponse();

            bool isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new RegistrationHandler(_unitOfWork).FinishRegistration(request);
            }

            return response;
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>        
        private RegistrationResponse ValidateForDelete(RegistrationRequest request)
        {
            var response = new RegistrationResponse();

            bool isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new RegistrationHandler(_unitOfWork).RemoveData(request);
            }

            return response;
        }
    }
}
