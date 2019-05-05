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
        [CustomAuthorize("VIEW_M_CASHIER")]
        public ActionResult ListPatien()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Detail(long patienid)
        {
            var response = new CashierHandler(_unitOfWork).GetDetail(patienid);
            ViewBag.Formmedicalid = _unitOfWork.FormMedicalRepository.Get(a => a.PatientID == patienid).Select(x => x.ID).FirstOrDefault();
            ViewBag.Detail = response.Data;
            ViewBag.Sum = response.Data.Sum(a => a.Price);
            return View();
        }

        [HttpPost]
        public ActionResult Save(long medicalid, FormMedical formMedical)
        {
            var response = new CashierHandler(_unitOfWork).update(medicalid, formMedical);
            return RedirectToAction("ListPatien", "Cashier");
        }
        #endregion
    }
}