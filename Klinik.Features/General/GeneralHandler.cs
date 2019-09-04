using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Features.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class GeneralHandler
    {
        private static string[] _privilege_names;
        public static string FormatDate(DateTime dateTime)
        {
           return dateTime.ToString("MM/dd/yy");
        }

        public static string stringincrement(string lastnumber, DateTime getmonth)
        {
            var substring = lastnumber.Substring(lastnumber.Length - 5);

            var removezero = substring.TrimStart(new Char[] { '0' });
            int? newprnumber = Convert.ToInt32(removezero) + 1;
            char matchChar = '0';
            int? zeroCount = substring.Count(x => x == matchChar);
            string lenght = Convert.ToString(newprnumber);
            string zero = Regex.Replace(substring, "[1-9]", "");
            string zerofinal = zero;
            if (Convert.ToInt32((lenght.Length + zeroCount)) > 5)
            {
                int countremovezero = Convert.ToInt32(zeroCount) - (Convert.ToInt32(lenght.Length) - 1);
                zerofinal = zero.Remove(Convert.ToInt32(countremovezero));
            }
            if (getmonth.Month != DateTime.Now.Month)
            {
                newprnumber = 1;
            }
            string prnumber = zerofinal + Convert.ToString(newprnumber);
            return prnumber; 
        }

        public static string authorized(params string[] privilege_name)
        {
            var _context = new KlinikDBEntities();
            _privilege_names = privilege_name;
            var account = OneLoginSession.Account;
            string IsAuthorized = "false";
            List<long> PrivilegeIds = account.Privileges.PrivilegeIDs;
            var _getPrivilegeName = _context.Privileges.Where(x => PrivilegeIds.Contains(x.ID)).Select(x => x.Privilege_Name);

            var cek_authorizes = _getPrivilegeName.Where(p => _privilege_names.Contains(p.ToString()));
            if (cek_authorizes.Any())
            {
                IsAuthorized = "true";
            }

            return IsAuthorized;
        }
    }
}
