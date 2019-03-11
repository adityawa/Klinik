using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Administration
{
    public class LogModel :BaseModel
    {
        public DateTime Start { get; set; }
        public string strStart { get; set; }

        public string Module { get; set; }

        public string Command { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string Status { get; set; }

        public string UserName { get; set; }

        public string Organization { get; set; }
    }
}
