using Klinik.Entities;
using Klinik.Entities.MasterData;

namespace Klinik.Features
{
    public class ProductRequest : BaseRequest<ProductModel>
    {
        public bool IsForShowInFarmasi { get; set; }
    }
}