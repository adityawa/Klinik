using System;

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
        public Nullable<int> SatuanTerbesar { get; set; }
        public Nullable<int> QtyConvertion { get; set; }
    }
}
