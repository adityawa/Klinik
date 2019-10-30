using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Administration
{
    public class LogonUserModel : BaseModel
    {
        public string IPAddress { get; set; }
        public string PCName { get; set; }
        public string UserName { get; set; }
        public string SessionID { get; set; }
        public bool Status { get; set; }
        public string Browser { get; set; }
    }
}
