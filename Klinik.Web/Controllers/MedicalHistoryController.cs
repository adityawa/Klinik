using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.MedicalHistoryEntity;
using Klinik.Features.HistoryMedical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class MedicalHistoryController : BaseController
    {
        public MedicalHistoryController(IUnitOfWork unitOfWork, KlinikDBEntities context):base(unitOfWork, context)
        {

        }
        // GET: MedicalHistory
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize("VIEW_MEDICAL_HISTORY")]
        public ActionResult ViewEmployeeFamilyData()
        {
            return View();
        }

        [CustomAuthorize("VIEW_MEDICAL_HISTORY")]
        public ActionResult ViewDetailExamine()
        {
            long idFrmMed = 0;
            if (Request.QueryString["frmMedical"] != null)
            {
                idFrmMed = Convert.ToInt64(Request.QueryString["frmMedical"].ToString());
            }

            var idRegistration = new MedicalHistoryHandler(_unitOfWork).GetRegNoBasedOnFormMedical(idFrmMed);
            string _url = Url.Action("FormExamine", "Poli");
            _url = _url + "?id=" + idRegistration+"&IsViewOnly=true";
            return Redirect(_url);
        }


        [CustomAuthorize("VIEW_MEDICAL_HISTORY")]
        public ActionResult ViewEmployeeData()
        {
            var account = new AccountModel();
            bool isCanViewAll = IsHaveAuthorization(Constants.ROLE_NAME.VIEW_MEDICAL_HISTORY_ALL.ToString());
          
            if (isCanViewAll)
            {
                ViewBag.CanViewAll = true;
                
            }
            else
            {
                ViewBag.CanViewAll = false;
                ViewBag.Nik = account.EmployeeID;
            }
            return View();
        }

        public ActionResult GetEmployeeByNik(string nik)
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;
            var employee = new EmployeeModel
            {
                EmpID = nik
            };

            var request = new MedicalHistoryRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new MedicalHistoryModel
                {
                    EmployeeData = employee
                }

            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new MedicalHistoryHandler(_unitOfWork).getEmployeeBaseOnEmpNo(request);

            return Json(new { data = response.Employees, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmployeeFamilyByNik(string nik)
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;
            var employee = new EmployeeModel
            {
                Id = Convert.ToInt64(nik)
            };

            var request = new MedicalHistoryRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new MedicalHistoryModel
                {
                    EmployeeData = employee
                }

            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new MedicalHistoryHandler(_unitOfWork).getMedicalActivityHist(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
    }
}