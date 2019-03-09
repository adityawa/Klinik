using Klinik.Common;
using Klinik.Data;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class OrganizationValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "ADD_M_ORG";
        private const string EDIT_PRIVILEGE_NAME = "EDIT_M_ORG";
        private const string DELETE_PRIVILEGE_NAME = "DELETE_M_ORG";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public OrganizationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void Validate(OrganizationRequest request, out OrganizationResponse response)
        {
            response = new OrganizationResponse();

            if (request.action != null && request.action.Equals(ClinicEnums.enumAction.DELETE.ToString()))
            {
                ValidateForDelete(request, out response);
            }
            else
            {
                response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();

                bool isHavePrivilege = true;

                if (request.RequestOrganizationData.KlinikID == 0)
                {
                    errorFields.Add("Klinik Id");
                }
                if (request.RequestOrganizationData.OrgCode == null || request.RequestOrganizationData.OrgCode.Equals(string.Empty))
                {
                    errorFields.Add("Organization Code");
                }

                if (request.RequestOrganizationData.OrgName == null || request.RequestOrganizationData.OrgName.Equals(string.Empty))
                {
                    errorFields.Add("Organization Name");
                }
                if (errorFields.Any())
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
                }

                if (request.RequestOrganizationData.Id == 0)
                {

                    var _cek = _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == request.RequestOrganizationData.OrgCode, includes: x => x.Clinic);
                    if (_cek != null)
                    {
                        if (_cek.ID > 0)
                        {
                            response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                            response.Message = $"Organization Code {request.RequestOrganizationData.OrgCode} was exist. Please use another";
                        }
                    }
                }

                if (request.RequestOrganizationData.Id == 0)
                {
                    isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.RequestOrganizationData.Account.Privileges.PrivilegeIDs);
                }
                else
                {
                    isHavePrivilege = IsHaveAuthorization(EDIT_PRIVILEGE_NAME, request.RequestOrganizationData.Account.Privileges.PrivilegeIDs);
                }

                if (!isHavePrivilege)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Unauthorized Access!";
                }

                if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
                {
                    response = new OrganizationHandler(_unitOfWork).CreateOrEditOrganization(request);
                }
            }
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ValidateForDelete(OrganizationRequest request, out OrganizationResponse response)
        {
            response = new OrganizationResponse();
            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();

            bool isHavePrivilege = true;

            if (request.action == ClinicEnums.enumAction.DELETE.ToString())
            {
                isHavePrivilege = IsHaveAuthorization(DELETE_PRIVILEGE_NAME, request.RequestOrganizationData.Account.Privileges.PrivilegeIDs);
            }

            if (!isHavePrivilege)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Unauthorized Access!";
            }

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new OrganizationHandler(_unitOfWork).RemoveOrganization(request);
            }
        }
    }
}