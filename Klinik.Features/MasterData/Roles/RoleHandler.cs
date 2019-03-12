using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class RoleHandler : BaseFeatures, IBaseFeatures<RoleResponse, RoleRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public RoleHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RoleResponse CreateOrEdit(RoleRequest request)
        {
            RoleResponse response = new RoleResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.RoleRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        qry.RoleName = request.Data.RoleName;
                        qry.OrgID = request.Data.OrgID;

                        _unitOfWork.RoleRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = $"Success Update Role {qry.RoleName} with Id {qry.ID}";
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = "Update Data Failed";
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Update Data Failed";
                    }
                }
                else
                {
                    var RoleEntity = Mapper.Map<RoleModel, OrganizationRole>(request.Data);

                    _unitOfWork.RoleRepository.Insert(RoleEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = $"Success Add new Role {RoleEntity.RoleName} with Id {RoleEntity.ID}";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Add Data Failed";
                    }
                }
            }
            catch
            {
                response.Status = false;
                response.Message = CommonUtils.GetGeneralErrorMesg();
            }

            return response;
        }

        /// <summary>
        /// Get role details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RoleResponse GetDetail(RoleRequest request)
        {
            RoleResponse response = new RoleResponse();

            var qry = _unitOfWork.RoleRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {

                response.Entity = Mapper.Map<OrganizationRole, RoleModel>(qry.FirstOrDefault());
            }
            return response;
        }

        /// <summary>
        /// Get role list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RoleResponse GetListData(RoleRequest request)
        {
            List<RoleModel> lists = new List<RoleModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<OrganizationRole>(true);
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.RoleName.Contains(request.SearchValue) || p.Organization.OrgName.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
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
                    switch (request.SortColumn.ToLower())
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
                var prData = Mapper.Map<OrganizationRole, RoleModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new RoleResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove role data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RoleResponse RemoveData(RoleRequest request)
        {
            RoleResponse response = new RoleResponse();

            try
            {
                var isExist = _unitOfWork.RoleRepository.GetById(request.Data.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.RoleRepository.Delete(isExist.ID);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = $"Success remove Role {isExist.RoleName} with Id {isExist.ID}";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = $"Remove Role Failed!";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = $"Remove Role Failed!";
                }
            }
            catch
            {
                response.Status = false;
                response.Message = CommonUtils.GetGeneralErrorMesg(); ;
            }

            return response;
        }
    }
}