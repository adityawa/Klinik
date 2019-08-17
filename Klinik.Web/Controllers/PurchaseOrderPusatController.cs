using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseOrderPusat;
using Klinik.Entities.PurchaseOrderPusatDetail;
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
    public class PurchaseOrderPusatController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;
        private const string ADD_M_PURCHASEORDERPUSAT = "ADD_M_PURCHASEORDERPUSAT";
        private const string EDIT_M_PURCHASEORDERPUSAT = "EDIT_M_PURCHASEORDERPUSAT";
        private const string DELETE_M_PURCHASEORDERPUSAT = "DELETE_M_PURCHASEORDERPUSAT";

        public PurchaseOrderPusatController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [CustomAuthorize("VIEW_M_PURCHASEORDERPUSAT")]
        public ActionResult PurchaseOrderPusatList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_PURCHASEORDERPUSAT")]
        [HttpPost]
        public ActionResult GetPurchaseOrderPusatData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PurchaseOrderPusatRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PurchaseOrderPusatHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_M_PURCHASEORDERPUSAT", "EDIT_M_PURCHASEORDERPUSAT")]
        public ActionResult CreateOrEditPurchaseOrderPusat()
        {
            PurchaseOrderPusatResponse _response = new PurchaseOrderPusatResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PurchaseOrderPusatRequest
                {
                    Data = new PurchaseOrderPusatModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PurchaseOrderPusatResponse resp = new PurchaseOrderPusatHandler(_unitOfWork).GetDetail(request);
                PurchaseOrderPusatModel _model = resp.Entity;
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
        public JsonResult CreateOrEditPurchaseOrderPusat(PurchaseOrderPusatModel _purchaseorderpusat, List<PurchaseOrderPusatDetailModel> purchaseOrderPusatDetailModels)
        {
            if (Session["UserLogon"] != null)
                _purchaseorderpusat.Account = (AccountModel)Session["UserLogon"];
            _purchaseorderpusat.Id = Convert.ToInt32(_purchaseorderpusat.Id) > 0 ? _purchaseorderpusat.Id : 0;
            var request = new PurchaseOrderPusatRequest
            {
                Data = _purchaseorderpusat
            };

            PurchaseOrderPusatResponse _response = new PurchaseOrderPusatResponse();

            new PurchaseOrderPusatValidator(_unitOfWork).Validate(request, out _response);
            if (purchaseOrderPusatDetailModels != null)
            {
                foreach (var item in purchaseOrderPusatDetailModels)
                {
                    var purchaseorderpusatdetailrequest = new PurchaseOrderPusatDetailRequest
                    {
                        Data = item
                    };
                    purchaseorderpusatdetailrequest.Data.PurchaseOrderPusatId = Convert.ToInt32(_response.Entity.Id);
                    purchaseorderpusatdetailrequest.Data.Account = (AccountModel)Session["UserLogon"];
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
                    purchaseorderpusatdetailrequest.Data.namabarang = namabarang.Entity.Name;
                    purchaseorderpusatdetailrequest.Data.namavendor = namavendor.Entity.namavendor;
                    PurchaseOrderPusatDetailResponse _purchaseorderpusatdetailresponse = new PurchaseOrderPusatDetailResponse();
                    new PurchaseOrderPusatDetailValidator(_unitOfWork).Validate(purchaseorderpusatdetailrequest, out _purchaseorderpusatdetailresponse);
                }
            }
            return Json(new { data = _response.Data }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("DELETE_M_PURCHASEORDERPUSAT")]
        [HttpPost]
        public JsonResult DeletePurchaseOrderPusat(int id)
        {
            PurchaseOrderPusatResponse _response = new PurchaseOrderPusatResponse();
            var request = new PurchaseOrderPusatRequest
            {
                Data = new PurchaseOrderPusatModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new PurchaseOrderPusatValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("EDIT_M_DELIVERYORDERPUSAT")]
        [HttpPost]
        public JsonResult ApprovePurchaseOrderPusat(int id)
        {
            PurchaseOrderPusatResponse _response = new PurchaseOrderPusatResponse();
            var request = new PurchaseOrderPusatRequest
            {
                Data = new PurchaseOrderPusatModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.APPROVE.ToString()
            };

            new PurchaseOrderPusatValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("VIEW_M_PURCHASEORDERPUSAT")]
        public ActionResult PrintPurchaseOrderPusat(int id)
        {
            PurchaseOrderPusatResponse _response = new PurchaseOrderPusatResponse();

            var request = new PurchaseOrderPusatRequest
            {
                Data = new PurchaseOrderPusatModel
                {
                    Id = id
                }
            };

            PurchaseOrderPusatResponse resp = new PurchaseOrderPusatHandler(_unitOfWork).GetDetail(request);
            PurchaseOrderPusatModel _model = resp.Entity;
            ViewBag.Response = _response;
            return new PartialViewAsPdf(_model)
            {
                PageOrientation = Orientation.Portrait,
                PageSize = Size.Folio,

                FileName = "PurchaseOrderPusat" + _model.ponumber + ".pdf"
            };
        }

        [CustomAuthorize("VIEW_M_PURCHASEORDERPUSAT")]
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
    }
}