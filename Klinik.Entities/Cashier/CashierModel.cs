using System;

namespace Klinik.Entities.Cashier
{
    public class CashierModel : BaseModel
    {
        public string ItemName { get; set; }
        public decimal? price { get; set; }
    }
}
