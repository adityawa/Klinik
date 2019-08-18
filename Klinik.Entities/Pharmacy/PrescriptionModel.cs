using Klinik.Entities.Form;
using System.Collections.Generic;

namespace Klinik.Entities.Pharmacy
{
	public class PrescriptionModel
	{
		public List<FormExamineMedicineModel> Medicines { get; set; }

		public PrescriptionModel()
		{
			Medicines = new List<FormExamineMedicineModel>();
		}
	}
}
