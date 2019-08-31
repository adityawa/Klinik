using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.DeliveryOrderDetail
{
    public class DeliveryOrderDetailModel : BaseModel
    {
        public int DeliveryOderId { get; set; }
        public int ProductId { get; set; }
        public string namabarang { get; set; }
        public Nullable<double> qty_request { get; set; }
        public string nama_by_ho { get; set; }
        public Nullable<double> qty_by_HP { get; set; }
        public string remark_by_ho { get; set; }
        public Nullable<double> qty_adj { get; set; }
        public string remark_adj { get; set; }

        public Nullable<bool> Recived { get; set; }
    }
}
