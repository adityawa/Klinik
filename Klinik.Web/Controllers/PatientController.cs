using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Features;
using Klinik.Features.MasterData.City;
using Klinik.Features.MasterData.FamilyRelationship;
using Klinik.Features.Patients.Pasien;
using Klinik.Resources;

namespace Klinik.Web.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public PatientController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        private List<SelectListItem> BindDropDownCity()
        {
            List<SelectListItem> _cities = new List<SelectListItem>();
            foreach (var item in new CityHandler(_unitOfWork).GetAllCity().ToList())
            {
                _cities.Add(new SelectListItem
                {
                    Text = item.City,
                    Value = item.Id.ToString()
                });
            }

            return _cities;
        }

        private List<SelectListItem> BindDropDownPatientType()
        {
            List<SelectListItem> _types = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(Constants.MasterType.PATIENT_TYPE.ToString()).ToList())
            {
                _types.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value.ToString()
                });
            }

            return _types;
        }

        private List<SelectListItem> BindDropDownMaritalStatus()
        {
            List<SelectListItem> _types = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(Constants.MasterType.MARITAL_STATUS.ToString()).ToList())
            {
                _types.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value.ToString()
                });
            }

            return _types;
        }

        private List<SelectListItem> BindDropDownRelation()
        {
            List<SelectListItem> _empTypes = new List<SelectListItem>();
            _empTypes.Insert(0, new SelectListItem
            {
                Text = "",
                Value = "0"
            });
            foreach (var item in new FamilyStatusHandler(_unitOfWork).GetAllFamilyStatus().ToList())
            {
                _empTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _empTypes;
        }

        private List<SelectListItem> BindDropDownReffRelation()
        {
            List<SelectListItem> _types = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(Constants.MasterType.RELATION.ToString()).ToList())
            {
                _types.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value.ToString()
                });
            }

            return _types;
        }

        private List<SelectListItem> BindDropDownEmployeeReff()
        {
            List<SelectListItem> _employeeActiveLists = new List<SelectListItem>();
            _employeeActiveLists.Insert(0, new SelectListItem
            {
                Text = "",
                Value = "0"
            });
            foreach (var item in new EmployeeHandler(_unitOfWork).GetActiveEmployee())
            {
                _employeeActiveLists.Add(new SelectListItem
                {
                    Text = $"{item.EmpID} - {item.EmpName}",
                    Value = item.Id.ToString()
                });
            }

            return _employeeActiveLists;
        }

        [CustomAuthorize("VIEW_M_PATIENT")]
        public ActionResult PasienList()
        {
            return View();
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

            var request = new PatientRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new PatientModel()
            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new PatientHandler(_unitOfWork, _context).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UseExistingPatientData(string isUseExisting)
        {
            var _model = new PatientModel { };
            var _response = new PatientResponse { };

            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            if (Session["PatientModel"] != null)
            {
                _model = (PatientModel)Session["PatientModel"];
                _model.IsUseExistingData = isUseExisting == "yes" ? true : false;
                var request = new PatientRequest
                {
                    Data = _model
                };
                _response = new PatientValidator(_unitOfWork, _context).Validate(request);
                ViewBag.Response = $"{_response.Status};{_response.Message}";

                ViewBag.Relation = BindDropDownRelation();
                ViewBag.PatientType = BindDropDownPatientType();
                ViewBag.City = BindDropDownCity();
                ViewBag.EmpReff = BindDropDownEmployeeReff();
                ViewBag.Marital = BindDropDownMaritalStatus();
                ViewBag.ReffRelation = BindDropDownReffRelation();
                ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;
            }
            else
            {
                _response.Status = false;
                _response.Message = Messages.SessionExpired;
            }

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateOrEditPatient(PatientModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            if (_model == null)
            {
                if (Session["PatientModel"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    _model = (PatientModel)Session["PatientModel"];
                }
            }
            var request = new PatientRequest
            {
                Data = _model
            };

            PatientResponse _response = new PatientValidator(_unitOfWork, _context).Validate(request);
            if (_response.IsNeedConfirmation)
            {
                Session["PatientModel"] = _model;
            }
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Confirmation = $"{_response.IsNeedConfirmation};{_response.Message}";
            ViewBag.Relation = BindDropDownRelation();
            ViewBag.PatientType = BindDropDownPatientType();
            ViewBag.City = BindDropDownCity();
            ViewBag.EmpReff = BindDropDownEmployeeReff();
            ViewBag.Marital = BindDropDownMaritalStatus();
            ViewBag.ReffRelation = BindDropDownReffRelation();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            if (_response.Status && _model.IsFromRegistration)
                return RedirectToAction("CreateRegistrationForNewPatient", "Registration", new { patientID = _response.Entity.Id });
            else
                return View("CreateOrEditPatient", _model);
        }

        [CustomAuthorize("ADD_M_PATIENT", "EDIT_M_PATIENT")]
        public ActionResult CreateOrEditPatient()
        {
            PatientResponse _response = new PatientResponse();
            if (Request.QueryString["id"] != null)
            {

                var request = new PatientRequest
                {
                    Data = new PatientModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString()),

                    }
                };
                if (Session["UserLogon"] != null)
                    request.Data.Account = (AccountModel)Session["UserLogon"];

                PatientResponse resp = new PatientHandler(_unitOfWork, _context).GetDetail(request);

                PatientModel _model = resp.Entity;

                ViewBag.Response = _response;
                ViewBag.Relation = BindDropDownRelation();
                ViewBag.PatientType = BindDropDownPatientType();
                ViewBag.City = BindDropDownCity();
                ViewBag.EmpReff = BindDropDownEmployeeReff();
                ViewBag.Marital = BindDropDownMaritalStatus();
                ViewBag.ReffRelation = BindDropDownReffRelation();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.Response = _response;
                ViewBag.Relation = BindDropDownRelation();
                ViewBag.PatientType = BindDropDownPatientType();
                ViewBag.EmpReff = BindDropDownEmployeeReff();
                ViewBag.Marital = BindDropDownMaritalStatus();
                ViewBag.City = BindDropDownCity();
                ViewBag.ReffRelation = BindDropDownReffRelation();
                return View("CreateOrEditPatient", new PatientModel());
            }
        }

        [CustomAuthorize("ADD_M_PATIENT", "EDIT_M_PATIENT")]
        public ActionResult CreateFromRegistration()
        {
            ViewBag.ActionType = ClinicEnums.Action.Add;
            ViewBag.Response = new PatientResponse();
            ViewBag.Relation = BindDropDownRelation();
            ViewBag.PatientType = BindDropDownPatientType();
            ViewBag.EmpReff = BindDropDownEmployeeReff();
            ViewBag.Marital = BindDropDownMaritalStatus();
            ViewBag.City = BindDropDownCity();
            ViewBag.ReffRelation = BindDropDownReffRelation();

            PatientModel model = new PatientModel { IsFromRegistration = true };

            return View("CreateOrEditPatient", model);
        }

        [HttpPost]
        public JsonResult DeletePatient(int id)
        {
            var request = new PatientRequest
            {
                Data = new PatientModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            PatientResponse _response = new PatientValidator(_unitOfWork, _context).Validate(request);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}