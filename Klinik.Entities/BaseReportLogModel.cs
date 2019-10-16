using Klinik.Data;
using Klinik.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities
{
    public class BaseReportLogModel
    {
        public AccountModel Account { get; set; }

        public List<string> Columns { get; set; }

        public string WorkSheetName { get; set; }

        public IUnitOfWork UnitOfWork { get; set; }
    }
}
