namespace Klinik.Entities.MasterData
{
    public class LabItemModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? LabItemCategoryID { get; set; }
        public string LabItemCategoryName { get; set; }
        public string Normal { get; set; }
        public decimal? Price { get; set; }
    }
}
