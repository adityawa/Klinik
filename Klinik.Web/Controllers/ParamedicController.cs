using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Features;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class ParamedicController : BaseController
    {
        public ParamedicController(IUnitOfWork unitOfWork, KlinikDBEntities context) :
            base(unitOfWork, context)
        {
        }

        [CustomAuthorize("VIEW_M_PARAMEDIC")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DoctorModel model)
        {
            model.Account = Account;

            // set as paramedic
            model.TypeID = 1;

            var request = new DoctorRequest { Data = model };

            DoctorResponse _response = new DoctorValidator(_unitOfWork, _context).Validate(request);

            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.DoctorTypes = BindDropDownDoctorType();
            ViewBag.OrganizationList = BindDropDownOrganization();

            return View();
        }

        [HttpPost]
        public ActionResult Edit(DoctorModel model)
        {
            model.Account = Account;

            var request = new DoctorRequest
            {
                Data = model
            };

            DoctorResponse _response = new DoctorValidator(_unitOfWork).Validate(request);
            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.DoctorTypes = BindDropDownParamedicType();

            return View();
        }

        [CustomAuthorize("ADD_M_PARAMEDIC")]
        public ActionResult Create()
        {
            DoctorResponse response = new DoctorResponse();
            ViewBag.Response = response;
            ViewBag.DoctorTypes = BindDropDownParamedicType();
            ViewBag.OrganizationList = BindDropDownOrganization();

            return View();
        }

        [CustomAuthorize("EDIT_M_PARAMEDIC")]
        public ActionResult Edit()
        {
            if (Request.QueryString["id"] != null)
            {
                var request = new DoctorRequest(Request.QueryString["id"].ToString());

                DoctorResponse response = new DoctorHandler(_unitOfWork).GetDetail(request);
                DoctorModel model = response.Entity;
                ViewBag.Response = response;
                ViewBag.DoctorTypes = BindDropDownParamedicType();

                return View(model);
            }
            else
            {
                return BadRequestResponse;
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var request = new DoctorRequest
            {
                Data = new DoctorModel { Id = id, Account = Account },
                Action = ClinicEnums.Action.DELETE.ToString()
            };

            DoctorResponse _response = new DoctorValidator(_unitOfWork).Validate(request);

            return Json(new { Status = _response.Status, Message = _response.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAll()
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new DoctorRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip
            };

            var response = new DoctorHandler(_unitOfWork).GetListData(request, false);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }
    }
}
