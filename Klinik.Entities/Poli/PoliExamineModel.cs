using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.PreExamine;

namespace Klinik.Entities.Poli
{
    public class PoliExamineModel : BaseModel
    {
        public LoketModel LoketData { get; set; }
        public PreExamineModel PreExamineData { get; set; }
        public FormExamineModel ExamineData { get; set; }
        public string PatientAge { get; set; }
        public string NecessityTypeStr { get; set; }
        public string PaymentTypeStr { get; set; }
    }
}
