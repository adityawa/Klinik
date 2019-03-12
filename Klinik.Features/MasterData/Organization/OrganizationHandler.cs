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
    public class OrganizationHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public OrganizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get orginazation data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationResponse GetOrganizationData(OrganizationRequest request)
        {
            List<OrganizationData> lists = new List<OrganizationData>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Organization>(true);
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.OrgCode.Contains(request.SearchValue) || p.OrgName.Contains(request.SearchValue) || p.Clinic.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "orgcode":
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.OrgCode), includes: x => x.Clinic);
                            break;
                        case "orgname":
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.OrgName), includes: x => x.Clinic);
                            break;
                        default:
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID), includes: x => x.Clinic);
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "orgcode":
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.OrgCode), includes: x => x.Clinic);
                            break;
                        case "orgname":
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.OrgName), includes: x => x.Clinic);
                            break;
                        default:
                            qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID), includes: x => x.Clinic);
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.OrganizationRepository.Get(searchPredicate, null, includes: x => x.Clinic);
            }

            foreach (var item in qry)
            {
                var orData = Mapper.Map<Organization, OrganizationData>(item);
                lists.Add(orData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new OrganizationResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Get organization data
        /// </summary>
        /// <returns></returns>
        public List<OrganizationModel> GetOrganizationList()
        {
            List<OrganizationModel> lists = new List<OrganizationModel>();
            var qry = _unitOfWork.OrganizationRepository.Get(null, null, includes: x => x.Clinic);
            foreach (var item in qry)
            {
                lists.Add(Mapper.Map<Organization, OrganizationModel>(item));
            }

            return lists;
        }

        /// <summary>
        /// Create or edit an organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationResponse CreateOrEditOrganization(OrganizationRequest request)
        {
            OrganizationResponse response = new OrganizationResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.OrganizationRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = MappingEntityToModel(qry);
                        qry.OrgCode = request.Data.OrgCode;
                        qry.OrgName = request.Data.OrgName;
                        qry.KlinikID = request.Data.KlinikID;
                        qry.ModifiedBy = request.Data.ModifiedBy;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.OrganizationRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = $"Success Update organization {qry.OrgName} with Id {qry.ID}";

                            CommandLog(ClinicEnums.Module.MASTER_ORGANIZATION, ClinicEnums.Status.SUCCESS, Constants.Command.ADD_NEW_ORG, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = "Update Data Failed";

                            CommandLog(ClinicEnums.Module.MASTER_ORGANIZATION, ClinicEnums.Status.ERROR, Constants.Command.ADD_NEW_ORG, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Update Data Failed";

                        CommandLog(ClinicEnums.Module.MASTER_ORGANIZATION, ClinicEnums.Status.ERROR, Constants.Command.ADD_NEW_ORG, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var OrganizationEntity = Mapper.Map<OrganizationModel, Organization>(request.Data);
                    OrganizationEntity.CreatedBy = request.Data.CreatedBy;
                    OrganizationEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.OrganizationRepository.Insert(OrganizationEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = $"Success Add new organization {OrganizationEntity.OrgName} with Id {OrganizationEntity.ID}";

                        CommandLog(ClinicEnums.Module.MASTER_ORGANIZATION, ClinicEnums.Status.SUCCESS, Constants.Command.ADD_NEW_ORG, request.Data.Account, OrganizationEntity);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Add Data Failed";

                        CommandLog(ClinicEnums.Module.MASTER_ORGANIZATION, ClinicEnums.Status.ERROR, Constants.Command.ADD_NEW_ORG, request.Data.Account, OrganizationEntity);
                    }
                }
            }
            catch
            {
                response.Status = false;
                response.Message = CommonUtils.GetGeneralErrorMesg();

                CommandLog(ClinicEnums.Module.MASTER_ORGANIZATION, ClinicEnums.Status.ERROR, Constants.Command.ADD_NEW_ORG, request.Data.Account, request.Data);
            }

            return response;
        }

        /// <summary>
        /// Get organization details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationResponse GetDetailOrganizationById(OrganizationRequest request)
        {
            OrganizationResponse response = new OrganizationResponse();

            var qry = _unitOfWork.OrganizationRepository.Query(x => x.ID == request.Data.Id, null, includes: x => x.Clinic);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Organization, OrganizationData>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Remove organizationd data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrganizationResponse RemoveOrganization(OrganizationRequest request)
        {
            OrganizationResponse response = new OrganizationResponse();

            try
            {
                var isExist = _unitOfWork.OrganizationRepository.GetById(request.Data.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.OrganizationRepository.Delete(isExist.ID);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = $"Success remove organization {isExist.OrgName} with Id {isExist.ID}";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = $"Remove Organization Failed!";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = $"Remove Organization Failed!";
                }
            }
            catch
            {
                response.Status = false;
                response.Message = CommonUtils.GetGeneralErrorMesg(); ;
            }

            return response;
        }

        /// <summary>
        /// Mapping entity to model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public OrganizationModel MappingEntityToModel(Organization entity)
        {
            var _entity = Mapper.Map<Organization, OrganizationModel>(entity);
            return _entity;
        }
    }
}