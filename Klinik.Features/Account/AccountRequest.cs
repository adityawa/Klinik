using Klinik.Entities.Account;

namespace Klinik.Features
{
    public class AccountRequest
    {
        public string IPAddress { get; set; }
        public string SessionID { get; set; }
        public string PCName { get; set; }

        public string BrowserName { get; set; }
        public AccountModel Data { get; set; }
    }
}