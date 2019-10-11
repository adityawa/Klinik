using System;

namespace Klinik.Entities.Cashier
{
    public class CashierModel : BaseModel
    {
        public int CategoryId { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> Price { get; set; }
        public long PatientID { get; set; }
    }
}
