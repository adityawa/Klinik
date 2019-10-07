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
using System;
using System.Collections.Generic;
using System.Linq;
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


        #region Genral Drop Down List

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
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Top 10 Dieaseas Report
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
            var chartByAge = ConstructSummaryByICDForTop10Diseases(response.Entity.DiseaseDataReports);
            charts.Add(chartByAge);
            response.Entity.Charts = charts;

            return View(response.Entity);
        }

        public ActionResult Top10DiseasesReport(Top10DiseaseReportModel model, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(model.DiseaseDataReports.ToPagedList(currentPageIndex, DefaultPageSize));
        }

        private Highcharts ConstructSummaryByICDForTop10Diseases(List<DiseaseReportDataModel> diseaseReportDataModels)
        {
            var icds = diseaseReportDataModels.Select(x => x.ICDCode).Distinct().ToList();
            var categories = diseaseReportDataModels.Select(x => x.Category).Distinct().ToList();
            var xnames = categories.ToList();

            var series = new List<Series>();


            foreach (var cat in categories)
            {
                var objects = new List<object>();
                foreach (var icd in icds)
                {
                    var result = diseaseReportDataModels.FindAll(x => x.ICDCode == icd && x.Category == cat);
                    if (result.Count > 0)
                    {
                        var total = 0;
                        foreach (var item in result)
                        {
                            total += item.Total;
                        }
                        objects.Add(total);
                    }
                    else
                    {
                        objects.Add(0);
                    }
                }
                series.Add(new Series
                {
                    Name = cat,
                    Data = new DotNet.Highcharts.Helpers.Data(objects.ToArray())
                });
            }

            Highcharts chart = new Highcharts("chart_by_category")
                 .InitChart(new Chart { DefaultSeriesType = ChartTypes.Bar })
                 .SetTitle(new Title { Text = "Total Pasien Berdasarkan tipe ICD" })
                 .SetXAxis(new XAxis { Categories = icds.ToArray() })
                 .SetYAxis(new YAxis
                 {
                     Title = new YAxisTitle { Text = "Total Perawatan" },
                     Min = 0
                 })
                 .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.series.name +' : '+ this.y +' </b>'; }" })
                 .SetSeries(series.ToArray());

            return chart;

        }


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

        #region Top 10 Referal Reports
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

            return View(response.Entity);
        }


        private List<SelectListItem> BindLookUpCategoryTypeForTop10Referal()
        {
            var _types = new List<SelectListItem>();
            var lookUpCategories = new List<LookupCategory>();
            var _masterHandler = new MasterHandler(_unitOfWork);

            lookUpCategories = _masterHandler.GetLookupCategories().Where(x => x.TypeName.Contains(Constants.LookUpCategoryConstant.PATIENT) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.DOCTORANDHOSPITAL) ||
                                                                            x.TypeName.Contains(Constants.LookUpCategoryConstant.ICDTHEME) 
                                                                            ).ToList();

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

            _categories.AddRange(icdCategories);
            _categories.AddRange(rujukanCategories);
            _categories.AddRange(patientCategories);

            return _categories;
        }


        #endregion

        #region Top 10 Cost Report
        [CustomAuthorize("VIEW_TOP_10_COST")]
        public ActionResult Top10Cost()
        {
            ViewBag.Years = BindYears();
            //ViewBag.Months = BindDropDownByName(Constants.LookUpCategoryConstant.MONTH);
            //ViewBag.BusinessUnits = BindDropDownByName(Constants.LookUpCategoryConstant.BUSINESSUNIT);
            //ViewBag.BusinessUnits = BindDropDownByName(Constants.LookUpCategoryConstant.DEPARTMENT);
            //ViewBag.Clinics = BindDropDownClinic();
            //ViewBag.Patients = BindDropDownPatient();

            return View();
        }


        [CustomAuthorize("VIEW_TOP_10_COST")]
        [HttpPost]
        public ActionResult Top10CostReport()
        {
            return View();
        }
        #endregion

        #region Top 10 Request Report
        [CustomAuthorize("VIEW_TOP_10_REQUEST")]
        public ActionResult Top10Request()
        {
            ViewBag.Years = BindYears();
            //ViewBag.Months = BindDropDownByName(Constants.LookUpCategoryConstant.MONTH);
            //ViewBag.Clinics = BindDropDownClinic();
            //ViewBag.RequestTypes = BindDropDownByName(Constants.LookUpCategoryConstant.REQUESTTYPE);

            return View();
        }

        [CustomAuthorize("VIEW_TOP_10_REQUEST")]
        [HttpPost]
        public ActionResult Top10RequestReport()
        {

            return View();
        }
        #endregion 

        #region Common
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