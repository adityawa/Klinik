using Klinik.Entities;
using Klinik.Entities.MasterData;

namespace Klinik.Features
{
    public class UserRequest : BaseGetRequest
    {
        public UserModel RequestUserData { get; set; }
    }
}