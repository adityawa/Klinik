using Klinik.Entities.Document;
using Klinik.Entities.Patient;
using System;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.MasterData
{
    public class PatientModel : BaseModel
    {
        public long EmployeeID { get; set; }
        public long familyRelationsgipID { get; set; }
        public string familyRelationshipDesc { get; set; }
        public string MRNumber { get; set; }
        [Required(ErrorMessage = "Please Enter a name")]
        public string Name { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Please Enter a birthdate")]
        public string BirthDateStr { get; set; }

        public string KTPNumber { get; set; }
        [Required(ErrorMessage = "Please Enter an Address")]
        public string Address { get; set; }
        public int CityID { get; set; }
        public short Type { get; set; }
        public string BPJSNumber { get; set; }
        public string BloodType { get; set; }

        public string PatientKey { get; set; }

        public DocumentModel Photo { get; set; }
        public bool IsaSamePerson { get; set; }
        public PatientClinicModel PatientClinic { get; set; }

    }
}
