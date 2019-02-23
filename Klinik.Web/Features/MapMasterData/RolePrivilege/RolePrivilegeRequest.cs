using Klinik.Web.Models;
using Klinik.Web.Models.MappingMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Features.MapMasterData.RolePrivilege
{
    public class RolePrivilegeRequest:BaseGetRequest
    {
        public RolePrivilegeModel RequestRolePrivData { get; set; }
    }
}