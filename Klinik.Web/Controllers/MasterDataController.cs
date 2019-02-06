using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.Web.Features.MasterData.Organization;

namespace Klinik.Web.Controllers
{
    public class MasterDataController : Controller
    {
        private KlinikDBEntities clinicContext = new KlinikDBEntities();
        // GET: MasterData
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult OrganizationList()
        {
            return View();
        }

        public ActionResult CreateOrEditOrganization()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetOrganizationData()
        {
            var _draw= Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new OrganizationRequest
            {
                draw=_draw,
                searchValue=_searchValue,
                sortColumn=_sortColumn,
                sortColumnDir=_sortColumnDir,
                pageSize=_pageSize,
                skip=_skip
            };

            var response = new OrganizationHandler(clinicContext).GetOrganizationData(request);

            return Json(new { data = response.Data, recordsFiltered = response.recordsFiltered, recordsTotal = response.recordsTotal, draw = response.draw }, JsonRequestBehavior.AllowGet);


        }
    }
}