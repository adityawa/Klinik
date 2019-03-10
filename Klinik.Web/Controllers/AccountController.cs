using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Features;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private IUnitOfWork _unitOfWork;
        private KlinikDBEntities _context;

        public AccountController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            // generate reset code
            string resetCode = Guid.NewGuid().ToString();

            // define request
            AccountRequest request = new AccountRequest
            {
                RequestAccountModel = new AccountModel
                {
                    Email = EmailID,
                    ResetPasswordCode = resetCode
                }
            };

            // set reset password code
            var response = new AccountHandler(_unitOfWork, _context).SetResetPasswordCode(request);
            if (response.Status == ClinicEnums.enumAuthResult.SUCCESS.ToString())
            {
                // send verification email
                SendVerificationLinkEmail(EmailID, resetCode);

                ViewBag.Message = "Reset password link has been sent to your email address.";
            }
            else
            {
                ViewBag.Message = "Employee with an email " + EmailID + " does not exist";
            }

            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            // validate
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Login", "Account");
            }

            // define request
            AccountRequest request = new AccountRequest
            {
                RequestAccountModel = new AccountModel
                {
                    ResetPasswordCode = id
                }
            };

            // validate reset password code
            var response = new AccountHandler(_unitOfWork, _context).ValidateResetPasswordCode(request);
            if (response.Status == ClinicEnums.enumAuthResult.SUCCESS.ToString())
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                // define request
                AccountRequest request = new AccountRequest
                {
                    RequestAccountModel = new AccountModel
                    {
                        Password = model.NewPassword,
                        ResetPasswordCode = model.ResetCode
                    }
                };

                // update user password 
                var response = new AccountHandler(_unitOfWork, _context).UpdateUserPassword(request);
                if (response.Status == ClinicEnums.enumAuthResult.SUCCESS.ToString())
                {
                    ViewBag.Message = "User password has been successfully updated";
                }
                else
                {
                    ViewBag.Message = "Failed update user password";
                }

                ViewBag.Status = response.Status.ToString();
                ViewBag.Message = message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SignIn(AccountModel _model)
        {
            AccountRequest request = new AccountRequest
            {
                RequestAccountModel = new AccountModel
                {
                    UserName = _model.UserName,
                    Password = _model.Password
                }
            };

            AccountResponse response = new AccountResponse();
            new AccountValidator(_unitOfWork).Validate(request, out response);
            if (response.Status == ClinicEnums.enumAuthResult.SUCCESS.ToString())
            {
                Session["UserLogon"] = response.Entity;
                if (response.Entity.Privileges.PrivilegeIDs != null)
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

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string resetCode)
        {
            try
            {
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/Account/ResetPassword/" + resetCode);

                var fromEmail = new MailAddress("verifikasi.klinik@gmail.com", "Medical Management Sistem");
                var toEmail = new MailAddress(emailID);
                var fromEmailPassword = "Klinik2019";

                string subject = "Reset Password";
                string body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                };

                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })

                    smtp.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}