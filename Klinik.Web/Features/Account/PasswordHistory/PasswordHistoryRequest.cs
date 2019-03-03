using Klinik.Web.Models;
using Klinik.Web.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Features.Account.PasswordHistory
{
    public class PasswordHistoryRequest :BaseGetRequest
    {
        public PasswordHistoryModel RequestPassHistData { get; set; }
    }
}