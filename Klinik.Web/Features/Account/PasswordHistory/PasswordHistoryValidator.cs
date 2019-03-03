using Klinik.Web.DataAccess;
using Klinik.Web.Enumerations;
using Klinik.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess.DataRepository;
namespace Klinik.Web.Features.Account.PasswordHistory
{
    public class PasswordHistoryValidator : BaseFeatures
    {
        public PasswordHistoryValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public PasswordHistoryResponse Validate(PasswordHistoryRequest request)
        {
            PasswordHistoryResponse response = new PasswordHistoryResponse();
            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
            if (String.IsNullOrEmpty(request.RequestPassHistData.UserName) || String.IsNullOrWhiteSpace(request.RequestPassHistData.UserName))
                errorFields.Add("User Name");
            if (String.IsNullOrEmpty(request.RequestPassHistData.Password) || String.IsNullOrWhiteSpace(request.RequestPassHistData.Password))
                errorFields.Add("Password");
            if (String.IsNullOrEmpty(request.RequestPassHistData.NewPassword) || String.IsNullOrWhiteSpace(request.RequestPassHistData.NewPassword))
                errorFields.Add("New Password");

            if (errorFields.Any())
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"Following Fields must be filled : {String.Join(",", errorFields)}";
            }

            var cekIsExpired = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.RequestPassHistData.UserName);
            if (cekIsExpired == null)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"User Not Found";
            }
            if (cekIsExpired != null)
            {
                if (cekIsExpired.Status == false || cekIsExpired.ExpiredDate < DateTime.Now)
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Password cannot be changed for non-active user";
                }
            }


            var validateCurrentUser = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.RequestPassHistData.UserName);
            if (validateCurrentUser != null)
            {
                if (request.RequestPassHistData.Password != Common.Decryptor(validateCurrentUser.Password, Common.KeyEncryptor))
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Password cannot be changed because user name and password not match";
                }

            }

            var IsExistPassinHist = _unitOfWork.PasswordHistoryRepository.Get(x => x.OrganizationID == request.RequestPassHistData.OrganizationID && x.UserName == request.RequestPassHistData.UserName).Select(x=>x.Password);
            foreach(string p in IsExistPassinHist)
            {
                if(request.RequestPassHistData.NewPassword==Common.Decryptor(p, Common.KeyEncryptor))
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Password already used in the past, please use another";
                    break;
                }
            }
            if (request.RequestPassHistData.Password.Equals(request.RequestPassHistData.NewPassword))
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = $"New Password cannot same with Old Password";
            }
            if (response.Status == ClinicEnums.enumStatus.SUCCESS.ToString())
            {
                response = new PasswordHistoryHandler(_unitOfWork, _context).ChangePassword(request);
            }

            return response;
        }
    }
}