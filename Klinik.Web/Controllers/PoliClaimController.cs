﻿using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using Klinik.Entities.MedicalHistoryEntity;
using Klinik.Entities.Poli;
using Klinik.Entities.PreExamine;
using Klinik.Features;
using Klinik.Features.HistoryMedical;
using Klinik.Features.ICDThemeFeatures;
using Klinik.Features.RealisasiSuratRujukan;
using Klinik.Web.Hubs;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class PoliClaimController : BaseController
    {
        public PoliClaimController(IUnitOfWork unitOfWork, KlinikDBEntities context) :
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
            public string category { get; set; }
            public string unit { get; set; }
            public string price { get; set; }
        }

        [HttpPost]
        public JsonResult AutoCompleteRadiology( string prefix)
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

                FormExamineLab lab = _unitOfWork.FormExamineLabRepository.GetFirstOrDefault(x => x.LabItemID == item.ID );
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
                    stock = "100" // hardcoded for now
                };

                resultList.Add(temp);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteICD(string prefix, string id1, string id2)
        {
            List<TempClass> resultList = new List<TempClass>();
            long lId1 = 0, lId2 = 0;
            if (!string.IsNullOrEmpty(id1))
                lId1 = Convert.ToInt64(id1);
            if (!String.IsNullOrEmpty(id2))
                lId2 = Convert.ToInt64(id2);
            var _qry = new ICDThemeHandler(_unitOfWork).Get(prefix, lId1, lId2);
            foreach (var item in _qry)
            {

                TempClass temp = new TempClass
                {
                    id = (Int32)item.Id,
                    label = item.Name,
                    code = item.Code,

                };

                resultList.Add(temp);
            }



            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCompleteMedicineAdvance(string prefix)
        {
            List<ProductModel> lists = new List<ProductModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Product>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0 && x.Name.ToLower().Contains(prefix.ToLower()));

            qry = _unitOfWork.ProductRepository.Get(searchPredicate, null);

            long clinicId = 0;
            if (Session["UserLogon"] != null)
            {
                var tempData = (AccountModel)Session["UserLogon"];
                clinicId = tempData.ClinicID;
            }


            var temp = (IEnumerable<Product>)qry;
            var productIdS = temp.Select(x => x.ID).Distinct().ToList();
            var gudangs = _unitOfWork.GudangRepository.Get(x => x.ClinicId == clinicId).Select(x => x.id).ToList();
            var stockCollection = _unitOfWork.ProductInGudangRepository.Get(x => productIdS.Contains(x.ProductId ?? 0) && gudangs.Contains(x.GudangId ?? 0)).Select(x => new
            {
                x.ProductId,
                x.stock
            });
            List<TempClass> resultList = new List<TempClass>();
            foreach (var item in qry)
            {
                var prData = new ProductModel();
                prData = Mapper.Map<Product, ProductModel>(item);
                //prData.stock = Convert.ToDecimal(stockCollection.SingleOrDefault(x => x.ProductId == prData.Id) == null ? 0 : stockCollection.SingleOrDefault(x => x.ProductId == prData.Id).stock);
                //lists.Add(prData);
                TempClass tmp = new TempClass
                {
                    id = (Int32)prData.Id,
                    label = prData.Name,
                    code = prData.Code,
                    stock = stockCollection.SingleOrDefault(x => x.ProductId == prData.Id) == null ? "0" : stockCollection.SingleOrDefault(x => x.ProductId == prData.Id).stock.ToString(),
                    category = prData.ProductCategoryName,
                    unit = prData.ProductUnitName,
                    price = prData.RetailPrice.ToString()
                };

                resultList.Add(tmp);
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


        


        [CustomAuthorize("REALISASI_SURAT_RUJUKAN")]
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
            string icdInformation,
        
            string caused,
            string condition,
        
            List<string> concoctionMedicineList,
            
            List<string> injectionList,
            List<string> labList,
            List<string> radiologyList,
            List<string> serviceList)
        {
            if (concoctionMedicineList == null)
                concoctionMedicineList = new List<string>();
           
            if (injectionList == null)
                injectionList = new List<string>();
            if (labList == null)
                labList = new List<string>();
            if (radiologyList == null)
                radiologyList = new List<string>();
            if (serviceList == null)
                serviceList = new List<string>();

           
            int iCaused = caused == null ? 0 :caused==""?0: Convert.ToInt32(caused);
            int iCondition = condition == null ? 0 : condition==""?0: Convert.ToInt32(condition);

            PoliExamineModel model = GeneratePoliExamineModel(formExamineID, loketID, anamnesa, diagnose, therapy, receipt, finalState, icdInformation,  iCaused, iCondition, concoctionMedicineList, injectionList, labList, radiologyList, serviceList);
            model.Account = Account;
            var request = new FormExamineRequest { Data = model, };

            FormExamineResponse _response = new RealisasiSuratRujukanValidator(_unitOfWork, _context).Validate(request);
            

            ViewBag.Response = $"{_response.Status};{_response.Message}";
          
            ViewBag.FinalStateList = BindDropDownFinalStateList();
        
            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("REALISASI_SURAT_RUJUKAN")]
        public ActionResult FormExamine()
        {
            var idPatient = Request.QueryString["idPatient"];
            var noSurat = Request.QueryString["noSurat"];
            var request = new PatientRequest
            {
                Data = new PatientModel
                {
                    Id = long.Parse(idPatient.ToString())
                }
            };
            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var patient_data = new PatientHandler(_unitOfWork, _context).GetDetail(request);

            PoliExamineModel model = new PoliExamineModel();
            model.NoSurat = noSurat;
            try
            {
               // model.LoketData = resp.Entity;
                model.PatientAge = CommonUtils.GetPatientAge(patient_data.Entity.BirthDate.ToString("yyyy-MM-dd"));

                var necessityTypeList = GetGeneralMasterByType(Constants.MasterType.NECESSITY_TYPE);
                var paymentTypeList = GetGeneralMasterByType(Constants.MasterType.PAYMENT_TYPE);

                model.NecessityTypeStr = necessityTypeList.FirstOrDefault(x => x.Text== "Berobat").Text;
                model.PaymentTypeStr = paymentTypeList.FirstOrDefault(x => x.Text== "Umum").Text;

                // get default services
                List<PoliService> poliServicelist = _unitOfWork.PoliServicesRepository.Get(x => x.RowStatus == 0).ToList();
                foreach (var item in poliServicelist)
                {
                    ServiceModel servModel = Mapper.Map<Service, ServiceModel>(item.Service);
                    model.DefaultServiceList.Add(servModel);
                }

                // get form examine if any
                FormPreExamine formPreExamine = _unitOfWork.FormPreExamineRepository.GetFirstOrDefault(x => x.FormMedicalID == 0);
                if (formPreExamine != null)
                {
                    model.PreExamineData = Mapper.Map<FormPreExamine, PreExamineModel>(formPreExamine);
                }

                // get form examine if any
                FormExamine formExamine = _unitOfWork.FormExamineRepository.GetFirstOrDefault(x => x.FormMedicalID == 0);
                if (formExamine != null)
                {
                    model.ExamineData = Mapper.Map<FormExamine, FormExamineModel>(formExamine);
                    if (model.ExamineData.ICDInformation != null)
                    {
                        string[] arrIcds = model.ExamineData.ICDInformation.Split('|');
                        if (arrIcds.Length > 0)
                        {
                            model.ICDInformation1 = arrIcds[0];
                            model.ICDInformation1Desc = arrIcds[0] == "" ? "" : GetICDDescription(Convert.ToInt64(arrIcds[0]));
                            model.ICDInformation2 = arrIcds[1];
                            model.ICDInformation2Desc = arrIcds[1] == "" ? "" : GetICDDescription(Convert.ToInt64(arrIcds[1]));
                            model.ICDInformation3 = arrIcds[2];
                            model.ICDInformation3Desc = arrIcds[2] == "" ? "" : GetICDDescription(Convert.ToInt64(arrIcds[2]));
                        }
                    }
                    // get form examine medicine if any
                    List<FormExamineMedicine> formExamineMedicines = _unitOfWork.FormExamineMedicineRepository.Get(x => x.FormExamineID == 0);
                    foreach (var formExamineMedicine in formExamineMedicines)
                    {
                        if (formExamineMedicine.TypeID == ((int)MedicineTypeEnum.Medicine).ToString() || formExamineMedicine.TypeID == ((int)MedicineTypeEnum.Concoction).ToString())
                            model.MedicineDataList.Add(Mapper.Map<FormExamineMedicine, FormExamineMedicineModel>(formExamineMedicine));
                        else if (formExamineMedicine.TypeID == ((int)MedicineTypeEnum.Injection).ToString())
                            model.InjectionDataList.Add(Mapper.Map<FormExamineMedicine, FormExamineMedicineModel>(formExamineMedicine));
                        //else if (formExamineMedicine.TypeID == ((int)MedicineTypeEnum.Concoction).ToString())
                        //    model.ConcoctionMedicine = formExamineMedicine.ConcoctionMedicine;
                    }

                    // get form examine lab and radiology if any
                    List<FormExamineLab> formExamineLabs = _unitOfWork.FormExamineLabRepository.Get(x => x.FormMedicalID == 0);
                    foreach (var formExamineLab in formExamineLabs)
                    {
                        if (formExamineLab.LabType == ((int)LabTypeEnum.Laboratorium).ToString())
                            model.LabDataList.Add(Mapper.Map<FormExamineLab, FormExamineLabModel>(formExamineLab));
                        else if (formExamineLab.LabType == ((int)LabTypeEnum.Radiology).ToString())
                            model.RadiologyDataList.Add(Mapper.Map<FormExamineLab, FormExamineLabModel>(formExamineLab));
                    }

                    // get form examine service if any
                    List<FormExamineService> formExamineServices = _unitOfWork.FormExamineServiceRepository.Get(x => x.FormExamineID == 0);
                    foreach (var formExamineService in formExamineServices)
                    {
                        if (!model.DefaultServiceList.Any(x => x.Id == formExamineService.ServiceID))
                            model.ServiceDataList.Add(Mapper.Map<FormExamineService, FormExamineServiceModel>(formExamineService));
                    }
                }
                else
                {
                    model.ExamineData.DoctorID = model.LoketData.DoctorID;
                    model.ExamineData.PoliID = model.LoketData.PoliToID;
                }


              //update letter table



               
                ViewBag.FinalStateList = BindDropDownFinalStateList();
              
                ViewBag.CausedList = BindDropDownCaused();
                ViewBag.ConditionList = BindDropDownCondition();
            }
            catch(Exception )
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
            //result.Add(new SelectListItem { Text = "Harus Kontrol Rutin", Value = "Harus Kontrol Rutin" });
            //result.Add(new SelectListItem { Text = "Opname / Rawat Inap", Value = "Opname / Rawat Inap" });
            //result.Add(new SelectListItem { Text = "Rujuk ke RS", Value = "Rujuk ke RS" });
            //result.Add(new SelectListItem { Text = "Drop Out", Value = "Drop Out" });
            //result.Add(new SelectListItem { Text = "Meninggal", Value = "Meninggal" });

            return result;
        }

        [NonAction]
        private List<SelectListItem> BindDropDownICDInfo()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "-choose Icd-"
            });
            var _qry = new ICDThemeHandler(_unitOfWork).GetAll();
            foreach (var item in _qry)
            {
                result.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"{ item.Code}-{item.Name}"
                });
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

        private List<SelectListItem> BindDropDownCaused()
        {
            var causedSelectList = GetGeneralMasterByType(Constants.MasterType.Caused);
            var lists = new List<SelectListItem>();
            foreach (var item in causedSelectList)
            {
                lists.Add(new SelectListItem
                {
                    Value = item.Value,
                    Text = item.Text
                });
            }
            return lists;
        }

        private List<SelectListItem> BindDropDownCondition()
        {
            var causedSelectList = GetGeneralMasterByType(Constants.MasterType.Condition);
            var lists = new List<SelectListItem>();
            foreach (var item in causedSelectList)
            {
                lists.Add(new SelectListItem
                {
                    Value = item.Value,
                    Text = item.Text
                });
            }
            return lists;
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
            string icdInformation,
      
         
            int valiCaused,
            int valiCondition,
            List<string> concoctionMedicineList,
           
            List<string> injectionList,
            List<string> labList,
            List<string> radiologyList,
            List<string> serviceList)
        {
         

            PoliExamineModel model = new PoliExamineModel();

            // For new registration data
          

            // Registration
            

            // FormExamine
            model.ExamineData.Anamnesa = anamnesa;
            model.ExamineData.Diagnose = diagnose;
            model.ExamineData.Therapy = therapy;
            model.ExamineData.Result = finalState;
            model.NoSurat = loketID;
            model.ExamineData.ICDInformation = icdInformation;
           
            model.ExamineData.Caused = valiCaused;
            model.ExamineData.Condition = valiCondition;
            // FormExamineMedicine
            

            //concoction Medicine
            foreach (var item in concoctionMedicineList)
            {
                string[] values = item.Split('|');
                FormExamineMedicineModel medModel = new FormExamineMedicineModel
                {
                    ConcoctionMedicine = values[0],
                    FormExamineID = long.Parse(formExamineID),
                    Dose = values[1],
                    MedicineJenis = values[2],
                    RemarkUse = values[3],
                    Qty =values[4]==""?0: Convert.ToDouble(values[4]),
                    TypeID = ((int)MedicineTypeEnum.Concoction).ToString()
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
                    TypeID = ((int)MedicineTypeEnum.Injection).ToString()
                };

                model.MedicineDataList.Add(medModel);
            }

            if (!string.IsNullOrEmpty(receipt))
            {
                FormExamineMedicineModel medModel = new FormExamineMedicineModel
                {
                    FormExamineID = long.Parse(formExamineID),
                    ConcoctionMedicine = receipt,
                    TypeID = ((int)MedicineTypeEnum.Concoction).ToString()
                };

                model.MedicineDataList.Add(medModel);
            }

            // FormExamineLab
            foreach (var item in labList)
            {
                FormExamineLabModel labModel = new FormExamineLabModel
                {
                    LabItemID = int.Parse(item),
                    LabType = ((int)LabTypeEnum.Laboratorium).ToString()
                };

                model.LabDataList.Add(labModel);
            }

            // FormExamineRadiology
            foreach (var item in radiologyList)
            {
                FormExamineLabModel labModel = new FormExamineLabModel
                {
                    LabItemID = int.Parse(item),
                    LabType = ((int)LabTypeEnum.Radiology).ToString()
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