using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.DeliveryOrder;
using Klinik.Entities.DeliveryOrderDetail;
using Klinik.Entities.HistoryProductInGudang;
using Klinik.Entities.MasterData;
using Klinik.Entities.ProductInGudang;
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
            DeliveryOrderDetailResponse _deliveryorderdetailresponse = new DeliveryOrderDetailResponse();
            if (deliveryOrderDetailModels != null) { 
                foreach (var item in deliveryOrderDetailModels)
                {
                    var deliveryorderdetailrequest = new DeliveryOrderDetailRequest
                    {
                        Data = item
                    };
                    deliveryorderdetailrequest.Data.Account = (AccountModel)Session["UserLogon"];
                    //
                    var requestnamabarang = new ProductRequest
                    {
                        Data = new ProductModel
                        {
                            Id = item.ProductId
                        }
                    };

                    ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                    deliveryorderdetailrequest.Data.namabarang = namabarang.Entity.Name;
                    new DeliveryOrderDetailValidator(_unitOfWork).Validate(deliveryorderdetailrequest, out _deliveryorderdetailresponse);
                }
            }

            return Json(new { data = _deliveryorderdetailresponse.Data }, JsonRequestBehavior.AllowGet);
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

        public ActionResult ReceivedOrder(int id)
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
            resp.Entity.Account = (AccountModel)Session["UserLogon"];
            resp.Entity.Recived = 1;
            var receiveddeliveryorder = new DeliveryOrderRequest
            {
                Data = resp.Entity
            };
            new DeliveryOrderValidator(_unitOfWork).Validate(receiveddeliveryorder, out _response);
            DeliveryOrderModel _model = resp.Entity;
            foreach(var item in resp.Entity.deliveryOrderDetailModels )
            {
                var requestproductingudang = new ProductInGudangRequest
                {
                    Data = new ProductInGudangModel
                    {
                        Account = (AccountModel)Session["UserLogon"],
                        stock = Convert.ToInt32(item.qty_request) > 0 ? Convert.ToInt32(item.qty_request) : Convert.ToInt32(item.qty_request),
                    }
                };

                var requesthistoryproductingudang = new HistoryProductInGudangRequest
                {
                    Data = new HistoryProductInGudangModel
                    {
                        Account = (AccountModel)Session["UserLogon"],
                        value = Convert.ToInt32(item.qty_request) > 0 ? Convert.ToInt32(item.qty_request) : Convert.ToInt32(item.qty_request),
                    }
                };

                var saveproductingudang = new ProductInGudangHandler(_unitOfWork).CreateOrEdit(requestproductingudang);
                var savehistoryproductingudang = new HistoryProductInGudangHandler(_unitOfWork).CreateOrEdit(requesthistoryproductingudang);
            }
            return RedirectToAction("DeliveryOrderList");
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
                Skip = _skip,
                Data = new ProductModel
                {
                    Account = (AccountModel)Session["UserLogon"]
                }
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

        #region ::DELIVERYORDERDETAIL::
        [CustomAuthorize("EDIT_M_PURCHASEREQUEST")]
        [HttpPost]
        public ActionResult EditDeliveryOrderDetail(DeliveryOrderDetailModel deliveryorderdetail)
        {
            if (Session["UserLogon"] != null)
                deliveryorderdetail.Account = (AccountModel)Session["UserLogon"];
            DeliveryOrderDetailResponse _purchaseorderdetailresponse = new DeliveryOrderDetailResponse();
            var purchaserequestdetailrequest = new DeliveryOrderDetailRequest
            {
                Data = deliveryorderdetail
            };
            var requestnamabarang = new ProductRequest
            {
                Data = new ProductModel
                {
                    Id = Convert.ToInt32(deliveryorderdetail.ProductId)
                }
            };

            ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
            purchaserequestdetailrequest.Data.namabarang = purchaserequestdetailrequest.Data.namabarang != null ? purchaserequestdetailrequest.Data.namabarang : namabarang.Entity.Name;
            new DeliveryOrderDetailValidator(_unitOfWork).Validate(purchaserequestdetailrequest, out _purchaseorderdetailresponse);
            return Json(new { data = _purchaseorderdetailresponse.Data }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}