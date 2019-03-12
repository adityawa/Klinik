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
            PrivilegeResponse response = new PrivilegeResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PrivilegeRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        qry.Privilege_Name = request.Data.Privilige_Name;
                        qry.Privilege_Desc = request.Data.Privilege_Desc;
                        qry.MenuID = request.Data.MenuID;
                        qry.ModifiedBy = request.Data.ModifiedBy;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.PrivilegeRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Privilege", qry.Privilege_Name, qry.ID);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Privilege");
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Privilege");
                    }
                }
                else
                {
                    var PrivilegeEntity = Mapper.Map<PrivilegeModel, Privilege>(request.Data);
                    PrivilegeEntity.CreatedBy = request.Data.CreatedBy ?? "SYSTEM";
                    PrivilegeEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.PrivilegeRepository.Insert(PrivilegeEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Privilege", PrivilegeEntity.Privilege_Name, PrivilegeEntity.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Privilege");
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
        /// Get privilege details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PrivilegeResponse GetDetail(PrivilegeRequest request)
        {
            PrivilegeResponse response = new PrivilegeResponse();

            var qry = _unitOfWork.PrivilegeRepository.Query(x => x.ID == request.Data.Id, null);
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

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Privilege_Name.Contains(request.SearchValue) || p.Privilege_Desc.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
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
                    switch (request.SortColumn.ToLower())
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
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PrivilegeResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
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

            try
            {
                var isExist = _unitOfWork.PrivilegeRepository.GetById(request.Data.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.PrivilegeRepository.Delete(isExist.ID);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Privilege", isExist.Privilege_Name, isExist.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Privilege");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Privilege");
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