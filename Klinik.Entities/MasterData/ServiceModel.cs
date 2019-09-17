namespace Klinik.Entities.MasterData
{
    public class ServiceModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
    }
}
