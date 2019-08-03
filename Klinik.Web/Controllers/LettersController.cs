﻿using Klinik.Common;
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
using Klinik.Features.SuratReferensi.SuratBadanSehat;
using Klinik.Features.SuratReferensi.SuratPersetujuanTindakan;

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

        public JsonResult CreateSuratPersetujuanTindakan()
        {
            var _model = new PersetujuanTindakanModel { };
            if (Request.Form["forPatient"] != null)
                _model.ForPatient = Request.Form["forPatient"].ToString() == "" ? 0 : long.Parse(Request.Form["forPatient"].ToString());
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            _model.CreatedDate = DateTime.Now;
            var request = new PersetujuanTindakanRequest
            {
                Data = _model
            };
            var response = new PersetujuanTindakanResponse { };
            response = new PersetujuanTimdakanValidator(_unitOfWork).Validate(request);

            return Json(new
            {
                Status = response.Status,
                Message = response.Message,
                PatientName = response.Entity.PatientData.Name,
                UmurPatient = $"{ response.Entity.UmurPatient} / {response.Entity.PatientData.BirthDateStr}",
                SAPPatient = response.Entity.SAPPatient,
                NamaPenjamin = response.Entity.EmployeeData.EmpName,
                Gender = response.Entity.EmployeeData.Gender,
                PhonePenjamin = response.Entity.EmployeeData.HPNumber,
                UmurPenjamin = response.Entity.UmurPenjamin == null ? "" : $"{response.Entity.UmurPenjamin} / {response.Entity.EmployeeData.BirthdateStr}",
                SAPPenjamin = response.Entity.EmployeeData.EmpID
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateSuratBerbadanSehat()
        {
            var _model = new HealthBodyLetterModel { };
            if (Request.Form["forPatient"] != null)
                _model.ForPatient = long.Parse(Request.Form["forPatient"].ToString());

            if (Request.Form["FormMedicalID"] != null)
                _model.FormMedicalID = long.Parse(Request.Form["FormMedicalID"].ToString());
            if (Request.Form["TglPeriksa"] != null)
                _model.Cekdate = DateTime.Parse(Request.Form["TglPeriksa"].ToString());
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            _model.CreatedDate = DateTime.Now;
            var request = new HealthBodyRequest
            {
                Data = _model
            };

            var response = new HealthBodyResponse { };
            response = new HealthBodyValidator(_unitOfWork, _context).Validate(request);
            return Json(new
            {
                Status = response.Status,
                Message = response.Message,
                Nama = response.Entity.PatientData == null ? "" : response.Entity.PatientData.Name,
                TTL = response.Entity.PatientData == null ? "" : response.Entity.PatientData.BirthDateStr,
                Alamat = response.Entity.PatientData == null ? "" : response.Entity.PatientData.Address,
                NoBPJS = response.Entity.PatientData == null ? "" : response.Entity.PatientData.BPJSNumber,
                TB = response.Entity.PreExamineData == null ? 0 : response.Entity.PreExamineData.Height,
                BB = response.Entity.PreExamineData == null ? 0 : response.Entity.PreExamineData.Weight,
                GolDarah = response.Entity.PatientData == null ? "" : response.Entity.PatientData.BloodType,
                TD = response.Entity.PreExamineData == null ? "" : $"{ response.Entity.PreExamineData.Systolic}/{ response.Entity.PreExamineData.Diastolic}",
                HR = response.Entity.PreExamineData == null ? 0 : response.Entity.PreExamineData.Pulse,
                RR = response.Entity.PreExamineData == null ? 0 : response.Entity.PreExamineData.Respitory,
                Visus = response.Entity.PreExamineData == null ? "" : $"{response.Entity.PreExamineData.RightEye}/{response.Entity.PreExamineData.LeftEye}"

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

        [HttpPost]
        public JsonResult SaveAndPreviewPersetujuanTindakan()
        {
            var _model = new PersetujuanTindakanModel();
            var _penjaminModel = new PenjaminModel();

            if (Request.Form["NamaPenjamin"] != null)
                _penjaminModel.Nama = Request.Form["NamaPenjamin"].ToString();
            if (Request.Form["RolePenjamin"] != null)
                _penjaminModel.Sebagai = Request.Form["RolePenjamin"].ToString();

            if (Request.Form["GenderPenjamin"] != null)
                _penjaminModel.Gender = Request.Form["GenderPenjamin"].ToString();

            if (Request.Form["UmurPenjamin"] != null)
                _penjaminModel.Umur = Request.Form["UmurPenjamin"].ToString();

            if (Request.Form["PhonePenjamin"] != null)
                _penjaminModel.Telepon = Request.Form["PhonePenjamin"].ToString();
            if (Request.Form["SAPPenjamin"] != null)
                _penjaminModel.SapId = Request.Form["SAPPenjamin"].ToString();
            if (Request.Form["AddrPenjamin"] != null)
                _penjaminModel.Alamat = Request.Form["AddrPenjamin"].ToString();

            if (Request.Form["DecisionPenjamin"] != null)
                _model.Decision = Request.Form["DecisionPenjamin"].ToString();
            if (Request.Form["DescTindakan"] != null)
                _model.Treatment = Request.Form["DescTindakan"].ToString();

            if (Request.Form["Action"] != null)
                _model.Action = Request.Form["Action"].ToString();
            if (Request.Form["forPatient"] != null)
                _model.ForPatient =long.Parse( Request.Form["forPatient"].ToString());
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            if (_model.PenjaminData == null)
                _model.PenjaminData = new PenjaminModel();
            _model.PenjaminData = _penjaminModel;

            var request = new PersetujuanTindakanRequest
            {
                Data = _model
            };

            var response = new PersetujuanTindakanResponse();
            response = new PersetujuanTimdakanValidator(_unitOfWork).ValidateBeforeSave(request);
            return Json(new
            {
                Status = response.Status,
                LetterId = response.Entity.Id
            });

        }

        [HttpPost]
        public JsonResult SaveAndPreviewHealthBody()
        {
            var _model = new HealthBodyLetterModel();
            if (Request.Form["Keperluan"] != null)
                _model.Keperluan = Request.Form["Keperluan"].ToString();
            if (Request.Form["Pekerjaan"] != null)
                _model.Pekerjaan = Request.Form["Pekerjaan"] == null ? "" : Request.Form["Pekerjaan"].ToString();
            if (Request.Form["FormMedicalID"] != null)
                _model.FormMedicalID = Convert.ToInt64(Request.Form["FormMedicalID"].ToString());
            if (Request.Form["ForPatient"] != null)
                _model.ForPatient = Request.Form["ForPatient"] == null ? 0 : long.Parse(Request.Form["ForPatient"].ToString());
            if (Request.Form["Decision"] != null)
                _model.Decision = Request.Form["Decision"] == null ? "" : Request.Form["Decision"].ToString();
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new HealthBodyRequest
            {
                Data = _model
            };

            var response = new HealthBodyResponse { };
            response = new HealthBodyValidator(_unitOfWork, _context).ValidateBeforePreview(request);

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

        public ActionResult ExportSuratBerbadanSehat(string formMedId)
        {
            long _frmMedicalId = Convert.ToInt64(formMedId);
            var response = new HealthBodyResponse { };
            var request = new HealthBodyRequest
            {
                Data = new HealthBodyLetterModel
                {
                    FormMedicalID = _frmMedicalId
                }
            };
            response = new HealthBodyHandler(_unitOfWork, _context).GetPatientAndPreExamineData(request);
            //return View(response.Entity);
            return new PartialViewAsPdf(response.Entity)
            {
                PageOrientation = Orientation.Landscape,
                PageSize = Size.Folio,

                FileName = "PrintSuratBerbadanSehat.pdf"
            };
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

        public ActionResult ExportSuratPersetujuanTindakan2Pdf(string letterId)
        {
            var response = new PersetujuanTindakanResponse();
            response = new PersetujuanTindakanHandler(_unitOfWork).GetDetailPersetujuanTindakan(Convert.ToInt64(letterId));
            // return View(response.Entity);
            return new PartialViewAsPdf(response.Entity)
            {
                PageOrientation = Orientation.Landscape,
                PageSize = Size.Folio,

                FileName = "SuratPersetujuanTindakan.pdf"
            };
        }
    }
}
