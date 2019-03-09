using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Klinik.Common
{
    /// <summary>
    /// Custom authorize attribute class
    /// </summary>
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] _privilege_names;
        private KlinikDBEntities _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="privilege_name"></param>
        public CustomAuthorizeAttribute(params string[] privilege_name)
        {
            this._privilege_names = privilege_name;

            _context = new KlinikDBEntities();
        }

        /// <summary>
        /// Override the onAuthorization
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var account = (AccountModel)filterContext.HttpContext.Session["UserLogon"];
            List<long> PrivilegeIds = account.Privileges.PrivilegeIDs;
            bool IsAuthorized = false;
            var _getPrivilegeName = _context.Privileges.Where(x => PrivilegeIds.Contains(x.ID)).Select(x => x.Privilege_Name);

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

        /// <summary>
        /// Override the Handle of unauthorized request
        /// </summary>
        /// <param name="filterContext"></param>
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



