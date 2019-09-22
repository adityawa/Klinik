﻿using DotNet.Highcharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class Top10DiseaseReportModel:BaseReportModel
    {
        public List<DiseaseReportDataModel> DiseaseDataReports { get; set; }
        public List<Highcharts> Charts { get; set; }
    }
}