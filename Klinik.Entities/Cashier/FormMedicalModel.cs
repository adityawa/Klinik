using System;

namespace Klinik.Entities.Cashier
{
    public class FormMedicalModel : BaseModel
    {
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<double> DiscountPercent { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> BenefitPaid { get; set; }
        public string BenefitPlan { get; set; }
        public string Remark { get; set; }
    }
}
