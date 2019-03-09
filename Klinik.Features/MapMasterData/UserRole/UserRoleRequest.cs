using Klinik.Entities;
using Klinik.Entities.MappingMaster;

namespace Klinik.Features
{
    public class UserRoleRequest : BaseGetRequest
    {
        public UserRoleModel RequestUserRoleData { get; set; }
    }
}