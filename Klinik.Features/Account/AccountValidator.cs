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
            response = new AccountResponse();

            if (String.IsNullOrEmpty(request.Data.UserName))
                errorFields.Add("User Name");
            if (String.IsNullOrEmpty(request.Data.Password))
                errorFields.Add("Password");

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = $"Following Fields must be filled : {String.Join(",", errorFields)}";
            }

            if (response.Status)
            {
                response = new AccountHandler(_unitOfWork, _context).AuthenticateUser(request);
            }

            return response;
        }
    }
}