using Klinik.Data.DataRepository;
using Klinik.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.MasterData
{
    public class EmployeeModel : BaseModel
    {
        [Required(ErrorMessage = "Please enter Employee ID"), MaxLength(50)]
        [Display(Name = "Employee ID")]
        public string EmpID { get; set; }
        [Required(ErrorMessage = "Please enter Employee Name"), MaxLength(100)]
        [Display(Name = "Name")]
        public string EmpName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        public string BirthdateStr { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter Employment Type")]
        public long EmpType { get; set; }

        public string EmpTypeDesc { get; set; }

        public long EmpDept { get; set; }

        public string EmpDeptDesc { get; set; }

        public List<GeneralMaster> lsMaster { get; set; }
    }
}