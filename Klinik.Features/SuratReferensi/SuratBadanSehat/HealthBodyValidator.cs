using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Interfaces;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratBadanSehat
{
    public class HealthBodyValidator : BaseFeatures, IValidator<HealthBodyResponse, HealthBodyRequest>
    {
        private const string CREATE_SURAT_RUJUKAN_PRIVILEGE_ = "CREATE_SURAT_RUJUKAN";

        public HealthBodyValidator(IUnitOfWork unitOfWork, KlinikDBEntities context=null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public HealthBodyResponse Validate(HealthBodyRequest request)
        {
            bool isHavePrivilege = true;
            var response = new HealthBodyResponse();
            try
            {
                if (request.Data.ForPatient == 0)
                {
                    errorFields.Add("Patient ID");
                }
                if (request.Data.Cekdate == null)
                {
                    errorFields.Add("Tanggal Pemeriksaan");
                }

                if (errorFields.Any())
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
                }

                isHavePrivilege = IsHaveAuthorization(CREATE_SURAT_RUJUKAN_PRIVILEGE_, request.Data.Account.Privileges.PrivilegeIDs);
                if (!isHavePrivilege)
                {
                    response.Status = false;
                    response.Message = Messages.UnauthorizedAccess;
                }

                if (response.Status)
                {
                    response = new HealthBodyHandler(_unitOfWork).GetPatientAndPreExamineData(request);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public HealthBodyResponse ValidateBeforePreview(HealthBodyRequest request)
        {
            var response = new HealthBodyResponse();
          
            if (request.Data.Decision == string.Empty)
                errorFields.Add("Dinyatakan");

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }
            else
            {
                response = new HealthBodyHandler(_unitOfWork).SaveSuratBadanSehat(request);
            }

            return response;
        }
    }
}
