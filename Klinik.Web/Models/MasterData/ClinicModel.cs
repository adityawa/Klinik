using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.Models;
namespace Klinik.Web.Models.MasterData
{
    public class ClinicModel :BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string LegalNumber { get; set; }
        public DateTime LegalDate { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public float Long { get; set; }
        public float Lat { get; set; }
        public int CityId { get; set; }
        public string ClinicType { get; set; }
        public int ReffID { get; set; }
        public int RowStatus { get; set; }
    }
}