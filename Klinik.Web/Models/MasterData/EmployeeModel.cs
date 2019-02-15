using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Models.MasterData
{
    public class EmployeeModel :BaseModel
    {
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string Email { get; set; }
    }
}