using System.Collections.Generic;

namespace Klinik.Entities
{
    public class BaseResponse<T> where T : class
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