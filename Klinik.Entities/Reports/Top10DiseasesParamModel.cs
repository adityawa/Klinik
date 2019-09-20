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

        public int? ClinicId { get; set; }

        public string DeptName { get; set; }

        public string BUName { get; set; }

        public string GenderType { get; set; }

        public string AgeCode { get; set; }

        public string PatientCategory { get; set; }

        public string FamilyStatus { get; set; }

        public string CategoryClinicStatus { get; set; }

        public string PaymentType { get; set; }

        public string NeedRest { get; set; }

        public string ExamineType { get; set; }

    } 
}
