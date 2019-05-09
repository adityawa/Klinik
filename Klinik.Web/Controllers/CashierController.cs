using Klinik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.Features.Cashier;
using Klinik.Entities.Cashier;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Features;
using Klinik.Entities.Loket;
using Klinik.Entities.Account;

namespace Klinik.Web.Controllers
{
    public class CashierController : Controller
    {

        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;
        private ClinicHandler _clinicHandler;
        #region ::DROPDOWN::
        private List<SelectListItem> BindDropDownStatus()
        {

            List<SelectListItem> _status = new List<SelectListItem>();
            _status.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = "-1"
            });

            _status.Insert(1, new SelectListItem
            {
                Text = "New",
                Value = "0"
            });

            _status.Insert(2, new SelectListItem
            {
                Text = "Process",
                Value = "1"
            });

            _status.Insert(3, new SelectListItem
            {
                Text = "Hold",
                Value = "2"
            });

            _status.Insert(4, new SelectListItem
            {
                Text = "Finish",
                Value = "3"
            });

            return _status;
        }

        private List<SelectListItem> BindDropDownClinic()
        {
            List<SelectListItem> _authorizedClinics = new List<SelectListItem>();
            if (Session["UserLogon"] != null)
            {
                var _account = (AccountModel)Session["UserLogon"];

                var _getClinics = _clinicHandler.GetAllClinic(_account.ClinicID);
                foreach (var item in _getClinics)
                {
                    _authorizedClinics.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
            }

            return _authorizedClinics;
        }
        #endregion
        public CashierController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public CashierController(IUnitOfWork unitOfWork, KlinikDBEntities context, ClinicHandler clinicHandler)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _clinicHandler = clinicHandler;
        }
        // GET: Cachier
        #region ::Cachier::
        [CustomAuthorize("VIEW_M_CASHIER")]
        public ActionResult ListPatien()
        {
            ViewBag.Status = BindDropDownStatus();
            ViewBag.Clinics = BindDropDownClinic();
            return View();
        }

        [CustomAuthorize("VIEW_M_CASHIER")]
        [HttpPost]
        public ActionResult GetListQueue(string clinics, string status)
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
                Data = new LoketModel { ClinicID = Convert.ToInt32(clinics), Status = Convert.ToInt32(status) }

            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new CashierHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("VIEW_M_CASHIER")]
        [HttpGet]
        public ActionResult Detail(long medicalid)
        {
            var response = new CashierHandler(_unitOfWork).GetDetail(medicalid);
            var formmedical = _unitOfWork.FormMedicalRepository.Get(a => a.ID == medicalid).FirstOrDefault();
            ViewBag.Formmedicalid = formmedical;
            ViewBag.Detail = response.Data;

            if (response.Data != null) { ViewBag.Sum = response.Data.Sum(a => a.Price); } else { ViewBag.Sum = ""; };
            return View(formmedical);
        }

        [HttpPost]
        public ActionResult Save(long medicalid, FormMedical formMedical)
        {
            var response = new CashierHandler(_unitOfWork).update(medicalid, formMedical);
            return RedirectToAction("ListPatien", "Cashier");
        }

        [CustomAuthorize("VIEW_M_CASHIER")]
        [HttpGet]
        public ActionResult Invoice(long medicalid)
        {
            var response = new CashierHandler(_unitOfWork).GetDetail(medicalid);
            var formmedical = _unitOfWork.FormMedicalRepository.Get(a => a.ID == medicalid).FirstOrDefault();
            ViewBag.Formmedicalid = formmedical;
            ViewBag.Detail = response.Data;
            ViewBag.ClinicName = formmedical.Clinic.Name;
            ViewBag.PatienName = formmedical.Patient.Name;

            if (response.Data != null) { ViewBag.Sum = response.Data.Sum(a => a.Price); } else { ViewBag.Sum = ""; };
            ViewBag.Total = Convert.ToInt32(response.Data.Sum(a => a.Price)) - Convert.ToInt32(formmedical.DiscountAmount);
            return View(formmedical);
        }
        #endregion
    }
}