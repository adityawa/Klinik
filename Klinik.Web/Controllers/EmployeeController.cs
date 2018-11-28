using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Klinik.DAL.Interfaces;
using Klinik.Models;
namespace Klinik.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private IEmployee _iEmployee;
        public EmployeeController(IEmployee empl)
        {
            this._iEmployee = empl;
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}