using Klinik.Entities;
using Klinik.Entities.MasterData;

namespace Klinik.Features
{
    public class PrivilegeRequest : BaseGetRequest
    {
        public PrivilegeModel RequestPrivilegeData { get; set; }
    }
}