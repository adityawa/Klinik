using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.DataAccess.DataRepository;

namespace Klinik.Web.Features
{
    public abstract class BaseFeatures
    {
        public IUnitOfWork _unitOfWork;
        public KlinikDBEntities _context;
        public IList<string> errorFields = new List<string>();

        public BaseFeatures()
        {

        }

        public BaseFeatures(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool IsHaveAuthorization(string privilege_name, List<long> PrivilegeIds)
        {
            bool IsAuthorized = false;
            var _getPrivilegeName = _unitOfWork.PrivilegeRepository.Get(x => PrivilegeIds.Contains(x.ID));
            foreach(var item in _getPrivilegeName)
            {
                if (privilege_name == item.Privilege_Name)
                    IsAuthorized = true;
            }

            return IsAuthorized;
        }

        
    }
}