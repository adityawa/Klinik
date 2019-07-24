using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Letter;
using Klinik.Entities.Loket;
using Klinik.Features.SuratReferensi.SuratLabReferensi;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa.Extensions;
using Rotativa.Options;
using Rotativa;
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

            if (response.ListLabs == null)
                response.ListLabs = new List<Entities.MasterData.LabItemModel>();
            response.ListLabs = new RujukanLabHandler(_unitOfWork).GetPreviousSelectedLabItem(_model.FormMedicalID);
            return Json(new
            {
                Status = response.Status,
                Message = response.Message,
                PatientName = response.Patient == null ? "" : response.Patient.Name,
                Gender = response.Patient == null ? "" : response.Patient.Gender,
                SAP = response.Patient == null ? "" : response.Patient.EmployeeID.ToString(),
                NoHP = response.Patient == null ? "" : response.Patient.HPNumber,
                BirthDate = response.Patient == null ? "" : response.Patient.BirthDateStr,
                Usia = response.Entity.PatientAge,
                TglPeriksa = response.Entity.strCekdate.ToString(),
                FormMedicalId = response.Entity.FormMedicalID,
                Data = response.ListLabs

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateSuratBerbadanSehat()
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

            if (response.ListLabs == null)
                response.ListLabs = new List<Entities.MasterData.LabItemModel>();
            response.ListLabs = new RujukanLabHandler(_unitOfWork).GetPreviousSelectedLabItem(_model.FormMedicalID);
            return Json(new
            {
                Status = response.Status,
                Message = response.Message,
                PatientName = response.Patient == null ? "" : response.Patient.Name,
                Gender = response.Patient == null ? "" : response.Patient.Gender,
                SAP = response.Patient == null ? "" : response.Patient.EmployeeID.ToString(),
                NoHP = response.Patient == null ? "" : response.Patient.HPNumber,
                BirthDate = response.Patient == null ? "" : response.Patient.BirthDateStr,
                Usia = response.Entity.PatientAge,
                TglPeriksa = response.Entity.strCekdate.ToString(),
                FormMedicalId = response.Entity.FormMedicalID,
                Data = response.ListLabs

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAndPreviewRujukanLab()
        {
            var _model = new LabReferenceLetterModel();
            if (_model.SuratRujukanLabKeluar == null)
                _model.SuratRujukanLabKeluar = new SuratRujukanKeluarModel();
            if (Request.Form["DokterPengirim"] != null)
                _model.SuratRujukanLabKeluar.DokterPengirim = Request.Form["DokterPengirim"] == null ? "" : Request.Form["DokterPengirim"].ToString();
            if (Request.Form["FormMedicalID"] != null)
                _model.SuratRujukanLabKeluar.FormMedicalID = Convert.ToInt64(Request.Form["FormMedicalID"].ToString());
            if (Request.Form["LabItems"] != null)
                _model.SuratRujukanLabKeluar.ListOfLabItemId = JsonConvert.DeserializeObject<List<int>>(Request.Form["LabItems"]);
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new RujukanLabRequest
            {
                Data = _model
            };

            var response = new RujukanLabResponse { };
            response = new RujukanLabValidator(_unitOfWork, _context).ValidateBeforePreview(request);

            return Json(new { Status = response.Status, FormMedicalId = response.Entity.FormMedicalID }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetTanggalPemeriksaan(string PatientId)
        {
            List<LoketModel> queueData = new RujukanLabHandler(_unitOfWork).GetFormMedicalIds(PatientId == null ? 0 : int.Parse(PatientId));
            var lists = queueData.DistinctBy(x => x.TransactionDateStr).ToList();
            return Json(lists, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("VIEW_SURAT")]
        public ActionResult PrintSuratRujukanLabKeluar(string FormMedId)
        {
            var _model = new LabReferenceLetterModel
            {
                FormMedicalID = FormMedId == null ? 0 : Convert.ToInt64(FormMedId)
            };
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            var request = new RujukanLabRequest
            {
                Data = _model
            };

            var response = new RujukanLabHandler(_unitOfWork, _context).GetDetailSuratRujukanLab(request);

            var _suratrujukankeluarmodel = new SuratRujukanKeluarModel
            {
                NoSurat = response.Entity.SuratRujukanLabKeluar.NoSurat,
                DokterPengirim = response.Entity.SuratRujukanLabKeluar.DokterPengirim,

            };
            var labrefmodel = new LabReferenceLetterModel
            {
                FormMedicalID = request.Data.FormMedicalID,
                PatientData = response.Patient,
                strCekdate = response.Entity.strCekdate,
                SuratRujukanLabKeluar = _suratrujukankeluarmodel
            };

            if (labrefmodel.LabItems == null)
                labrefmodel.LabItems = new List<Entities.MasterData.LabItemModel>();
            labrefmodel.LabItems = response.ListLabs;
            labrefmodel.PatientAge = response.Entity.PatientAge;
            ViewBag.NoSurat = response.Entity.SuratRujukanLabKeluar.NoSurat;
            return new PartialViewAsPdf(labrefmodel)
            {
                PageOrientation = Orientation.Landscape,
                PageSize = Size.Folio,

                FileName = "PrintSuratRujukanKeluar.pdf"
            };
            // return View(labrefmodel);
        }

        public ActionResult ExportSuratRujukanLabKeluar2Pdf(string FormMedId)
        {

            var _model = new LabReferenceLetterModel
            {
                FormMedicalID = FormMedId == null ? 0 : Convert.ToInt64(FormMedId)
            };
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            var request = new RujukanLabRequest
            {
                Data = _model
            };

            var response = new RujukanLabHandler(_unitOfWork, _context).GetDetailSuratRujukanLab(request);

            var _suratrujukankeluarmodel = new SuratRujukanKeluarModel
            {
                NoSurat = response.Entity.SuratRujukanLabKeluar.NoSurat,
                DokterPengirim = response.Entity.SuratRujukanLabKeluar.DokterPengirim,

            };
            var labrefmodel = new LabReferenceLetterModel
            {
                FormMedicalID = request.Data.FormMedicalID,
                PatientData = response.Patient,
                strCekdate = response.Entity.strCekdate,
                SuratRujukanLabKeluar = _suratrujukankeluarmodel
            };

            if (labrefmodel.LabItems == null)
                labrefmodel.LabItems = new List<Entities.MasterData.LabItemModel>();
            labrefmodel.LabItems = response.ListLabs;
            labrefmodel.PatientAge = response.Entity.PatientAge;
            ViewBag.NoSurat = response.Entity.SuratRujukanLabKeluar.NoSurat;
            return new PartialViewAsPdf(labrefmodel)
            {
                PageOrientation = Orientation.Landscape,
                PageSize = Size.Folio,

                FileName = "PrintSuratRujukanKeluar.pdf"
            };
            //return View(labrefmodel);

        }

    }
}
