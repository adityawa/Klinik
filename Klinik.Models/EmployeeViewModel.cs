using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Klinik.Models
{
    public class EmployeeViewModel : BaseViewModel
    {
        [DisplayName("Employee ID")]
        [Required(ErrorMessage="Employee ID Required")]
        public string EmpID { get; set; }

        [DisplayName("Employee Name")]
        [Required(ErrorMessage = "Employee Name Required")]
        public string EmpName { get; set; }
        [Required(ErrorMessage = "Birthdate Required")]
        [DataType(DataType.Date)]
        
        public DateTime Birthdate { get; set; }

        public char Gender { get; set; }
        [DisplayName("Employee Type")]
        public int EmpType { get; set; }
        [DisplayName("Department")]
        public int EmpDept { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        public int RowStatus { get; set; }
    }
}
