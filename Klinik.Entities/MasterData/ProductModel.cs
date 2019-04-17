namespace Klinik.Entities.MasterData
{
    public class ProductModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProductCategoryID { get; set; }
        public int ProductUnitID { get; set; }
        public decimal RetailPrice { get; set; }
    }
}
