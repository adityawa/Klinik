using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.PurchaseRequestDetail
{
    public class PurchaseRequestDetailModel : BaseModel
    {
        public int PurchaseRequestId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string namabarang { get; set; }
        public Nullable<double> tot_pemakaian { get; set; }
        public Nullable<double> sisa_stok { get; set; }
        public Nullable<double> qty { get; set; }
        public Nullable<double> qty_add { get; set; }
        public string reason_add { get; set; }
        public double total { get; set; }
        public string nama_by_ho { get; set; }
        public Nullable<double> qty_by_ho { get; set; }
        public string remark_by_ho { get; set; }
        public Nullable<int> statusop { get; set; }
    }
}
