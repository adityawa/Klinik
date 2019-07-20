using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Letter;
using Klinik.Entities.Loket;
using Klinik.Features.SuratReferensi.SuratLabReferensi;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class LettersController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public LettersController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region ::MISC::
        private List<SelectListItem> BindDropDownFormMedicalID()
        {
            List<SelectListItem> listFormMedId = new List<SelectListItem>();
            listFormMedId.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = ""
            });


            return listFormMedId;
        }
        #endregion

        [CustomAuthorize("VIEW_SURAT")]
        public ActionResult SuratRujukan()
        {
            ViewBag.FormMedicalIds = BindDropDownFormMedicalID();
            return View();
        }

        [HttpPost]
        public JsonResult CreateSuratRujukanLab()
        {
            var _model = new LabReferenceLetterModel { };
            if (Request.Form["forPatient"] != null)
                _model.ForPatient = long.Parse(Request.Form["forPatient"].ToString());
            if (Request.Form["FormMedicalID"] != null)
                _model.FormMedicalID = long.Parse(Request.Form["FormMedicalID"].ToString());
            if (Request.Form["TglPeriksa"] != null)
                _model.Cekdate = DateTime.Parse(Request.Form["TglPeriksa"].ToString());
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            _model.CreatedDate = DateTime.Now;
            var request = new RujukanLabRequest
            {
                Data = _model
            };

            var response = new RujukanLabResponse { };
            response = new RujukanLabValidator(_unitOfWork, _context).Validate(request);

            return Json(new
            {
                Status = response.Status,
                Message = response.Message,
                PatientName = response.Patient.Name
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTanggalPemeriksaan(string PatientId)
        {
            List<LoketModel> queueData = new RujukanLabHandler(_unitOfWork).GetFormMedicalIds(PatientId == null ? 0 : int.Parse(PatientId));
            var lists = queueData.DistinctBy(x => x.TransactionDateStr).ToList();
            return Json(lists, JsonRequestBehavior.AllowGet);
        }
    }
}