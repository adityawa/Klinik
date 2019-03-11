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
    public class PrivilegeHandler : BaseFeatures, IBaseFeatures<PrivilegeResponse, PrivilegeRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public PrivilegeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit privilege
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PrivilegeResponse CreateOrEdit(PrivilegeRequest request)
        {
            int resultAffected = 0;

            PrivilegeResponse response = new PrivilegeResponse();
            try
            {
                if (request.RequestPrivilegeData.Id > 0)
                {
                    var qry = _unitOfWork.PrivilegeRepository.GetById(request.RequestPrivilegeData.Id);
                    if (qry != null)
                    {
                        qry.Privilege_Name = request.RequestPrivilegeData.Privilige_Name;
                        qry.Privilege_Desc = request.RequestPrivilegeData.Privilege_Desc;
                        qry.MenuID = request.RequestPrivilegeData.MenuID;
                        qry.ModifiedBy = request.RequestPrivilegeData.ModifiedBy;
                        qry.ModifiedDate = DateTime.Now;
                        _unitOfWork.PrivilegeRepository.Update(qry);
                        resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Status = ClinicEnums.Status.SUCCESS.ToString();
                            response.Message = $"Success Update Privilege {qry.Privilege_Name} with Id {qry.ID}";
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
                    var PrivilegeEntity = Mapper.Map<PrivilegeModel, Privilege>(request.RequestPrivilegeData);
                    PrivilegeEntity.CreatedBy = request.RequestPrivilegeData.CreatedBy ?? "SYSTEM";
                    PrivilegeEntity.CreatedDate = DateTime.Now;
                    _unitOfWork.PrivilegeRepository.Insert(PrivilegeEntity);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.Status.SUCCESS.ToString();
                        response.Message = $"Success Add new Privilege {PrivilegeEntity.Privilege_Name} with Id {PrivilegeEntity.ID}";
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
        /// Get privilege details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PrivilegeResponse GetDetail(PrivilegeRequest request)
        {
            PrivilegeResponse response = new PrivilegeResponse();

            var qry = _unitOfWork.PrivilegeRepository.Query(x => x.ID == request.RequestPrivilegeData.Id, null);
            if (qry.FirstOrDefault() != null)
            {

                response.Entity = Mapper.Map<Privilege, PrivilegeModel>(qry.FirstOrDefault());
            }
            return response;
        }

        /// <summary>
        /// Get list of privilege data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PrivilegeResponse GetListData(PrivilegeRequest request)
        {
            List<PrivilegeModel> lists = new List<PrivilegeModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Privilege>(true);

            if (!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Privilege_Name.Contains(request.searchValue) || p.Privilege_Desc.Contains(request.searchValue));
            }

            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDir)))
            {
                if (request.sortColumnDir == "asc")
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "privilige_name":
                            qry = _unitOfWork.PrivilegeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Privilege_Name));
                            break;

                        default:
                            qry = _unitOfWork.PrivilegeRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "privilige_name":
                            qry = _unitOfWork.PrivilegeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Privilege_Name));
                            break;

                        default:
                            qry = _unitOfWork.PrivilegeRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PrivilegeRepository.Get(searchPredicate, null);
            }
            foreach (var item in qry)
            {
                var prData = Mapper.Map<Privilege, PrivilegeModel>(item);

                lists.Add(prData);
            }


            int totalRequest = lists.Count();
            var data = lists.Skip(request.skip).Take(request.pageSize).ToList();


            var response = new PrivilegeResponse
            {
                draw = request.draw,
                recordsFiltered = totalRequest,
                recordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove privilege data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PrivilegeResponse RemoveData(PrivilegeRequest request)
        {
            PrivilegeResponse response = new PrivilegeResponse();
            int resultAffected = 0;
            try
            {
                var isExist = _unitOfWork.PrivilegeRepository.GetById(request.RequestPrivilegeData.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.PrivilegeRepository.Delete(isExist.ID);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.Status.SUCCESS.ToString();
                        response.Message = $"Success remove Privilege {isExist.Privilege_Name} with Id {isExist.ID}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.Status.ERROR.ToString();
                        response.Message = $"Remove Privilege Failed!";
                    }
                }
                else
                {
                    response.Status = ClinicEnums.Status.ERROR.ToString();
                    response.Message = $"Remove Privilege Failed!";
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