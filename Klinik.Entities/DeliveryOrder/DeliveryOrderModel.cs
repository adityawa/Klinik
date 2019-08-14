using Klinik.Entities.DeliveryOrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.DeliveryOrder
{
    public class DeliveryOrderModel : BaseModel
    {
        public int poid { get; set; }
        public string donumber { get; set; }
        public Nullable<System.DateTime> dodate { get; set; }
        public string dodest { get; set; }
        public Nullable<int> approve_by { get; set; }
        public Nullable<int> approve { get; set; }
        public List<DeliveryOrderDetailModel> deliveryOrderDetailModels { get; set; }
        public DeliveryOrderModel()
        {
            deliveryOrderDetailModels = new List<DeliveryOrderDetailModel>();
        }

    }
}
