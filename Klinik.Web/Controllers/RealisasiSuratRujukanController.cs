using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.RealisasiSuratRujukanEntities;
using Klinik.Features.RealisasiSuratRujukan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class RealisasiSuratRujukanController : Controller
    {
        // GET: RealisasiSuratRujukan

        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public RealisasiSuratRujukanController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public ActionResult ListSurat()
        {
            return View();
        }

        public JsonResult GetListSuratRujukan()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new RealisasiSuratRujukanRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new RealisasiSuratRujukanModel()
            };
            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new RealisasiSuratRujukanHandler(_unitOfWork, _context).GetSuratRujukan(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
    }
}