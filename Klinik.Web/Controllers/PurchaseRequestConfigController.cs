using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.PurchaseRequestConfig;
using Klinik.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class PurchaseRequestConfigController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public PurchaseRequestConfigController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [CustomAuthorize("VIEW_M_PURCHASEREQUESTCONFIG")]
        public ActionResult PurchaseRequestConfigList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_PURCHASEREQUESTCONFIG")]
        [HttpPost]
        public ActionResult GetPurchaseRequestConfigData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PurchaseRequestConfigRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PurchaseRequestConfigHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_M_PURCHASEREQUESTCONFIG", "EDIT_M_PURCHASEREQUESTCONFIG")]
        public ActionResult CreateOrEditPurchaseRequestConfig()
        {
            PurchaseRequestResponse _response = new PurchaseRequestResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PurchaseRequestConfigRequest
                {
                    Data = new PurchaseRequestConfigModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PurchaseRequestConfigResponse resp = new PurchaseRequestConfigHandler(_unitOfWork).GetDetail(request);
                PurchaseRequestConfigModel _model = resp.Entity;
                ViewBag.Response = _response;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [CustomAuthorize("ADD_M_PURCHASEREQUESTCONFIG", "EDIT_M_PURCHASEREQUESTCONFIG")]
        [HttpPost]
        public ActionResult CreateOrEditPurchaseRequest(PurchaseRequestConfigModel _purchaserequestconfig)
        {
            _purchaserequestconfig.Account = (AccountModel)Session["UserLogon"];
            var request = new PurchaseRequestConfigRequest
            {
                Data = _purchaserequestconfig
            };

            PurchaseRequestConfigResponse _response = new PurchaseRequestConfigResponse();

            new PurchaseRequestConfigValidator(_unitOfWork).Validate(request, out _response);

            return RedirectToAction("PurchaseRequestConfigList");
        }

        [CustomAuthorize("DELETE_M_PURCHASEREQUESTCONFIG")]
        [HttpPost]
        public JsonResult DeletePurchaseRequestConfig(int id)
        {
            PurchaseRequestConfigResponse _response = new PurchaseRequestConfigResponse();
            var request = new PurchaseRequestConfigRequest
            {
                Data = new PurchaseRequestConfigModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new PurchaseRequestConfigValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}