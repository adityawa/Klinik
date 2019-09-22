namespace Klinik.Entities.MasterData
{
    public class ProductModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProductCategoryID { get; set; }
        public int ProductUnitID { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductUnitName { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal stock { get; set; }
        public string CreatedDateStr { get; set; }
        public string ModifiedDateStr { get; set; }
    }
}
