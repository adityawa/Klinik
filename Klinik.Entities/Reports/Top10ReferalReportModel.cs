using DotNet.Highcharts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Klinik.Entities.Reports
{
    public class Top10ReferalReportModel:BaseReportModel
    {
        public long ProcessId { get; set; }

        public List<ReferalReportDataModel> ReferalReportDataModels { get; set; }

        public List<Highcharts> Charts { get; set; }

        public string Category { get; set; }
    }
}
