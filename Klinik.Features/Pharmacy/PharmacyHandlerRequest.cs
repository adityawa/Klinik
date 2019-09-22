using Klinik.Entities;
using Klinik.Entities.Account;
using Klinik.Entities.Pharmacy;
using System.Collections.Generic;

namespace Klinik.Features.Pharmacy
{
	public class PharmacyRequest :  BaseRequest<PrescriptionModel>
    {
		public AccountModel Account { get; set; }		
        public List<long> idSelectedobat { get; set; }
	}
}
