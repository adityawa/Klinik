namespace Klinik.Entities
{
    public class BaseGetRequest
    {
        public string draw { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDir { get; set; }
        public string searchValue { get; set; }
        public string action { get; set; }       
    }
}