using Klinik.Entities;
using Klinik.Entities.MasterData;

namespace Klinik.Features
{
    public class EmployeeRequest : BaseGetRequest
    {
        public EmployeeModel RequestEmployeeData { get; set; }
    }
}