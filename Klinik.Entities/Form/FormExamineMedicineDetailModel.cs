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

    public class KomponenObatRacikan
    {
        public string Id { get; set; }
        public string Amount { get; set; }
        public string Name { get; set; }
        public string Idx { get; set; }
        public string ParentID { get; set; }
        public string Stock { get; set; }
    }
}
