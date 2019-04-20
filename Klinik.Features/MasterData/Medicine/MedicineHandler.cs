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
    public class MedicineHandler : BaseFeatures, IBaseFeatures<MedicineResponse, MedicineRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public MedicineHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit medicine 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MedicineResponse CreateOrEdit(MedicineRequest request)
        {
            MedicineResponse response = new MedicineResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.MedicineRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Medicine, MedicineModel>(qry);
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        // update data
                        qry.Name = request.Data.Name;

                        _unitOfWork.MedicineRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Medicine", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_MEDICINE, Constants.Command.EDIT_MEDICINE, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Medicine");

                            CommandLog(false, ClinicEnums.Module.MASTER_MEDICINE, Constants.Command.EDIT_MEDICINE, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Medicine");

                        CommandLog(false, ClinicEnums.Module.MASTER_MEDICINE, Constants.Command.EDIT_MEDICINE, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var productEntity = Mapper.Map<MedicineModel, Medicine>(request.Data);
                    productEntity.CreatedBy = request.Data.Account.UserCode;
                    productEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.MedicineRepository.Insert(productEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Medicine", productEntity.Name, productEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_MEDICINE, Constants.Command.ADD_MEDICINE, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Medicine");

                        CommandLog(false, ClinicEnums.Module.MASTER_MEDICINE, Constants.Command.ADD_MEDICINE, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_MEDICINE, Constants.Command.EDIT_MEDICINE, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_MEDICINE, Constants.Command.ADD_MEDICINE, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get medicine  details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MedicineResponse GetDetail(MedicineRequest request)
        {
            MedicineResponse response = new MedicineResponse();

            var qry = _unitOfWork.MedicineRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Medicine, MedicineModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get medicine  list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MedicineResponse GetListData(MedicineRequest request)
        {
            List<MedicineModel> lists = new List<MedicineModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Medicine>(true);

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
                            qry = _unitOfWork.MedicineRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.MedicineRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.MedicineRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.MedicineRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.MedicineRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Medicine, MedicineModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new MedicineResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove medicine  data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MedicineResponse RemoveData(MedicineRequest request)
        {
            MedicineResponse response = new MedicineResponse();

            try
            {
                var medicine = _unitOfWork.MedicineRepository.GetById(request.Data.Id);
                if (medicine.ID > 0)
                {
                    medicine.RowStatus = -1;
                    medicine.ModifiedBy = request.Data.Account.UserCode;
                    medicine.ModifiedDate = DateTime.Now;

                    _unitOfWork.MedicineRepository.Update(medicine);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Medicine", medicine.Name, medicine.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Medicine");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Medicine");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_MEDICINE, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}