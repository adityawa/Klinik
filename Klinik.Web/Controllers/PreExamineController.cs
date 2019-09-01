using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using Klinik.Entities.PreExamine;
using Klinik.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class PreExamineController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public PreExamineController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region ::MISC::
        private List<SelectListItem> BindDropDownPoli(int exclId)
        {
            IList<PoliModel> PoliData = new PoliHandler(_unitOfWork).GetAllPoli(exclId);
            List<SelectListItem> _poliList = new List<SelectListItem>();

            _poliList.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = "0"
            });

            foreach (var item in PoliData)
            {
                _poliList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _poliList;
        }

        private List<SelectListItem> BindDropDownDokter()
        {
            IList<DoctorModel> Doctors = new DoctorHandler(_unitOfWork).GetAllDoctor();
            List<SelectListItem> _doctorList = new List<SelectListItem>();

            _doctorList.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = "0"
            });

            foreach (var item in Doctors)
            {
                _doctorList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return _doctorList;
        }

        private List<SelectListItem> BindDropDownAlreadyPreExamine()
        {

            List<SelectListItem> _poliIsPreExamine = new List<SelectListItem>();

            _poliIsPreExamine.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = string.Empty
            });

            _poliIsPreExamine.Insert(0, new SelectListItem
            {
                Text = "Yes",
                Value = "True"
            });

            _poliIsPreExamine.Insert(0, new SelectListItem
            {
                Text = "No",
                Value = "False"
            });

            return _poliIsPreExamine;
        }
        #endregion

        [CustomAuthorize("VIEW_PREEXAMINE")]
        public ActionResult ListQueue()
        {
            ViewBag.PoliSelection = BindDropDownPoli(1);
            ViewBag.AlreadyPreExamine = BindDropDownAlreadyPreExamine();
            return View();
        }

        [HttpPost]
        public ActionResult GetListQueue(string poli, string preexamine)
        {
            var _draw = Request.Form.GetValues("draw").FirstOrDefault();
            var _start = Request.Form.GetValues("start").FirstOrDefault();
            var _length = Request.Form.GetValues("length").FirstOrDefault();
            var _sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var _sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var _searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            int _skip = _start != null ? Convert.ToInt32(_start) : 0;

            var request = new LoketRequest
            {
                Draw = _draw,
                SearchValue = _searchValue,
                SortColumn = _sortColumn,
                SortColumnDir = _sortColumnDir,
                PageSize = _pageSize,
                Skip = _skip,
                Data = new LoketModel { PoliToID = Convert.ToInt32(poli), strIsPreExamine = preexamine }

            };

            if (Session["UserLogon"] != null)
                request.Data.Account = (AccountModel)Session["UserLogon"];

            var response = new PreExamineHandler(_unitOfWork).GetListData(request);

            return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("ADD_PREEXAMINE", "EDIT_PREEXAMINE")]
        public ActionResult CreateOrEditPreExamine()
        {
            PreExamineResponse _response = new PreExamineResponse();
            if (Request.QueryString["id"] != null)
            {
                var request = new PreExamineRequest
                {
                    Data = new PreExamineModel
                    {
                        LoketData = new LoketModel
                        {
                            Id = long.Parse(Request.QueryString["id"].ToString()),
                        },

                    }
                };
                if (Session["UserLogon"] != null)
                    request.Data.Account = (AccountModel)Session["UserLogon"];

                PreExamineResponse resp = new PreExamineHandler(_unitOfWork).GetDetailNotPreExamine(request);

                PreExamineModel _model = resp.Entity;

                ViewBag.Doctors = BindDropDownDokter();
                return View(_model);
            }
            else
            {
                ViewBag.Doctors = BindDropDownDokter();
                return View();
            }
        }

        [CustomAuthorize("ADD_PREEXAMINE", "EDIT_PREEXAMINE")]
        [HttpPost]
        public ActionResult CreateOrEditPreExamine(PreExamineModel _model)
        {
            if (Session["UserLogon"] != null)
                _model.Account = (AccountModel)Session["UserLogon"];
            var loketId = _model.LoketData.Id;

            var request = new PreExamineRequest
            {
                Data = _model
            };

            PreExamineResponse _response = new PreExamineValidator(_unitOfWork, _context).Validate(request);

            ViewBag.Response = $"{_response.Status};{_response.Message}";
            ViewBag.Doctors = BindDropDownDokter();
            ViewBag.ActionType = request.Data.Id > 0 ? ClinicEnums.Action.Edit : ClinicEnums.Action.Add;
            return View(_response.Entity);
        }
    }
}