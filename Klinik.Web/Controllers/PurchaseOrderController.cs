using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.PurchaseOrder;
using Klinik.Entities.PurchaseOrderDetail;
using Klinik.Features;
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
        private const string ADD_M_PURCHASEORDER = "ADD_M_PURCHASEORDER";
        private const string EDIT_M_PURCHASEORDER = "EDIT_M_PURCHASEORDER";
        private const string DELETE_M_PURCHASEORDER = "DELETE_M_PURCHASEORDER";
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

        [CustomAuthorize("ADD_M_DELIVERYORDER", "EDIT_M_DELIVERYORDER")]
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
            foreach (var item in purchaseOrderDetailModels)
            {
                var purchaseorderdetailrequest = new PurchaseOrderDetailRequest
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
    }
}