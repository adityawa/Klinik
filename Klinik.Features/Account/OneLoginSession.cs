using Klinik.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Klinik.Features.Account
{
    public class OneLoginSession
    {
        public static AccountModel Account
        {
            get; set;
        }
    }
}
