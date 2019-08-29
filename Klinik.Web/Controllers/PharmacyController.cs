using Klinik.Data;
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
                //if (medicineModel.MedicineJenis != null)
                //{
                //    switch (medicineModel.MedicineJenis.ToLower())
                //    {
                //        case "racikan":
                //            medicineModel.Process = "Racik";
                //            break;

                //        case "non racikan":
                //            medicineModel.Process = "Request";
                //            break;

                //        default:
                //            medicineModel.Process = string.Empty;
                //            break;
                //    }
                //}

               
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

            ViewBag.Response =$" {_response.Status.ToString().Trim()};{_response.Message}".TrimStart();
            return View(model);
        }

        // GET: Pharmacy
        public ActionResult PatientList()
        {
            ViewBag.Clinics = BindDropDownClinic();
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
                IsForShowInFarmasi=true
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
                Data = new LoketModel { ClinicID = Convert.ToInt32(clinics), PoliToID = (int)PoliEnum.Farmasi }
            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new PharmacyHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
    }
}