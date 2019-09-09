using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class Top10DiseasesParamModel
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public int ClinicId { get; set; }
    }
}
