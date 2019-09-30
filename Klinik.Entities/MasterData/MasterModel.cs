using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.MasterData
{
    public class MasterModel:BaseModel 
    {
        public int CategoryId { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
