using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.Models;
using Klinik.Web.Models.MappingMaster;

namespace Klinik.Web.Features.MapMasterData.OrganizationPrivilege
{
    public class OrganizationPrivilegeRequest :BaseGetRequest
    {
      public  OrganizationPrivilegeModel RequestOrgPrivData { get; set; }
    }
}