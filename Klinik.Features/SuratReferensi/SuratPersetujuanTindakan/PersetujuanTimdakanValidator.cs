using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Interfaces;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratPersetujuanTindakan
{
    public class PersetujuanTimdakanValidator : BaseFeatures, IValidator<PersetujuanTindakanResponse, PersetujuanTindakanRequest>
    {
        private const string CREATE_SURAT_RUJUKAN_PRIVILEGE_ = "CREATE_SURAT_RUJUKAN";

        public PersetujuanTimdakanValidator(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public PersetujuanTindakanResponse Validate(PersetujuanTindakanRequest request)
        {
            bool isHavePrivilege = true;
            var response = new PersetujuanTindakanResponse();
            if (request.Data.ForPatient == 0)
            {
                errorFields.Add("Patient ID");
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
                response = new PersetujuanTindakanHandler(_unitOfWork).GetPatientPenjaminData(request.Data.ForPatient);
            }
            return response;
        }

        public PersetujuanTindakanResponse ValidateBeforeSave(PersetujuanTindakanRequest request)
        {
            bool isHavePrivilege = true;
            var response = new PersetujuanTindakanResponse();
            if (request.Data.ForPatient == 0)
            {
                errorFields.Add("Patient ID");
            }
            if (request.Data.Decision == null || string.IsNullOrWhiteSpace(request.Data.Decision))
            {
                errorFields.Add("Pernyataan");
            }
            if (request.Data.Action == null || string.IsNullOrWhiteSpace(request.Data.Action))
            {
                errorFields.Add("Tindakan");
            }

            if (request.Data.Treatment == null || string.IsNullOrWhiteSpace(request.Data.Treatment))
            {
                errorFields.Add("Treatment");
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
                response = new PersetujuanTindakanHandler(_unitOfWork).SavePersetujuanTindakan(request);
            }
            return response;
        }
    }
}
