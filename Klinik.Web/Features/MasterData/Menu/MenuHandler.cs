using AutoMapper;
using Klinik.Web.DataAccess;
using Klinik.Web.Models.MasterData;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Klinik.Web.Features.MasterData.Menu
{
    public class MenuHandler : BaseFeatures
    {
        public MenuHandler()
        {
            
        }
        public MenuHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<MenuModel> GetVisibleMenu(int level = 0, int parentmenuid = 0)
        {
            IList<MenuModel> menus = new List<MenuModel>();
            var qryPredicate = PredicateBuilder.True<Klinik.Web.DataAccess.DataRepository.Menu>();
            qryPredicate = qryPredicate.And(x => x.IsMenu == true);
            if (level > 0)
                qryPredicate = qryPredicate.And(x => x.Level == level);
            if (parentmenuid > 0)
                qryPredicate = qryPredicate.And(x => x.ParentMenuId == parentmenuid);
            var qry = _unitOfWork.MenuRepository.Get(qryPredicate);
            foreach (var item in qry)
            {
                var _mdl = Mapper.Map<Web.DataAccess.DataRepository.Menu, MenuModel>(item);
                menus.Add(_mdl);
            }

            return menus;
        }

        public IList<MenuModel> GetMenuBasedOnPrivilege(List<long> privileges)
        {
            var qry_menuid = _unitOfWork.PrivilegeRepository.Get(x => privileges.Contains(x.ID )).Select(x => x.MenuID);
            var qry2menu = _unitOfWork.MenuRepository.Get(x => qry_menuid.ToList().Contains(x.Id), orderBy: q => q.OrderBy(x => x.Level).ThenBy(x => x.SortIndex));
            IList<MenuModel> _authmenu = new List<MenuModel>();
            foreach (var item in qry2menu)
            {
                var _menu = Mapper.Map<Web.DataAccess.DataRepository.Menu, MenuModel>(item);
                _authmenu.Add(_menu);
               
            }
            return _authmenu;
        }
    }
}