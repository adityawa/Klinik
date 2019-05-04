using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using Klinik.Entities.Poli;
using Klinik.Features;
using Klinik.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class PoliController : BaseController
    {
        public PoliController(IUnitOfWork unitOfWork, KlinikDBEntities context) :
            base(unitOfWork, context)
        {
        }

        #region ::PUBLIC METHODS::
        public class TempClass
        {
            public int id { get; set; }
            public string code { get; set; }
            public string label { get; set; }
            public string value { get; set; }
            public string stock { get; set; }
        }

        [HttpPost]
        public JsonResult AutoCompleteRadiology(string prefix)
        {
            List<LabItem> labList = _unitOfWork.LabItemRepository.Get(x => x.LabItemCategory.LabType == "Radiology" && x.RowStatus == 0).ToList();

            var filteredList = labList.Where(t => t.Name.ToLower().Contains(prefix.ToLower()));

            List<TempClass> resultList = new List<TempClass>();
            foreach (var item in filteredList)
            {
                TempClass temp = new TempClass
                {
                    id = item.ID,
                    label = item.Name,
                    code = item.Code,
                    stock = "911" // hardcoded for now
                };

                FormExamineLab lab = _unitOfWork.FormExamineLabRepository.GetFirstOrDefault(x => x.LabItemID == item.ID);
                temp.value = lab == null ? string.Empty : lab.Result;

                resultList.Add(temp);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteLaborat(string prefix)
        {
            List<LabItem> labList = _unitOfWork.LabItemRepository.Get(x => x.LabItemCategory.LabType == "Laboratorium" && x.RowStatus == 0).ToList();

            var filteredList = labList.Where(t => t.Name.ToLower().Contains(prefix.ToLower()));

            List<TempClass> resultList = new List<TempClass>();
            foreach (var item in filteredList)
            {
                TempClass temp = new TempClass
                {
                    id = item.ID,
                    label = item.Name,
                    code = item.Code,
                    stock = "911" // hardcoded for now
                };

                FormExamineLab lab = _unitOfWork.FormExamineLabRepository.GetFirstOrDefault(x => x.LabItemID == item.ID);
                temp.value = lab == null ? string.Empty : lab.Result;

                resultList.Add(temp);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteInjection(string prefix)
        {
            List<Product> productList = _unitOfWork.ProductRepository.Get(x => x.ProductCategoryID == 2 && x.RowStatus == 0).ToList();

            var filteredList = productList.Where(t => t.Name.ToLower().Contains(prefix.ToLower()));

            List<TempClass> resultList = new List<TempClass>();
            foreach (var item in filteredList)
            {
                TempClass temp = new TempClass
                {
                    id = item.ID,
                    label = item.Name,
                    code = item.Code,
                    stock = "911" // hardcoded for now
                };

                resultList.Add(temp);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteMedicine(string prefix)
        {
            List<Product> productList = _unitOfWork.ProductRepository.Get(x => x.ProductCategoryID == 1 && x.RowStatus == 0).ToList();

            var filteredList = productList.Where(t => t.Name.ToLower().Contains(prefix.ToLower()));

            List<TempClass> resultList = new List<TempClass>();
            foreach (var item in filteredList)
            {
                TempClass temp = new TempClass
                {
                    id = item.ID,
                    label = item.Name,
                    code = item.Code,
                    stock = "911" // hardcoded for now
                };

                resultList.Add(temp);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteService(string poliID, string prefix)
        {
            int _poliID = int.Parse(poliID);
            List<Service> serviceList = _unitOfWork.ServicesRepository.Get().ToList();
            List<PoliService> poliServiceList = _unitOfWork.PoliServicesRepository.Get(x => x.PoliID == _poliID && x.RowStatus == 0).ToList();

            var filteredList = serviceList.Where(x => !poliServiceList.Any(p => p.ServicesID == x.ID) && x.Name.ToLower().Contains(prefix.ToLower())).ToList();

            List<TempClass> resultList = new List<TempClass>();
            foreach (var item in filteredList)
            {
                TempClass temp = new TempClass
                {
                    id = item.ID,
                    label = item.Name,
                    code = item.Code,
                    stock = item.Price.ToString()
                };

                resultList.Add(temp);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("VIEW_POLI_PATIENT_LIST")]
        public ActionResult PatientList()
        {
            var poliEnum = PoliEnum.PoliUmum;
            var poliName = Regex.Replace(poliEnum.ToString(), "([A-Z])", " $1").Trim();

            var model = new PatientListModel
            {
                PoliFromID = (int)poliEnum,
                CurrentPoliID = (int)poliEnum,
                PoliFromName = poliName
            };

            return View("PatientList", model);
        }

        [HttpPost]
        public ActionResult FormExamine(
            string formExamineID,
            string loketID,
            string anamnesa,
            string diagnose,
            string therapy,
            string receipt,
            string finalState,
            string poliToID,
            string doctorToID,
            List<string> medicineList,
            List<string> injectionList,
            List<string> labList,
            List<string> radiologyList,
            List<string> serviceList)
        {
            if (medicineList == null)
                medicineList = new List<string>();
            if (injectionList == null)
                injectionList = new List<string>();
            if (labList == null)
                labList = new List<string>();
            if (radiologyList == null)
                radiologyList = new List<string>();
            if (serviceList == null)
                serviceList = new List<string>();

            PoliExamineModel model = GeneratePoliExamineModel(formExamineID, loketID, anamnesa, diagnose, therapy, receipt, finalState, poliToID, doctorToID, medicineList, injectionList, labList, radiologyList, serviceList);
            model.Account = Account;

            var request = new FormExamineRequest { Data = model, };

            FormExamineResponse _response = new FormExamineValidator(_unitOfWork, _context).Validate(request);
            if (_response.Status)
            {
                // Notify to all
                RegistrationHub.BroadcastDataToAllClients();
            }

            ViewBag.Response = $"{_response.Status};{_response.Message}";
            var tempPoliList = BindDropDownPoliList(model.LoketData.PoliToID);
            ViewBag.PoliList = tempPoliList;
            ViewBag.DoctorList = BindDropDownDoctorList(int.Parse(tempPoliList[0].Value));
            ViewBag.FinalStateList = BindDropDownFinalStateList();

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormExamine()
        {
            var id = Request.QueryString["id"];
            if (id == null)
                return View("Index", "Home", null);

            var request = new LoketRequest
            {
                Data = new LoketModel
                {
                    Id = long.Parse(id.ToString())
                }
            };

            LoketResponse resp = new LoketHandler(_unitOfWork).GetDetail(request);

            PoliExamineModel model = new PoliExamineModel();

            try
            {
                model.LoketData = resp.Entity;
                model.PatientAge = CommonUtils.GetPatientAge(model.LoketData.PatientBirthDateStr);

                var necessityTypeList = GetGeneralMasterByType(Constants.MasterType.NECESSITY_TYPE);
                var paymentTypeList = GetGeneralMasterByType(Constants.MasterType.PAYMENT_TYPE);

                model.NecessityTypeStr = necessityTypeList.FirstOrDefault(x => x.Value == model.LoketData.NecessityType.ToString()).Text;
                model.PaymentTypeStr = paymentTypeList.FirstOrDefault(x => x.Value == model.LoketData.PaymentType.ToString()).Text;

                // get default services
                List<PoliService> poliServicelist = _unitOfWork.PoliServicesRepository.Get(x => x.PoliID == model.LoketData.PoliToID && x.RowStatus == 0).ToList();
                foreach (var item in poliServicelist)
                {
                    ServiceModel servModel = Mapper.Map<Service, ServiceModel>(item.Service);
                    model.DefaultServiceList.Add(servModel);
                }

                // get form examine if any
                FormExamine formExamine = _unitOfWork.FormExamineRepository.GetFirstOrDefault(x => x.FormMedicalID == model.LoketData.FormMedicalID);
                if (formExamine != null)
                {
                    model.ExamineData = Mapper.Map<FormExamine, FormExamineModel>(formExamine);

                    // get form examine medicine if any
                    List<FormExamineMedicine> formExamineMedicines = _unitOfWork.FormExamineMedicineRepository.Get(x => x.FormExamineID == formExamine.ID);
                    if (formExamineMedicines.Count > 0)
                    {
                        foreach (var formExamineMedicine in formExamineMedicines)
                        {
                            model.MedicineDataList.Add(Mapper.Map<FormExamineMedicine, FormExamineMedicineModel>(formExamineMedicine));
                        }
                    }

                    // get form examine lab if any
                    List<FormExamineLab> formExamineLabs = _unitOfWork.FormExamineLabRepository.Get(x => x.FormMedicalID == formExamine.FormMedicalID && x.LabType == "Laboratory");
                    if (formExamineLabs.Count > 0)
                    {
                        foreach (var formExamineLab in formExamineLabs)
                        {
                            model.LabDataList.Add(Mapper.Map<FormExamineLab, FormExamineLabModel>(formExamineLab));
                        }
                    }

                    // get form examine radiologi if any
                    List<FormExamineLab> formExamineRadiologies = _unitOfWork.FormExamineLabRepository.Get(x => x.FormMedicalID == formExamine.FormMedicalID && x.LabType == "Radiology");
                    if (formExamineRadiologies.Count > 0)
                    {
                        foreach (var formExamineRadiology in formExamineRadiologies)
                        {
                            model.RadiologyDataList.Add(Mapper.Map<FormExamineLab, FormExamineLabModel>(formExamineRadiology));
                        }
                    }

                    // get form examine service if any
                    List<FormExamineService> formExamineServices = _unitOfWork.FormExamineServiceRepository.Get(x => x.FormExamineID == formExamine.ID);
                    if (formExamineServices.Count > 0)
                    {
                        foreach (var formExamineService in formExamineServices)
                        {
                            model.ServiceDataList.Add(Mapper.Map<FormExamineService, FormExamineServiceModel>(formExamineService));
                        }
                    }
                }
                else
                {
                    model.ExamineData.DoctorID = model.LoketData.DoctorID;
                    model.ExamineData.PoliID = model.LoketData.PoliToID;
                }

                var tempPoliList = BindDropDownPoliList(model.LoketData.PoliToID);
                ViewBag.PoliList = tempPoliList;
                ViewBag.DoctorList = BindDropDownDoctorList(int.Parse(tempPoliList[0].Value));
                ViewBag.FinalStateList = BindDropDownFinalStateList();
            }
            catch
            {
                return BadRequestResponse;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult GetPatientListPoliID(int poliId)
        {
            var response = GetRegistrationPatientByPoliID(poliId);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw, Status = response.Status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDoctorPatientListPoliID(int poliId)
        {
            LoketResponse response = GetRegistrationPatientByPoliID(poliId);

            int doctorID = GetDoctorID(Account.EmployeeID);
            if (doctorID < 0)
                return BadRequestResponse;

            var filteredData = response.Data.Where(x => x.DoctorID == doctorID);

            return Json(new { data = filteredData, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw, Status = response.Status }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRIVATE METHODS::
        [NonAction]
        private List<SelectListItem> BindDropDownFinalStateList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem { Text = "Sembuh", Value = "Sembuh" });
            result.Add(new SelectListItem { Text = "Harus Kontrol Rutin", Value = "Harus Kontrol Rutin" });
            result.Add(new SelectListItem { Text = "Opname / Rawat Inap", Value = "Opname / Rawat Inap" });
            result.Add(new SelectListItem { Text = "Rujuk ke RS", Value = "Rujuk ke RS" });
            result.Add(new SelectListItem { Text = "Drop Out", Value = "Drop Out" });
            result.Add(new SelectListItem { Text = "Meninggal", Value = "Meninggal" });

            return result;
        }

        [NonAction]
        private int GetDoctorID(long employeeID)
        {
            var doctor = _unitOfWork.DoctorRepository.GetFirstOrDefault(x => x.EmployeeID == employeeID);
            if (doctor != null)
                return doctor.ID;

            return -1;
        }

        [NonAction]
        private LoketResponse GetRegistrationPatientByPoliID(int poliID)
        {
            string _draw = Request.Form.Count > 0 ? Request.Form.GetValues("draw").FirstOrDefault() : string.Empty;
            string _start = Request.Form.Count > 0 ? Request.Form.GetValues("start").FirstOrDefault() : string.Empty;
            string _length = Request.Form.Count > 0 ? Request.Form.GetValues("length").FirstOrDefault() : string.Empty;
            string _sortColumn = Request.Form.Count > 0 ? Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault() : string.Empty;
            string _sortColumnDir = Request.Form.Count > 0 ? Request.Form.GetValues("order[0][dir]").FirstOrDefault() : string.Empty;
            string _searchValue = Request.Form.Count > 0 ? Request.Form.GetValues("search[value]").FirstOrDefault() : string.Empty;

            int _pageSize = string.IsNullOrEmpty(_length) ? 0 : Convert.ToInt32(_length);
            int _skip = string.IsNullOrEmpty(_start) ? 0 : Convert.ToInt32(_start);

            var request = new LoketRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new LoketHandler(_unitOfWork).GetListData(request, poliID);

            return response;
        }

        [NonAction]
        private PoliExamineModel GeneratePoliExamineModel(
            string formExamineID,
            string loketID,
            string anamnesa,
            string diagnose,
            string therapy,
            string receipt,
            string finalState,
            string poliToID,
            string doctorToID,
            List<string> medicineList,
            List<string> injectionList,
            List<string> labList,
            List<string> radiologyList,
            List<string> serviceList)
        {
            QueuePoli queue = _unitOfWork.RegistrationRepository.GetById(int.Parse(loketID));

            PoliExamineModel model = new PoliExamineModel();

            // For new registration data
            if (!string.IsNullOrEmpty(doctorToID))
                model.DoctorToID = int.Parse(doctorToID);

            model.PoliToID = int.Parse(poliToID);

            // Registration
            model.LoketData = Mapper.Map<QueuePoli, LoketModel>(queue);

            // FormExamine
            model.ExamineData.Anamnesa = anamnesa;
            model.ExamineData.Diagnose = diagnose;
            model.ExamineData.Therapy = therapy;
            model.ExamineData.Result = finalState;
            model.ExamineData.PoliID = queue.PoliTo;
            model.ExamineData.DoctorID = queue.DoctorID;
            model.ExamineData.FormMedicalID = queue.FormMedicalID;

            // FormExamineMedicine
            foreach (var item in medicineList)
            {
                string[] values = item.Split('|');
                FormExamineMedicineModel medModel = new FormExamineMedicineModel
                {
                    ProductID = int.Parse(values[0]),
                    FormExamineID = long.Parse(formExamineID),
                    Qty = int.Parse(values[2]),
                    RemarkUse = values[1],
                    TypeID = "Medicine"
                };

                model.MedicineDataList.Add(medModel);
            }

            // FormExamineInjection
            foreach (var item in injectionList)
            {
                string[] values = item.Split('|');
                FormExamineMedicineModel medModel = new FormExamineMedicineModel
                {
                    ProductID = int.Parse(values[0]),
                    FormExamineID = long.Parse(formExamineID),
                    RemarkUse = values[2],
                    TypeID = "Injection"
                };

                model.MedicineDataList.Add(medModel);
            }

            // FormExamineLab
            foreach (var item in labList)
            {
                FormExamineLabModel labModel = new FormExamineLabModel
                {
                    LabItemID = int.Parse(item),
                    LabType = "Laboratorium"
                };

                model.LabDataList.Add(labModel);
            }

            // FormExamineRadiology
            foreach (var item in radiologyList)
            {
                FormExamineLabModel labModel = new FormExamineLabModel
                {
                    LabItemID = int.Parse(item),
                    LabType = "Radiology"
                };

                model.LabDataList.Add(labModel);
            }

            // FormExamineRadiology
            foreach (var item in serviceList)
            {
                FormExamineServiceModel serviceModel = new FormExamineServiceModel
                {
                    ServiceID = int.Parse(item)
                };

                model.ServiceDataList.Add(serviceModel);
            }

            return model;
        }
        #endregion
    }
}
