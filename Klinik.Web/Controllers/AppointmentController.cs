﻿using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.AppointmentEntities;
using Klinik.Entities.PoliSchedules;
using Klinik.Features;
using Klinik.Features.AppointmentFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{

    public class TempClass
    {
        public long id { get; set; }
        public string nik { get; set; }
        public string label { get; set; }


    }
    public class AppointmentController : BaseController
    {
        // GET: Appointment

        public AppointmentController(IUnitOfWork unitOfWork, KlinikDBEntities context) :
            base(unitOfWork, context)
        {
        }
        [CustomAuthorize("VIEW_APPOINTMENT")]
        public ActionResult MakeAppointment()
        {
            var _model = new AppointmentModel();
            _model.ListPoli = new AppointmentHandler(_unitOfWork).GetAllPoli();
            List<SelectListItem> poliColls = new List<SelectListItem>();
            foreach (var item in _model.ListPoli)
            {
                poliColls.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });
            }

            ViewBag.PoliList = poliColls;
            ViewBag.McuPackages = BindMCUPackage();
            ViewBag.Necessities = BindNecesity();
            _model.AppointmentDate = DateTime.Now;
            return View(_model);
        }
        [CustomAuthorize("VIEW_APPOINTMENT")]
        public ActionResult ViewDoctorSchedule()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetPoliScheduleData(string poliId, string tanggal)
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            if (String.IsNullOrEmpty(tanggal))
                tanggal = DateTime.Now.ToString("dd/MM/yyyy");

            var request = new AppointmentRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new AppointmentModel
                {
                    PoliID = Convert.ToInt64(poliId),
                    AppointmentDate = CommonUtils.ConvertStringDate2Datetime(tanggal)
                }

            };

            var response = new AppointmentHandler(_unitOfWork).GetDoctorBasedOnPoli(request);

            return Json(new { data = response.schedules, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPoliScheduleInfo(string poliScheduleID, string poliID, string tanggal)
        {
            var response = new PoliScheduleResponse();
            long _idPoliSchedule = 0;
            if (poliScheduleID != null)
            {
                _idPoliSchedule = Convert.ToInt64(poliScheduleID);
            }
            var _poliScheduleModel = new PoliScheduleModel
            {
                Id = _idPoliSchedule
            };

            var request = new PoliScheduleRequest
            {
                Data = _poliScheduleModel
            };
            response = new PoliScheduleHandler(_unitOfWork).GetDetail(request);
            return Json(new
            {
                Status = response.Status,
                Message = response.Message,
                ClinicName = response.Entity.ClinicName,
                PoliName = response.Entity.PoliName,
                DoctorName = response.Entity.DoctorName,
                ClinicId = response.Entity.ClinicID,
                DoctorId = response.Entity.DoctorID,
                PoliID = poliID
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateAppointment(string employeeID, string clinicId, string poliId, string doctorId, string necesity, string AppointmentDate, string MCUPackage)
        {
            var response = new AppointmentResponse();
            var _model = new AppointmentModel
            {
                AppointmentDate = CommonUtils.ConvertStringDate2Datetime(AppointmentDate),
                ClinicID = Convert.ToInt64(clinicId),
                EmployeeID = Convert.ToInt64(employeeID),
                DoctorID = Convert.ToInt64(doctorId),
                PoliID = Convert.ToInt64(poliId),
                RequirementID = Convert.ToInt16(necesity),
                MCUPakageID = Convert.ToInt64(MCUPackage),

            };

            if (Session["UserLogon"] != null)
            {
                _model.Account = (AccountModel)Session["UserLogon"];
            }
            var request = new AppointmentRequest
            {
                Data = _model
            };

            response = new AppointmentValidator(_unitOfWork).Validate(request);

            return Json(new { Status = response.Status, Message = response.Message }, JsonRequestBehavior.AllowGet);
        }


        #region ::MISCELANOUS::
        public List<SelectListItem> BindNecesity()
        {
            return GetGeneralMasterByType(Constants.MasterType.NECESSITY_TYPE);
        }

        public List<SelectListItem> BindMCUPackage()
        {
            List<SelectListItem> mcuPackages = new List<SelectListItem>();
            var qryData = new AppointmentHandler(_unitOfWork).GetMCUPackage();
            foreach (var item in qryData)
            {
                mcuPackages.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });
            }
            return mcuPackages;
        }

        [HttpPost]
        public JsonResult AutoCompleteEmployee(string prefix)
        {
            long _employeeId = 0;
            List<Employee> employeeList = new List<Employee>();
            if (Session["UserLogon"] != null)
            {
              var _account   = (AccountModel)Session["UserLogon"];
                if(_account!=null)
                {
                    if (_account.EmployeeID > 0)
                        _employeeId = _account.EmployeeID;
                }
            }
            List<string> exceptionStatus = new List<string>();
            exceptionStatus.Add("Na");
            exceptionStatus.Add("R");

            var except = _unitOfWork.EmployeeStatusRepository.Get(x => exceptionStatus.Contains(x.Code)).Select(x => x.ID).ToList();
            if (_employeeId == 0)
            {
                employeeList = _unitOfWork.EmployeeRepository.Get(x => x.RowStatus == 0 && !except.Contains(x.Status ?? 0)).ToList();

            }
            else
            {
                employeeList = _unitOfWork.EmployeeRepository.Get(x => x.RowStatus == 0 && x.ID==_employeeId && !except.Contains(x.Status ?? 0)).ToList();

            }

            var filteredList = employeeList.Where(t => t.EmpName.ToLower().Contains(prefix.ToLower()));

            List<TempClass> resultList = new List<TempClass>();
            foreach (var item in filteredList)
            {
                TempClass temp = new TempClass
                {
                    id = item.ID,
                    nik = item.EmpID,
                    label = item.EmpName,

                };

                resultList.Add(temp);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}