using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.MCUPackageEntities
{
    public class MCUPackageModel:BaseModel
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public int AgeStart { get; set; }
        public int AgeEnd { get; set; }
        public int GradeID { get; set; }
    }
}
