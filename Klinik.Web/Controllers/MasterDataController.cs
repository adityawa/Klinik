using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
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
        private KlinikDBEntities _context;

        public MasterDataController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region ::MISC::      
        private List<SelectListItem> BindDropDownLabCategory()
        {
            List<LabItemCategory> labItemCatList = _context.LabItemCategories.Where(x => x.RowStatus == 0).ToList();
            List<SelectListItem> _labItemCatList = new List<SelectListItem>();

            foreach (var item in labItemCatList)
            {
                _labItemCatList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _labItemCatList;
        }

        private List<SelectListItem> BindDropDownPoli()
        {
            List<Poli> poliList = _context.Polis.Where(x => x.RowStatus == 0).ToList();
            List<SelectListItem> _poliList = new List<SelectListItem>();

            foreach (var item in poliList)
            {
                _poliList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _poliList;
        }

        

        private List<SelectListItem> BindDropDownService()
        {
            List<Service> serviceList = _context.Services.Where(x => x.RowStatus == 0).ToList();
            List<SelectListItem> _serviceList = new List<SelectListItem>();

            foreach (var item in serviceList)
            {
                _serviceList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _serviceList;
        }

        private List<SelectListItem> BindDropDownProductCategory()
        {
            List<ProductCategory> productCategoryList = _context.ProductCategories.Where(x => x.RowStatus == 0).ToList();
            List<SelectListItem> _productCategoryList = new List<SelectListItem>();

            foreach (var item in productCategoryList)
            {
                _productCategoryList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _productCategoryList;
        }

        private List<SelectListItem> BindDropDownProductUnit()
        {
            List<ProductUnit> productUnitList = _context.ProductUnits.Where(x => x.RowStatus == 0).ToList();
            List<SelectListItem> _productUnitList = new List<SelectListItem>();

            foreach (var item in productUnitList)
            {
                _productUnitList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _productUnitList;
        }

        private List<SelectListItem> BindDropDownProduct()
        {
            List<Product> productList = _context.Products.Where(x => x.RowStatus == 0).ToList();
            List<SelectListItem> _productList = new List<SelectListItem>();

            foreach (var item in productList)
            {
                _productList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _productList;
        }

        private List<SelectListItem> BindDropDownMedicine()
        {
            List<Medicine> medicineList = _context.Medicines.Where(x => x.RowStatus == 0).ToList();
            List<SelectListItem> _medicineList = new List<SelectListItem>();

            foreach (var item in medicineList)
            {
                _medicineList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _medicineList;
        }

        private List<SelectListItem> BindDropDownParentMenuList()
        {
            List<Menu> menuList = _context.Menus.Where(x => x.IsMenu.Value).ToList();
            List<SelectListItem> _menuList = new List<SelectListItem>();

            _menuList.Add(new SelectListItem
            {
                Text = "None",
                Value = "0"
            });

            foreach (var item in menuList)
            {
                _menuList.Add(new SelectListItem
                {
                    Text = item.Description,
                    Value = item.ID.ToString()
                });
            }

            return _menuList;
        }

        private List<SelectListItem> BindDropDownRoleList(int orgId)
        {
            List<OrganizationRole> orgRoleList = _context.OrganizationRoles.Where(x => x.OrgID == orgId).ToList();
            List<SelectListItem> _roleList = new List<SelectListItem>();

            foreach (var item in orgRoleList)
            {
                _roleList.Add(new SelectListItem
                {
                    Text = item.RoleName,
                    Value = item.ID.ToString()
                });
            }

            return _roleList;
        }

        private List<SelectListItem> BindDropDownClinic()
        {
            List<SelectListItem> _clinicLists = new List<SelectListItem>();
            foreach (var item in new ClinicHandler(_unitOfWork).GetAllClinic())
            {
                _clinicLists.Insert(0, new SelectListItem
                {
                    Text = "",
                    Value = "0"
                });
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
            _employeeLists.Insert(0, new SelectListItem
            {
                Text = "",
                Value = "0"
            });
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

        private List<SelectListItem> BindDropDownEmployementType()
        {
            List<SelectListItem> _empTypes = new List<SelectListItem>();

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

        private List<SelectListItem> BindDropDownEmployeeStatus()
        {
            List<SelectListItem> _empStatus = new List<SelectListItem>();
            _empStatus.Insert(0, new SelectListItem
            {
                Text = "",
                Value = "0"
            });
            foreach (var item in new EmployeeStatusHandler(_unitOfWork).GetAllEmployeeStatus())
            {
                _empStatus.Add(new SelectListItem
                {
                    Text = item.Description,
                    Value = item.Id.ToString()
                });
            }

            return _empStatus;
        }

        private List<SelectListItem> BindDropDownCity()
        {
            List<SelectListItem> _cities = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.City.ToString()).ToList())
            {
                _cities.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _cities;
        }

        private List<SelectListItem> BindDropDownDoctorType()
        {
            List<SelectListItem> _doctorTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.DoctorType.ToString()).ToList())
            {
                _doctorTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value
                });
            }

            return _doctorTypes;
        }

        private List<SelectListItem> BindDropDownParamedicType()
        {
            List<SelectListItem> _paramedicTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.ParamedicType.ToString()).ToList())
            {
                _paramedicTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value
                });
            }

            return _paramedicTypes;
        }

        private List<SelectListItem> BindDropDownClinicType()
        {
            List<SelectListItem> _clinicTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.ClinicType.ToString()).ToList())
            {
                _clinicTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
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

        private List<SelectListItem> BindDropDownPoliType()
        {
            List<SelectListItem> _PoliTypes = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.PoliType.ToString()).ToList())
            {
                _PoliTypes.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                });
            }

            return _PoliTypes;
        }

        private List<SelectListItem> PoliType()
        {
            List<SelectListItem> politype = new List<SelectListItem>();
            for (var i = 1; i <= 7; i++)
            {
                politype.Add(new SelectListItem
                {
                    Text = "Type " + i,
                    Value = i.ToString(),
                });
            }

            return politype;
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
                Data = _model,
            };

            OrganizationResponse _response = new OrganizationResponse();

            new OrganizationValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.clinics = BindDropDownClinic();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

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
                    Data = new OrganizationModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                OrganizationResponse resp = new OrganizationHandler(_unitOfWork).GetDetailOrganizationById(request);
                OrganizationModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.clinics = BindDropDownClinic();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.Response = _response;
                ViewBag.clinics = BindDropDownClinic();
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
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new OrganizationHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterOrganisasi(int id)
        {
            OrganizationResponse _response = new OrganizationResponse();
            var request = new OrganizationRequest
            {
                Data = new OrganizationModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
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
                Data = _model
            };

            PrivilegeResponse _response = new PrivilegeResponse();

            new PrivilegeValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Menu = BindDropDownMenu();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

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
                    Data = new PrivilegeModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PrivilegeResponse resp = new PrivilegeHandler(_unitOfWork).GetDetail(request);
                PrivilegeModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Menu = BindDropDownMenu();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Menu = BindDropDownMenu();
                ViewBag.ActionType = ClinicEnums.Action.Add;
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
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PrivilegeHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterPrivilege(int id)
        {
            PrivilegeResponse _response = new PrivilegeResponse();
            var request = new PrivilegeRequest
            {
                Data = new PrivilegeModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
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
                Data = _model
            };

            RoleResponse _response = new RoleResponse();

            new RoleValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Organisasi = BindDropDownOrganization();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

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
                    Data = new RoleModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                RoleResponse resp = new RoleHandler(_unitOfWork).GetDetail(request);
                RoleModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Organisasi = BindDropDownOrganization();
                ViewBag.ActionType = ClinicEnums.Action.Add;
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
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new RoleHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterRole(int id)
        {
            RoleResponse _response = new RoleResponse();
            var request = new RoleRequest
            {
                Data = new RoleModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
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
                Data = _model
            };

            UserResponse _response = new UserResponse();

            new UserValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            var tempOrgList = BindDropDownOrganization();
            ViewBag.Organisasi = tempOrgList;
            ViewBag.Employees = BindDropDownEmployee();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;
            ViewBag.RoleList = BindDropDownRoleList(int.Parse(tempOrgList[0].Value));

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
                    Data = new UserModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString()),

                    }
                };

                UserResponse resp = new UserHandler(_unitOfWork).GetDetail(request);
                UserModel _model = resp.Entity;
                ViewBag.Response = _response;
                var tempOrgList = BindDropDownOrganization();
                ViewBag.Organisasi = tempOrgList;
                ViewBag.Employees = BindDropDownEmployee();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                ViewBag.RoleList = BindDropDownRoleList(int.Parse(tempOrgList[0].Value));
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                var tempOrgList = BindDropDownOrganization();
                ViewBag.Organisasi = tempOrgList;
                ViewBag.Employees = BindDropDownEmployee();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.RoleList = BindDropDownRoleList(int.Parse(tempOrgList[0].Value));
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
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new UserHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterUser(int id)
        {
            UserResponse _response = new UserResponse();
            var request = new UserRequest
            {
                Data = new UserModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new UserValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoleList(int orgId)
        {
            // prevent circular reference
            _context.Configuration.ProxyCreationEnabled = false;

            List<OrganizationRole> orgRoleList = _context.OrganizationRoles.Where(x => x.OrgID == orgId).ToList();

            return Json(orgRoleList, JsonRequestBehavior.AllowGet);
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
                Data = _model
            };

            EmployeeResponse _response = new EmployeeValidator(_unitOfWork, _context).Validate(request);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.EmpTypes = BindDropDownEmployementType();
            ViewBag.EmpStatus = BindDropDownEmployeeStatus();
            ViewBag.EmpActive = BindDropDownEmployeeReff();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

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
                    Data = new EmployeeModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                EmployeeResponse resp = new EmployeeHandler(_unitOfWork).GetDetail(request);
                EmployeeModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.EmpTypes = BindDropDownEmployementType();
                ViewBag.EmplStatus = BindDropDownEmployeeStatus();
                ViewBag.EmpActive = BindDropDownEmployeeReff();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.ActionType = ClinicEnums.Action.Add;
                ViewBag.Response = _response;
                ViewBag.EmpTypes = BindDropDownEmployementType();
                ViewBag.EmplStatus = BindDropDownEmployeeStatus();
                ViewBag.EmpActive = BindDropDownEmployeeReff();
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
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new EmployeeHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterEmployee(int id)
        {
            var request = new EmployeeRequest
            {
                Data = new EmployeeModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            EmployeeResponse _response = new EmployeeValidator(_unitOfWork, _context).Validate(request);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::CLINIC::
        [CustomAuthorize("VIEW_M_CLINIC")]
        public ActionResult ClinicList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditClinic(ClinicModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ClinicRequest
            {
                Data = _model
            };

            ClinicResponse _response = new ClinicResponse();

            new ClinicValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Cities = BindDropDownCity();
            ViewBag.ClinicTypes = BindDropDownClinicType();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_CLINIC", "EDIT_M_CLINIC")]
        public ActionResult CreateOrEditClinic()
        {
            ClinicResponse _response = new ClinicResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ClinicRequest
                {
                    Data = new ClinicModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ClinicResponse resp = new ClinicHandler(_unitOfWork).GetDetail(request);
                ClinicModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Cities = BindDropDownCity();
                ViewBag.ClinicTypes = BindDropDownClinicType();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Cities = BindDropDownCity();
                ViewBag.ClinicTypes = BindDropDownClinicType();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetClinic()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ClinicRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new ClinicModel()
            };
            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new ClinicHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterClinic(int id)
        {
            ClinicResponse _response = new ClinicResponse();
            var request = new ClinicRequest
            {
                Data = new ClinicModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ClinicValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::POLI::
        [CustomAuthorize("VIEW_M_POLI")]
        public ActionResult PoliList()
        {
            PoliModel clinicModel = new PoliModel();
            clinicModel.Account = (AccountModel)Session["UserLogon"];
            ViewBag.ClinicID = clinicModel.Account.ClinicID;
            return View();
        }

        [HttpPost]
        public ActionResult GetPoliData(int? clinicid)
        {
            var requests = Request.Form;
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PoliRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                ClinicID = clinicid
            };

            var response = new PoliHandler(_unitOfWork).GetListData(request);

            if (clinicid > 0)
            {
                response = new PoliHandler(_unitOfWork).GetListDataFilter(request);
            }
            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_M_POLI", "EDIT_M_POLI")]
        public ActionResult CreateOrEditPoli()
        {
            PoliResponse _response = new PoliResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PoliRequest
                {
                    Data = new PoliModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PoliResponse resp = new PoliHandler(_unitOfWork).GetDetail(request);
                PoliModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.Type = new SelectList(PoliType(), "Value", "Text").ToList();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.Type = new SelectList(PoliType(), "Value", "Text").ToList();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateOrEditPoli(PoliModel _model)
        {

            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new PoliRequest
            {
                Data = _model
            };

            PoliResponse _response = new PoliResponse();

            new PoliValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Type = new SelectList(PoliType(), "Value", "Text", _model.Type).ToList();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [HttpPost]
        public JsonResult DeleteMasterPoli(int id)
        {
            PoliResponse _response = new PoliResponse();
            var request = new PoliRequest
            {
                Data = new PoliModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new PoliValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getClinicData()
        {
            return Json(new { data = BindDropDownClinic() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::MENU::
        [CustomAuthorize("VIEW_M_MENU")]
        public ActionResult MenuList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditMenu(MenuModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new MenuRequest
            {
                Data = _model
            };

            MenuResponse _response = new MenuResponse();

            new MenuValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ParentMenuList = BindDropDownParentMenuList();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_MENU", "EDIT_M_MENU")]
        public ActionResult CreateOrEditMenu()
        {
            MenuResponse _response = new MenuResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new MenuRequest
                {
                    Data = new MenuModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                MenuResponse resp = new MenuHandler(_unitOfWork).GetDetail(request);
                MenuModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.ParentMenuList = BindDropDownParentMenuList();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ParentMenuList = BindDropDownParentMenuList();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetMenuData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new MenuRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new MenuHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterMenu(int id)
        {
            MenuResponse _response = new MenuResponse();
            var request = new MenuRequest
            {
                Data = new MenuModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new MenuValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::MEDICINE::
        [CustomAuthorize("VIEW_M_MEDICINE")]
        public ActionResult MedicineList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditMedicine(MedicineModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new MedicineRequest
            {
                Data = _model
            };

            MedicineResponse _response = new MedicineResponse();

            new MedicineValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_MEDICINE", "EDIT_M_MEDICINE")]
        public ActionResult CreateOrEditMedicine()
        {
            MedicineResponse _response = new MedicineResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new MedicineRequest
                {
                    Data = new MedicineModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                MedicineResponse resp = new MedicineHandler(_unitOfWork).GetDetail(request);
                MedicineModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetMedicineData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new MedicineRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new MedicineHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterMedicine(int id)
        {
            MedicineResponse _response = new MedicineResponse();
            var request = new MedicineRequest
            {
                Data = new MedicineModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new MedicineValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::LAB ITEM::
        [CustomAuthorize("VIEW_M_LAB_ITEM")]
        public ActionResult LabItemList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditLabItem(LabItemModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new LabItemRequest
            {
                Data = _model
            };

            LabItemResponse _response = new LabItemResponse();

            new LabItemValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.LabItemCategoryList = BindDropDownLabCategory();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_LAB_ITEM", "EDIT_M_LAB_ITEM")]
        public ActionResult CreateOrEditLabItem()
        {
            LabItemResponse _response = new LabItemResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new LabItemRequest
                {
                    Data = new LabItemModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                LabItemResponse resp = new LabItemHandler(_unitOfWork).GetDetail(request);
                LabItemModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.LabItemCategoryList = BindDropDownLabCategory();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.LabItemCategoryList = BindDropDownLabCategory();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetLabItemData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new LabItemRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new LabItemHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterLabItem(int id)
        {
            LabItemResponse _response = new LabItemResponse();
            var request = new LabItemRequest
            {
                Data = new LabItemModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new LabItemValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::LAB ITEM CATEGORY::
        [CustomAuthorize("VIEW_M_LAB_ITEM_CATEGORY")]
        public ActionResult LabItemCategoryList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditLabItemCategory(LabItemCategoryModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new LabItemCategoryRequest
            {
                Data = _model
            };

            LabItemCategoryResponse _response = new LabItemCategoryResponse();

            new LabItemCategoryValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.PoliList = BindDropDownPoli();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_LAB_ITEM_CATEGORY", "EDIT_M_LAB_ITEM_CATEGORY")]
        public ActionResult CreateOrEditLabItemCategory()
        {
            LabItemCategoryResponse _response = new LabItemCategoryResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new LabItemCategoryRequest
                {
                    Data = new LabItemCategoryModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                LabItemCategoryResponse resp = new LabItemCategoryHandler(_unitOfWork).GetDetail(request);
                LabItemCategoryModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.PoliList = BindDropDownPoli();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.PoliList = BindDropDownPoli();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetLabItemCategoryData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new LabItemCategoryRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new LabItemCategoryHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterLabItemCategory(int id)
        {
            LabItemCategoryResponse _response = new LabItemCategoryResponse();
            var request = new LabItemCategoryRequest
            {
                Data = new LabItemCategoryModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new LabItemCategoryValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRODUCT::
        [CustomAuthorize("VIEW_M_PRODUCT")]
        public ActionResult ProductList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditProduct(ProductModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductRequest
            {
                Data = _model
            };

            ProductResponse _response = new ProductResponse();

            new ProductValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ProductCategoryList = BindDropDownProductCategory();
            ViewBag.ProductUnitList = BindDropDownProductUnit();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRODUCT", "EDIT_M_PRODUCT")]
        public ActionResult CreateOrEditProduct()
        {
            ProductResponse _response = new ProductResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ProductRequest
                {
                    Data = new ProductModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ProductResponse resp = new ProductHandler(_unitOfWork).GetDetail(request);
                ProductModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.ProductCategoryList = BindDropDownProductCategory();
                ViewBag.ProductUnitList = BindDropDownProductUnit();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ProductCategoryList = BindDropDownProductCategory();
                ViewBag.ProductUnitList = BindDropDownProductUnit();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetProductData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ProductHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterProduct(int id)
        {
            ProductResponse _response = new ProductResponse();
            var request = new ProductRequest
            {
                Data = new ProductModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ProductValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRODUCT CATEGORY::
        [CustomAuthorize("VIEW_M_PRODUCT_CATEGORY")]
        public ActionResult ProductCategoryList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditProductCategory(ProductCategoryModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductCategoryRequest
            {
                Data = _model
            };

            ProductCategoryResponse _response = new ProductCategoryResponse();

            new ProductCategoryValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRODUCT_CATEGORY", "EDIT_M_PRODUCT_CATEGORY")]
        public ActionResult CreateOrEditProductCategory()
        {
            ProductCategoryResponse _response = new ProductCategoryResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ProductCategoryRequest
                {
                    Data = new ProductCategoryModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ProductCategoryResponse resp = new ProductCategoryHandler(_unitOfWork).GetDetail(request);
                ProductCategoryModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetProductCategoryData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductCategoryRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ProductCategoryHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterProductCategory(int id)
        {
            ProductCategoryResponse _response = new ProductCategoryResponse();
            var request = new ProductCategoryRequest
            {
                Data = new ProductCategoryModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ProductCategoryValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRODUCT MEDICINE::
        [CustomAuthorize("VIEW_M_PRODUCT_MEDICINE")]
        public ActionResult ProductMedicineList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditProductMedicine(ProductMedicineModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductMedicineRequest
            {
                Data = _model
            };

            ProductMedicineResponse _response = new ProductMedicineResponse();

            new ProductMedicineValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ProductList = BindDropDownProduct();
            ViewBag.MedicineList = BindDropDownMedicine();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRODUCT_MEDICINE", "EDIT_M_PRODUCT_MEDICINE")]
        public ActionResult CreateOrEditProductMedicine()
        {
            ProductMedicineResponse _response = new ProductMedicineResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ProductMedicineRequest
                {
                    Data = new ProductMedicineModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ProductMedicineResponse resp = new ProductMedicineHandler(_unitOfWork).GetDetail(request);
                ProductMedicineModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.ProductList = BindDropDownProduct();
                ViewBag.MedicineList = BindDropDownMedicine();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ProductList = BindDropDownProduct();
                ViewBag.MedicineList = BindDropDownMedicine();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetProductMedicineData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductMedicineRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ProductMedicineHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterProductMedicine(int id)
        {
            ProductMedicineResponse _response = new ProductMedicineResponse();
            var request = new ProductMedicineRequest
            {
                Data = new ProductMedicineModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ProductMedicineValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRODUCT UNIT::
        [CustomAuthorize("VIEW_M_PRODUCT_UNIT")]
        public ActionResult ProductUnitList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditProductUnit(ProductUnitModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ProductUnitRequest
            {
                Data = _model
            };

            ProductUnitResponse _response = new ProductUnitResponse();

            new ProductUnitValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_PRODUCT_UNIT", "EDIT_M_PRODUCT_UNIT")]
        public ActionResult CreateOrEditProductUnit()
        {
            ProductUnitResponse _response = new ProductUnitResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ProductUnitRequest
                {
                    Data = new ProductUnitModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ProductUnitResponse resp = new ProductUnitHandler(_unitOfWork).GetDetail(request);
                ProductUnitModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetProductUnitData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ProductUnitRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ProductUnitHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterProductUnit(int id)
        {
            ProductUnitResponse _response = new ProductUnitResponse();
            var request = new ProductUnitRequest
            {
                Data = new ProductUnitModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ProductUnitValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::SERVICE::
        [CustomAuthorize("VIEW_M_SERVICE")]
        public ActionResult ServiceList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditService(ServiceModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new ServiceRequest
            {
                Data = _model
            };

            ServiceResponse _response = new ServiceResponse();

            new ServiceValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_SERVICE", "EDIT_M_SERVICE")]
        public ActionResult CreateOrEditService()
        {
            ServiceResponse _response = new ServiceResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new ServiceRequest
                {
                    Data = new ServiceModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                ServiceResponse resp = new ServiceHandler(_unitOfWork).GetDetail(request);
                ServiceModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetServiceData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new ServiceRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new ServiceHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterService(int id)
        {
            ServiceResponse _response = new ServiceResponse();
            var request = new ServiceRequest
            {
                Data = new ServiceModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new ServiceValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::POLI SERVICE::
        [CustomAuthorize("VIEW_M_POLI_SERVICE")]
        public ActionResult PoliServiceList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditPoliService(PoliServiceModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new PoliServiceRequest
            {
                Data = _model
            };

            PoliServiceResponse _response = new PoliServiceResponse();

            new PoliServiceValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.PoliList = BindDropDownPoli();
            ViewBag.ClinicList = BindDropDownClinic();
            ViewBag.ServiceList = BindDropDownService();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;

            return View();
        }

        [CustomAuthorize("ADD_M_POLI_SERVICE", "EDIT_M_POLI_SERVICE")]
        public ActionResult CreateOrEditPoliService()
        {
            PoliServiceResponse _response = new PoliServiceResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PoliServiceRequest
                {
                    Data = new PoliServiceModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                PoliServiceResponse resp = new PoliServiceHandler(_unitOfWork).GetDetail(request);
                PoliServiceModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.PoliList = BindDropDownPoli();
                ViewBag.ClinicList = BindDropDownClinic();
                ViewBag.ServiceList = BindDropDownService();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.PoliList = BindDropDownPoli();
                ViewBag.ClinicList = BindDropDownClinic();
                ViewBag.ServiceList = BindDropDownService();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetPoliServiceData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new PoliServiceRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new PoliServiceHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMasterPoliService(int id)
        {
            PoliServiceResponse _response = new PoliServiceResponse();
            var request = new PoliServiceRequest
            {
                Data = new PoliServiceModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new PoliServiceValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::GUDANG::
        [CustomAuthorize("VIEW_M_GUDANG")]
        public ActionResult GudangList()
        {
            return View();
        }

        [CustomAuthorize("VIEW_M_GUDANG")]
        [HttpPost]
        public ActionResult GetGudangData()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new GudangRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new GudangHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("ADD_M_GUDANG", "EDIT_M_GUDANG")]
        public ActionResult CreateOrEditGudang()
        {
            GudangResponse _response = new GudangResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new GudangRequest
                {
                    Data = new GudangModel
                    {
                        Id = long.Parse(Request.QueryString["id"].ToString())
                    }
                };

                GudangResponse resp = new GudangHandler(_unitOfWork).GetDetail(request);
                GudangModel _model = resp.Entity;
                ViewBag.Response = _response;
                ViewBag.clinics = BindDropDownClinic();
                ViewBag.ActionType = ClinicEnums.Action.Edit;
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
                ViewBag.clinics = BindDropDownClinic();
                ViewBag.ActionType = ClinicEnums.Action.Add;
                return View();
            }
        }

        [CustomAuthorize("ADD_M_GUDANG", "EDIT_M_GUDANG")]
        [HttpPost]
        public ActionResult CreateOrEditGudang(GudangModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];

            var request = new GudangRequest
            {
                Data = _model
            };

            GudangResponse _response = new GudangResponse();

            new GudangValidator(_unitOfWork).Validate(request, out _response);

            return RedirectToAction("GudangList");
        }

        [HttpPost]
        public JsonResult DeleteMasterGudang(int id)
        {
            GudangResponse _response = new GudangResponse();
            var request = new GudangRequest
            {
                Data = new GudangModel
                {
                    Id = id,
                    Account = Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"]
                },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            new GudangValidator(_unitOfWork).Validate(request, out _response);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}