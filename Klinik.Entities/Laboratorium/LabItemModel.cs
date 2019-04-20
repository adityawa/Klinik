using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Laboratorium
{
    public class LabItemModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int LabCategoryID { get; set; }
        public string LabCategory { get; set; }
        public string Normal { get; set; }
        public string Price { get; set; }
    }
}
