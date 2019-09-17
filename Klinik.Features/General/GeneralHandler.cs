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

        public static string PurchaseRequestStatus(int? id)
        {
            var db = new KlinikDBEntities();
            var PR = db.PurchaseRequests.Where(a => a.id == id).FirstOrDefault();
            var status = "PR Created";

            if(PR.PurchaseOrders.Count > 0)
            {
                if (PR.PurchaseOrders.FirstOrDefault().DeliveryOrders.Count > 0)
                {
                    if (PR.PurchaseOrders.FirstOrDefault().DeliveryOrders.FirstOrDefault().Recived != null)
                    {
                        status = "Recived";
                    }
                    else if (PR.PurchaseOrders.First().Validasi != null)
                    {
                        status = "DO created and send";
                    }
                    else if (PR.PurchaseOrders.First().approve != null && PR.PurchaseOrders.First().Validasi == null)
                    {
                        status = "PO approved";
                    }
                    else if (PR.Validasi != null)
                    {
                        status = "PO created";
                    }
                    else if (PR.approve != null && PR.Validasi == null)
                    {
                        status = "PR approved";
                    }
                }
            }
            return status;
        }

        public static string PurchaseRequestPusatStatus(int id)
        {
            var db = new KlinikDBEntities();
            var PR = db.PurchaseRequestPusats.Where(a => a.id == id).FirstOrDefault();
            var status = "PRP Created";

            if (PR.PurchaseOrderPusats.Count > 0)
            {
                if (PR.PurchaseOrderPusats.FirstOrDefault().DeliveryOrderPusats.Count > 0)
                {
                    if (PR.PurchaseOrderPusats.FirstOrDefault().DeliveryOrderPusats.FirstOrDefault().Recived != null)
                    {
                        status = "Recived";
                    }
                    else if (PR.PurchaseOrderPusats.First().Validasi != null)
                    {
                        status = "DOP created and send";
                    }
                    else if (PR.PurchaseOrderPusats.First().approve != null && PR.PurchaseOrderPusats.First().Validasi == null)
                    {
                        status = "POP approved";
                    }
                    else if (PR.Validasi != null)
                    {
                        status = "POP created";
                    }
                    else if (PR.approve != null && PR.Validasi == null)
                    {
                        status = "PRP approved";
                    }
                }
            }
            return status;
        }
    }
}
