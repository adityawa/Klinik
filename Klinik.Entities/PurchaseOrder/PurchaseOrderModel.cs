using Klinik.Entities.PurchaseOrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.PurchaseOrder
{
    public class PurchaseOrderModel : BaseModel
    {
        public Nullable<int> PurchaseRequestId { get; set; }
        public string ponumber { get; set; }
        public Nullable<System.DateTime> podate { get; set; }
        public string request_by { get; set; }
        public string approve_by { get; set; }
        public Nullable<int> approve { get; set; }
        public Nullable<int> statusop { get; set; }
        public List<PurchaseOrderDetailModel> purchaseOrderdetailModels { get; set; }
        public PurchaseOrderModel()
        {
            purchaseOrderdetailModels = new List<PurchaseOrderDetailModel>();
        }
    }
}
