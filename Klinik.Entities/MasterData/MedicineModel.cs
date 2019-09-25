namespace Klinik.Entities.MasterData
{
    public class MedicineModel : BaseModel
    {
        public string Name { get; set; }

        public string CreatedDateStr { get; set; }
        public string ModifiedDateStr { get; set; }
    }
}
