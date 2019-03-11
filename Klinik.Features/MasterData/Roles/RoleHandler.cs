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
            int resultAffected = 0;

            RoleResponse response = new RoleResponse();
            try
            {

                if (request.RequestRoleData.Id > 0)
                {
                    var qry = _unitOfWork.RoleRepository.GetById(request.RequestRoleData.Id);
                    if (qry != null)
                    {
                        qry.RoleName = request.RequestRoleData.RoleName;
                        qry.OrgID = request.RequestRoleData.OrgID;

                        _unitOfWork.RoleRepository.Update(qry);
                        resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Status = ClinicEnums.Status.SUCCESS.ToString();
                            response.Message = $"Success Update Role {qry.RoleName} with Id {qry.ID}";
                        }
                        else
                        {
                            response.Status = ClinicEnums.Status.ERROR.ToString();
                            response.Message = "Update Data Failed";
                        }
                    }
                    else
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = "Update Data Failed";
                    }
                }
                else
                {
                    var RoleEntity = Mapper.Map<RoleModel, OrganizationRole>(request.RequestRoleData);

                    _unitOfWork.RoleRepository.Insert(RoleEntity);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.Status.SUCCESS.ToString();
                        response.Message = $"Success Add new Role {RoleEntity.RoleName} with Id {RoleEntity.ID}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = "Add Data Failed";
                    }
                }
            }
            catch
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
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

            var qry = _unitOfWork.RoleRepository.Query(x => x.ID == request.RequestRoleData.Id, null);
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
            if (!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.RoleName.Contains(request.searchValue) || p.Organization.OrgName.Contains(request.searchValue));
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
                var prData = Mapper.Map<OrganizationRole, RoleModel>(item);

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

        /// <summary>
        /// Remove role data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RoleResponse RemoveData(RoleRequest request)
        {
            RoleResponse response = new RoleResponse();
            int resultAffected = 0;
            try
            {
                var isExist = _unitOfWork.RoleRepository.GetById(request.RequestRoleData.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.RoleRepository.Delete(isExist.ID);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.Status.SUCCESS.ToString();
                        response.Message = $"Success remove Role {isExist.RoleName} with Id {isExist.ID}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = $"Remove Role Failed!";
                    }
                }
                else
                {
                    response.Status = ClinicEnums.Status.ERROR.ToString();
                    response.Message = $"Remove Role Failed!";
                }
            }
            catch
            {
                response.Status = ClinicEnums.Status.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg(); ;
            }
            return response;
        }
    }
}