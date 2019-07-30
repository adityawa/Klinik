﻿using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Klinik.Features;
using Klinik.Entities.Loket;
using Klinik.Entities.Account;
using Klinik.Features.Farmasi;
using Klinik.Common;

namespace Klinik.Web.Controllers
{
    public class FarmasiController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        #region ::DROPDOWN::
        private List<SelectListItem> BindDropDownStatus()
        {
            List<SelectListItem> _status = new List<SelectListItem>();
            _status.Insert(0, new SelectListItem
            {
                Text = "Open",
                Value = "0"
            });

            _status.Insert(1, new SelectListItem
            {
                Text = "Waiting",
                Value = "1"
            });

            return _status;
        }

		private List<SelectListItem> BindDropDownClinic()
		{
			List<SelectListItem> _authorizedClinics = new List<SelectListItem>();
			if (Session["UserLogon"] != null)
			{
				var _account = (AccountModel)Session["UserLogon"];

				var _getClinics = new ClinicHandler(_unitOfWork).GetAllClinic(_account.ClinicID);
				foreach (var item in _getClinics)
				{
					_authorizedClinics.Add(new SelectListItem
					{
						Text = item.Name,
						Value = item.Id.ToString()
					});
				}
			}

			return _authorizedClinics;
		}
		#endregion

		public FarmasiController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        // GET: Farmasi
        public ActionResult PatientList()
        {
			ViewBag.Clinics = BindDropDownClinic();
			return View();
        }

		[HttpPost]
		public ActionResult GetFarmasiQueueFromPoli(string clinics, string status)
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
				Data = new LoketModel { ClinicID = Convert.ToInt32(clinics), PoliToID = (int)PoliEnum.Farmasi }
			};

			if (Session["UserLogon"] != null)
				request.Data.Account = (AccountModel)Session["UserLogon"];

			var response = new FarmasiHandler(_unitOfWork).GetListData(request);

			return Json(new { data = response.Data, recordsFiltered = response.RecordsFiltered, recordsTotal = response.RecordsTotal, draw = response.Draw }, JsonRequestBehavior.AllowGet);
		}
	}
}