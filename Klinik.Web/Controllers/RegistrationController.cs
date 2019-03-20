using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.Registration;
using Klinik.Features.Registration;
using Klinik.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class RegistrationController : Controller
    {
        private const int CURRENT_POLI_ID = 1;
        private const string CURRENT_POLI_NAME = "Loket";

        // GET: Account
        private IUnitOfWork _unitOfWork;

        public RegistrationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region DropDown methods
        private List<SelectListItem> BindDropDownPoliList(short poliType = 0)
        {
            // get valid poli from type
            var filteredPoliList = _unitOfWork.PoliFlowTemplateRepository.Get(x => x.PoliTypeID.Value == poliType);

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

        private List<SelectListItem> BindDropDownPatientList()
        {
            var qry = _unitOfWork.PatientRepository.Get();
            IList<PatientModel> _patientModelList = new List<PatientModel>();
            foreach (var item in qry)
            {
                var _poli = Mapper.Map<Patient, PatientModel>(item);
                _patientModelList.Add(_poli);
            }

            List<SelectListItem> _patientList = new List<SelectListItem>();
            foreach (var item in _patientModelList)
            {
                _patientList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _patientList;
        }

        private List<SelectListItem> BindDropDownTypeList()
        {
            List<SelectListItem> _typeList = new List<SelectListItem>();
            foreach (RegistrationTypeEnum item in Enum.GetValues(typeof(RegistrationTypeEnum)))
            {
                _typeList.Add(new SelectListItem
                {
                    Text = item.ToString().Replace("WalkIn", "Walk-In"),
                    Value = ((int)item).ToString()
                });
            }

            return _typeList;
        }

        private List<SelectListItem> BindDropDownDoctorList(int poliID)
        {
            List<SelectListItem> _typeList = new List<SelectListItem>();
            List<PoliSchedule> scheduleList = _unitOfWork.PoliScheduleRepository.Get(x => x.PoliID == poliID);
            foreach (var item in scheduleList)
            {
                var doctor = _unitOfWork.DoctorRepository.GetFirstOrDefault(x => x.ID == item.DoctorID);
                if (doctor != null)
                {
                    _typeList.Add(new SelectListItem
                    {
                        Text = doctor.Name,
                        Value = doctor.ID.ToString()
                    });
                }
            }

            return _typeList;
        }

        public JsonResult GetDoctorList(int poliID)
        {
            List<Doctor> doctorList = new List<Doctor>();
            List<PoliSchedule> scheduleList = _unitOfWork.PoliScheduleRepository.Get(x => x.PoliID == poliID);
            foreach (var item in scheduleList)
            {
                var doctor = _unitOfWork.DoctorRepository.GetFirstOrDefault(x => x.ID == item.DoctorID);
                if (doctor != null)
                {
                    doctorList.Add(new Doctor { ID = doctor.ID, Name = doctor.Name });
                }
            }

            return Json(doctorList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [CustomAuthorize("VIEW_REGISTRATION")]
        public ActionResult Index()
        {
            if (Session["UserLogon"] != null)
            {
                AccountModel account = (AccountModel)Session["UserLogon"];
                bool isHasPrivilege = IsHaveAuthorization("ADD_REGISTRATION", account.Privileges.PrivilegeIDs);
                ViewBag.IsHasAddPrivilege = isHasPrivilege;
            }

            return View();
        }

        [CustomAuthorize("VIEW_REGISTRATION_UMUM")]
        public ActionResult PoliUmum()
        {
            if (Session["UserLogon"] != null)
            {
                AccountModel account = (AccountModel)Session["UserLogon"];
                bool isHasPrivilege = IsHaveAuthorization("ADD_REGISTRATION", account.Privileges.PrivilegeIDs);
                ViewBag.IsHasAddPrivilege = isHasPrivilege;
            }

            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditRegistration(RegistrationModel model)
        {
            if (Session["UserLogon"] != null)
                model.Account = (AccountModel)Session["UserLogon"];

            var request = new RegistrationRequest
            {
                Data = model,
            };

            RegistrationResponse _response = new RegistrationValidator(_unitOfWork).Validate(request);
            if (_response.Status)
            {
                // Notify to all
                RegistrationHub.BroadcastDataToAllClients();
            }

            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;
            ViewBag.PoliList = BindDropDownPoliList();
            ViewBag.PatientList = BindDropDownPatientList();
            ViewBag.RegistrationTypeList = BindDropDownTypeList();
            ViewBag.DoctorList = BindDropDownDoctorList(2);
            return View();
        }

        [CustomAuthorize("ADD_REGISTRATION", "EDIT_REGISTRATION")]
        public ActionResult CreateOrEditRegistration()
        {
            RegistrationResponse _response = new RegistrationResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new RegistrationRequest
                {
                    Data = new RegistrationModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                RegistrationResponse resp = new RegistrationHandler(_unitOfWork).GetDetail(request);
                RegistrationModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.PoliList = BindDropDownPoliList();
                ViewBag.PatientList = BindDropDownPatientList();
                ViewBag.RegistrationTypeList = BindDropDownTypeList();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                ViewBag.DoctorList = BindDropDownDoctorList(2);
                return View(_model);
            }
            else
            {
                // hardcoded for now
                var model = new RegistrationModel
                {
                    PoliFromID = CURRENT_POLI_ID,
                    PoliFromName = CURRENT_POLI_NAME
                };

                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.Response = _response;
                ViewBag.PoliList = BindDropDownPoliList();
                ViewBag.PatientList = BindDropDownPatientList();
                ViewBag.RegistrationTypeList = BindDropDownTypeList();
                ViewBag.DoctorList = BindDropDownDoctorList(2);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult GetRegistrationData()
        {
            string _draw = Request.Form.Count > 0 ? Request.Form.GetValues("draw").FirstOrDefault() : string.Empty;
            string _start = Request.Form.Count > 0 ? Request.Form.GetValues("start").FirstOrDefault() : string.Empty;
            string _length = Request.Form.Count > 0 ? Request.Form.GetValues("length").FirstOrDefault() : string.Empty;
            string _sortColumn = Request.Form.Count > 0 ? Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault() : string.Empty;
            string _sortColumnDir = Request.Form.Count > 0 ? Request.Form.GetValues("order[0][dir]").FirstOrDefault() : string.Empty;
            string _searchValue = Request.Form.Count > 0 ? Request.Form.GetValues("search[value]").FirstOrDefault() : string.Empty;

            int _pageSize = string.IsNullOrEmpty(_length) ? 0 : Convert.ToInt32(_length);
            int _skip = string.IsNullOrEmpty(_start) ? 0 : Convert.ToInt32(_start);

            var request = new RegistrationRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new RegistrationHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw, Status = response.Status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRegistrationDataUmum()
        {
            string _draw = Request.Form.Count > 0 ? Request.Form.GetValues("draw").FirstOrDefault() : string.Empty;
            string _start = Request.Form.Count > 0 ? Request.Form.GetValues("start").FirstOrDefault() : string.Empty;
            string _length = Request.Form.Count > 0 ? Request.Form.GetValues("length").FirstOrDefault() : string.Empty;
            string _sortColumn = Request.Form.Count > 0 ? Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault() : string.Empty;
            string _sortColumnDir = Request.Form.Count > 0 ? Request.Form.GetValues("order[0][dir]").FirstOrDefault() : string.Empty;
            string _searchValue = Request.Form.Count > 0 ? Request.Form.GetValues("search[value]").FirstOrDefault() : string.Empty;

            int _pageSize = string.IsNullOrEmpty(_length) ? 0 : Convert.ToInt32(_length);
            int _skip = string.IsNullOrEmpty(_start) ? 0 : Convert.ToInt32(_start);

            var request = new RegistrationRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            // get poli umum (2) registration list
            var response = new RegistrationHandler(_unitOfWork).GetListData(request, 2);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw, Status = response.Status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteRegistration(int id)
        {
            var request = new RegistrationRequest
            {
                Data = new RegistrationModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            RegistrationResponse _response = new RegistrationValidator(_unitOfWork).Validate(request);
            if (_response.Status)
            {
                RegistrationHub.BroadcastDataToAllClients();
            }

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProcessRegistration(int id)
        {
            var request = new RegistrationRequest
            {
                Data = new RegistrationModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.Process.ToString()
            };

            RegistrationResponse _response = new RegistrationValidator(_unitOfWork).Validate(request);
            if (_response.Status)
            {
                RegistrationHub.BroadcastDataToAllClients();
            }

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult HoldRegistration(int id)
        {
            var request = new RegistrationRequest
            {
                Data = new RegistrationModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.Hold.ToString()
            };

            RegistrationResponse _response = new RegistrationValidator(_unitOfWork).Validate(request);
            if (_response.Status)
            {
                RegistrationHub.BroadcastDataToAllClients();
            }

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FinishRegistration(int id)
        {
            var request = new RegistrationRequest
            {
                Data = new RegistrationModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.Finish.ToString()
            };

            RegistrationResponse _response = new RegistrationValidator(_unitOfWork).Validate(request);
            if (_response.Status)
            {
                RegistrationHub.BroadcastDataToAllClients();
            }

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public bool IsHaveAuthorization(string privilege_name, List<long> PrivilegeIds)
        {
            bool IsAuthorized = false;
            var privilegeNameList = _unitOfWork.PrivilegeRepository.Get(x => PrivilegeIds.Contains(x.ID));

            IsAuthorized = privilegeNameList.Any(x => x.Privilege_Name == privilege_name);

            return IsAuthorized;
        }
    }
}