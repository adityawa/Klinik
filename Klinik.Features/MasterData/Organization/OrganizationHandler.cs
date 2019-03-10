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
            if (!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.OrgCode.Contains(request.searchValue) || p.OrgName.Contains(request.searchValue) || p.Clinic.Name.Contains(request.searchValue));
            }

            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDir)))
            {
                if (request.sortColumnDir == "asc")
                {
                    switch (request.sortColumn.ToLower())
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
                    switch (request.sortColumn.ToLower())
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
            var data = lists.Skip(request.skip).Take(request.pageSize).ToList();

            var response = new OrganizationResponse
            {
                draw = request.draw,
                recordsFiltered = totalRequest,
                recordsTotal = totalRequest,
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
            int resultAffected = 0;
            OrganizationResponse response = new OrganizationResponse();

            try
            {
                if (request.RequestOrganizationData.Id > 0)
                {
                    var qry = _unitOfWork.OrganizationRepository.GetById(request.RequestOrganizationData.Id);
                    if (qry != null)
                    {
                        qry.OrgCode = request.RequestOrganizationData.OrgCode;
                        qry.OrgName = request.RequestOrganizationData.OrgName;
                        qry.KlinikID = request.RequestOrganizationData.KlinikID;
                        qry.ModifiedBy = request.RequestOrganizationData.ModifiedBy;
                        qry.ModifiedDate = DateTime.Now;
                        _unitOfWork.OrganizationRepository.Update(qry);
                        resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                            response.Message = $"Success Update organization {qry.OrgName} with Id {qry.ID}";
                        }
                        else
                        {
                            response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                            response.Message = "Update Data Failed";
                        }
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = "Update Data Failed";
                    }
                }
                else
                {
                    var OrganizationEntity = Mapper.Map<OrganizationModel, Organization>(request.RequestOrganizationData);
                    OrganizationEntity.CreatedBy = request.RequestOrganizationData.CreatedBy;
                    OrganizationEntity.CreatedDate = DateTime.Now;
                    _unitOfWork.OrganizationRepository.Insert(OrganizationEntity);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                        response.Message = $"Success Add new organization {OrganizationEntity.OrgName} with Id {OrganizationEntity.ID}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = "Add Data Failed";
                    }
                }
            }
            catch
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg();
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

            var qry = _unitOfWork.OrganizationRepository.Query(x => x.ID == request.RequestOrganizationData.Id, null, includes: x => x.Clinic);
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
            int resultAffected = 0;
            try
            {
                var isExist = _unitOfWork.OrganizationRepository.GetById(request.RequestOrganizationData.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.OrganizationRepository.Delete(isExist.ID);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                        response.Message = $"Success remove organization {isExist.OrgName} with Id {isExist.ID}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = $"Remove Organization Failed!";
                    }
                }
                else
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Remove Organization Failed!";
                }
            }
            catch
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg(); ;
            }

            return response;
        }
    }
}