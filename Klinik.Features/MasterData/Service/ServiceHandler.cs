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
    public class ServiceHandler : BaseFeatures, IBaseFeatures<ServiceResponse, ServiceRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ServiceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit service
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse CreateOrEdit(ServiceRequest request)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.ServicesRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Service, ServiceModel>(qry);
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        // update data
                        qry.Code = request.Data.Code;
                        qry.Name = request.Data.Name;
                        qry.Price = request.Data.Price;

                        _unitOfWork.ServicesRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Service", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_SERVICE, Constants.Command.EDIT_SERVICE, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Service");

                            CommandLog(false, ClinicEnums.Module.MASTER_SERVICE, Constants.Command.EDIT_SERVICE, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Service");

                        CommandLog(false, ClinicEnums.Module.MASTER_SERVICE, Constants.Command.EDIT_SERVICE, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var serviceEntity = Mapper.Map<ServiceModel, Service>(request.Data);
                    serviceEntity.CreatedBy = request.Data.Account.UserCode;
                    serviceEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.ServicesRepository.Insert(serviceEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Service", serviceEntity.Name, serviceEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_SERVICE, Constants.Command.ADD_SERVICE, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Service");

                        CommandLog(false, ClinicEnums.Module.MASTER_SERVICE, Constants.Command.ADD_SERVICE, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_SERVICE, Constants.Command.EDIT_SERVICE, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_SERVICE, Constants.Command.ADD_SERVICE, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get service details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse GetDetail(ServiceRequest request)
        {
            ServiceResponse response = new ServiceResponse();

            var qry = _unitOfWork.ServicesRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Service, ServiceModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get service list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse GetListData(ServiceRequest request)
        {
            List<ServiceModel> lists = new List<ServiceModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Service>(true);

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
                            qry = _unitOfWork.ServicesRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ServicesRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.ServicesRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ServicesRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.ServicesRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Service, ServiceModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ServiceResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove service data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse RemoveData(ServiceRequest request)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var service = _unitOfWork.ServicesRepository.GetById(request.Data.Id);
                if (service.ID > 0)
                {
                    service.RowStatus = -1;
                    service.ModifiedBy = request.Data.Account.UserCode;
                    service.ModifiedDate = DateTime.Now;

                    _unitOfWork.ServicesRepository.Update(service);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Service", service.Name, service.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Service");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Service");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_SERVICE, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}