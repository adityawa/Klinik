using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Features;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Klinik.Web
{
    public abstract class BaseController : Controller
    {
        #region ::Properties::
        public IUnitOfWork _unitOfWork;
        public KlinikDBEntities _context;

        protected AccountModel Account
        {
            get
            {
                return Session["UserLogon"] == null ? new AccountModel() : (AccountModel)Session["UserLogon"];
            }
        }

        protected long ClinicID
        {
            get
            {
                if (Session["UserLogon"] != null)
                {
                    AccountModel account = (AccountModel)Session["UserLogon"];
                    Organization organization = _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == account.Organization);

                    if (organization.Clinic != null)
                        return organization.Clinic.ID;
                }

                return -1;
            }
        }

        protected HttpStatusCodeResult BadRequestResponse
        {
            get
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public BaseController(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region ::Dropdown Methods::
        protected List<SelectListItem> BindDropDownClinic()
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

        protected List<SelectListItem> BindDropDownOrganization()
        {
            List<SelectListItem> _organizationLists = new List<SelectListItem>();
            foreach (var item in new OrganizationHandler(_unitOfWork).GetOrganizationList())
            {
                _organizationLists.Add(new SelectListItem
                {
                    Text = item.OrgName,
                    Value = item.Id.ToString()
                });
            }

            return _organizationLists;
        }

        protected List<SelectListItem> BindDropDownEmployee()
        {
            List<SelectListItem> _employeeLists = new List<SelectListItem>();
            foreach (var item in new EmployeeHandler(_unitOfWork).GetAllEmployee())
            {
                _employeeLists.Add(new SelectListItem
                {
                    Text = item.EmpName,
                    Value = item.Id.ToString()
                });
            }

            return _employeeLists;
        }

        protected List<SelectListItem> BindDropDownDoctorType()
        {
            return GetGeneralMasterByType(Constants.MasterType.DOCTOR);
        }

        protected List<SelectListItem> BindDropDownParamedicType()
        {
            return GetGeneralMasterByType(Constants.MasterType.PARAMEDIC);
        }

        protected List<SelectListItem> BindDropDownCityType()
        {
            return GetGeneralMasterByType(Constants.MasterType.CITY);
        }

        protected List<SelectListItem> BindDropDownClinicType()
        {
            return GetGeneralMasterByType(Constants.MasterType.CLINIC);
        }

        protected List<SelectListItem> BindDropDownDayType()
        {
            return GetGeneralMasterByType(Constants.MasterType.DAY);
        }

        protected List<SelectListItem> BindDropDownDepartmentType()
        {
            return GetGeneralMasterByType(Constants.MasterType.DEPARTMENT);
        }

        protected List<SelectListItem> BindDropDownEmploymentType()
        {
            return GetGeneralMasterByType(Constants.MasterType.EMPLOYMENT);
        }

        private List<SelectListItem> GetGeneralMasterByType(string type)
        {
            List<SelectListItem> _dataList = new List<SelectListItem>();
            foreach (var item in new MasterHandler(_unitOfWork).GetMasterDataByType(type).ToList())
            {
                _dataList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Value
                });
            }

            return _dataList;
        }
        #endregion
    }
}