using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Features;
using Klinik.Features.Laboratorium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class LabController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        #region ::DROPDOWN::
        protected List<SelectListItem> BindLabCategory(string _poliNm)
        {
            List<SelectListItem> _dataList = new List<SelectListItem>();
            foreach (var item in new LabHandler(_unitOfWork).GetLaboratoriumCategory(_poliNm).ToList())
            {
                _dataList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _dataList;
        }
        #endregion

        public LabController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        // GET: Lab
        public ActionResult ListQueueLaboratorium()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetListQueue(string poli, string preexamine)
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new LoketRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new LoketModel { PoliToID = Convert.ToInt32(poli), strIsPreExamine = preexamine }

            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new LabHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateItemLab()
        {
            LabResponse response = new LabResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new LabRequest
                {
                    Data = new FormExamineLabModel
                    {
                        LoketData = new LoketModel
                        {
                            Id = long.Parse(Request.QueryString["id"].ToString()),
                        },

                    }
                };
                if (Session["UserLogon"] != null)
                        request.Data.Account = (AccountModel)Session["UserLogon"];

                   LabResponse resp = new LabHandler(_unitOfWork).GetDetailPatient(request.Data.LoketData.Id);
                   FormExamineLabModel _model = resp.Entity;

                   //ViewBag.Doctors = BindDropDownDokter();
                   return View(_model);
            }
            ViewBag.LabCategory = BindLabCategory(Constants.NameConstant.Laboratorium);
            return View();
        }
    }
}