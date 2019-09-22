using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities
{
    public class BaseReportModel
    {
        public string ReportHeader { get; set; }

        public int TotalRecord { get; set; }

        public string ReportGenereated { get { return DateTime.Now.ToShortDateString();  } }
    }
}
