using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.PoliSchedules;
using Klinik.Features;
using Klinik.Features.PoliSchedules;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class PoliScheduleController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public PoliScheduleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Dropdown Methods
        private List<SelectListItem> BindDropDownPoli(short poliType = 0)
        {
            // get valid poli from type
            var filteredPoliList = _unitOfWork.PoliFlowTemplateRepository.Get(x => x.PoliTypeID == poliType);

            // get all poli
            var qry = _unitOfWork.PoliRepository.Get();

            IList<PoliModel> _poliListModel = new List<PoliModel>();
            foreach (var item in qry)
            {
                if (filteredPoliList.Any(x => x.PoliTypeIDTo == item.Type))
                {
                    var _poli = Mapper.Map<Poli, PoliModel>(item);
                    _poliListModel.Add(_poli);
                }
            }

            List<SelectListItem> _poliList = new List<SelectListItem>();
            foreach (var item in _poliListModel)
            {
                _poliList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _poliList;
        }

        private dynamic BindDropDownDoctor()
        {
            List<SelectListItem> _typeList = new List<SelectListItem>();
            List<Doctor> doctorList = _unitOfWork.DoctorRepository.Get();
            foreach (var item in doctorList)
            {
                _typeList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _typeList;
        }

        private dynamic BindDropDownClinic()
        {
            List<SelectListItem> _typeList = new List<SelectListItem>();
            List<Clinic> clinicList = _unitOfWork.ClinicRepository.Get();
            foreach (var item in clinicList)
            {
                _typeList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _typeList;
        }

        private List<SelectListItem> BindDropDownDay()
        {
            List<SelectListItem> _dayList = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.Day.ToString()).ToList())
            {
                _dayList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value
                });
            }

            return _dayList;
        }
        #endregion

        #region ::POLISCHEDULEMASTER::
        [CustomAuthorize("VIEW_M_POLISCHEDULE_M")]
        public ActionResult Master() => View();

        [HttpPost]
        public ActionResult CreateOrEditPoliScheduleMaster(PoliScheduleMasterModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            // convert the time
            DateTime startDate = DateTime.ParseExact(_model.StartTimeStr, "HH:mm", CultureInfo.InvariantCulture);
            _model.StartTime = startDate.TimeOfDay;

            DateTime endDate = DateTime.ParseExact(_model.EndTimeStr, "HH:mm", CultureInfo.InvariantCulture);
            _model.EndTime = endDate.TimeOfDay;

            var request = new PoliScheduleMasterRequest
            {
                Data = _model,
            };

            PoliScheduleMasterResponse _response = new PoliScheduleMasterValidator(_unitOfWork).Validate(request);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Clinics = BindDropDownClinic();
            ViewBag.Doctors = BindDropDownDoctor();
            ViewBag.Polis = BindDropDownPoli();
            ViewBag.Days = BindDropDownDay();
            ViewBag.ActionType = _model.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_POLISCHEDULE_M", "EDIT_M_POLISCHEDULE_M")]
        public ActionResult CreateOrEditPoliScheduleMaster()
        {
            PoliScheduleMasterResponse _response = new PoliScheduleMasterResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PoliScheduleMasterRequest
                {
                    Data = new PoliScheduleMasterModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PoliScheduleMasterResponse resp = new PoliScheduleMasterHandler(_unitOfWork).GetDetail(request);
                PoliScheduleMasterModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Clinics = BindDropDownClinic();
                ViewBag.Doctors = BindDropDownDoctor();
                ViewBag.Polis = BindDropDownPoli();
                ViewBag.Days = BindDropDownDay();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.Response = _response;
                ViewBag.Clinics = BindDropDownClinic();
                ViewBag.Doctors = BindDropDownDoctor();
                ViewBag.Polis = BindDropDownPoli();
                ViewBag.Days = BindDropDownDay();
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetListDataMaster()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PoliScheduleMasterRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PoliScheduleMasterHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePoliScheduleMaster(int id)
        {
            var request = new PoliScheduleMasterRequest
            {
                Data = new PoliScheduleMasterModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            PoliScheduleMasterResponse _response = new PoliScheduleMasterValidator(_unitOfWork).Validate(request);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::POLISCHEDULE::
        [CustomAuthorize("VIEW_M_POLISCHEDULE")]
        public ActionResult Index() => View();

        [HttpPost]
        public ActionResult CreateOrEditPoliSchedule(PoliScheduleModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new PoliScheduleRequest
            {
                Data = _model,
            };

            if (_model.ReffID != 0)
                request.Action = ClinicEnums.Action.Reschedule.ToString();

            PoliScheduleResponse _response = new PoliScheduleValidator(_unitOfWork).Validate(request);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Clinics = BindDropDownClinic();
            ViewBag.Doctors = BindDropDownDoctor();
            ViewBag.Polis = BindDropDownPoli();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            if (_model.ReffID != 0)
                return View("Index");
            else
                return View();
        }

        [CustomAuthorize("ADD_M_POLISCHEDULE", "EDIT_M_POLISCHEDULE")]
        public ActionResult CreateOrEditPoliSchedule()
        {
            PoliScheduleResponse _response = new PoliScheduleResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PoliScheduleRequest
                {
                    Data = new PoliScheduleModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PoliScheduleResponse resp = new PoliScheduleHandler(_unitOfWork).GetDetail(request);
                PoliScheduleModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Clinics = BindDropDownClinic();
                ViewBag.Doctors = BindDropDownDoctor();
                ViewBag.Polis = BindDropDownPoli();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.Response = _response;
                ViewBag.Clinics = BindDropDownClinic();
                ViewBag.Doctors = BindDropDownDoctor();
                ViewBag.Polis = BindDropDownPoli();
                return View();
            }
        }

        [CustomAuthorize("ADD_M_POLISCHEDULE", "EDIT_M_POLISCHEDULE")]
        public ActionResult Reschedule()
        {
            PoliScheduleResponse _response = new PoliScheduleResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PoliScheduleRequest
                {
                    Data = new PoliScheduleModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PoliScheduleResponse resp = new PoliScheduleHandler(_unitOfWork).GetDetail(request);
                PoliScheduleModel _model = resp.Entity;
                _model.ReffID = _model.Id;
                ViewBag.Response = _response;
                ViewBag.Clinics = BindDropDownClinic();
                ViewBag.Doctors = BindDropDownDoctor();
                ViewBag.Polis = BindDropDownPoli();
                ViewBag.ActionType = ClinicEnums.Action.Reschedule;

                return View("CreateOrEditPoliSchedule", _model);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult GetListData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PoliScheduleRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PoliScheduleHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePoliSchedule(int id)
        {
            var request = new PoliScheduleRequest
            {
                Data = new PoliScheduleModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            PoliScheduleResponse _response = new PoliScheduleValidator(_unitOfWork).Validate(request);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
