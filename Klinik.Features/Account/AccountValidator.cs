using Klinik.Common;
using Klinik.Data;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class AccountValidator : BaseFeatures
    {
        public AccountValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AccountResponse Validate(AccountRequest request, out AccountResponse response)
        {
            response = new AccountResponse
            {
                Status = ClinicEnums.enumStatus.SUCCESS.ToString()
            };

            if (String.IsNullOrEmpty(request.RequestAccountModel.UserName))
                errorFields.Add("User Name");
            if (String.IsNullOrEmpty(request.RequestAccountModel.Password))
                errorFields.Add("Password");

            if (errorFields.Any())
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Following Fields must be filled : {String.Join(",", errorFields)}";
            }

            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new AccountHandler(_unitOfWork).AuthenticateUser(request);
            }

            return response;
        }
    }
}