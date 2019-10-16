using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.RealisasiSuratRujukan
{
    public class RealisasiSuratRujukanValidator : BaseFeatures
    {
        private const string ADD_PRIVILEGE_NAME = "REALISASI_SURAT_RUJUKAN";

        public RealisasiSuratRujukanValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public FormExamineResponse Validate(FormExamineRequest request)
        {
            var response = new FormExamineResponse();

            bool isHavePrivilege = true;

            if (request.Data.Id == 0)
            {
                isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            }
           

            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new RealisasiSuratRujukanHandler(_unitOfWork, _context).CreateOrEdit(request);
            }

            return response;
        }
    }
}
