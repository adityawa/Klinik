using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web;
using System.Web.Mvc;
using Klinik.Web.DataAccess;
using Klinik.Web.Enumerations;
namespace Klinik.Web.Features.MasterData.Organization
{
    public class OrganizationValidator : BaseFeatures
    {

        public OrganizationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(OrganizationRequest request, out OrganizationResponse response)
        {

            response = new OrganizationResponse();
            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
            IList<string> errorFields = new List<string>();
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

            else if (request.RequestOrganizationData.Id == 0)
            {
                //cek if Org Code exist
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

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
                response = new OrganizationHandler(_unitOfWork).CreateOrEditOrganization(request);

        }
    }
}