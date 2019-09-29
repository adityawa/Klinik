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


        #region Drop Down List 

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

        private List<SelectListItem> BindDropDownPatient()
        {
            List<SelectListItem> _patients= new List<SelectListItem>();

            _patients.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (var item in new PatientHandler(_unitOfWork, _context).GetAll().ToList())
            {
                _patients.Add(new SelectListItem {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _patients;
        }


        private List<SelectListItem> BindDropDownEmployeeStatus()
        {
            List<SelectListItem> _empStatus = new List<SelectListItem>();

            _empStatus.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (var item in new EmployeeStatusHandler(_unitOfWork).GetAllEmployeeStatus())
            {
                _empStatus.Add(new SelectListItem
                {
                    Text = item.Description,
                    Value = item.Code
                });
            }

            return _empStatus;
        }

        private List<SelectListItem> BindDropDownFamilyStatus()
        {
            List<SelectListItem> _famStatus = new List<SelectListItem>();

            _famStatus.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (var item in new FamilyStatusHandler(_unitOfWork).GetAllFamilyStatus())
            {
                _famStatus.Add(new SelectListItem
                {
                    Text = item.Description,
                    Value = item.Code
                });
            }

            return _famStatus;
        }

        private List<SelectListItem> BindDropDownClinic()
        {
            List<SelectListItem> _clinics = new List<SelectListItem>();

            _clinics.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (var item in new ClinicHandler(_unitOfWork).GetAllClinic().ToList())
            {
                _clinics.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _clinics;
        }

        private List<SelectListItem> BindICDThemes()
        {
            List<SelectListItem> icds = new List<SelectListItem>();

            icds.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (var item in new ICDThemeHandler(_unitOfWork).GetAll().ToList())
            {
                icds.Add(new SelectListItem
                {
                    Text = item.Name, 
                    Value = item.Id.ToString()
                });
            }

            return icds;
        }

        private List<SelectListItem> BindRujukan()
        {
            List<SelectListItem> rujukans = new List<SelectListItem>();

            rujukans.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (var item in new MasterHandler(_unitOfWork).GetRujukans())
            {
                rujukans.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }
            return rujukans;
        }

        private List<SelectListItem> BindLookUpCategoryType()
        {
            var _types = new List<SelectListItem>();

            var lookUpCategories = new MasterHandler(_unitOfWork).GetLookupCategories();

            _types.Insert(0, new SelectListItem { Text = Klinik.Resources.UIMessages.SelectOneCategory, Value = "0" });

            foreach (var item in lookUpCategories)
            {
                _types.Add(new SelectListItem { Text = item.TypeName, Value = item.TypeName });
            }
            return _types;
        }


        #endregion
               
        // GET: Reports
        [CustomAuthorize("VIEW_REPORTS")]
        public ActionResult Index()
        {
            return View();
        }

        #region Top 10 Dieaseas Report
        [CustomAuthorize("VIEW_TOP_10_DISEASES")]
        public ActionResult Top10Dieases()
        {
            var model = new Top10DiseasesParamModel();

            ViewBag.Months = BindMonths();
            ViewBag.Years = BindYears();
            ViewBag.Clinics = BindDropDownClinic();
            ViewBag.LookUpCategories = BindLookUpCategoryType();

            return View(model);
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
            var chartByAge = ConstructSummaryByICDVsAge(response.Entity.DiseaseDataReports);
            charts.Add(chartByAge);
            response.Entity.Charts = charts;

            return View(response.Entity);
        }

        public ActionResult Top10DiseasesReport(Top10DiseaseReportModel model, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(model.DiseaseDataReports.ToPagedList(currentPageIndex, DefaultPageSize));
        }

        private Highcharts ConstructSummaryByICDVsAge(List<DiseaseReportDataModel> diseaseReportDataModels)
        {
            var icds = diseaseReportDataModels.Distinct().Select(x => x.ICDCode);
            var ages = diseaseReportDataModels.Distinct().Select(x => x.Age);
            var xnames = icds.ToList();

            var series = new List<Series>();

            foreach (var icd in icds)
            {
                var objects = new List<object>();

                foreach (var age in ages)
                {
                    var result = diseaseReportDataModels.FindAll(x => x.ICDCode == icd && x.Age == age);
                    if (result.Count > 0)
                    {
                        objects.Add(result.Count);
                    }
                    else
                    {
                        objects.Add(0);
                    }
                }
                
                series.Add(new Series
                {
                    Name = icd,
                    Data = new DotNet.Highcharts.Helpers.Data(objects.ToArray())
                });
            }

            Highcharts chart = new Highcharts("chart_by_age")
                 .InitChart(new Chart { DefaultSeriesType = ChartTypes.Spline })
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

        #endregion



        [CustomAuthorize("VIEW_TOP_10_REFERALS")]
        public ActionResult Top10Referals()
        {
            ViewBag.Years = BindYears();
            ViewBag.Months = BindMonths();
            ViewBag.ICDThemes = BindICDThemes();
            ViewBag.Hospitals = BindRujukan();
            ViewBag.Patients = BindDropDownPatient();

            return View();
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

        
        [CustomAuthorize("VIEW_TOP_10_COST")]
        public ActionResult Top10Cost()
        {
            ViewBag.Years = BindYears();
            //ViewBag.Months = BindDropDownByName(Constants.LookUpCategoryConstant.MONTH);
            //ViewBag.BusinessUnits = BindDropDownByName(Constants.LookUpCategoryConstant.BUSINESSUNIT);
            //ViewBag.BusinessUnits = BindDropDownByName(Constants.LookUpCategoryConstant.DEPARTMENT);
            ViewBag.Clinics = BindDropDownClinic();
            ViewBag.Patients = BindDropDownPatient();

            return View();
        }


        [CustomAuthorize("VIEW_TOP_10_COST")]
        [HttpPost]
        public ActionResult Top10CostReport()
        {
            return View();
        }

        [CustomAuthorize("VIEW_TOP_10_REQUEST")]
        public ActionResult Top10Request()
        {
            ViewBag.Years = BindYears();
            //ViewBag.Months = BindDropDownByName(Constants.LookUpCategoryConstant.MONTH);
            ViewBag.Clinics = BindDropDownClinic();
            //ViewBag.RequestTypes = BindDropDownByName(Constants.LookUpCategoryConstant.REQUESTTYPE);

            return View();
        }

        [CustomAuthorize("VIEW_TOP_10_REQUEST")]
        [HttpPost]
        public ActionResult Top10RequestReport()
        {

            return View();
        }

        #region Charts 

        #endregion 
    }
}