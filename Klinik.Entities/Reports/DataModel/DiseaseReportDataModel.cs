using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class DiseaseReportDataModel
    {
        public int ICDId { get; set; }

        public string ICDCode { get; set; }

        public string ICDName { get; set; }

        public string Category { get; set; }

        public int Total { get; set; }
    }
}
