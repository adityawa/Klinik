﻿using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseOrder;
using Klinik.Entities.PurchaseOrderDetail;
using Klinik.Entities.PurchaseRequest;
using Klinik.Entities.PurchaseRequestDetail;
using Klinik.Features;
using Klinik.Features.PurchaseRequest;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                Skip = _skip,
                Data = new PurchaseRequestModel
                {
                    Account = (AccountModel)Session["UserLogon"]
                }
            };

            var response = new PurchaseRequestHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_M_PURCHASEREQUEST", "EDIT_M_PURCHASEREQUEST", "VIEW_M_PURCHASEREQUEST")]
        public ActionResult CreateOrEditPurchaseRequest()
        {
            var lastprnumber = _context.PurchaseRequests.OrderByDescending(x => x.CreatedDate).Select(a => a.prnumber).FirstOrDefault();
            DateTime? getmonth = _context.PurchaseRequests.OrderByDescending(x => x.CreatedDate).Select(a => a.CreatedDate).FirstOrDefault();
            DateTime? month = getmonth != null ? getmonth : DateTime.Now;
            string prnumber = lastprnumber != null ?  GeneralHandler.stringincrement(lastprnumber, Convert.ToDateTime(month)) : "00001";
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
                if((GeneralHandler.authorized("ADD_M_PURCHASEREQUEST") == "false"))
                {
                    return RedirectToAction("PurchaseRequestList");
                }
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.prnumber = "PR" + ((AccountModel)Session["UserLogon"]).Organization + DateTime.Now.Year + DateTime.Now.Month + prnumber;
                return View();
            }
        }

        [CustomAuthorize("ADD_M_PURCHASEREQUEST", "EDIT_M_PURCHASEREQUEST")]
        [HttpPost]
        public JsonResult CreateOrEditPurchaseRequest(PurchaseRequestModel _purchaserequest, List<PurchaseRequestDetailModel> purchaserequestDetailModels)
        {
            if (Session["UserLogon"] != null)
                _purchaserequest.Account = (AccountModel)Session["UserLogon"];
            _purchaserequest.Id = Convert.ToInt32(_purchaserequest.Id) > 0 ? _purchaserequest.Id : 0;
            var gudangid = _unitOfWork.GudangRepository.Query(a => a.ClinicId == _purchaserequest.Account.ClinicID).Select(x => x.id).FirstOrDefault();
            _purchaserequest.GudangId = gudangid > 0 ? gudangid : 0;
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
                            Id = Convert.ToInt32(item.ProductId)
                        }
                    };

                    ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                    purchaserequestdetailrequest.Data.namabarang = namabarang.Entity.Name ;
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

        [CustomAuthorize("APPROVE_M_PURCHASEREQUEST")]
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

        [CustomAuthorize("VALIDATION_M_PURCHASEREQUEST")]
        [HttpPost]
        public JsonResult ValidationPurchaseRequest(int id)
        {
            PurchaseRequestResponse _response = new PurchaseRequestResponse();
            var request = new PurchaseRequestRequest
            {
                Data = new PurchaseRequestModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.VALIDASI.ToString()
            };

            new PurchaseRequestValidator(_unitOfWork).Validate(request, out _response);
            _response.Entity.Account = (AccountModel)Session["UserLogon"];
            new  CreatePoByPr(_unitOfWork).Create(_response);

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

        [CustomAuthorize("VIEW_M_PURCHASEREQUEST")]
        public JsonResult GetStatus(int? id)
        {
            var status = GeneralHandler.PurchaseRequestStatus(id);
            return Json(new { data = status }, JsonRequestBehavior.AllowGet);
        }

        #region ::PURCHASEREQUESTDETAIL::
        [CustomAuthorize("EDIT_M_PURCHASEREQUEST")]
        [HttpPost]
        public ActionResult EditPurchaseRequestDetail(PurchaseRequestDetailModel purchaseRequestDetail)
        {
            if (Session["UserLogon"] != null)
                purchaseRequestDetail.Account = (AccountModel)Session["UserLogon"];
            PurchaseRequestDetailResponse _purchaserequestdetailresponse = new PurchaseRequestDetailResponse();
            var purchaserequestdetailrequest = new PurchaseRequestDetailRequest
            {
                Data = purchaseRequestDetail
            };
            var requestnamabarang = new ProductRequest
            {
                Data = new ProductModel
                {
                    Id = Convert.ToInt32(purchaseRequestDetail.ProductId)
                }
            };

            ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
            purchaserequestdetailrequest.Data.namabarang = purchaserequestdetailrequest.Data.namabarang != null ? purchaserequestdetailrequest.Data.namabarang : namabarang.Entity.Name;
            new PurchaseRequestDetailValidator(_unitOfWork).Validate(purchaserequestdetailrequest, out _purchaserequestdetailresponse);
            return Json(new { data = _purchaserequestdetailresponse.Data }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PURCHASEREQUESTADDNEWPROCUT::
        [CustomAuthorize("ADD_M_PURCHASEORDER", "EDIT_M_PURCHASEORDER")]
        [HttpPost]
        public JsonResult CreateOrEditNewProduct(ProductModel productRequest)
        {
            if (Session["UserLogon"] != null)
                productRequest.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductRequest
            {
                Data = productRequest
            };

            ProductResponse _response = new ProductResponse();

            new ProductValidator(_unitOfWork).Validate(request, out _response);
            return Json(new { data = _response.Entity }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}