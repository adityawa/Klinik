using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Models
{
    public class BaseModel
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}