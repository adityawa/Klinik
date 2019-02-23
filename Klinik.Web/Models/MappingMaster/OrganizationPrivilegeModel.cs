using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Klinik.Web.Models.MappingMaster
{
    public class OrganizationPrivilegeModel:BaseModel
    {
        [Required(ErrorMessage = "Organization ID must be filled")]
        [Display(Name ="Organization ID")]
        public long OrgID { get; set; }

        [Required(ErrorMessage = "Privilege ID must be filled")]
        public long PrivilegeID { get; set; }

        public List<long> PrivilegeIDs { get; set; }
       
        public string OrganizationName { get; set; }

        [Display(Name = "PrivilegeName")]
        public string PrivileveName { get; set; }

        public string PrivilegeDesc { get; set; }
    }
}