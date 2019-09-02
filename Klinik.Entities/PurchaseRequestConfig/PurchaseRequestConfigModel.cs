using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.PurchaseRequestConfig
{
    public class PurchaseRequestConfigModel : BaseModel
    {
        public int id { get; set; }
        public Nullable<int> GudangId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }

        public virtual GudangModel Gudang { get; set; }
    }
}
