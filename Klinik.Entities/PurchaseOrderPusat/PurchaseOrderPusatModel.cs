using Klinik.Entities.PurchaseOrderPusatDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.PurchaseOrderPusat
{
    public class PurchaseOrderPusatModel : BaseModel
    {
        public Nullable<int> PurchaseRequestId { get; set; }
        public string ponumber { get; set; }
        public Nullable<System.DateTime> podate { get; set; }
        public string request_by { get; set; }
        public string approve_by { get; set; }
        public Nullable<int> approve { get; set; }
        public Nullable<int> statusop { get; set; }
        public List<PurchaseOrderPusatDetailModel> purchaseOrderdetailpusatModels { get; set; }
        public PurchaseOrderPusatModel()
        {
            purchaseOrderdetailpusatModels = new List<PurchaseOrderPusatDetailModel>();
        }
    }
}
