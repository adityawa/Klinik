using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Klinik.Web.Models.MasterData
{
    public class Organization : BaseModel
    {
        [Required(ErrorMessage = "Please enter Organization Code"), MaxLength(30)]
        [Display(Name ="Organization Code")]
        public string OrgCode { get; set; }

        [Required(ErrorMessage = "Please enter Organization Name"), MaxLength(50)]
        [Display(Name = "Organization Name")]
        public string OrgName { get; set; }

        [Display(Name = "Klinik")]
        public int KlinikID { get; set; }
    }
}