﻿using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseRequestPusat;
using Klinik.Entities.PurchaseRequestPusatDetail;
using Klinik.Features;
using Klinik.Features.Account;
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

        [CustomAuthorize("ADD_M_PURCHASEREQUESTPUSAT", "EDIT_M_PURCHASEREQUESTPUSAT")]
        [HttpPost]
        public JsonResult CreateOrEditPurchaseRequestPusat(PurchaseRequestPusatModel _purchaserequestpusat, List<PurchaseRequestPusatDetailModel> purchaserequestpusatDetailModels)
        {
            if (Session["UserLogon"] != null)
                _purchaserequestpusat.Account = (AccountModel)Session["UserLogon"];
            _purchaserequestpusat.Id = Convert.ToInt32(_purchaserequestpusat.Id) > 0 ? _purchaserequestpusat.Id : 0;
            var request = new PurchaseRequestPusatRequest
            {
                Data = _purchaserequestpusat
            };

            PurchaseRequestPusatResponse _response = new PurchaseRequestPusatResponse();

            new PurchaseRequestPusatValidator(_unitOfWork).Validate(request, out _response);
            if (purchaserequestpusatDetailModels != null)
            {
                foreach (var item in purchaserequestpusatDetailModels)
                {
                    var purchaserequestpusatdetailrequest = new PurchaseRequestPusatDetailRequest
                    {
                        Data = item
                    };
                    purchaserequestpusatdetailrequest.Data.PurchaseRequestPusatId = Convert.ToInt32(_response.Entity.Id);
                    purchaserequestpusatdetailrequest.Data.Account = (AccountModel)Session["UserLogon"];
                    //
                    var requestnamabarang = new ProductRequest
                    {
                        Data = new ProductModel
                        {
                            Id = item.ProductId
                        }
                    };

                    var requestnamavendor = new VendorRequest
                    {
                        Data = new VendorModel
                        {
                            Id = item.VendorId
                        }
                    };

                    ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                    VendorResponse namavendor = new VendorHandler(_unitOfWork).GetDetail(requestnamavendor);
                    purchaserequestpusatdetailrequest.Data.namabarang = namabarang.Entity.Name;
                    purchaserequestpusatdetailrequest.Data.namavendor = namavendor.Entity.namavendor;
                    PurchaseRequestPusatDetailResponse _purchaserequestpusatdetailresponse = new PurchaseRequestPusatDetailResponse();
                    new PurchaseRequestPusatDetailValidator(_unitOfWork).Validate(purchaserequestpusatdetailrequest, out _purchaserequestpusatdetailresponse);
                }
            }
            return Json(new { data = _response.Data }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("APPROVE_M_PURCHASEREQUESTPUSAT")]
        [HttpPost]
        public JsonResult ApprovePurchaseRequestPusat(int id)
        {
            PurchaseRequestPusatResponse _response = new PurchaseRequestPusatResponse();
            var request = new PurchaseRequestPusatRequest
            {
                Data = new PurchaseRequestPusatModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.APPROVE.ToString()
            };

            new PurchaseRequestPusatValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
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

        [CustomAuthorize("VIEW_M_PURCHASEREQUESTPUSAT")]
        [HttpGet]
        public JsonResult GetStokdatabyProductId(int? productid)
        {
            var historyroductnGudangs = _context.HistoryProductInGudangs.Where(a => a.ProductId == productid && a.GudangId == OneLoginSession.Account.GudangID);
            var purchaseRequestdetails = _context.PurchaseRequestDetails.Where(a => a.ProductId == productid);
            var deliveryorderdetails = _context.DeliveryOrderDetails.Where(a => a.ProductId == productid && a.Recived == true);
            var productingudangs = _context.ProductInGudangs.Where(a => a.ProductId == productid && a.GudangId == OneLoginSession.Account.GudangID);
            int? stock = 0;
            int? datapo = 0;
            int? datado = 0;
            int? sisastock = 0;
            if(historyroductnGudangs.Count() > 0)
            {
                stock = _context.HistoryProductInGudangs.Where(a => a.ProductId == productid && a.GudangId == OneLoginSession.Account.GudangID).Sum(a => a.value);
            }
            if(purchaseRequestdetails.Count() > 0)
            {
                datapo = Convert.ToInt32(_context.PurchaseRequestDetails.Where(a => a.ProductId == productid).Sum(a => a.total));
            }
            if(deliveryorderdetails.Count() > 0)
            {
                datado = Convert.ToInt32(_context.DeliveryOrderDetails.Where(a => a.ProductId == productid && a.Recived == true).Sum(a => a.qty_request));
            }
            if (productingudangs.Count() > 0)
            {
                sisastock = Convert.ToInt32(_context.ProductInGudangs.Where(a => a.ProductId == productid && a.GudangId == OneLoginSession.Account.GudangID).Select(a => a.stock));
            }
            Dictionary<string, int?> data = new Dictionary<string, int?> {
                                            { "stock", stock },
                                            { "datapo", datapo },
                                            { "datado", datado },
                                            { "sisastock", sisastock },
                                        };

            return Json( new { data}, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}