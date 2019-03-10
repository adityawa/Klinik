using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class MasterDataController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public MasterDataController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region ::MISC::
        private List<SelectListItem> BindDropDownKlinik()
        {
            List<SelectListItem> _clinicLists = new List<SelectListItem>();
            foreach (var item in new ClinicHandler(_unitOfWork).GetAllClinic())
            {
                _clinicLists.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _clinicLists;
        }

        private List<SelectListItem> BindDropDownOrganization()
        {
            List<SelectListItem> _organizationLists = new List<SelectListItem>();
            foreach (var item in new OrganizationHandler(_unitOfWork).GetOrganizationList())
            {
                _organizationLists.Add(new SelectListItem
                {
                    Text = item.OrgName,
                    Value = item.Id.ToString()
                });
            }

            return _organizationLists;
        }

        private List<SelectListItem> BindDropDownEmployee()
        {
            List<SelectListItem> _employeeLists = new List<SelectListItem>();
            foreach (var item in new EmployeeHandler(_unitOfWork).GetAllEmployee())
            {
                _employeeLists.Add(new SelectListItem
                {
                    Text = item.EmpName,
                    Value = item.Id.ToString()
                });
            }

            return _employeeLists;
        }

        private List<SelectListItem> BindDropDownEmployementType()
        {
            List<SelectListItem> _empTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.enumMasterTypes.EmploymentType.ToString()).ToList())
            {
                _empTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _empTypes;
        }

        private List<SelectListItem> BindDropDownDepartment()
        {
            List<SelectListItem> _departments = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.enumMasterTypes.Department.ToString()).ToList())
            {
                _departments.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _departments;
        }

        private List<SelectListItem> BindDropDownCity()
        {
            List<SelectListItem> _cities = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.enumMasterTypes.City.ToString()).ToList())
            {
                _cities.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _cities;
        }

        private List<SelectListItem> BindDropDownClinicType()
        {
            List<SelectListItem> _clinicTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.enumMasterTypes.ClinicType.ToString()).ToList())
            {
                _clinicTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _clinicTypes;
        }

        private List<SelectListItem> BindDropDownMenu()
        {
            List<SelectListItem> _menus = new List<SelectListItem>();
            _menus.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = ""
            });
            foreach (var item in new MenuHandler(_unitOfWork).GetVisibleMenu().ToList())
            {
                _menus.Add(new SelectListItem
                {
                    Text = item.Description,
                    Value = item.Id.ToString()
                });
            }

            return _menus;
        }
        #endregion

        // GET: MasterData
        public ActionResult Index()
        {
            return View();
        }

        #region ::ORGANIZATION::
        [CustomAuthorize("VIEW_M_ORG")]
        public ActionResult OrganizationList() => View();

        [HttpPost]
        public ActionResult CreateOrEditOrganization(OrganizationModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new OrganizationRequest
            {
                RequestOrganizationData = _model,

            };

            OrganizationResponse _response = new OrganizationResponse();

            new OrganizationValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";

            ViewBag.clinics = BindDropDownKlinik();
            return View();
        }

        [CustomAuthorize("ADD_M_ORG", "EDIT_M_ORG")]
        public ActionResult CreateOrEditOrganization()
        {
            OrganizationResponse _response = new OrganizationResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new OrganizationRequest
                {
                    RequestOrganizationData = new OrganizationModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                OrganizationResponse resp = new OrganizationHandler(_unitOfWork).GetDetailOrganizationById(request);
                OrganizationModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.clinics = BindDropDownKlinik();
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.clinics = BindDropDownKlinik();
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetOrganizationData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new OrganizationRequest
            {
                draw = _draw,
                searchValue = _searchValue,
                sortColumn = _sortColumn,
                sortColumnDir = _sortColumnDir,
                pageSize = _pageSize,
                skip = _skip
            };

            var response = new OrganizationHandler(_unitOfWork).GetOrganizationData(request);

            return Json(new { data = response.Data, recordsFiltered = response.recordsFiltered, recordsTotal = response.recordsTotal, draw = response.draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterOrganisasi(int id)
        {
            OrganizationResponse _response = new OrganizationResponse();
            var request = new OrganizationRequest
            {
                RequestOrganizationData = new OrganizationModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                action = ClinicEnums.enumAction.DELETE.ToString()
            };

            new OrganizationValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRIVILEGE::
        [CustomAuthorize("VIEW_M_PRIVILEGE")]
        public ActionResult PrivilegeList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditPrivilege(PrivilegeModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new PrivilegeRequest
            {
                RequestPrivilegeData = _model
            };

            PrivilegeResponse _response = new PrivilegeResponse();

            new PrivilegeValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Menu = BindDropDownMenu();
            return View();
        }

        [CustomAuthorize("ADD_M_PRIVILEGE", "EDIT_M_PRIVILEGE")]
        public ActionResult CreateOrEditPrivilege()
        {
            PrivilegeResponse _response = new PrivilegeResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PrivilegeRequest
                {
                    RequestPrivilegeData = new PrivilegeModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PrivilegeResponse resp = new PrivilegeHandler(_unitOfWork).GetDetail(request);
                PrivilegeModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Menu = BindDropDownMenu();
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Menu = BindDropDownMenu();
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetPrivilegeData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PrivilegeRequest
            {
                draw = _draw,
                searchValue = _searchValue,
                sortColumn = _sortColumn,
                sortColumnDir = _sortColumnDir,
                pageSize = _pageSize,
                skip = _skip
            };

            var response = new PrivilegeHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.recordsFiltered, recordsTotal = response.recordsTotal, draw = response.draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterPrivilege(int id)
        {
            PrivilegeResponse _response = new PrivilegeResponse();
            var request = new PrivilegeRequest
            {
                RequestPrivilegeData = new PrivilegeModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                action = ClinicEnums.enumAction.DELETE.ToString()
            };

            new PrivilegeValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::ROLE::
        [CustomAuthorize("VIEW_M_ROLE")]
        public ActionResult RoleList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditRole(RoleModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new RoleRequest
            {
                RequestRoleData = _model
            };

            RoleResponse _response = new RoleResponse();

            new RoleValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Organisasi = BindDropDownOrganization();
            return View();
        }

        [CustomAuthorize("ADD_M_ROLE", "EDIT_M_ROLE")]
        public ActionResult CreateOrEditRole()
        {
            RoleResponse _response = new RoleResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new RoleRequest
                {
                    RequestRoleData = new RoleModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                RoleResponse resp = new RoleHandler(_unitOfWork).GetDetail(request);
                RoleModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetRoleData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new RoleRequest
            {
                draw = _draw,
                searchValue = _searchValue,
                sortColumn = _sortColumn,
                sortColumnDir = _sortColumnDir,
                pageSize = _pageSize,
                skip = _skip
            };

            var response = new RoleHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.recordsFiltered, recordsTotal = response.recordsTotal, draw = response.draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterRole(int id)
        {
            RoleResponse _response = new RoleResponse();
            var request = new RoleRequest
            {
                RequestRoleData = new RoleModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                action = ClinicEnums.enumAction.DELETE.ToString()
            };

            new RoleValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::USER::
        [CustomAuthorize("VIEW_M_USER")]
        public ActionResult UserList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditUser(UserModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new UserRequest
            {
                RequestUserData = _model
            };

            UserResponse _response = new UserResponse();

            new UserValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Organisasi = BindDropDownOrganization();
            ViewBag.Employees = BindDropDownEmployee();
            return View();
        }

        [CustomAuthorize("ADD_M_USER", "EDIT_M_USER")]
        public ActionResult CreateOrEditUser()
        {
            UserResponse _response = new UserResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new UserRequest
                {
                    RequestUserData = new UserModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString()),

                    }
                };

                UserResponse resp = new UserHandler(_unitOfWork).GetDetail(request);
                UserModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.Employees = BindDropDownEmployee();
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.Employees = BindDropDownEmployee();
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetUserData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new UserRequest
            {
                draw = _draw,
                searchValue = _searchValue,
                sortColumn = _sortColumn,
                sortColumnDir = _sortColumnDir,
                pageSize = _pageSize,
                skip = _skip
            };

            var response = new UserHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.recordsFiltered, recordsTotal = response.recordsTotal, draw = response.draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterUser(int id)
        {
            UserResponse _response = new UserResponse();
            var request = new UserRequest
            {
                RequestUserData = new UserModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                action = ClinicEnums.enumAction.DELETE.ToString()
            };

            new UserValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::EMPLOYEE::
        [CustomAuthorize("VIEW_M_EMPLOYEE")]
        public ActionResult EmployeeList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditEmployee(EmployeeModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new EmployeeRequest
            {
                RequestEmployeeData = _model
            };

            EmployeeResponse _response = new EmployeeResponse();

            new EmployeeValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.EmpTypes = BindDropDownEmployementType();
            ViewBag.Departments = BindDropDownDepartment();
            return View();
        }

        [CustomAuthorize("ADD_M_EMPLOYEE", "EDIT_M_EMPLOYEE")]
        public ActionResult CreateOrEditEmployee()
        {
            EmployeeResponse _response = new EmployeeResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new EmployeeRequest
                {
                    RequestEmployeeData = new EmployeeModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                EmployeeResponse resp = new EmployeeHandler(_unitOfWork).GetDetail(request);
                EmployeeModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.EmpTypes = BindDropDownEmployementType();
                ViewBag.Departments = BindDropDownDepartment();
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.EmpTypes = BindDropDownEmployementType();
                ViewBag.Departments = BindDropDownDepartment();
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetEmployee()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new EmployeeRequest
            {
                draw = _draw,
                searchValue = _searchValue,
                sortColumn = _sortColumn,
                sortColumnDir = _sortColumnDir,
                pageSize = _pageSize,
                skip = _skip
            };

            var response = new EmployeeHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.recordsFiltered, recordsTotal = response.recordsTotal, draw = response.draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterEmployee(int id)
        {
            EmployeeResponse _response = new EmployeeResponse();
            var request = new EmployeeRequest
            {
                RequestEmployeeData = new EmployeeModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                action = ClinicEnums.enumAction.DELETE.ToString()
            };

            new EmployeeValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::CLINIC::

        #endregion
    }
}