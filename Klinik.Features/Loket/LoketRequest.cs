using Klinik.Entities;
using Klinik.Entities.Loket;

namespace Klinik.Features
{
    public class LoketRequest : BaseRequest<LoketModel>
    {
        public PanggilanPoliModel CallRequest { get; set; }
    }
}
