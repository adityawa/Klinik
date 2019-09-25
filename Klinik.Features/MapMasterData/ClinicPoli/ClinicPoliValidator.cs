using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Linq;

namespace Klinik.Features
{
    public class ClinicPoliValidator : BaseFeatures
    {
        public ClinicPoliValidator(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public void Validate(ClinicPoliRequest request, out ClinicPoliResponse response)
        {
            response = new ClinicPoliResponse();

            if (request.Data.ClinicID == 0)
            {
                errorFields.Add("Clinic");
            }
            if (request.Data.PoliIDs.Count == 0)
            {
                errorFields.Add("Poli");
            }

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }

            if (response.Status)
            {
                response = new ClinicPoliHandler(_unitOfWork, _context).CreateOrEdit(request);
            }
        }
    }
}
