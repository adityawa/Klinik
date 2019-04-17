using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class MenuHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MenuHandler()
        {
        }

        /// <summary>
        /// Contructor with parameter
        /// </summary>
        /// <param name="unitOfWork"></param>
        public MenuHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit menu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MenuResponse CreateOrEdit(MenuRequest request)
        {
            MenuResponse response = new MenuResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.MenuRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Menu, MenuModel>(qry);

                        // update data
                        qry.Name = request.Data.Name;

                        _unitOfWork.MenuRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Menu", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_MENU, Constants.Command.EDIT_MENU, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Menu");

                            CommandLog(false, ClinicEnums.Module.MASTER_MENU, Constants.Command.EDIT_MENU, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Menu");

                        CommandLog(false, ClinicEnums.Module.MASTER_MENU, Constants.Command.EDIT_MENU, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var menuEntity = Mapper.Map<MenuModel, Menu>(request.Data);
                    menuEntity.CreatedBy = request.Data.Account.UserCode;
                    menuEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.MenuRepository.Insert(menuEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Menu", menuEntity.Name, menuEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_MENU, Constants.Command.ADD_MENU, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Menu");

                        CommandLog(false, ClinicEnums.Module.MASTER_MENU, Constants.Command.ADD_MENU, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_MENU, Constants.Command.EDIT_MENU, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_MENU, Constants.Command.ADD_MENU, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get menu details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MenuResponse GetDetail(MenuRequest request)
        {
            MenuResponse response = new MenuResponse();

            var qry = _unitOfWork.MenuRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Menu, MenuModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get menu list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MenuResponse GetListData(MenuRequest request)
        {
            List<MenuModel> lists = new List<MenuModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Menu>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.MenuRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.MenuRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.MenuRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.MenuRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.MenuRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Menu, MenuModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new MenuResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove menu data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MenuResponse RemoveData(MenuRequest request)
        {
            MenuResponse response = new MenuResponse();

            try
            {
                var menu = _unitOfWork.MenuRepository.GetById(request.Data.Id);
                if (menu.ID > 0)
                {
                    menu.RowStatus = -1;
                    menu.ModifiedBy = request.Data.Account.UserCode;
                    menu.ModifiedDate = DateTime.Now;

                    _unitOfWork.MenuRepository.Update(menu);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Menu", menu.Name, menu.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Menu");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Menu");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_MENU, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get visible menu based on level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="parentmenuid"></param>
        /// <returns></returns>
        public IList<MenuModel> GetVisibleMenu(int level = 0, int parentmenuid = 0)
        {
            IList<MenuModel> menus = new List<MenuModel>();
            var qryPredicate = PredicateBuilder.New<Menu>(true);
            qryPredicate = qryPredicate.And(x => x.IsMenu == true);
            if (level > 0)
                qryPredicate = qryPredicate.And(x => x.Level == level);
            if (parentmenuid > 0)
                qryPredicate = qryPredicate.And(x => x.ParentMenuId == parentmenuid);
            var qry = _unitOfWork.MenuRepository.Get(qryPredicate);

            foreach (var item in qry)
            {
                var _mdl = Mapper.Map<Menu, MenuModel>(item);
                menus.Add(_mdl);
            }

            return menus;
        }

        /// <summary>
        /// Get menu based on privilege
        /// </summary>
        /// <param name="privileges"></param>
        /// <returns></returns>
        public IList<MenuModel> GetMenuBasedOnPrivilege(List<long> privileges)
        {
            var qry_menuid = _unitOfWork.PrivilegeRepository.Get(x => privileges.Contains(x.ID)).Select(x => x.MenuID);
            var qry2menu = _unitOfWork.MenuRepository.Get(x => qry_menuid.ToList().Contains(x.ID), orderBy: q => q.OrderBy(x => x.Level).ThenBy(x => x.SortIndex));
            IList<MenuModel> _authmenu = new List<MenuModel>();
            foreach (var item in qry2menu)
            {
                var _menu = Mapper.Map<Menu, MenuModel>(item);
                _authmenu.Add(_menu);

            }
            return _authmenu;
        }
    }
}