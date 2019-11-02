using AutoMapper;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Klinik.Common;
using Klinik.Common.Paging;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.Reports;
using Klinik.Features;
using Klinik.Features.ICDThemeFeatures;
using Klinik.Features.Reports;
using Klinik.Features.Reports.ReportLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class ReportsController : Controller
    {

        // GET: Patient
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;
        private const int DefaultPageSize = 10;

        public ReportsController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }


        #region General Drop Down List

        private List<SelectListItem> BindYears()
        {
            List<SelectListItem> _years = new List<SelectListItem>();
            var items = new List<int>();
            var currDate = DateTime.Now;

            for (int i = -5; i < 5; i++)
            {
                items.Add(currDate.Year + i);
            }


            _years.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (var item in items)
            {
                _years.Add(new SelectListItem
                {
                    Text = item.ToString(),
                    Value = item.ToString()
                });
            }

            return _years;
        }

        private List<SelectListItem> BindMonths()
        {
            var _months = new List<SelectListItem>();
            var _items = new List<GeneralMaster>();
            var masterHandler = new MasterHandler(_unitOfWork);

            _items = masterHandler.GetMasterDataByType(Constants.LookUpCategoryConstant.MONTH).ToList();

            _months.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (var item in _items)
            {
                _months.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value
                });
            }

            return _months;
        }

        private List<SelectListItem> BindDropDownClinic(string organisationId)
        {
            var _clinics = new List<SelectListItem>();
            var _clinicModels = new List<ClinicModel>();
            var _clinicHandler = new ClinicHandler(_unitOfWork);

            _clinics.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            var clinicId = new OrganizationHandler(_unitOfWork).GetOrganizationList().FirstOrDefault(x => x.OrgCode == organisationId).KlinikID;

            if (clinicId != 0)
            {
                _clinicModels = _clinicHandler.GetAllClinic(Convert.ToInt64(clinicId)).ToList();
            }
            else
            {
                _clinicModels = _clinicHandler.GetAllClinic().ToList();
            }


            foreach (var item in _clinicModels)
            {
                _clinics.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _clinics;
        }

        private List<SelectListItem> BindDropDownGudang()
        {
            var _gudangs = new List<SelectListItem>();

            _gudangs.Insert(0, new SelectListItem { Text = "All", Value = "0"});
            _gudangs.Insert(1, new SelectListItem { Text = Resources.UIMessages.Warehouse, Value = Constants.WareHouseTypes.Warehouse });
            _gudangs.Insert(2, new SelectListItem { Text = Resources.UIMessages.CentralWarehouse, Value = Constants.WareHouseTypes.CentralWarehouse });

            return _gudangs;
        }



        #endregion
               
        // GET: Reports
        [CustomAuthorize("VIEW_REPORTS")]
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetGeneralMasterByType(string type)
        {
            var result = new List<MasterModel>();

            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("Type");

            if (type != "0")
            {
                var items = new MasterHandler(_unitOfWork).GetMasterDataByType(type).ToList();

                foreach (var item in items)
                {
                    result.Add(Mapper.Map<GeneralMaster, MasterModel>(item));
                }
            }
            else
            {
                result = null;
            }
            return Json(
                         result, 
                        "application/json",
                         Encoding.UTF8,
                         JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult ExportExcel(int Id, string fileName)
        {
            var accountModel = new AccountModel();

            if (Session["UserLogon"] != null)
                accountModel = (AccountModel)Session["UserLogon"];

            var reportLog = new ReportLogHandler(_unitOfWork).GetReportLogById(Id, accountModel);
            return File(reportLog.Data[0].ExcelResult, ExcelExportHelper.ExcelContentType, fileName);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }
        #region Top 10 Dieaseas Report

        #region Public Methods 
        [CustomAuthorize("VIEW_TOP_10_DISEASES")]
        public ActionResult Top10Dieases()
        {
            var model = new Top10DiseasesParamModel();

            if (Session["UserLogon"] != null)
                model.Account = (AccountModel)Session["UserLogon"];

            ViewBag.Months = BindMonths();
            ViewBag.Years = BindYears();
            ViewBag.Clinics = BindDropDownClinic(model.Account.Organization);
            

            model.Categories = BindLookUpCategoryTypesForTop10Diseases();
            model.CategoryItems = BindLookUpCategoriesForTop10Diseases();

            return View(model);
        }

        [CustomAuthorize("VIEW_TOP_10_DISEASES")]
        [HttpPost]
        public ActionResult Top10DiseasesReport(Top10DiseasesParamModel model)
        {
            var response = new Top10DiseaseReportResponse();
            if (Session["UserLogon"] != null)
                model.Account = (AccountModel)Session["UserLogon"];

            var request = new Top10DiseaseReportRequest { Data = model };
            new ReportsValidator(_unitOfWork, _context).ValidateTop10DiseaseReport(request, out response);

            var charts = new List<Highcharts>();

            if (response.Entity.DiseaseDataReports.Count > 0)
            {
                var chartByAge = ReportLogHelper.GenerateChart(ClinicEnums.ReportType.Top10DiseaseReport, response.Entity);
                charts.Add(chartByAge);
                response.Entity.Charts = charts;
                response.Entity.ProcessId = ReportLogHelper.GenerateExcel(ClinicEnums.ReportType.Top10DiseaseReport, response.Entity, _unitOfWork, model.Account);
            }
            else
            {
                response.Entity.Charts = charts;
                response.Entity.ProcessId = 0;
            }
            return View(response.Entity);
        }
        
        #endregion

        #region Private Methods 
        private List<SelectListItem> BindLookUpCategoryTypesForTop10Diseases()
        {
            var _types = new List<SelectListItem>();
            var lookUpCategories = new List<LookupCategory>();
            var _masterHandler = new MasterHandler(_unitOfWork);

            lookUpCategories = _masterHandler.GetLookupCategories().Where(x => x.TypeName.Contains(Constants.LookUpCategoryConstant.DEPARTMENT) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.BUSINESSUNIT) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.GENDER) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.AGE) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.EMPLOYMENTTYPE) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.FAMILYSTATUS) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.NECESSITYTYPE) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.PAYMENTTYPE) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.NEEDREST) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.EXAMINETYPE)
                                                                            ).ToList();

            _types.Insert(0, new SelectListItem { Text = Klinik.Resources.UIMessages.SelectOneCategory, Value = "0" });

            foreach (var item in lookUpCategories)
            {
                if (item.RowStatus == 0)
                    _types.Add(new SelectListItem { Text = item.TypeName, Value = item.TypeName });
            }
            return _types;
        }

        private List<SelectListItem> BindLookUpCategoriesForTop10Diseases()
        {
            var _categories = new List<SelectListItem>();
            var _masters = new List<GeneralMaster>();

            var deptCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.DEPARTMENT);
            var businessUnitCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.BUSINESSUNIT);
            var genderCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.GENDER);
            var ageCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.AGE);
            var empStatusCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.EMPLOYMENTTYPE);
            var familyStatusCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.FAMILYSTATUS);
            var clinicStatusCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.NECESSITYTYPE);
            var paymentTypeCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.PAYMENTTYPE);
            var needRestCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.NEEDREST);
            var examineTypeCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.EXAMINETYPE);

            _categories.AddRange(deptCategories);
            _categories.AddRange(businessUnitCategories);
            _categories.AddRange(genderCategories);
            _categories.AddRange(ageCategories);
            _categories.AddRange(empStatusCategories);
            _categories.AddRange(familyStatusCategories);
            _categories.AddRange(clinicStatusCategories);
            _categories.AddRange(paymentTypeCategories);
            _categories.AddRange(needRestCategories);
            _categories.AddRange(examineTypeCategories);

            return _categories;
        }
        #endregion

        #endregion

        #region Top 10 Referal Reports
        #region Public Methods 
            [CustomAuthorize("VIEW_TOP_10_REFERALS")]
            public ActionResult Top10Referals()
            {
                var model = new Top10ReferalParamModel();

                if (Session["UserLogon"] != null)
                    model.Account = (AccountModel)Session["UserLogon"];

                ViewBag.Years = BindYears();
                ViewBag.Months = BindMonths();

                model.Categories = BindLookUpCategoryTypeForTop10Referal();
                model.CategoryItems = BindLookUpCategoriesForTop10Referals();

                return View(model);
            }

            [CustomAuthorize("VIEW_TOP_10_REFERALS")]
            [HttpPost]
            public ActionResult Top10ReferalsReport(Top10ReferalParamModel model)
            {
                var response = new Top10ReferalReportResponse();

                if (Session["UserLogon"] != null)
                        model.Account = (AccountModel)Session["UserLogon"];

                var request = new Top10ReferalReportRequest { Data = model };
            
                new ReportsValidator(_unitOfWork, _context).ValidateTop10ReferalReport(request, out response);

                var charts = new List<Highcharts>();
                if (response.Entity.ReferalReportDataModels.Count > 0)
                {
                    var chartByAge = ReportLogHelper.GenerateChart(ClinicEnums.ReportType.Top10ReferalReport, response.Entity);
                    charts.Add(chartByAge);
                    response.Entity.Charts = charts;
                    response.Entity.ProcessId = ReportLogHelper.GenerateExcel(ClinicEnums.ReportType.Top10ReferalReport, response.Entity, _unitOfWork, model.Account);
                }
                else
                {
                    response.Entity.Charts = charts;
                    response.Entity.ProcessId = 0;
                }
                
                return View(response.Entity);
            }

        #endregion

        #region Private Methods 
            private List<SelectListItem> BindLookUpCategoryTypeForTop10Referal()
            {
                var _types = new List<SelectListItem>();
             
                var _masterHandler = new MasterHandler(_unitOfWork);
                var patientCategory = _masterHandler.GetLookupCategories().FirstOrDefault(x => x.TypeName == Constants.LookUpCategoryConstant.PATIENT);
                var icdCategory = _masterHandler.GetLookupCategories().FirstOrDefault(x => x.TypeName == Constants.LookUpCategoryConstant.ICDTHEME);
                var doctorCategory = _masterHandler.GetLookupCategories().FirstOrDefault(x => x.TypeName == Constants.LookUpCategoryConstant.DOCTORANDHOSPITAL);

                var lookUpCategories = new List<LookupCategory>() { patientCategory, icdCategory, doctorCategory};

                _types.Insert(0, new SelectListItem { Text = Klinik.Resources.UIMessages.SelectOneCategory, Value = "0" });

                foreach (var item in lookUpCategories)
                {
                    if (item.RowStatus == 0)
                        _types.Add(new SelectListItem { Text = item.TypeName, Value = item.TypeName });
                }
                return _types;
            }

            private List<SelectListItem> BindLookUpCategoriesForTop10Referals()
            {
                var _categories = new List<SelectListItem>();
                var _masters = new List<GeneralMaster>();

                var patientCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.PATIENT);
                var icdCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.ICDTHEME);
                var rujukanCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.DOCTORANDHOSPITAL);

                _categories.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                _categories.AddRange(icdCategories);
                _categories.AddRange(rujukanCategories);
                _categories.AddRange(patientCategories);

                return _categories;
            }

        #endregion

        #endregion

        #region Top 10 Cost Report

        #region Public Methods 
        [CustomAuthorize("VIEW_TOP_10_COST")]
        public ActionResult Top10Cost()
        {
            var model = new Top10CostParamModel();
            
            if (Session["UserLogon"] != null)
                model.Account = (AccountModel)Session["UserLogon"];

            ViewBag.Months = BindMonths();
            ViewBag.Years = BindYears();
            ViewBag.Clinics = BindDropDownClinic(model.Account.Organization);
            ViewBag.WareHouses = BindDropDownGudang();

            model.Categories = BindLookUpCategoryTypeForTop10Cost();
            model.CategoryItems = BindLookUpCategoriesForTop10Cost();

            return View(model);
        }


        [CustomAuthorize("VIEW_TOP_10_COST")]
        [HttpPost]
        public ActionResult Top10CostReport()
        {
            return View();
        }
        #endregion

        #region Private Methods 
        private List<SelectListItem> BindLookUpCategoryTypeForTop10Cost()
        {
            var _types = new List<SelectListItem>();
            var lookUpCategories = new List<LookupCategory>();
            var _masterHandler = new MasterHandler(_unitOfWork);

            lookUpCategories = _masterHandler.GetLookupCategories().Where(x => x.TypeName.Contains(Constants.LookUpCategoryConstant.DEPARTMENT) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.BUSINESSUNIT) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.PATIENT)
                                                                            ).ToList();

            _types.Insert(0, new SelectListItem { Text = Klinik.Resources.UIMessages.SelectOneCategory, Value = "0" });

            foreach (var item in lookUpCategories)
            {
                if (item.RowStatus == 0)
                    _types.Add(new SelectListItem { Text = item.TypeName, Value = item.TypeName });
            }
            return _types;
        }

        private List<SelectListItem> BindLookUpCategoriesForTop10Cost()
        {
            var _categories = new List<SelectListItem>();
            var _masters = new List<GeneralMaster>();

            var deptCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.DEPARTMENT);
            var patientCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.PATIENT);
            var buCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.BUSINESSUNIT);

            _categories.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            _categories.AddRange(deptCategories);
            _categories.AddRange(buCategories);
            _categories.AddRange(patientCategories);

            return _categories;
        }
        #endregion

        #endregion

        #region Top 10 Request Report

        #region Public Methods 
        [CustomAuthorize("VIEW_TOP_10_REQUEST")]
        public ActionResult Top10Request()
        {
            var model = new Top10RequestParamModel();

            if (Session["UserLogon"] != null)
                model.Account = (AccountModel)Session["UserLogon"];

            ViewBag.Months = BindMonths();
            ViewBag.Years = BindYears();
            ViewBag.Clinics = BindDropDownClinic(model.Account.Organization);
            ViewBag.WareHouses = BindDropDownGudang();

            model.Categories = BindLookUpCategoryTypeForTop10Request();
            model.CategoryItems = BindLookUpCategoriesForTop10Request();

            return View(model);
        }

        [CustomAuthorize("VIEW_TOP_10_REQUEST")]
        [HttpPost]
        public ActionResult Top10RequestReport()
        {

            return View();
        }
        #endregion

        #region Private Methods 
        private List<SelectListItem> BindLookUpCategoryTypeForTop10Request()
        {
            var _types = new List<SelectListItem>();
            var lookUpCategories = new List<LookupCategory>();
            var _masterHandler = new MasterHandler(_unitOfWork);

            lookUpCategories = _masterHandler.GetLookupCategories().Where(x => x.TypeName.Contains(Constants.LookUpCategoryConstant.REQUESTTYPE)).ToList();

            _types.Insert(0, new SelectListItem { Text = Klinik.Resources.UIMessages.SelectOneCategory, Value = "0" });

            foreach (var item in lookUpCategories)
            {
                if (item.RowStatus == 0)
                    _types.Add(new SelectListItem { Text = item.TypeName, Value = item.TypeName });
            }
            return _types;
        }

        private List<SelectListItem> BindLookUpCategoriesForTop10Request()
        {
            var _categories = new List<SelectListItem>();
            var _masters = new List<GeneralMaster>();

            var reqTypeCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.REQUESTTYPE);
            var wareHouseCategories = ConstructGeneralMasterList(Constants.LookUpCategoryConstant.GUDANGTYPE);

            _categories.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            _categories.AddRange(reqTypeCategories);
            _categories.AddRange(wareHouseCategories);
            
            return _categories;
        }
        #endregion 
        #endregion

        #region Common
        /// <summary>
        /// Construct General Master List
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<SelectListItem> ConstructGeneralMasterList(string type)
        {
            var _items = new List<SelectListItem>();
            var _generalMasters = new List<GeneralMaster>();
            var _masterHandler = new MasterHandler(_unitOfWork);

            var category = _masterHandler.GetLookupCategoryByName(type).FirstOrDefault();

            if (category != null)
            {
                var masters = _masterHandler.GetMasterDataByType(category.TypeName);
                if (masters.Count() > 0)
                {
                    foreach (var item in masters)
                    {
                        _items.Add(new SelectListItem {
                            Text = item.Name,
                            Value = item.Value
                        });
                    }
                }
            }

            return _items;
        }

        #endregion 
    }
}