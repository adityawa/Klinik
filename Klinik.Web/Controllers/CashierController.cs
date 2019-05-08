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

namespace Klinik.Web.Controllers
{
    [CustomAuthorize("VIEW_M_CASHIER")]
    public class CashierController : Controller
    {

        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public CashierController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        // GET: Cachier
        #region ::Cachier::
        public ActionResult ListPatien()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Detail(long patienid)
        {
            var response = new CashierHandler(_unitOfWork).GetDetail(patienid);
            var formmedical = _unitOfWork.FormMedicalRepository.Get(a => a.PatientID == patienid).FirstOrDefault();
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

        [HttpGet]
        public ActionResult Invoice(long patienid)
        {
            var response = new CashierHandler(_unitOfWork).GetDetail(patienid);
            var formmedical = _unitOfWork.FormMedicalRepository.Get(a => a.PatientID == patienid).FirstOrDefault();
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