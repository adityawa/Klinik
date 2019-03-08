﻿using AutoMapper;
using Klinik.Web.DataAccess;
using Klinik.Web.DataAccess.DataRepository;
using Klinik.Web.Enumerations;
using Klinik.Web.Features.MasterData.Roles;
using Klinik.Web.Infrastructure;
using Klinik.Web.Models.MappingMaster;
using Klinik.Web.Models.MasterData;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Features.MapMasterData.UserRole
{
    public class UserRoleHandler:BaseFeatures
    {
        public UserRoleHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public UserRoleResponse CreateOrEdit(UserRoleRequest request)
        {
            int resultAffected = 0;

            UserRoleResponse response = new UserRoleResponse();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toberemove = _context.UserRoles.Where(x => x.UserID == request.RequestUserRoleData.UserID);
                    _context.UserRoles.RemoveRange(toberemove);
                    _context.SaveChanges();

                    //insert new
                    foreach (long _roleid in request.RequestUserRoleData.RoleIds)
                    {
                        var _userrole = new Web.DataAccess.DataRepository.UserRole
                        {
                            UserID = request.RequestUserRoleData.UserID,
                            RoleID = _roleid
                        };
                        _context.UserRoles.Add(_userrole);
                    }

                    resultAffected = _context.SaveChanges();

                    transaction.Commit();
                    response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                    response.Message = "Data Successfully Saved";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = Common.GetGeneralErrorMesg();
                }
            }


            return response;
        }

        public UserRoleResponse GetListData(UserRoleRequest request)
        {
            var qry = _unitOfWork.UserRoleRepository.Get(x => x.UserID == request.RequestUserRoleData.UserID);
            UserRoleModel _model = new UserRoleModel();

            if (qry.Count > 0)
                _model.UserID = qry.FirstOrDefault().UserID;

            if (_model.RoleIds == null)
                _model.RoleIds = new List<long>();
            foreach (var item in qry)
            {
                _model.RoleIds.Add(item.RoleID);
            }
            var response = new UserRoleResponse
            {
                Entity = _model
            };
            return response;
        }

        public RoleResponse GetRoleBasedOnOrganization(UserRoleRequest request)
        {
            var _orgId = _unitOfWork.UserRepository.GetById(request.RequestUserRoleData.UserID) == null ? 0 : _unitOfWork.UserRepository.GetById(request.RequestUserRoleData.UserID).OrganizationID;

            List<RoleModel> lists = new List<RoleModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.True<OrganizationRole>();
            searchPredicate = searchPredicate.And(x => x.OrgID == _orgId);
            if (!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.RoleName.Contains(request.searchValue));
            }


            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDir)))
            {
                if (request.sortColumnDir == "asc")
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "rolename":
                            qry = _unitOfWork.RoleRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.RoleName));
                            break;

                        default:
                            qry = _unitOfWork.RoleRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "rolename":
                            qry = _unitOfWork.RoleRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.RoleName));
                            break;

                        default:
                            qry = _unitOfWork.RoleRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.RoleRepository.Get(searchPredicate, null);
            }
            foreach (var item in qry)
            {
                var prData = Mapper.Map<Web.DataAccess.DataRepository.OrganizationRole, RoleModel>(item);

                lists.Add(prData);
            }


            int totalRequest = lists.Count();
            var data = lists.Skip(request.skip).Take(request.pageSize).ToList();


            var response = new RoleResponse
            {
                draw = request.draw,
                recordsFiltered = totalRequest,
                recordsTotal = totalRequest,
                Data = data
            };

            return response;
        }
    }
}