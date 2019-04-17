namespace Klinik.Entities.MasterData
{
    public class LabItemModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? LabCategoryID { get; set; }
        public string Normal { get; set; }
        public decimal? Price { get; set; }
    }
}
