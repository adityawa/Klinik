using Klinik.Entities;
using Klinik.Entities.Account;

namespace Klinik.Features
{
    public class PasswordHistoryRequest : BaseGetRequest
    {
        public PasswordHistoryModel RequestPassHistData { get; set; }
    }
}