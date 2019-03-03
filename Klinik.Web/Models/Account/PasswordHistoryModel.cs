using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Models.Account
{
    public class PasswordHistoryModel :AccountModel
    {
        public long OrganizationID { get; set; }
        public String NewPassword { get; set; }
    }
}