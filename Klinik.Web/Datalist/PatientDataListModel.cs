using Datalist;
using System;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Web
{
    public class PatientDataListModel
    {
        [Key]
        public long Id { get; set; }

        [DatalistColumn]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DatalistColumn]
        [Display(Name = "MR Number")]
        public string MRNumber { get; set; }

        [DatalistColumn]
        [Display(Name = "KTP Number")]
        public string KTPNumber { get; set; }

        [Display(Name = "Birthdate")]
        [DatalistColumn(Format = "{0:dd/MM/yyyy}")]
        public DateTime Birthdate { get; set; }

        [DatalistColumn]
        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}