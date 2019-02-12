using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Klinik.Web.Models.MasterData
{
    public class RoleModel :BaseModel
    {
        [Required]
        public long OrgID { get; set; }
        public string OrganizationName { get; set; }
        [Required(ErrorMessage = "Please enter Role Name"), MaxLength(30)]
        public string RoleName { get; set; }

    }
}