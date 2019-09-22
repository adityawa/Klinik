using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseRequestPusat;
using Klinik.Entities.PurchaseRequestPusatDetail;
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
    public class PurchaseRequestPusatController : Controller
    {
        private const string ADD_M_PURCHASEREQUESTPUSAT = "ADD_M_PURCHASEREQUESTPUSAT";
        private const string EDIT_M_PURCHASEREQUESTPUSAT = "EDIT_M_PURCHASEREQUESTPUSAT";
        private const string DELETE_M_PURCHASEREQUESTPUSAT = "DELETE_M_PURCHASEREQUESTPUSAT";

        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public PurchaseRequestPusatController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [CustomAuthorize("VIEW_M_PURCHASEREQUESTPUSAT")]
        public ActionResult PurchaseRequestPusatList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_PURCHASEREQUESTPUSAT")]
        [HttpPost]
        public ActionResult GetPurchaseRequestPusatData()
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
        public ActionResult CreateOrEditPurchaseRequestPusat()
        {
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

        [CustomAuthorize("DELETE_M_PURCHASEREQUESTPUSAT")]
        [HttpPost]
        public JsonResult DeletePurchaseRequestPusat(int id)
        {
            PurchaseRequestPusatResponse _response = new PurchaseRequestPusatResponse();
            var request = new PurchaseRequestPusatRequest
            {
                Data = new PurchaseRequestPusatModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new PurchaseRequestPusatValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("EDIT_M_PURCHASEREQUESTPUSAT")]
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

        [CustomAuthorize("VIEW_M_PURCHASEREQUESTPUSAT")]
        public ActionResult PrintPurchaseRequestPusat(int id)
        {
            PurchaseRequestPusatResponse _response = new PurchaseRequestPusatResponse();

            var request = new PurchaseRequestPusatRequest
            {
                Data = new PurchaseRequestPusatModel
                {
                    Id = id
                }
            };

            PurchaseRequestPusatResponse resp = new PurchaseRequestPusatHandler(_unitOfWork).GetDetail(request);
            PurchaseRequestPusatModel _model = resp.Entity;
            ViewBag.Response = _response;
            return new PartialViewAsPdf(_model)
            {
                PageOrientation = Orientation.Portrait,
                PageSize = Size.Folio,

                FileName = "PurchaseRequestPusat" + _model.prnumber + ".pdf"
            };
        }
    }
}