using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class PasswordHistoryValidator : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public PasswordHistoryValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Validate the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PasswordHistoryResponse Validate(PasswordHistoryRequest request)
        {
            PasswordHistoryResponse response = new PasswordHistoryResponse();
            response.Status = ClinicEnums.Status.SUCCESS.ToString();
            if (String.IsNullOrEmpty(request.RequestPassHistData.UserName) || String.IsNullOrWhiteSpace(request.RequestPassHistData.UserName))
                errorFields.Add("User Name");
            if (String.IsNullOrEmpty(request.RequestPassHistData.Password) || String.IsNullOrWhiteSpace(request.RequestPassHistData.Password))
                errorFields.Add("Password");
            if (String.IsNullOrEmpty(request.RequestPassHistData.NewPassword) || String.IsNullOrWhiteSpace(request.RequestPassHistData.NewPassword))
                errorFields.Add("New Password");

            if (errorFields.Any())
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
                response.Message = $"Following Fields must be filled : {String.Join(",", errorFields)}";
            }

            var cekIsExpired = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.RequestPassHistData.UserName);
            if (cekIsExpired == null)
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
                response.Message = $"User Not Found";
            }
            else
            {
                if (cekIsExpired.Status == false || cekIsExpired.ExpiredDate < DateTime.Now)
                {
                    response.Status = ClinicEnums.Status.ERROR.ToString();
                    response.Message = $"Password cannot be changed for non-active user";
                }
            }

            var validateCurrentUser = _unitOfWork.UserRepository.GetFirstOrDefault(x => x.UserName == request.RequestPassHistData.UserName);
            if (validateCurrentUser != null)
            {
                if (request.RequestPassHistData.Password != CommonUtils.Decryptor(validateCurrentUser.Password, CommonUtils.KeyEncryptor))
                {
                    response.Status = ClinicEnums.Status.ERROR.ToString();
                    response.Message = $"Password cannot be changed because user name and password not match";
                }
            }

            var IsExistPassinHist = _unitOfWork.PasswordHistoryRepository.Get(x => x.OrganizationID == request.RequestPassHistData.OrganizationID && x.UserName == request.RequestPassHistData.UserName).Select(x => x.Password);
            foreach (string p in IsExistPassinHist)
            {
                if (request.RequestPassHistData.NewPassword == CommonUtils.Decryptor(p, CommonUtils.KeyEncryptor))
                {
                    response.Status = ClinicEnums.Status.ERROR.ToString();
                    response.Message = $"Password already used in the past, please use another";
                    break;
                }
            }

            if (request.RequestPassHistData.Password.Equals(request.RequestPassHistData.NewPassword))
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
                response.Message = $"New Password cannot same with Old Password";
            }

            if (response.Status == ClinicEnums.Status.SUCCESS.ToString())
            {
                response = new PasswordHistoryHandler(_unitOfWork, _context).ChangePassword(request);
            }

            return response;
        }
    }
}