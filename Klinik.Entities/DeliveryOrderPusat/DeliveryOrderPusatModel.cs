using Klinik.Entities.DeliveryOrderPusatDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.DeliveryOrderPusat
{
    public class DeliveryOrderPusatModel : BaseModel
    {
        public int poid { get; set; }
        public string donumber { get; set; }
        public Nullable<System.DateTime> dodate { get; set; }
        public string dodest { get; set; }
        public Nullable<int> approve_by { get; set; }
        public Nullable<int> approve { get; set; }
        public Nullable<int> GudangId { get; set; }
        public string createformat { get; set; }
        public Nullable<int> Validasi { get; set; }
        public string ponumber { get; set; }
        public Nullable<System.DateTime> podate { get; set; }
        public string prnumber { get; set; }
        public string namagundang { get; set; }
        public Nullable<System.DateTime> prdate { get; set; }
        public List<DeliveryOrderPusatDetailModel> deliveryOrderDetailpusatModels { get; set; }
        public DeliveryOrderPusatModel()
        {
            deliveryOrderDetailpusatModels = new List<DeliveryOrderPusatDetailModel>();
        }
    }
}
