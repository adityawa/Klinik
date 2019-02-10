using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Models
{
    public class BaseResponse<T> where T:class
    {
        public string draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<T> Data { get; set; }
        public T Entity { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}