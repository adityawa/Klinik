using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Loket;
using Klinik.Entities.Poli;
using Klinik.Features.Loket;
using Klinik.Resources;
using System;
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

        public ActionResult FormExamine()
        {
            var request = new LoketRequest
            {
                Data = new LoketModel
                {
                    Id = long.Parse(Request.QueryString["id"].ToString())
                }
            };

            LoketResponse resp = new LoketHandler(_unitOfWork).GetDetail(request);
            PoliExamineModel model = new PoliExamineModel();
            model.LoketData = resp.Entity;

            int age = CalculateAge(model.LoketData.PatientBirthDateStr);
            if (age > 0)
            {
                model.PatientAge = age.ToString() + " " + UIMessages.Years;
            }
            else
            {
                age = CalculateAgeInMonth(model.LoketData.PatientBirthDateStr);
                model.PatientAge = age.ToString() + " " + UIMessages.Month;
            }

            var necessityTypeList = GetGeneralMasterByType(Constants.MasterType.NECESSITY_TYPE);
            var paymentTypeList = GetGeneralMasterByType(Constants.MasterType.PAYMENT_TYPE);

            model.NecessityTypeStr = necessityTypeList.FirstOrDefault(x => x.Value == model.LoketData.NecessityType.ToString()).Text;
            model.PaymentTypeStr = paymentTypeList.FirstOrDefault(x => x.Value == model.LoketData.PaymentType.ToString()).Text;

            return View(model);
        }

        private int CalculateAge(string dateOfBirth)
        {
            DateTime dob = Convert.ToDateTime(dateOfBirth);
            int age = 0;
            age = DateTime.Now.Year - dob.Year;
            if (DateTime.Now.DayOfYear < dob.DayOfYear)
                age = age - 1;

            return age;
        }

        private int CalculateAgeInMonth(string dateOfBirth)
        {
            DateTime dob = Convert.ToDateTime(dateOfBirth);
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(dob).Ticks).Year - 1;
            DateTime PastYearDate = dob.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Now)
                {
                    Months = i - 1;
                    break;
                }
            }

            return Months;
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
    }
}
