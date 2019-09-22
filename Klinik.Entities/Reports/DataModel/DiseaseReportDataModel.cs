using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Reports
{
    public class DiseaseReportDataModel
    {
        public int FormExamineId { get; set; }

        public int ICDId { get; set; }

        public int ClinicId { get; set; }

        public string ClinicName { get; set; }

        public string PatientName { get; set; }

        public string EmpName { get; set; }

        public string Department { get; set; }

        public string BusinessUnit { get; set; }

        public string Region { get; set; }

        public string StatusName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Gender { get; set; }
             
        public string BPJSNumber { get; set; }

        public decimal Age { get; set; }

        public string AgeCode { get; set; }

        public string FamCode { get; set; }

        public string FamName { get; set; }

        public DateTime TransDate { get; set; }

        public string NeedRest { get; set; }

        public string IsAccident { get; set; }

        public string Diagnose { get; set; }

        public string Necessity { get; set; }

        public string PaymentType { get; set; }

        public string ICDCode { get; set; }

    }
}
