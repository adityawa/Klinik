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
        public string ponumber { get; set; }
        public Nullable<int> poid { get; set; }
        public Nullable<int> doid { get; set; }
        public Nullable<System.DateTime> podate { get; set; }
        public string donumber { get; set; }
        public Nullable<System.DateTime> dodate { get; set; }
        public List<PurchaseRequestDetailModel> purchaserequestdetailModels { get; set; }

        public string createformat { get; set; }
        public string createdo { get; set; }
        public string createpo { get; set; }
        public Nullable<int> Validasi { get; set; }
        public Nullable<int> Recived { get; set; }
        public string namaklinik { get; set; }
        public string namaGudang { get; set; }
        public Nullable<int> GudangId { get; set; }
        public PurchaseRequestModel()
        {
            purchaserequestdetailModels = new List<PurchaseRequestDetailModel>();
        }

        public string status { get; set; }
    }
}
