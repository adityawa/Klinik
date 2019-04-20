using Klinik.Entities.MasterData;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using System.Collections.Generic;

namespace Klinik.Entities.Form
{
    public class FormExamineLabModel : BaseModel
    {
        public long? FormExamineID { get; set; }
        public LoketModel LoketData { get; set; }
        public string LabType { get; set; }
        public int? LabItemID { get; set; }
        public string LabItemDesc { get; set; }
        public string Result { get; set; }
        public string ResultIndicator { get; set; }
        public long FormMedicalID { get; set; }
        public PatientModel PatientData { get; set; }
        public List<LabItemModel> LabItemColls { get; set; }
    }
}
