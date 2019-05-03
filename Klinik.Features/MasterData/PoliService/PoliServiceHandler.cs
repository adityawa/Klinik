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
    public class PoliServiceHandler : BaseFeatures, IBaseFeatures<PoliServiceResponse, PoliServiceRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public PoliServiceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit poli service
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliServiceResponse CreateOrEdit(PoliServiceRequest request)
        {
            PoliServiceResponse response = new PoliServiceResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PoliServicesRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<PoliService, PoliServiceModel>(qry);
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        // update data
                        qry.PoliID = request.Data.PoliID;
                        qry.ServicesID = request.Data.ServicesID;
                        qry.ClinicID = request.Data.ClinicID;

                        _unitOfWork.PoliServicesRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PoliService", qry.Service.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_POLI_SERVICE, Constants.Command.EDIT_POLI_SERVICE, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PoliService");

                            CommandLog(false, ClinicEnums.Module.MASTER_POLI_SERVICE, Constants.Command.EDIT_POLI_SERVICE, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PoliService");

                        CommandLog(false, ClinicEnums.Module.MASTER_POLI_SERVICE, Constants.Command.EDIT_POLI_SERVICE, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var serviceEntity = Mapper.Map<PoliServiceModel, PoliService>(request.Data);
                    serviceEntity.CreatedBy = request.Data.Account.UserCode;
                    serviceEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.PoliServicesRepository.Insert(serviceEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PoliService", serviceEntity.Service.Name, serviceEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_POLI_SERVICE, Constants.Command.ADD_POLI_SERVICE, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PoliService");

                        CommandLog(false, ClinicEnums.Module.MASTER_POLI_SERVICE, Constants.Command.ADD_POLI_SERVICE, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_POLI_SERVICE, Constants.Command.EDIT_POLI_SERVICE, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_POLI_SERVICE, Constants.Command.ADD_POLI_SERVICE, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get poli service details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliServiceResponse GetDetail(PoliServiceRequest request)
        {
            PoliServiceResponse response = new PoliServiceResponse();

            var qry = _unitOfWork.PoliServicesRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<PoliService, PoliServiceModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get poli service list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliServiceResponse GetListData(PoliServiceRequest request)
        {
            List<PoliServiceModel> lists = new List<PoliServiceModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<PoliService>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Service.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.PoliServicesRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Service.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliServicesRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.PoliServicesRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Service.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliServicesRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PoliServicesRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<PoliService, PoliServiceModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PoliServiceResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove poli service data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliServiceResponse RemoveData(PoliServiceRequest request)
        {
            PoliServiceResponse response = new PoliServiceResponse();

            try
            {
                var service = _unitOfWork.PoliServicesRepository.GetById(request.Data.Id);
                if (service.ID > 0)
                {
                    service.RowStatus = -1;
                    service.ModifiedBy = request.Data.Account.UserCode;
                    service.ModifiedDate = DateTime.Now;

                    _unitOfWork.PoliServicesRepository.Update(service);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PoliService", service.Service.Name, service.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PoliService");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PoliService");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_POLI_SERVICE, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}