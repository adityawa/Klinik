using Klinik.Entities.MasterData;
using System;

namespace Klinik.Entities.ProductInGudang
{
    public class ProductInGudangModel : BaseModel
    {
        public Nullable<int> GudangId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> stock { get; set; }

        public string ProductName { get; set; }
        public string GudangName { get; set; }
        public Nullable<int> limited_stock { get; set; }
        public Nullable<decimal> RetailPrice { get; set; }

        public virtual GudangModel Gudang { get; set; }
        public virtual ProductModel Product { get; set; }
    }
}
