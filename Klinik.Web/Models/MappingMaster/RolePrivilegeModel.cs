using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Models.MappingMaster
{
    public class RolePrivilegeModel:BaseModel
    {
        public long RoleID { get; set; }
        public long PrivilegeID { get; set; }
        public string RoleDesc { get; set; }
        public string PrivilegeName { get; set; }
        public string PrivilegeDesc { get; set; }

        public List<long> PrivilegeIDs { get; set; }
    }
}