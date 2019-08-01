using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Letter
{
    public class PersetujuanTindakanModel:LetterModel
    {
        public string Decision { get; set; }
        public string Action { get; set; }
        public string Treatment { get; set; }
        public string ReferenceTo { get; set; }
        public string ResponsiblePerson { get; set; }
        public string SAPPatient { get; set; }
        public string NoSurat { get; set; }
        public string SAPPenjamin { get; set; }

        public string UmurPatient { get; set; }

        public string UmurPenjamin { get; set; }
        public EmployeeModel EmployeeData { get; set; }
        public PenjaminModel PenjaminData { get; set; }

        public string strPenjaminData { get; set; }
    }

    public class PenjaminModel
    {
        public string Nama { get; set; }
        public string Gender { get; set; }
        public string Umur { get; set; }
        public string Alamat { get; set; }
        public string SapId { get; set; }
        public string Telepon { get; set; }
        public string Sebagai { get; set; }
    }
}
