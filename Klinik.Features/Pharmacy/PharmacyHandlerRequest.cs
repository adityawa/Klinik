using Klinik.Entities;
using Klinik.Entities.Account;
using Klinik.Entities.Pharmacy;

namespace Klinik.Features.Pharmacy
{
	public class PharmacyRequest :  BaseRequest<PrescriptionModel>
    {
		public AccountModel Account { get; set; }		
	}
}
