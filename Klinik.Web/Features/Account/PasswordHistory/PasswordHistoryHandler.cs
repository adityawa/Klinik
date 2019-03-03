using Klinik.Web.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess.DataRepository;
using Klinik.Web.Enumerations;
using Klinik.Web.Infrastructure;

namespace Klinik.Web.Features.Account.PasswordHistory
{
    public class PasswordHistoryHandler : BaseFeatures
    {
        public PasswordHistoryHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public PasswordHistoryResponse ChangePassword(PasswordHistoryRequest request)
        {
            int result = 0;
            PasswordHistoryResponse response = new PasswordHistoryResponse();
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toBeUpdate = _context.Users.SingleOrDefault(x => x.ID == request.RequestPassHistData.UserID);
                    if (toBeUpdate != null)
                    {
                        toBeUpdate.Password = Common.Encryptor( request.RequestPassHistData.NewPassword, Common.KeyEncryptor);
                        _context.SaveChanges();
                    }

                    var _passHistoryEntity = new Web.DataAccess.DataRepository.PasswordHistory
                    {
                        OrganizationID = request.RequestPassHistData.OrganizationID,
                        UserName = request.RequestPassHistData.UserName, 
                        Password = Common.Encryptor( request.RequestPassHistData.Password, Common.KeyEncryptor) 
                    };

                    _context.PasswordHistories.Add(_passHistoryEntity);
                    result = _context.SaveChanges();

                    transaction.Commit();
                    response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                    response.Message = "Password has been changed successfully";
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = Common.GetGeneralErrorMesg();
                }
               
            }

            return response;
        }
    }
}