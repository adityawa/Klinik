using System.Collections.Generic;

namespace Klinik.Entities.MappingMaster
{
    public class ClinicPoliModel : BaseModel
    {
        public long ClinicID { get; set; }
        
        public int PoliID { get; set; }

        public List<int> PoliIDs { get; set; }

        public string ClinicName { get; set; }

        public string PoliName { get; set; }

        public string PoliCode { get; set; }

        public ClinicPoliModel()
        {
            PoliIDs = new List<int>();
        }
    }
}
