using System;

namespace Klinik.Entities.MasterData
{
    public class PatientModel : BaseModel
    {
        public string MRNumber { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime BirthDate { get; set; }
        public string KTPNumber { get; set; }
        public string Address { get; set; }
        public int CityID { get; set; }
        public short Type { get; set; }
        public string BPJSNumber { get; set; }
        public string BloodType { get; set; }
    }
}
