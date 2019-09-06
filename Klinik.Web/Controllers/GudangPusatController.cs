using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.PurchaseRequestPusat;
using Klinik.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class GudangPusatController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public GudangPusatController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        // GET: GudangPusat
        #region ::PURCHASEREQUEST::
        [CustomAuthorize("VIEW_M_PURCHASEREQUESTPUSAT")]
        public ActionResult PurchaseRequestList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_PURCHASEREQUESTPUSAT")]
        [HttpPost]
        public ActionResult GetPurchaseRequestData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PurchaseRequestPusatRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PurchaseRequestPusatHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_M_PURCHASEREQUESTPUSAT", "EDIT_M_PURCHASEREQUESTPUSAT")]
        public ActionResult CreateOrEditPurchaseRequest()
        {
            var lastprnumber = _context.PurchaseRequestPusats.OrderByDescending(x => x.CreatedDate).Select(a => a.prnumber).FirstOrDefault();
            DateTime? getmonth = _context.PurchaseRequestPusats.OrderByDescending(x => x.CreatedDate).Select(a => a.CreatedDate).FirstOrDefault();
            DateTime? month = getmonth != null ? getmonth : DateTime.Now;
            string prnumber = lastprnumber != null ? GeneralHandler.stringincrement(lastprnumber, Convert.ToDateTime(month)) : "00001";
            PurchaseRequestPusatResponse _response = new PurchaseRequestPusatResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PurchaseRequestPusatRequest
                {
                    Data = new PurchaseRequestPusatModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PurchaseRequestPusatResponse resp = new PurchaseRequestPusatHandler(_unitOfWork).GetDetail(request);
                PurchaseRequestPusatModel _model = resp.Entity;
                ViewBag.Response = _response;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.prnumber = "PR" + ((AccountModel)Session["UserLogon"]).Organization + DateTime.Now.Year + DateTime.Now.Month + prnumber;
                return View();
            }
        }
        #endregion


        #region :: GENERAL ::
        [CustomAuthorize("VIEW_M_PURCHASEREQUESTPUSAT")]
        [HttpGet]
        public JsonResult searchvendor(string prefix)
        {
            var _draw = "1";
            var _start = "0";
            var _length = "10";
            var _sortColumn = "Id";
            var _sortColumnDir = "asc";
            var _searchValue = prefix;

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new VendorRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };
            var response = new VendorResponse();
            if (request.SearchValue != null)
            {
                response = new VendorHandler(_unitOfWork).GetListData(request);
            }

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}