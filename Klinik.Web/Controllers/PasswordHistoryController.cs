﻿using Klinik.Web.DataAccess;
using Klinik.Web.Features.Account.PasswordHistory;
using Klinik.Web.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.Web.DataAccess.DataRepository;
namespace Klinik.Web.Controllers
{
    public class PasswordHistoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public PasswordHistoryController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        // GET: PasswordHistory
        public ActionResult ChangePassword()
        {
            if (Session["UserLogon"] != null)
            {
                AccountModel acc = (AccountModel)Session["UserLogon"];
                PasswordHistoryModel _model = new PasswordHistoryModel();
                _model.UserName = acc.UserName;
                _model.UserID = acc.UserID;
                return View(_model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult ChangeUserPassword(PasswordHistoryModel _model)
        {
            PasswordHistoryRequest request = new PasswordHistoryRequest
            {
                RequestPassHistData = new PasswordHistoryModel
                {
                    UserID = _model.UserID,
                    UserName = _model.UserName,
                    Password = _model.Password,
                    NewPassword = _model.NewPassword
                }
            };

            PasswordHistoryResponse response = new PasswordHistoryResponse();
            response = new PasswordHistoryValidator(_unitOfWork, _context).Validate(request);
            ViewBag.Response = $"{response.Status};{response.Message}" ;
            return View("ChangePassword");
        }
    }
}