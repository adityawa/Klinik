using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.Enumerations;

namespace Klinik.Web.Features.MasterData.Privileges
{
    public class PrivilegeValidator : BaseFeatures
    {

        public PrivilegeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Validate(PrivilegeRequest request, out PrivilegeResponse response)
        {
            response = new PrivilegeResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };
           
            if (request.RequestPrivilegeData.Privilige_Name==null || request.RequestPrivilegeData.Privilige_Name == null)
            {
                errorFields.Add("Privilege Name");
            }

            if (errorFields.Any())
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Validation Error for following fields : {String.Join(",", errorFields)}";
            }
            else if (request.RequestPrivilegeData.Privilige_Name.Length > 150)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Maximum Character for Privilege Name is 150";
            }

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
                response = new PrivilegeHandler(_unitOfWork).CreateOrEdit(request);
        }
    }
}