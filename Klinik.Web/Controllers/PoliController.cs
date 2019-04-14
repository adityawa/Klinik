using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.Poli;
using Klinik.Features.FormExamines;
using Klinik.Features.Loket;
using Klinik.Resources;
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
            PoliExamineModel model = GeneratePoliExamineModel(loketID, anamnesa, diagnose, therapy, receipt, finalState, poliToID, doctorToID, medicineList, injectionList, labList, radiologyList, serviceList);
            model.Account = Account;

            var request = new FormExamineRequest { Data = model, };

            FormExamineResponse _response = new FormExamineValidator(_unitOfWork).Validate(request);
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

            return View(model);
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
                model.PatientAge = GetPatientAge(model.LoketData.PatientBirthDateStr);

                var necessityTypeList = GetGeneralMasterByType(Constants.MasterType.NECESSITY_TYPE);
                var paymentTypeList = GetGeneralMasterByType(Constants.MasterType.PAYMENT_TYPE);

                model.NecessityTypeStr = necessityTypeList.FirstOrDefault(x => x.Value == model.LoketData.NecessityType.ToString()).Text;
                model.PaymentTypeStr = paymentTypeList.FirstOrDefault(x => x.Value == model.LoketData.PaymentType.ToString()).Text;

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
                    List<FormExamineLab> formExamineLabs = _unitOfWork.FormExamineLabRepository.Get(x => x.FormExamineID == formExamine.ID && x.LabType == "Laboratory");
                    if (formExamineLabs.Count > 0)
                    {
                        foreach (var formExamineLab in formExamineLabs)
                        {
                            model.LabDataList.Add(Mapper.Map<FormExamineLab, FormExamineLabModel>(formExamineLab));
                        }
                    }

                    // get form examine radiologi if any
                    List<FormExamineLab> formExamineRadiologies = _unitOfWork.FormExamineLabRepository.Get(x => x.FormExamineID == formExamine.ID && x.LabType == "Radiology");
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
            catch (Exception ex)
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
        private string GetPatientAge(string patientBirthDateStr)
        {
            string result = string.Empty;

            DateTime dob = Convert.ToDateTime(patientBirthDateStr);
            int age = DateTime.Now.Year - dob.Year;
            if (DateTime.Now.DayOfYear < dob.DayOfYear)
                age = age - 1;

            if (age > 0)
            {
                result = age.ToString() + " " + UIMessages.Years;
            }
            else
            {
                DateTime dateNow = DateTime.Now;
                int years = new DateTime(dateNow.Subtract(dob).Ticks).Year - 1;
                DateTime pastYearDate = dob.AddYears(years);
                int month = 0;
                for (int i = 1; i <= 12; i++)
                {
                    if (pastYearDate.AddMonths(i) == dateNow)
                    {
                        month = i;
                        break;
                    }
                    else if (pastYearDate.AddMonths(i) >= dateNow)
                    {
                        month = i - 1;
                        break;
                    }
                }

                result = month.ToString() + " " + UIMessages.Month;
            }

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
            // FormExamineInjection
            // FormExamineLab
            // FormExamineRadiology
            // FormExamineService

            return model;
        }
        #endregion
    }
}
