using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Models
{
    public class BaseViewModel
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
