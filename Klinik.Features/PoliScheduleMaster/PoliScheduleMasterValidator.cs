using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;

namespace Klinik.Features
{
    public class PoliScheduleMasterValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_POLISCHEDULE_M";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_POLISCHEDULE_M";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_POLISCHEDULE_M";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public PoliScheduleMasterValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public PoliScheduleMasterResponse Validate(PoliScheduleMasterRequest request)
        {
            var response = new PoliScheduleMasterResponse();

            if (request.Action != null)
            {
                if (request.Action.Equals(ClinicEnums.Action.DELETE.ToString()))
                    response = ValidateForDelete(request);
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
                    response = new PoliScheduleMasterHandler(_unitOfWork).CreateOrEdit(request);
                }
            }

            return response;
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>        
        private PoliScheduleMasterResponse ValidateForDelete(PoliScheduleMasterRequest request)
        {
            var response = new PoliScheduleMasterResponse();

            bool isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new PoliScheduleMasterHandler(_unitOfWork).RemoveData(request);
            }

            return response;
        }
    }
}
