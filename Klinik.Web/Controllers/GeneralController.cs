using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class GeneralController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public GeneralController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public JsonResult searchpurchaseorder(string prefix)
        {
            var _draw = "1";
            var _start = "0";
            var _length = "10";
            var _sortColumn = "Id";
            var _sortColumnDir = "asc";
            var _searchValue = prefix;

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
            var response = new PurchaseOrderResponse();
            if (request.SearchValue != null)
            {
                response = new PurchaseOrderHandler(_unitOfWork).GetListData(request);
            }

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult searchpurchaseorderpusat(string prefix)
        {
            var _draw = "1";
            var _start = "0";
            var _length = "10";
            var _sortColumn = "Id";
            var _sortColumnDir = "asc";
            var _searchValue = prefix;

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
            var response = new PurchaseOrderPusatResponse();
            if (request.SearchValue != null)
            {
                response = new PurchaseOrderPusatHandler(_unitOfWork).GetListData(request);
            }

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult searchpurchaserequest(string prefix)
        {
            var _draw = "1";
            var _start = "0";
            var _length = "10";
            var _sortColumn = "Id";
            var _sortColumnDir = "asc";
            var _searchValue = prefix;

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
            var response = new PurchaseRequestResponse();
            if (request.SearchValue != null)
            {
                response = new PurchaseRequestHandler(_unitOfWork).GetListData(request);
            }

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult searchpurchaserequestpusat(string prefix)
        {
            var _draw = "1";
            var _start = "0";
            var _length = "10";
            var _sortColumn = "Id";
            var _sortColumnDir = "asc";
            var _searchValue = prefix;

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
            var response = new PurchaseRequestPusatResponse();
            if (request.SearchValue != null)
            {
                response = new PurchaseRequestPusatHandler(_unitOfWork).GetListData(request);
            }

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
    }
}