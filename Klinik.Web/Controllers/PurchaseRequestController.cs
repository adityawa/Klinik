using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseRequest;
using Klinik.Entities.PurchaseRequestDetail;
using Klinik.Features;
using Klinik.Features.PurchaseRequest;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class PurchaseRequestController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        // GET: PurchaseOrder
        public PurchaseRequestController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [CustomAuthorize("VIEW_M_PURCHASEREQUEST")]
        public ActionResult PurchaseRequestList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_PURCHASEREQUEST")]
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

            var request = new PurchaseRequestRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PurchaseRequestHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_M_PURCHASEREQUEST", "EDIT_M_PURCHASEREQUEST")]
        public ActionResult CreateOrEditPurchaseRequest()
        {
            PurchaseRequestResponse _response = new PurchaseRequestResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PurchaseRequestRequest
                {
                    Data = new PurchaseRequestModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PurchaseRequestResponse resp = new PurchaseRequestHandler(_unitOfWork).GetDetail(request);
                PurchaseRequestModel _model = resp.Entity;
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
        public JsonResult CreateOrEditPurchaseRequest(PurchaseRequestModel _purchaserequest, List<PurchaseRequestDetailModel> purchaserequestDetailModels)
        {
            if (Session["UserLogon"] != null)
                _purchaserequest.Account = (AccountModel)Session["UserLogon"];
            _purchaserequest.Id = Convert.ToInt32(_purchaserequest.Id) > 0 ? _purchaserequest.Id : 0;
            var request = new PurchaseRequestRequest
            {
                Data = _purchaserequest
            };

            PurchaseRequestResponse _response = new PurchaseRequestResponse();

            new PurchaseRequestValidator(_unitOfWork).Validate(request, out _response);
            if (purchaserequestDetailModels != null)
            {
                foreach (var item in purchaserequestDetailModels)
                {
                    var purchaserequestdetailrequest = new PurchaseRequestDetailRequest
                    {
                        Data = item
                    };
                    purchaserequestdetailrequest.Data.PurchaseRequestId = Convert.ToInt32(_response.Entity.Id);
                    purchaserequestdetailrequest.Data.Account = (AccountModel)Session["UserLogon"];
                    //
                    var requestnamabarang = new ProductRequest
                    {
                        Data = new ProductModel
                        {
                            Id = item.ProductId
                        }
                    };

                    ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                    purchaserequestdetailrequest.Data.namabarang = namabarang.Entity.Name;
                    PurchaseRequestDetailResponse _purchaserequestdetailresponse = new PurchaseRequestDetailResponse();
                    new PurchaseRequestDetailValidator(_unitOfWork).Validate(purchaserequestdetailrequest, out _purchaserequestdetailresponse);
                }
            }
            return Json(new { data = _response.Data }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("DELETE_M_PURCHASEREQUEST")]
        [HttpPost]
        public JsonResult DeletePurchaseRequest(int id)
        {
            PurchaseRequestResponse _response = new PurchaseRequestResponse();
            var request = new PurchaseRequestRequest
            {
                Data = new PurchaseRequestModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new PurchaseRequestValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("EDIT_M_PURCHASEREQUEST")]
        [HttpPost]
        public JsonResult ApprovePurchaseRequest(int id)
        {
            PurchaseRequestResponse _response = new PurchaseRequestResponse();
            var request = new PurchaseRequestRequest
            {
                Data = new PurchaseRequestModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.APPROVE.ToString()
            };

            new PurchaseRequestValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("VIEW_M_PURCHASEREQUEST")]
        public ActionResult PrintPurchaseRequest(int id)
        {
            PurchaseRequestResponse _response = new PurchaseRequestResponse();

            var request = new PurchaseRequestRequest
            {
                Data = new PurchaseRequestModel
                {
                    Id = id
                }
            };

            PurchaseRequestResponse resp = new PurchaseRequestHandler(_unitOfWork).GetDetail(request);
            PurchaseRequestModel _model = resp.Entity;
            ViewBag.Response = _response;
            return new PartialViewAsPdf(_model)
            {
                PageOrientation = Orientation.Portrait,
                PageSize = Size.Folio,

                FileName = "PurchaseRequest" + _model.prnumber + ".pdf"
            };
        }
    }
}