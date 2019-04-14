using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.PreExamine;
using System;
using System.Collections.Generic;

namespace Klinik.Entities.Poli
{
    public class PoliExamineModel : BaseModel
    {
        public LoketModel LoketData { get; set; }
        public PreExamineModel PreExamineData { get; set; }
        public FormExamineModel ExamineData { get; set; }
        public List<FormExamineLabModel> LabDataList { get; set; }
        public List<FormExamineLabModel> RadiologyDataList { get; set; }
        public List<FormExamineMedicineModel> MedicineDataList { get; set; }
        public List<FormExamineServiceModel> ServiceDataList { get; set; }
        public string PatientAge { get; set; }
        public string NecessityTypeStr { get; set; }
        public string PaymentTypeStr { get; set; }
        public string ConcoctionMedicine { get; set; }
        public int PoliToID { get; set; }
        public int DoctorToID { get; set; }

        public PoliExamineModel()
        {
            LoketData = new LoketModel();
            PreExamineData = new PreExamineModel();
            ExamineData = new FormExamineModel { TransDate = DateTime.Now };
            LabDataList = new List<FormExamineLabModel>();
            RadiologyDataList = new List<FormExamineLabModel>();
            MedicineDataList = new List<FormExamineMedicineModel>();
            ServiceDataList = new List<FormExamineServiceModel>();
        }
    }
}
