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
        public int? ProductId_Po { get; set; }
        public string namabarang_po { get; set; }
        public Nullable<double> qty_po { get; set; }
        public Nullable<double> qty_po_final { get; set; }
        public int ProductId { get; set; }
        public string namabarang { get; set; }
        public Nullable<int> GudangId { get; set; }
        public Nullable<long> ClinicId { get; set; }
        public Nullable<double> qty_do { get; set; }
        public string remark_do { get; set; }
        public Nullable<double> qty_adj { get; set; }
        public string remark_adj { get; set; }
        public string namagudang { get; set; }
        public string namaklinik { get; set; }
        public Nullable<int> type { get; set; }
    }
}
