﻿using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Klinik.Features;
using Klinik.Entities.Loket;
using Klinik.Entities.Account;
using Klinik.Features.Pharmacy;
using Klinik.Common;
using Klinik.Entities.Form;
using AutoMapper;
using Klinik.Entities.Pharmacy;
using Newtonsoft.Json;
using Klinik.Entities.MasterData;
using LinqKit;

namespace Klinik.Web.Controllers
{
    public class PharmacyController : BaseController
    {
        #region ::DROPDOWN::
        private List<SelectListItem> BindDropDownStatus()
        {
            List<SelectListItem> _status = new List<SelectListItem>();
            _status.Insert(0, new SelectListItem
            {
                Text = "Open",
                Value = "0"
            });

            _status.Insert(1, new SelectListItem
            {
                Text = "Waiting",
                Value = "1"
            });

            return _status;
        }

        private List<SelectListItem> BindDropDownClinic()
        {
            List<SelectListItem> _authorizedClinics = new List<SelectListItem>();
            if (Session["UserLogon"] != null)
            {
                var _account = (AccountModel)Session["UserLogon"];

                var _getClinics = new ClinicHandler(_unitOfWork).GetAllClinic(_account.ClinicID);
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

        public PharmacyController(IUnitOfWork unitOfWork, KlinikDBEntities context) :
            base(unitOfWork, context)
        {
        }

        // GET: Prescription details
        [CustomAuthorize("VIEW_FARMASI")]
        public ActionResult Prescription()
        {
            string id = Request.QueryString["id"];
            if (string.IsNullOrEmpty(id))
                return View("PatientList", "Pharmacy", null);

            List<FormExamineMedicine> medicinelist = _unitOfWork.FormExamineMedicineRepository.Get(x => x.FormExamine.FormMedicalID.Value.ToString() == id && x.RowStatus == 0).ToList();
            PrescriptionModel prescriptionModel = new PrescriptionModel();
            prescriptionModel.FormMedicalID = long.Parse(id);

            //get stock
            var _ids = medicinelist.Select(x => x.ProductID).Distinct();
            var _stockCollections = _unitOfWork.ProductInGudangRepository.Get(x => _ids.Contains(x.ProductId)).Select(x => new { x.ProductId, x.stock });
            foreach (var item in medicinelist)
            {
                FormExamineMedicineModel medicineModel = Mapper.Map<FormExamineMedicine, FormExamineMedicineModel>(item);

                FormExamineMedicineDetail detail = _unitOfWork.FormExamineMedicineDetailRepository.Get(x => x.FormExamineMedicineID.Value == item.ID && x.RowStatus == 0).FirstOrDefault();
                if (detail != null)
                {
                    medicineModel.Detail = Mapper.Map<FormExamineMedicineDetail, FormExamineMedicineDetailModel>(detail);
                }

                if (medicineModel.MedicineJenis != null)
                {
                    switch (medicineModel.MedicineJenis.ToLower())
                    {
                        case "racikan":
                            medicineModel.Detail.ProcessType = "Racik";
                            break;

                        case "non racikan":
                            medicineModel.Detail.ProcessType = "Request";
                            break;

                        default:
                            medicineModel.Detail.ProcessType = string.Empty;
                            break;
                    }
                }

                prescriptionModel.Medicines.Add(medicineModel);
            }

            return View(prescriptionModel);
        }

        [HttpPost]
        public ActionResult Prescription(PrescriptionModel model)
        {
            var request = new PharmacyRequest { Data = model, Account = Account };

            PharmacyResponse _response = new PharmacyResponse();
            // do the validation

            _response = new PharmacyValidator(_unitOfWork, _context).Validate(request);

            ViewBag.Response = $" {_response.Status.ToString().Trim()};{_response.Message}".TrimStart();
            return View(model);
        }

        // GET: Pharmacy
        public ActionResult PatientList()
        {
            ViewBag.Clinics = BindDropDownClinic();
            return View();
        }

        [CustomAuthorize("VIEW_PENGAMBILAN_OBAT")]
        public ActionResult PengambilanObat()
        {
            return View();
        }

        public ActionResult ModalPopUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetProductList()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;



            var request = new ProductRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                IsForShowInFarmasi = true
            };
            request.Data = new Entities.MasterData.ProductModel();
            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];
            var response = new ProductHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPharmacyQueueFromPoli(string clinics, string status)
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
                Data = new LoketModel { ClinicID = Convert.ToInt32(clinics), PoliToID = PoliHandler.GetPoliIDBasedOnName(PoliEnum.Farmasi.ToString()) }
            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new PharmacyHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetListPengambilanObat()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PharmacyRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,

            };

            if (Session["UserLogon"] != null)
                request.Account = (AccountModel)Session["UserLogon"];

            var response = new PharmacyHandler(_unitOfWork).GetListPengambilanObat(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("VIEW_PENGAMBILAN_OBAT")]
        public ActionResult ListAllGivenMedicine()
        {
            long _formMedId = 0;
            if (Request.QueryString["frmmedid"] != null)
            {
                _formMedId = Convert.ToInt64(Request.QueryString["frmmedid"].ToString());
            }
            var _model = new FormExamineMedicineDetailModel
            {

            };
            _model.IdDetailsChecked = new List<long>();//new PharmacyHandler(_unitOfWork).GetMedicineWasReceivedByPatient(_formMedId);
            var _pasien = new PharmacyHandler(_unitOfWork).GetPatientDataBasedOnFrmMedical(_formMedId);
            ViewBag.NamaPatient = _pasien.Name;
            ViewBag.Birthdate = _pasien.BirthDateStr;
            return View(_model);
        }

        [HttpPost]
        public ActionResult GetListObatPasien()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
       
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PharmacyRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,

                PageSize = _pageSize,
                Skip = _skip,

            };

            if (Session["UserLogon"] != null)
                request.Account = (AccountModel)Session["UserLogon"];

            long _formMedId = 0;
            if (Request.QueryString["frmmedid"] != null)
            {
                _formMedId = Convert.ToInt64(Request.QueryString["frmmedid"].ToString());
            }

            if (request.Data == null)
                request.Data = new PrescriptionModel();
            request.Data.FormMedicalID = _formMedId;
            var response = new PharmacyHandler(_unitOfWork).ListAllObat(request);
            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateStatusObat()
        {
            var response = new PharmacyResponse();
            var request = new PharmacyRequest
            {

            };
            if (Request.Form["idFrmMedDetail"] != null)
                request.idSelectedobat = JsonConvert.DeserializeObject<List<long>>(Request.Form["idFrmMedDetail"]);
            if (Session["UserLogon"] != null)
                request.Account = (AccountModel)Session["UserLogon"];

            response = new PharmacyHandler(_unitOfWork, _context).UpdateStatusObat(request);
            return Json(new { Status = response.Status.ToString().TrimStart(), Message = response.Message, Notes=response.AdditionalMessages }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetDetailObat(string codeObat)
        {
            
            var qry = _unitOfWork.ProductRepository.GetFirstOrDefault(x=>x.RowStatus==0 && x.Code==codeObat);

            long clinicId = 0;
            if (Session["UserLogon"] != null)
            {
                var tempData = (AccountModel)Session["UserLogon"];
                clinicId = tempData.ClinicID;
            }



            var productIdS = qry.ID;
            var gudangs = _unitOfWork.GudangRepository.Get(x => x.ClinicId == clinicId).Select(x => x.id).ToList();
            var stockCollection = _unitOfWork.ProductInGudangRepository.Get(x => x.ProductId==productIdS && gudangs.Contains(x.GudangId ?? 0)).Select(x => new
            {
                x.ProductId,
                x.stock
            });

            var stockRepo = stockCollection.GroupBy(x => x.ProductId).Select(c => new
            {
                ProductID = c.First().ProductId,
                CurrStock = c.Sum(x => x.stock)
            });



            var prData = new ProductModel();
            prData = Mapper.Map<Product, ProductModel>(qry);

            return Json(new
            {
                id = (Int32)prData.Id,
                label = prData.Name,
                code = prData.Code,
                stock = stockRepo.FirstOrDefault(x => x.ProductID == prData.Id) == null ? "0" : stockRepo.FirstOrDefault(x => x.ProductID == prData.Id).CurrStock.ToString(),
                category = prData.ProductCategoryName,
                unit = prData.ProductUnitName,
                price = prData.RetailPrice.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}