using Klinik.Web.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.Web.Features.Account;
using Klinik.Web.DataAccess;
using Klinik.Web.Enumerations;
using Klinik.Web.Models.MasterData;
using Klinik.Web.Features.MasterData.Menu;

namespace Klinik.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private IUnitOfWork _unitOfWork;
        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(AccountModel _model)
        {
            AccountRequest request = new AccountRequest
            {
                RequestAccountModel = new AccountModel
                {
                    UserName = _model.UserName,
                    Password=_model.Password
                }
            };
            AccountResponse response = new AccountResponse();
            new AccountValidator(_unitOfWork).Validate(request, out response);
            if (response.Status == ClinicEnums.enumAuthResult.SUCCESS.ToString())
            {
                Session["UserLogon"] = response.Entity;
                if (response.Entity.Privileges.PrivilegeIDs!= null)
                {
                    IList<MenuModel> Menu = new MenuHandler(_unitOfWork).GetMenuBasedOnPrivilege(response.Entity.Privileges.PrivilegeIDs);
                    Session["AuthMenu"] = Menu;
                    //Get
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Status = response.Status.ToString();
                ViewBag.Message = response.Message.ToString();
                return View("Login");
            }
           
        }

        [HttpGet]
        public ActionResult Logout()
        {

            try
            {
                
                HttpContext.Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NonAction]
        public void remove_Anonymous_Cookies()
        {
            try
            {

                if (Request.Cookies["WebTime"] != null)
                {
                    var option = new HttpCookie("WebTime");
                    option.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(option);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }
}