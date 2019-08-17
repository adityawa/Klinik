using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseOrder;
using Klinik.Entities.PurchaseOrderDetail;
using Klinik.Features;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        // GET: PurchaseOrder
        public PurchaseOrderController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [CustomAuthorize("VIEW_M_PURCHASEORDER")]
        public ActionResult PurchaseOrderList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_PURCHASEORDER")]
        [HttpPost]
        public ActionResult GetPurchaseOrderData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PurchaseOrderRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PurchaseOrderHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_M_PURCHASEORDER", "EDIT_M_PURCHASEORDER")]
        public ActionResult CreateOrEditPurchaseOrder()
        {
            PurchaseOrderResponse _response = new PurchaseOrderResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PurchaseOrderRequest
                {
                    Data = new PurchaseOrderModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PurchaseOrderResponse resp = new PurchaseOrderHandler(_unitOfWork).GetDetail(request);
                PurchaseOrderModel _model = resp.Entity;
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

        [CustomAuthorize("ADD_M_PURCHASEORDER", "EDIT_M_PURCHASEORDER")]
        [HttpPost]
        public JsonResult CreateOrEditPurchaseOrder(PurchaseOrderModel _purchaseorder, List<PurchaseOrderDetailModel> purchaseOrderDetailModels)
        {
            if (Session["UserLogon"] != null)
                _purchaseorder.Account = (AccountModel)Session["UserLogon"];
            _purchaseorder.Id = Convert.ToInt32(_purchaseorder.Id) > 0 ? _purchaseorder.Id : 0;
            var request = new PurchaseOrderRequest
            {
                Data = _purchaseorder
            };

            PurchaseOrderResponse _response = new PurchaseOrderResponse();

            new PurchaseOrderValidator(_unitOfWork).Validate(request, out _response);
            if(purchaseOrderDetailModels != null) { 
                foreach (var item in purchaseOrderDetailModels)
                {
                    var purchaseorderdetailrequest = new PurchaseOrderDetailRequest
                    {
                        Data = item
                    };
                    purchaseorderdetailrequest.Data.PurchaseOrderId = Convert.ToInt32(_response.Entity.Id);
                    purchaseorderdetailrequest.Data.Account = (AccountModel)Session["UserLogon"];
                    //
                    var requestnamabarang = new ProductRequest
                    {
                        Data = new ProductModel
                        {
                            Id = item.ProductId
                        }
                    };

                    ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                    purchaseorderdetailrequest.Data.namabarang = namabarang.Entity.Name;
                    PurchaseOrderDetailResponse _purchaseorderdetailresponse = new PurchaseOrderDetailResponse();
                    new PurchaseOrderDetailValidator(_unitOfWork).Validate(purchaseorderdetailrequest, out _purchaseorderdetailresponse);
                }
            }
            return Json(new { data = _response.Data }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("DELETE_M_PURCHASEORDER")]
        [HttpPost]
        public JsonResult DeletePurchaseOrder(int id)
        {
            PurchaseOrderResponse _response = new PurchaseOrderResponse();
            var request = new PurchaseOrderRequest
            {
                Data = new PurchaseOrderModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new PurchaseOrderValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("EDIT_M_DELIVERYORDER")]
        [HttpPost]
        public JsonResult ApprovePurchaseOrder(int id)
        {
            PurchaseOrderResponse _response = new PurchaseOrderResponse();
            var request = new PurchaseOrderRequest
            {
                Data = new PurchaseOrderModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.APPROVE.ToString()
            };

            new PurchaseOrderValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("VIEW_M_PURCHASEORDER")]
        public ActionResult PrintPurchaseOrder(int id)
        {
            PurchaseOrderResponse _response = new PurchaseOrderResponse();

            var request = new PurchaseOrderRequest
            {
                Data = new PurchaseOrderModel
                {
                    Id = id
                }
            };

            PurchaseOrderResponse resp = new PurchaseOrderHandler(_unitOfWork).GetDetail(request);
            PurchaseOrderModel _model = resp.Entity;
            ViewBag.Response = _response;
            return new PartialViewAsPdf(_model)
            {
                PageOrientation = Orientation.Portrait,
                PageSize = Size.Folio,

                FileName = "PurchaseOrder" + _model.ponumber + ".pdf"
            };
        }
    }
}