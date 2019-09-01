using Klinik.Entities.MappingMaster;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.Account
{
    public class AccountModelSession
    {
        [Required(ErrorMessage = "Please fill User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Fill Password")]
        public string Password { get; set; }
        public string Email { get; set; }
        public string ResetPasswordCode { get; set; }
        public long UserID { get; set; }
        public long EmployeeID { get; set; }
        public List<long> Roles { get; set; }
        public RolePrivilegeModel Privileges { get; set; }
        public ClinicPoliModel Polis { get; set; }
        public string Organization { get; set; }
        public string UserCode
        {
            get
            {
                return Organization + "-" + UserName;
            }
        }

        public long ClinicID { get; set; }

        public int GudangID { get; set; }

        public AccountModelSession()
        {
            Roles = new List<long>();
            Privileges = new RolePrivilegeModel();
            Polis = new ClinicPoliModel();
        }
    }
}