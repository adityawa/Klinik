using Klinik.Data.DataRepository;

namespace Klinik.Entities.MasterData
{
    public class ProductMedicineModel : BaseModel
    {
        public Product Product { get; set; }
        public Medicine Medicine { get; set; }
        public int ProductID { get; set; }
        public int MedicineID { get; set; }
        public double? Amount { get; set; }
    }
}
