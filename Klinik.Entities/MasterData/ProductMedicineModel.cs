using Klinik.Data.DataRepository;

namespace Klinik.Entities.MasterData
{
    public class ProductMedicineModel : BaseModel
    {
        public int ProductID { get; set; }
        public int MedicineID { get; set; }
        public string ProductName { get; set; }
        public string MedicineName { get; set; }
        public double? Amount { get; set; }
        public string CreatedDateStr { get; set; }
        public string ModifiedDateStr { get; set; }
    }
}
