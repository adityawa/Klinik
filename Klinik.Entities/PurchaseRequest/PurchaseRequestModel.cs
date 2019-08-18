using Klinik.Entities.PurchaseRequestDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.PurchaseRequest
{
    public class PurchaseRequestModel : BaseModel
    {
        public string prnumber { get; set; }
        public Nullable<System.DateTime> prdate { get; set; }
        public string request_by { get; set; }
        public string approve_by { get; set; }
        public Nullable<int> approve { get; set; }
        public Nullable<int> statusop { get; set; }
        public List<PurchaseRequestDetailModel> purchaserequestdetailModels { get; set; }

        public string createformat { get; set; }
        public PurchaseRequestModel()
        {
            purchaserequestdetailModels = new List<PurchaseRequestDetailModel>();
        }
    }
}
