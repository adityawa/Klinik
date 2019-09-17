using Klinik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        [CustomAuthorize("VIEW_REPORTS")]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize("VIEW_TOP_10_DISEASES")]
        public ActionResult Top10Dieases()
        {
            return View();
        }

        [CustomAuthorize("VIEW_TOP_10_REFERALS")]
        public ActionResult Top10Referals()
        {
            return View();
        }
    }
}