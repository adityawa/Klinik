using Klinik.Web.DataAccess;
using Klinik.Web.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.Web.DataAccess.DataRepository;
using System.Web.Routing;

namespace Klinik.Web.Infrastructure
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] _privilege_names;
        private KlinikDBEntities _context;

        public CustomAuthorizeAttribute(params string[] privilege_name)
        {
            this._privilege_names = privilege_name;

            _context = new KlinikDBEntities();
        }



        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var account = (AccountModel)filterContext.HttpContext.Session["UserLogon"];
            List<long> PrivilegeIds = account.Privileges.PrivilegeIDs;
            bool IsAuthorized = false;
            var _getPrivilegeName = _context.Privileges.Where(x => PrivilegeIds.Contains(x.ID)).Select(x=>x.Privilege_Name);

            var cek_authorizes = _getPrivilegeName.Where(p => _privilege_names.Contains(p.ToString()));

            if (cek_authorizes.Any())
            {
                IsAuthorized = true;
            }
            //foreach (var item in _getPrivilegeName)
            //{
            //    if (this._privilege_name == item.Privilege_Name)
            //    {
            //        IsAuthorized = true;
            //    }

            //}

            if (!IsAuthorized)
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            filterContext.Result = new RedirectToRouteResult(
                                       new RouteValueDictionary
                                       {
                                       { "action", "Index" },
                                       { "controller", "Unauthorized" }
                                       });
        }

    }
}



