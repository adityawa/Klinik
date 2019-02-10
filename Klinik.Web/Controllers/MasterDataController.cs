using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.Web.Features.MasterData.Organization;
using Klinik.Web.Features.MasterData.Clinic;
using Klinik.Web.DataAccess;
using Klinik.Web.Models.MasterData;
using Newtonsoft.Json;
using Klinik.Web.Features.MasterData.Privileges;

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
        #endregion

        // GET: MasterData
        public ActionResult Index()
        {
            return View();
        }

        #region ::ORGANIZATION::
        public ActionResult OrganizationList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditOrganization(OrganizationModel _model)
        {
            var request = new OrganizationRequest
            {
                RequestOrganizationData = _model
            };

            OrganizationResponse _response = new OrganizationResponse();

            new OrganizationValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";

            ViewBag.clinics = BindDropDownKlinik();
            return View();
        }

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
            //long _id = 0;
            //if (Request.QueryString["id"] != null)
            //    _id = Convert.ToInt64(Request.QueryString["id"].ToString());
            OrganizationResponse _response = new OrganizationResponse();
            var request = new OrganizationRequest
            {
                RequestOrganizationData = new OrganizationModel
                {
                    Id = id
                }
            };

            _response = new OrganizationHandler(_unitOfWork).RemoveOrganization(request);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::PRIVILEGE::
        public ActionResult PrivilegeList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrEditPrivilege(PrivilegeModel _model)
        {
            var request = new PrivilegeRequest
            {
                RequestPrivilegeData = _model
            };

            PrivilegeResponse _response = new PrivilegeResponse();

            new PrivilegeValidator(_unitOfWork).Validate(request, out _response);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            return View();
        }

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
              
                return View(_model);
            }
            else
            {
                ViewBag.Response = _response;
               
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
                    Id = id
                }
            };

            _response = new PrivilegeHandler(_unitOfWork).RemoveData(request);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}