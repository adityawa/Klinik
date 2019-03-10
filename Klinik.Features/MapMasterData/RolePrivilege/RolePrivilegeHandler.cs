using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MappingMaster;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class RolePrivilegeHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="context"></param>
        public RolePrivilegeHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Create or edit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RolePrivilegeResponse CreateOrEdit(RolePrivilegeRequest request)
        {
            int resultAffected = 0;
            RolePrivilegeResponse response = new RolePrivilegeResponse();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toberemove = _context.RolePrivileges.Where(x => x.RoleID == request.RequestRolePrivData.RoleID);
                    _context.RolePrivileges.RemoveRange(toberemove);
                    _context.SaveChanges();

                    //insert new
                    foreach (long _privid in request.RequestRolePrivData.PrivilegeIDs)
                    {
                        var rolepprivilege = new RolePrivilege
                        {
                            RoleID = request.RequestRolePrivData.RoleID,
                            PrivilegeID = _privid
                        };
                        _context.RolePrivileges.Add(rolepprivilege);
                    }

                    resultAffected = _context.SaveChanges();

                    transaction.Commit();
                    response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                    response.Message = "Data Successfully Saved";
                }
                catch
                {
                    transaction.Rollback();
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = CommonUtils.GetGeneralErrorMesg();
                }
            }

            return response;
        }

        /// <summary>
        /// Get list of role privilege data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RolePrivilegeResponse GetListData(RolePrivilegeRequest request)
        {
            var qry = _unitOfWork.RolePrivRepository.Get(x => x.RoleID == request.RequestRolePrivData.RoleID);
            RolePrivilegeModel _model = new RolePrivilegeModel();

            if (qry.Count > 0)
                _model.RoleID = qry.FirstOrDefault().RoleID;

            if (_model.PrivilegeIDs == null)
                _model.PrivilegeIDs = new List<long>();
            foreach (var item in qry)
            {
                _model.PrivilegeIDs.Add(item.PrivilegeID);
            }
            var response = new RolePrivilegeResponse
            {
                Entity = _model
            };
            return response;
        }

        /// <summary>
        /// Get privilege based on organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationPrivilegeResponse GetPrivilegeBasedOnOrganization(RolePrivilegeRequest request)
        {
            var _orgId = _unitOfWork.RoleRepository.GetById(request.RequestRolePrivData.RoleID) == null ? 0 : _unitOfWork.RoleRepository.GetById(request.RequestRolePrivData.RoleID).OrgID;

            List<OrganizationPrivilegeModel> lists = new List<OrganizationPrivilegeModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<OrganizationPrivilege>(true);
            searchPredicate = searchPredicate.And(x => x.OrgID == _orgId);
            if (!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Privilege.Privilege_Name.Contains(request.searchValue) || p.Privilege.Privilege_Desc.Contains(request.searchValue));
            }

            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDir)))
            {
                if (request.sortColumnDir == "asc")
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "privilevename":
                            qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Privilege.Privilege_Name));
                            break;

                        default:
                            qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "privilevename":
                            qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Privilege.Privilege_Name));
                            break;

                        default:
                            qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.OrgPrivRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<OrganizationPrivilege, OrganizationPrivilegeModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.skip).Take(request.pageSize).ToList();

            var response = new OrganizationPrivilegeResponse
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