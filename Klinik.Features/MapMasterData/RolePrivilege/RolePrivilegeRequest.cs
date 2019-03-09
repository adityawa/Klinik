using Klinik.Entities;
using Klinik.Entities.MappingMaster;

namespace Klinik.Features
{
    public class RolePrivilegeRequest : BaseGetRequest
    {
        public RolePrivilegeModel RequestRolePrivData { get; set; }
    }
}