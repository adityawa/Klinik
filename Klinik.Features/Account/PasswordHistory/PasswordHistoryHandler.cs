using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Linq;

namespace Klinik.Features
{
    /// <summary>
    /// Password history handler class
    /// </summary>
    public class PasswordHistoryHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public PasswordHistoryHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PasswordHistoryResponse ChangePassword(PasswordHistoryRequest request)
        {
            int result = 0;
            PasswordHistoryResponse response = new PasswordHistoryResponse();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toBeUpdate = _context.Users.SingleOrDefault(x => x.ID == request.RequestPassHistData.UserID);
                    if (toBeUpdate != null)
                    {
                        toBeUpdate.Password = CommonUtils.Encryptor(request.RequestPassHistData.NewPassword, CommonUtils.KeyEncryptor);
                        _context.SaveChanges();
                    }

                    var _passHistoryEntity = new PasswordHistory
                    {
                        OrganizationID = request.RequestPassHistData.OrganizationID,
                        UserName = request.RequestPassHistData.UserName,
                        Password = CommonUtils.Encryptor(request.RequestPassHistData.Password, CommonUtils.KeyEncryptor)
                    };

                    _context.PasswordHistories.Add(_passHistoryEntity);
                    result = _context.SaveChanges();

                    transaction.Commit();
                    response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                    response.Message = "Password has been changed successfully";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = CommonUtils.GetGeneralErrorMesg();
                }
            }

            return response;
        }
    }
}