using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.DeliveryOrderPusat;
using Klinik.Entities.DeliveryOrderPusatDetail;
using Klinik.Entities.MasterData;
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
    public class DeliveryOrderPusatController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public DeliveryOrderPusatController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [CustomAuthorize("VIEW_M_DELIVERYORDERPUSAT")]
        // GET: DeliveryOrder
        public ActionResult DeliveryOrderPusatList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_DELIVERYORDERPUSAT")]
        [HttpPost]
        public ActionResult GetDeliveryOrderPusatData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new DeliveryOrderPusatRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new DeliveryOrderPusatHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_M_DELIVERYORDERPUSAT", "EDIT_M_DELIVERYORDERPUSAT")]
        public ActionResult CreateOrEditDeliveryOrderPusat()
        {
            DeliveryOrderPusatResponse _response = new DeliveryOrderPusatResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new DeliveryOrderPusatRequest
                {
                    Data = new DeliveryOrderPusatModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                DeliveryOrderPusatResponse resp = new DeliveryOrderPusatHandler(_unitOfWork).GetDetail(request);
                DeliveryOrderPusatModel _model = resp.Entity;
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

        [CustomAuthorize("ADD_M_DELIVERYORDERPUSAT", "EDIT_M_DELIVERYORDERPUSAT")]
        [HttpPost]
        public JsonResult CreateOrEditDeliveryOrderPusat(DeliveryOrderPusatModel _deliveryorder, List<DeliveryOrderPusatDetailModel> deliveryOrderDetailModels)
        {
            if (Session["UserLogon"] != null)
                _deliveryorder.Account = (AccountModel)Session["UserLogon"];
            _deliveryorder.Id = Convert.ToInt32(_deliveryorder.Id) > 0 ? _deliveryorder.Id : 0;
            var request = new DeliveryOrderPusatRequest
            {
                Data = _deliveryorder
            };

            DeliveryOrderPusatResponse _response = new DeliveryOrderPusatResponse();

            new DeliveryOrderPusatValidator(_unitOfWork).Validate(request, out _response);
            foreach (var item in deliveryOrderDetailModels)
            {
                var deliveryorderpusatdetailrequest = new DeliveryOrderPusatDetailRequest
                {
                    Data = item
                };
                deliveryorderpusatdetailrequest.Data.DeliveryOderPusatId = Convert.ToInt32(_response.Entity.Id);
                deliveryorderpusatdetailrequest.Data.Account = (AccountModel)Session["UserLogon"];
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

                var requestnamaklink = new ClinicRequest
                {
                    Data = new ClinicModel
                    {
                        Id = Convert.ToInt32(item.ClinicId)
                    }
                };
                var requestnamagudang = new GudangRequest
                {
                    Data = new GudangModel
                    {
                        Id = Convert.ToInt32(item.GudangId)
                    }
                };

                ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                ProductResponse namabarangpo = new ProductHandler(_unitOfWork).GetDetail(requestnamabarangpo);
                ClinicResponse namaklinik = new ClinicHandler(_unitOfWork).GetDetail(requestnamaklink);
                GudangResponse namagudang = new GudangHandler(_unitOfWork).GetDetail(requestnamagudang);
                deliveryorderpusatdetailrequest.Data.namabarang = namabarang.Entity.Name;
                deliveryorderpusatdetailrequest.Data.namabarang_po = namabarangpo.Entity.Name;
                deliveryorderpusatdetailrequest.Data.namaklinik = namaklinik.Entity.Name;
                deliveryorderpusatdetailrequest.Data.namagudang = namagudang.Entity.name;
                DeliveryOrderPusatDetailResponse _deliveryorderpusatdetailresponse = new DeliveryOrderPusatDetailResponse();
                new DeliveryOrderPusatDetailValidator(_unitOfWork).Validate(deliveryorderpusatdetailrequest, out _deliveryorderpusatdetailresponse);
            }

            return Json(new { data = _response.Data }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("DELETE_M_DELIVERYORDERPUSAT")]
        [HttpPost]
        public JsonResult DeleteDeliveryOrderPusat(int id)
        {
            DeliveryOrderPusatResponse _response = new DeliveryOrderPusatResponse();
            var request = new DeliveryOrderPusatRequest
            {
                Data = new DeliveryOrderPusatModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new DeliveryOrderPusatValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("EDIT_M_DELIVERYORDERPUSAT")]
        [HttpPost]
        public JsonResult ApproveDeliveryOrderPusat(int id)
        {
            DeliveryOrderPusatResponse _response = new DeliveryOrderPusatResponse();
            var request = new DeliveryOrderPusatRequest
            {
                Data = new DeliveryOrderPusatModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.APPROVE.ToString()
            };

            new DeliveryOrderPusatValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintDeliveryOrderPusat(int id)
        {
            DeliveryOrderPusatResponse _response = new DeliveryOrderPusatResponse();

            var request = new DeliveryOrderPusatRequest
            {
                Data = new DeliveryOrderPusatModel
                {
                    Id = id
                }
            };

            DeliveryOrderPusatResponse resp = new DeliveryOrderPusatHandler(_unitOfWork).GetDetail(request);
            DeliveryOrderPusatModel _model = resp.Entity;
            ViewBag.Response = _response;
            return new PartialViewAsPdf(_model)
            {
                PageOrientation = Orientation.Portrait,
                PageSize = Size.Folio,

                FileName = "DeliveryOrderPusat" + _model.donumber + ".pdf"
            };
        }
    }
}