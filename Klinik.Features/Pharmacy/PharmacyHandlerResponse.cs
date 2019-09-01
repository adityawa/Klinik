using Klinik.Entities;
using Klinik.Entities.Form;
using Klinik.Entities.Pharmacy;
using System.Collections.Generic;

namespace Klinik.Features.Pharmacy
{
	public class PharmacyResponse : BaseResponse<PrescriptionModel>
    {        
        
    }

    public class ListObatResponse : BaseResponse<FormExamineMedicineDetailModel> { }

}
