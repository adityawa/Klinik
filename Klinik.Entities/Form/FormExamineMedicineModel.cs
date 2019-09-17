using Klinik.Entities.Loket;

namespace Klinik.Entities.Form
{
    public class FormExamineMedicineModel : BaseModel
    {
        public long? FormExamineID { get; set; }
        public string TypeID { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public double? Qty { get; set; }
        public string ConcoctionMedicine { get; set; }
		public string Dose { get; set; }
		public string RemarkUse { get; set; }
        public decimal Stock { get; set; }
        public long FormMedicalID { get; set; }

        public string MedicineJenis { get; set; }

        public string Process { get; set; }
        public LoketModel LoketData { get; set; }
		public FormExamineMedicineDetailModel Detail { get; set; }
       

		public FormExamineMedicineModel()
		{
			Detail = new FormExamineMedicineDetailModel();
		}
	}
}
