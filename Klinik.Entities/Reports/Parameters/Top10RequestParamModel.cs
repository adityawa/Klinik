using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Klinik.Entities.Reports
{
    public class Top10RequestParamModel:BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "MonthYearOfStartPeriodIsMandatory")]
        public int MonthStart { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "YearOfStartPeriodIsMandatory")]
        public int YearStart { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "MonthYearOfEndPeriodIsMandatory")]
        public int MonthEnd { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "YearOfEndPeriodIsMandatory")]
        public int YearEnd { get; set; }

        public int? ClinicId { get; set; }

        public string WarehouseType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "CategoryIsMandatory")]
        public string SelectedCategory { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "CategoryItemIsMandatory")]
        public string SelectedCategoryItem { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public List<SelectListItem> CategoryItems { get; set; }
    }
}
