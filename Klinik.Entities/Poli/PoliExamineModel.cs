using Klinik.Entities.Document;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
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
        public List<FormExamineMedicineModel> InjectionDataList { get; set; }
        public List<FormExamineServiceModel> ServiceDataList { get; set; }
        public List<ServiceModel> DefaultServiceList { get; set; }
        public string PatientAge { get; set; }
        public string NecessityTypeStr { get; set; }
        public string PaymentTypeStr { get; set; }
        public string ConcoctionMedicine { get; set; }
        public int PoliToID { get; set; }
        public int DoctorToID { get; set; }
        public string ICDInformation1 { get; set; }
        public string ICDInformation1Desc { get; set; }
        public string ICDInformation2 { get; set; }
        public string ICDInformation2Desc { get; set; }
        public string ICDInformation3 { get; set; }
        public string ICDInformation3Desc { get; set; }
        public List<DocumentModel> fileData { get; set; }
        public string NoSurat { get; set; }

        public PoliExamineModel()
        {
            LoketData = new LoketModel();
            PreExamineData = new PreExamineModel();
            ExamineData = new FormExamineModel { TransDate = DateTime.Now };
            LabDataList = new List<FormExamineLabModel>();
            RadiologyDataList = new List<FormExamineLabModel>();
            MedicineDataList = new List<FormExamineMedicineModel>();
            InjectionDataList = new List<FormExamineMedicineModel>();
            ServiceDataList = new List<FormExamineServiceModel>();
            DefaultServiceList = new List<ServiceModel>();
            fileData = new List<DocumentModel>();
        }
    }
}
