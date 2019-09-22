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

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string ReportGenerated { get { return DateTime.Now.ToShortDateString();  } }
    }
}
