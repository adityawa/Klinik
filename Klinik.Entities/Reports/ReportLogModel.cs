using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities
{
    public class ReportLogModel:BaseModel
    { 
        public byte[] ExcelResult { get; set; }

        public byte[] ChartResult { get; set; }
    }
}
