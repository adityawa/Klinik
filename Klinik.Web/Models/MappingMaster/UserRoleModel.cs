using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Models.MappingMaster
{
    public class UserRoleModel:BaseModel
    {
        public long UserID { get; set; }
        public long RoleID { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }

        public List<long> RoleIds { get; set; }
    }
}