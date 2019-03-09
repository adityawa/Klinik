using Klinik.Entities;
using Klinik.Entities.MasterData;

namespace Klinik.Features
{
    public class RoleRequest : BaseGetRequest
    {
        public RoleModel RequestRoleData { get; set; }
    }
}