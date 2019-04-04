using System;

namespace Klinik.Entities.Form
{
    public class FormPreExamineModel : BaseModel
    {
        public long FormMedicalID { get; set; }
        public DateTime TransDate { get; set; }
        public int? DoctorID { get; set; }
        public string Anamnesa { get; set; }
        public double? Temperature { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public double? Respiratory { get; set; }
        public int? Pulse { get; set; }
        public int? Systolic { get; set; }
        public int? Diastolic { get; set; }
        public string Others { get; set; }
        public string RightEye { get; set; }
        public string LeftEye { get; set; }
        public string ColorBlind { get; set; }
        public DateTime? MenstrualDate { get; set; }
        public DateTime? KBDate { get; set; }
        public string DailyGlasses { get; set; }
        public string ExamineGlasses { get; set; }
    }
}
