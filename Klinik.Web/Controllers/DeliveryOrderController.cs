using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.DeliveryOrder;
using Klinik.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class DeliveryOrderController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public DeliveryOrderController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        // GET: DeliveryOrder
        public ActionResult DeliveryOrderList()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult GetDeliveryOrderData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new DeliveryOrderRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new DeliveryOrderHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateOrEditDeliveryOrder()
        {
            DeliveryOrderResponse _response = new DeliveryOrderResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new DeliveryOrderRequest
                {
                    Data = new DeliveryOrderModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                DeliveryOrderResponse resp = new DeliveryOrderHandler(_unitOfWork).GetDetail(request);
                DeliveryOrderModel _model = resp.Entity;
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

        [HttpPost]
        public ActionResult CreateOrEditDeliveryOrder(DeliveryOrderModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new DeliveryOrderRequest
            {
                Data = _model
            };

            DeliveryOrderResponse _response = new DeliveryOrderResponse();

            new DeliveryOrderValidator(_unitOfWork).Validate(request, out _response);

            return RedirectToAction("GudangList");
        }

        [HttpPost]
        public JsonResult DeleteDeliveryOrder(int id)
        {
            DeliveryOrderResponse _response = new DeliveryOrderResponse();
            var request = new DeliveryOrderRequest
            {
                Data = new DeliveryOrderModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new DeliveryOrderValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}