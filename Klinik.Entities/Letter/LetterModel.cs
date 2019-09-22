using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Klinik.Entities.MasterData;

namespace Klinik.Entities.Letter
{
    public class LetterModel : BaseModel
    {
        [Required(ErrorMessage = "Please fill Letter Type")]
        public string LetterType { get; set; }

        [Required(ErrorMessage = "Please Fill Number")]
        public Int64 AutoNumber { get; set; }

        [Required(ErrorMessage = "Please Fill Year")]
        public int Year { get; set; }
        [Required]
        public long ForPatient { get; set; }
        public long FormMedicalID { get; set; }
        public DateTime? Cekdate { get; set; }
        public string strCekdate { get; set; }
        public PatientModel PatientData { get; set; }
    }
}
