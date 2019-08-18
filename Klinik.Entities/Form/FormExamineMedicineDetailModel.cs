namespace Klinik.Entities.Form
{
	public class FormExamineMedicineDetailModel : BaseModel
    {
		public long? FormExamineMedicineID { get; set; }
		public int? ProductID { get; set; }		
		public string ProductName { get; set; }
		public double? Qty { get; set; }		
		public string Note { get; set; }
		public string ProcessType { get; set; }
	}
}
