using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class ReportsController : Controller
    {

        // GET: Patient
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public ReportsController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }


        #region Drop Down List 

        private List<SelectListItem> BindDropDownClinic()
        {
            List<SelectListItem> _clinics = new List<SelectListItem>();

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

        private List<SelectListItem> BindDropDownByName(string name)
        {
            var _types = new List<SelectListItem>();
            var items = new List<LookupCategory>();
            var masterHandler = new MasterHandler(_unitOfWork);

            switch (name)
            {
                case Constants.LookUpCategoryConstant.DEPARTMENT:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.DEPARTMENT).ToList();
                    break;
                case Constants.LookUpCategoryConstant.AGE:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.AGE).ToList();
                    break;
                case Constants.LookUpCategoryConstant.BUSINESSUNIT:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.BUSINESSUNIT).ToList();
                    break;
                case Constants.LookUpCategoryConstant.FAMILYSTATUS:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.FAMILYSTATUS).ToList();
                    break;
                case Constants.LookUpCategoryConstant.NECESSITYTYPE:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.NECESSITYTYPE).ToList();
                    break;
                case Constants.LookUpCategoryConstant.GENDER:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.AGE).ToList();
                    break;
                case Constants.LookUpCategoryConstant.PATIENTCATEGORY:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.PATIENTCATEGORY).ToList();
                    break;
                case Constants.LookUpCategoryConstant.PAYMENTTYPE:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.PAYMENTTYPE).ToList();
                    break;
                case Constants.LookUpCategoryConstant.NEEDREST:
                    items = masterHandler.GetLookupCategoryByName(Constants.LookUpCategoryConstant.NEEDREST).ToList();
                    break;
            }

            foreach (var item in items)
            {
                _types.Add(new SelectListItem {
                    Text = item.LookUpContent,
                    Value = item.LookUpCode
                });
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

        [CustomAuthorize("VIEW_TOP_10_DISEASES")]
        public ActionResult Top10Dieases()
        {
            ViewBag.Clinics = BindDropDownClinic();
            ViewBag.Departments = BindDropDownByName(Constants.LookUpCategoryConstant.DEPARTMENT);
            ViewBag.BusinessUnits = BindDropDownByName(Constants.LookUpCategoryConstant.BUSINESSUNIT);
            ViewBag.Genders = BindDropDownByName(Constants.LookUpCategoryConstant.GENDER);
            ViewBag.Ages = BindDropDownByName(Constants.LookUpCategoryConstant.AGE);
            ViewBag.PatientCategories = BindDropDownByName(Constants.LookUpCategoryConstant.PATIENTCATEGORY);
            ViewBag.FamilyStatuses = BindDropDownByName(Constants.LookUpCategoryConstant.FAMILYSTATUS);
            ViewBag.NecessityTypes = BindDropDownByName(Constants.LookUpCategoryConstant.NECESSITYTYPE);
            ViewBag.NeedRests = BindDropDownByName(Constants.LookUpCategoryConstant.NEEDREST);
            return View();
        }

        [CustomAuthorize("VIEW_TOP_10_REFERALS")]
        public ActionResult Top10Referals()
        {

            return View();
        }
        
        [CustomAuthorize("VIEW_TOP_10_COST")]
        public ActionResult Top10Cost()
        {
            return View();
        }

        [CustomAuthorize("VIEW_TOP_10_REQUEST")]
        public ActionResult Top10Request()
        {
            return View();
        }
    }
}