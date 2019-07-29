using Klinik.Data;
using Klinik.Data.DataRepository;
using System.Collections.Generic;
using System.Web.Mvc;

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

        #endregion
        public FarmasiController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        // GET: Farmasi
        public ActionResult PatientList()
        {
            return View();
        }
    }
}