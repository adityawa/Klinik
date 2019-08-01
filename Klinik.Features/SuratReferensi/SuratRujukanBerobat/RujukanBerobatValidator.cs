using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratRujukanBerobat
{
    public class RujukanBerobatValidator:BaseFeatures,IValidator<RujukanBerobatResponse, RujukanBerobatRequest>
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
            //if (request.Data.ForPatient == 0)
            //{
            //    errorFields.Add("Patient ID");
            //}
            //if (request.Data.Decision == null || string.IsNullOrWhiteSpace(request.Data.Decision))
            //{
            //    errorFields.Add("Pernyataan");
            //}
            //if (request.Data.Action == null || string.IsNullOrWhiteSpace(request.Data.Action))
            //{
            //    errorFields.Add("Tindakan");
            //}

            //if (request.Data.Treatment == null || string.IsNullOrWhiteSpace(request.Data.Treatment))
            //{
            //    errorFields.Add("Treatment");
            //}

            return response;
        }
    }
}
