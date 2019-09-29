using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Klinik.Entities.Reports
{
    public class Top10DiseasesParamModel:BaseModel
    {
        public int MonthStart { get; set; }

        public int YearStart { get; set; }

        public int MonthEnd { get; set; }

        public int YearEnd { get; set; }

        public int? ClinicId { get; set; }

        public string SelectedCategory { get; set; }

        public string SelectedCategoryItem { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public List<SelectListItem> CategoryItems { get; set; }

        //public string DeptName { get; set; }

        //public string BUName { get; set; }

        //public string GenderType { get; set; }

        //public string AgeCode { get; set; }

        //public string PatientCategory { get; set; }

        //public string FamilyStatus { get; set; }

        //public string CategoryClinicStatus { get; set; }

        //public string PaymentType { get; set; }

        //public string NeedRest { get; set; }

        //public string ExamineType { get; set; }

    } 
}
