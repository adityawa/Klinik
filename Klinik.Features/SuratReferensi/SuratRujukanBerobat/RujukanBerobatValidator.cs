using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Interfaces;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratRujukanBerobat
{
    public class RujukanBerobatValidator : BaseFeatures, IValidator<RujukanBerobatResponse, RujukanBerobatRequest>
    {
        private const string CREATE_SURAT_RUJUKAN_PRIVILEGE_ = "CREATE_SURAT_RUJUKAN";

        public RujukanBerobatValidator(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public RujukanBerobatResponse Validate(RujukanBerobatRequest request)
        {
            bool isHavePrivilege = true;
            var response = new RujukanBerobatResponse();
            if (request.Data.ForPatient == 0)
            {
                errorFields.Add("Patient ID");
            }
            if (request.Data.FormMedicalID == 0)
            {
                errorFields.Add("Form Medical ID");
            }
            if (String.IsNullOrEmpty(request.Data.InfoRujukanData.RSRujukan) || String.IsNullOrWhiteSpace(request.Data.InfoRujukanData.RSRujukan))
            {
                errorFields.Add("Rs Rujukan");
            }
            if (String.IsNullOrEmpty(request.Data.InfoRujukanData.Phone) || String.IsNullOrWhiteSpace(request.Data.InfoRujukanData.Phone))
            {
                errorFields.Add("No Telp RS");
            }
            if (String.IsNullOrEmpty(request.Data.InfoRujukanData.NamaDokter) || String.IsNullOrWhiteSpace(request.Data.InfoRujukanData.NamaDokter))
            {
                errorFields.Add("Nama Dokter");
            }
            if (String.IsNullOrEmpty(request.Data.InfoRujukanData.HariPraktek) || String.IsNullOrWhiteSpace(request.Data.InfoRujukanData.HariPraktek))
            {
                errorFields.Add("Hari Praktek");
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
                response = new RujukanBerobatHandler(_unitOfWork).SaveSuratRujukanBerobat(request);
            }
            return response;
        }
    }
}
