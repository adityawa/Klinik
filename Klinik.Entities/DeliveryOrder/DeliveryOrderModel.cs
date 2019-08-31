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
        public Nullable<int> poid { get; set; }
        public string donumber { get; set; }
        public Nullable<System.DateTime> dodate { get; set; }
        public string dodest { get; set; }
        public Nullable<int> approve_by { get; set; }
        public Nullable<int> approve { get; set; }
        public List<DeliveryOrderDetailModel> deliveryOrderDetailModels { get; set; }
        public string createformat { get; set; }
        public Nullable<int> Recived { get; set; }
        public Nullable<int> GudangId { get; set; }
        public Nullable<int> SourceId { get; set; }
        public string gudangasal { get; set; }
        public string gudangtujuan { get; set; }
        public string sendby { get; set; }
        public string ponumber { get; set; }
        public Nullable<System.DateTime> podate { get; set; }
        public string processby { get; set; }
        public string prnumber { get; set; }
        public Nullable<System.DateTime> prdate { get; set; }
        public string prrequestby { get; set; }
        public DeliveryOrderModel()
        {
            deliveryOrderDetailModels = new List<DeliveryOrderDetailModel>();
        }

    }
}
