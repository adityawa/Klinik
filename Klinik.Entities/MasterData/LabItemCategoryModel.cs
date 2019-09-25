namespace Klinik.Entities.MasterData
{
    public class LabItemCategoryModel : BaseModel
    {
        public string LabType { get; set; }
        public int? PoliID { get; set; }
        public string PoliName { get; set; }
        public string Name { get; set; }
        public string CreatedDateStr { get; set; }
        public string ModifiedDateStr { get; set; }
    }
}
