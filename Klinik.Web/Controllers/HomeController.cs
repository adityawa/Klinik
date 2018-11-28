<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Test()
        {
            return View();
        }
    }
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Klinik.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        // GET: Home
        public ActionResult Test()
        {
            return View();
        }
    }
>>>>>>> 7b81c8768a356099c930436946ba999c9146b22c
}