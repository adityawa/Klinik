using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.DeliveryOrder;
using Klinik.Entities.DeliveryOrderDetail;
using Klinik.Entities.MasterData;
using Klinik.Features;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [CustomAuthorize("VIEW_M_DELIVERYORDER")]
        // GET: DeliveryOrder
        public ActionResult DeliveryOrderList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_DELIVERYORDER")]
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

        [CustomAuthorize("ADD_M_DELIVERYORDER", "EDIT_M_DELIVERYORDER")]
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

        [CustomAuthorize("ADD_M_DELIVERYORDER", "EDIT_M_DELIVERYORDER")]
        [HttpPost]
        public JsonResult CreateOrEditDeliveryOrder(DeliveryOrderModel _deliveryorder, List<DeliveryOrderDetailModel> deliveryOrderDetailModels)
        {
            if (Session["UserLogon"] != null)
                _deliveryorder.Account = (AccountModel)Session["UserLogon"];
            _deliveryorder.Id = Convert.ToInt32(_deliveryorder.Id) > 0 ? _deliveryorder.Id : 0;
            var request = new DeliveryOrderRequest
            {
                Data = _deliveryorder
            };

            DeliveryOrderResponse _response = new DeliveryOrderResponse();

            new DeliveryOrderValidator(_unitOfWork).Validate(request, out _response);
            foreach (var item in deliveryOrderDetailModels)
            {
                var deliveryorderdetailrequest = new DeliveryOrderDetailRequest
                {
                    Data = item
                };
                deliveryorderdetailrequest.Data.DeliveryOderId = Convert.ToInt32(_response.Entity.Id);
                deliveryorderdetailrequest.Data.Account = (AccountModel)Session["UserLogon"];
                //
                var requestnamabarang = new ProductRequest
                {
                    Data = new ProductModel
                    {
                        Id = item.ProductId
                    }
                };
                var requestnamabarangpo = new ProductRequest
                {
                    Data = new ProductModel
                    {
                        Id = Convert.ToInt32(item.ProductId_Po)
                    }
                };

                ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                ProductResponse namabarangpo = new ProductHandler(_unitOfWork).GetDetail(requestnamabarangpo);
                deliveryorderdetailrequest.Data.namabarang = namabarang.Entity.Name;
                deliveryorderdetailrequest.Data.namabarang_po = namabarangpo.Entity.Name;
                DeliveryOrderDetailResponse _deliveryorderdetailresponse = new DeliveryOrderDetailResponse();
                new DeliveryOrderDetailValidator(_unitOfWork).Validate(deliveryorderdetailrequest, out _deliveryorderdetailresponse);
            }

            return Json(new { data = _response.Data }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("DELETE_M_DELIVERYORDER")]
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

        [CustomAuthorize("EDIT_M_DELIVERYORDER")]
        [HttpPost]
        public JsonResult ApproveDeliveryOrder(int id)
        {
            DeliveryOrderResponse _response = new DeliveryOrderResponse();
            var request = new DeliveryOrderRequest
            {
                Data = new DeliveryOrderModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.APPROVE.ToString()
            };

            new DeliveryOrderValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintDeliveryOrder(int id)
        {
            DeliveryOrderResponse _response = new DeliveryOrderResponse();

            var request = new DeliveryOrderRequest
            {
                Data = new DeliveryOrderModel
                {
                    Id = id
                }
            };

            DeliveryOrderResponse resp = new DeliveryOrderHandler(_unitOfWork).GetDetail(request);
            DeliveryOrderModel _model = resp.Entity;
            ViewBag.Response = _response;
            return new PartialViewAsPdf(_model)
            {
                PageOrientation = Orientation.Portrait,
                PageSize = Size.Folio,

                FileName = "DeliveryOrder" + _model.donumber + ".pdf"
            };
        }

        [HttpGet]
        public JsonResult searchproduct(string prefix)
        {
            var _draw = "1";
            var _start = "0";
            var _length = "10";
            var _sortColumn = "Id";
            var _sortColumnDir = "asc";
            var _searchValue = prefix;

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };
            var response = new ProductResponse();
            if (request.SearchValue != null)
            {
                response = new ProductHandler(_unitOfWork).GetListData(request);
            }

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult searchgudang(string prefix)
        {
            var _draw = "1";
            var _start = "0";
            var _length = "10";
            var _sortColumn = "Id";
            var _sortColumnDir = "asc";
            var _searchValue = prefix;

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new GudangRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };
            var response = new GudangResponse();
            if (request.SearchValue != null)
            {
                response = new GudangHandler(_unitOfWork).GetListData(request);
            }

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult searchklinik(string prefix)
        {
            var _draw = "1";
            var _start = "0";
            var _length = "10";
            var _sortColumn = "Id";
            var _sortColumnDir = "asc";
            var _searchValue = prefix;

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ClinicRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };
            var response = new ClinicResponse();
            if (request.SearchValue != null)
            {
                response = new ClinicHandler(_unitOfWork).GetAllData(request);
            }

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
    }
}