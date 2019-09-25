using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Interfaces;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratLabReferensi
{
    public class RujukanLabValidator : BaseFeatures, IValidator<RujukanLabResponse, RujukanLabRequest>
    {
        private const string CREATE_SURAT_RUJUKAN_PRIVILEGE_ = "CREATE_SURAT_RUJUKAN";

        public RujukanLabValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public RujukanLabResponse Validate(RujukanLabRequest request)
        {
            bool isHavePrivilege = true;
            var response = new RujukanLabResponse();
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
                    response = new RujukanLabHandler(_unitOfWork).SaveSuratRujukanLab(request);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public RujukanLabResponse ValidateBeforePreview(RujukanLabRequest request)
        {
            var response = new RujukanLabResponse();
            if (request.Data.SuratRujukanLabKeluar.ListOfLabItemId.Count <= 0)
                errorFields.Add("At least one Lab Item Should be selected");
            if (request.Data.SuratRujukanLabKeluar.DokterPengirim==string.Empty)
                errorFields.Add("Dokter Pengirim");

            if(errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }
            else
            {
                response = new RujukanLabHandler(_unitOfWork).SaveAndPreview(request);
            }

            return response;
        }
    }
}
