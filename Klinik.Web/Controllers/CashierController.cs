using Klinik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class CashierController : Controller
    {
        // GET: Cachier
        #region ::Cachier::
        [CustomAuthorize("VIEW_M_CASHIER")]
        public ActionResult ListPatien()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Detail(long? patienid)
        {

            return View();
        }
        #endregion
    }
}