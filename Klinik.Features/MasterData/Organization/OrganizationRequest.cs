using Klinik.Entities;
using Klinik.Entities.MasterData;

namespace Klinik.Features
{
    public class OrganizationRequest : BaseGetRequest
    {
        public OrganizationModel RequestOrganizationData { get; set; }
    }
}