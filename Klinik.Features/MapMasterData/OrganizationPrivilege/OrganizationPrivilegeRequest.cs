using Klinik.Entities;
using Klinik.Entities.MappingMaster;

namespace Klinik.Features
{
    public class OrganizationPrivilegeRequest : BaseGetRequest
    {
        public OrganizationPrivilegeModel RequestOrgPrivData { get; set; }
    }
}