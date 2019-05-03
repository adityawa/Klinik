namespace Klinik.Entities.MasterData
{
    public class PoliServiceModel : BaseModel
    {
        public int? ServicesID { get; set; }
        public string ServicesName { get; set; }
        public long? ClinicID { get; set; }
        public string ClinicName { get; set; }
        public int? PoliID { get; set; }
        public string PoliName { get; set; }
    }
}
