using AutoMapper;
using Datalist;
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
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class RegistrationController : BaseController
    {
        private const int CURRENT_POLI_ID = 1;
        private const string CURRENT_POLI_NAME = "Loket";

        public RegistrationController(IUnitOfWork unitOfWork, KlinikDBEntities context) :
            base(unitOfWork, context)
        {
        }

        #region ::DROPDOWN METHODS::
        private List<SelectListItem> BindDropDownPoliList(int poliType = 0)
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

        private List<SelectListItem> BindDropDownPatientList()
        {
            var qry = _unitOfWork.PatientRepository.Get();
            IList<PatientModel> _patientModelList = new List<PatientModel>();
            foreach (var item in qry)
            {
                var _poli = Mapper.Map<Patient, PatientModel>(item);
                _patientModelList.Add(_poli);
            }

            RegistrationResponse response = GetTodayRegistrationList();

            List<SelectListItem> _patientList = new List<SelectListItem>();
            foreach (var item in _patientModelList)
            {
                // validate if patient already registered and the status is New
                if (!response.Data.Any(x => x.PatientID == item.Id && x.Status == 0))
                {
                    _patientList.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
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
            List<PoliSchedule> scheduleList = new List<PoliSchedule>();
            var clinicID = GetClinicID();
            if (clinicID <= 0)
                scheduleList = _unitOfWork.PoliScheduleRepository.Get(x => x.PoliID == poliID);
            else
                scheduleList = _unitOfWork.PoliScheduleRepository.Get(x => x.PoliID == poliID && x.ClinicID == clinicID);

            foreach (var item in scheduleList)
            {
                var doctor = _unitOfWork.DoctorRepository.GetFirstOrDefault(x => x.ID == item.DoctorID);
                if (doctor != null)
                {
                    if (!_typeList.Any(x => x.Value == doctor.ID.ToString()))
                    {
                        _typeList.Add(new SelectListItem
                        {
                            Text = doctor.Name,
                            Value = doctor.ID.ToString()
                        });
                    }
                }
            }

            return _typeList;
        }

        public JsonResult GetDoctorList(int poliID)
        {
            List<Doctor> doctorList = new List<Doctor>();
            List<PoliSchedule> scheduleList = new List<PoliSchedule>();
            var _clinicId = GetClinicID();
            if (_clinicId < 0)
                scheduleList = _unitOfWork.PoliScheduleRepository.Get(x => x.PoliID == poliID);
            else
                scheduleList = _unitOfWork.PoliScheduleRepository.Get(x => x.PoliID == poliID && x.ClinicID == _clinicId);

            foreach (var item in scheduleList)
            {
                var doctor = _unitOfWork.DoctorRepository.GetFirstOrDefault(x => x.ID == item.DoctorID);
                if (doctor != null && !doctorList.Any(x => x.ID == doctor.ID))
                {
                    doctorList.Add(new Doctor { ID = doctor.ID, Name = doctor.Name });
                }
            }

            return Json(doctorList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::HTTP METHODS::
        [HttpGet]
        public JsonResult AllPatient(DatalistFilter filter)
        {
            PatientDataList datalist = new PatientDataList(_context, GetClinicID()) { Filter = filter };

            DatalistData patientList = datalist.GetData();

            return Json(patientList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateOrEditRegistration(RegistrationModel model)
        {
            model.Account = Account;

            var request = new RegistrationRequest { Data = model, };

            RegistrationResponse _response = new RegistrationValidator(_unitOfWork).Validate(request);
            if (_response.Status)
            {
                // Notify to all
                RegistrationHub.BroadcastDataToAllClients();
            }

            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;
            var tempPoliList = BindDropDownPoliList(GetPoliType(model.PoliFromID));
            ViewBag.PoliList = tempPoliList;
            ViewBag.PatientList = BindDropDownPatientList();
            ViewBag.RegistrationTypeList = BindDropDownTypeList();
            ViewBag.DoctorList = BindDropDownDoctorList(int.Parse(tempPoliList[0].Value));

            return View("CreateOrEditRegistration", model);
        }

        [CustomAuthorize("EDIT_REGISTRATION")]
        public ActionResult EditRegistration()
        {
            RegistrationResponse _response = new RegistrationResponse();
            var request = new RegistrationRequest
            {
                Data = new RegistrationModel
                {
                    Id = long.Parse(Request.QueryString["id"].ToString())
                }
            };

            RegistrationResponse resp = new RegistrationHandler(_unitOfWork).GetDetail(request);
            RegistrationModel _model = resp.Entity;
            _model.CurrentPoliID = GetUserPoliID();
            ViewBag.Response = _response;
            var tempPoliList = BindDropDownPoliList(GetPoliType(_model.PoliFromID));
            ViewBag.PoliList = tempPoliList;
            ViewBag.PatientList = BindDropDownPatientList();
            ViewBag.RegistrationTypeList = BindDropDownTypeList();
            ViewBag.ActionType = ClinicEnums.Action.Edit;
            ViewBag.DoctorList = BindDropDownDoctorList(int.Parse(tempPoliList[0].Value));

            return View("CreateOrEditRegistration", _model);
        }

        [CustomAuthorize("ADD_REGISTRATION")]
        public ActionResult CreateRegistration(int poliId = 1)
        {
            RegistrationResponse _response = new RegistrationResponse();

            var poliName = Regex.Replace(((PoliEnum)poliId).ToString(), "([A-Z])", " $1").Trim();

            var model = new RegistrationModel
            {
                PoliFromID = poliId,
                CurrentPoliID = poliId,
                PoliFromName = poliName
            };

            ViewBag.ActionType = ClinicEnums.Action.Add;
            ViewBag.Response = _response;
            var tempPoliList = BindDropDownPoliList(GetPoliType(poliId));
            ViewBag.PoliList = tempPoliList;
            ViewBag.PatientList = BindDropDownPatientList();
            ViewBag.RegistrationTypeList = BindDropDownTypeList();
            ViewBag.DoctorList = BindDropDownDoctorList(int.Parse(tempPoliList[0].Value));

            return View("CreateOrEditRegistration", model);
        }

        [HttpPost]
        public ActionResult GetRegistrationDataByPoliID(int poliId)
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

            var response = new RegistrationHandler(_unitOfWork).GetListData(request, poliId);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw, Status = response.Status }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::INDEX FROM MENU::
        [CustomAuthorize("VIEW_REGISTRATION")]
        public ActionResult Index()
        {
            return GenericIndex(PoliEnum.Loket);
        }

        [CustomAuthorize("VIEW_REGISTRATION_UMUM")]
        public ActionResult PoliUmum()
        {
            return GenericIndex(PoliEnum.PoliUmum);
        }

        [CustomAuthorize("VIEW_REGISTRATION_GIGI")]
        public ActionResult PoliGigi()
        {
            return GenericIndex(PoliEnum.PoliGigi);
        }

        [CustomAuthorize("VIEW_REGISTRATION_INTERNIS")]
        public ActionResult PoliInternis()
        {
            return GenericIndex(PoliEnum.PoliPenyakitDalam);
        }

        [CustomAuthorize("VIEW_REGISTRATION_KULIT")]
        public ActionResult PoliKulit()
        {
            return GenericIndex(PoliEnum.PoliKulit);
        }

        [CustomAuthorize("VIEW_REGISTRATION_MATA")]
        public ActionResult PoliMata()
        {
            return GenericIndex(PoliEnum.PoliMata);
        }

        [CustomAuthorize("VIEW_REGISTRATION_THT")]
        public ActionResult PoliTHT()
        {
            return GenericIndex(PoliEnum.PoliTHT);
        }

        [CustomAuthorize("VIEW_REGISTRATION_ANAK")]
        public ActionResult PoliAnak()
        {
            return GenericIndex(PoliEnum.PoliAnak);
        }

        [CustomAuthorize("VIEW_REGISTRATION_SYARAF")]
        public ActionResult PoliSyaraf()
        {
            return GenericIndex(PoliEnum.PoliSyaraf);
        }

        [CustomAuthorize("VIEW_REGISTRATION_RADIOLOGI")]
        public ActionResult Radiologi()
        {
            return GenericIndex(PoliEnum.Radiologi);
        }

        [CustomAuthorize("VIEW_REGISTRATION_LABORATORIUM")]
        public ActionResult Laboratorium()
        {
            return GenericIndex(PoliEnum.Laboratorium);
        }

        [CustomAuthorize("VIEW_REGISTRATION_FARMASI")]
        public ActionResult Farmasi()
        {
            return GenericIndex(PoliEnum.Farmasi);
        }

        [CustomAuthorize("VIEW_REGISTRATION_REKAMMEDIS")]
        public ActionResult RekamMedis()
        {
            return GenericIndex(PoliEnum.RekamMedis);
        }

        [CustomAuthorize("VIEW_REGISTRATION_KASIR")]
        public ActionResult Kasir()
        {
            return GenericIndex(PoliEnum.Kasir);
        }

        private ActionResult GenericIndex(PoliEnum poliEnum)
        {
            if (Session["UserLogon"] != null)
            {
                AccountModel account = (AccountModel)Session["UserLogon"];
                bool isHasPrivilege = IsHaveAuthorization("ADD_REGISTRATION");
                ViewBag.IsHasAddPrivilege = isHasPrivilege;
            }

            var model = new RegistrationModel
            {
                PoliFromID = (int)poliEnum
            };

            return View("Index", model);
        }
        #endregion

        #region ::ACTION MENU::
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
        #endregion

        #region ::DASHBOARD REGISTER::
        [CustomAuthorize("ADD_REGISTRATION", "EDIT_REGISTRATION")]
        public ActionResult RegisterUmum()
        {
            return GenericController(2);
        }

        [CustomAuthorize("ADD_REGISTRATION", "EDIT_REGISTRATION")]
        public ActionResult RegisterGigi()
        {
            return GenericController(3);
        }

        [CustomAuthorize("ADD_REGISTRATION", "EDIT_REGISTRATION")]
        public ActionResult RegisterKulit()
        {
            return GenericController(5);
        }

        [CustomAuthorize("ADD_REGISTRATION", "EDIT_REGISTRATION")]
        public ActionResult RegisterTHT()
        {
            return GenericController(7);
        }

        [CustomAuthorize("ADD_REGISTRATION", "EDIT_REGISTRATION")]
        public ActionResult RegisterFarmasi()
        {
            return GenericController(12);
        }

        [CustomAuthorize("ADD_REGISTRATION", "EDIT_REGISTRATION")]
        public ActionResult RegisterLab()
        {
            return GenericController(11);
        }

        [CustomAuthorize("ADD_REGISTRATION", "EDIT_REGISTRATION")]
        public ActionResult RegisterRadiologi()
        {
            return GenericController(10);
        }
        #endregion

        #region ::PRIVATE METHODS::
        [NonAction]
        private int GetPoliType(int poliId)
        {
            Poli poli = _unitOfWork.PoliRepository.GetFirstOrDefault(x => x.ID == poliId);

            return poli.Type;
        }

        [NonAction]
        private RegistrationResponse GetTodayRegistrationList()
        {
            var request = new RegistrationRequest();

            var response = new RegistrationHandler(_unitOfWork).GetListData(request);

            return response;
        }

        [NonAction]
        private ActionResult GenericController(int poliToID)
        {
            var userPoliID = GetUserPoliID();
            var poliName = Regex.Replace(((PoliEnum)userPoliID).ToString(), "([A-Z])", " $1").Trim();

            var model = new RegistrationModel
            {
                PoliFromID = userPoliID,
                PoliFromName = poliName,
                CurrentPoliID = userPoliID,
                PoliToID = poliToID
            };

            ViewBag.ActionType = ClinicEnums.Action.Add;
            var tempPoliList = BindDropDownPoliList(GetPoliType(model.CurrentPoliID));
            ViewBag.PoliList = tempPoliList;
            ViewBag.PatientList = BindDropDownPatientList();
            ViewBag.RegistrationTypeList = BindDropDownTypeList();
            ViewBag.DoctorList = BindDropDownDoctorList(model.PoliToID);

            return View("CreateOrEditRegistration", model);
        }
        #endregion
    }
}