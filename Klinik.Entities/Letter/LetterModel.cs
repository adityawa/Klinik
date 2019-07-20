using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Klinik.Entities.Letter
{
    public class LetterModel : BaseModel
    {
        [Required(ErrorMessage = "Please fill Letter Type")]
        public string LetterType { get; set; }

        [Required(ErrorMessage ="Please Fill Number")]
        public Int64 AutoNumber { get; set; }

        [Required(ErrorMessage = "Please Fill Year")]
        public int Year { get; set; }
        [Required]
       public long ForPatient { get; set; }
    }
}
