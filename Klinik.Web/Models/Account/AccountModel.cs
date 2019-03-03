using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.Models.MappingMaster;
using System.ComponentModel.DataAnnotations;
namespace Klinik.Web.Models.Account
{
    public class AccountModel
    {
        [Required(ErrorMessage ="Please fill User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Please Fill Password")]
        public string Password { get; set; }
        public long UserID { get; set; }
        public long EmployeeID { get; set; }
        public List<long> Roles { get; set; }
        public RolePrivilegeModel Privileges { get; set; }
    }
}