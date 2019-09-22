using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.HistoryProductInGudang
{
    public class HistoryProductInGudangModel : BaseModel
    {
        public int ProductId { get; set; }
        public int GudangId { get; set; }
        public int value { get; set; }

        public virtual GudangModel Gudang { get; set; }
        public virtual ProductModel Product { get; set; }
    }
}
