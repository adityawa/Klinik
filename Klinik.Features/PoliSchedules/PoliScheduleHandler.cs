using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities;
using Klinik.Entities.PoliSchedules;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Klinik.Features.PoliSchedules
{
    public class PoliScheduleHandler : BaseFeatures, IBaseFeatures<PoliScheduleResponse, PoliScheduleRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public PoliScheduleHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit PoliSchedule
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliScheduleResponse CreateOrEdit(PoliScheduleRequest request)
        {
            PoliScheduleResponse response = new PoliScheduleResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PoliScheduleRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<PoliSchedule, PoliScheduleModel>(qry);

                        // update data
                        qry.Status = request.Data.Status;
                        qry.ClinicID = request.Data.ClinicID;
                        qry.PoliID = request.Data.PoliID;
                        qry.DoctorID = request.Data.DoctorID;
                        qry.StartDate = request.Data.StartDate;
                        qry.EndDate = request.Data.EndDate;
                        qry.ReffID = request.Data.ReffID;

                        _unitOfWork.PoliScheduleRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated2, "PoliSchedule", qry.ID);

                            CommandLog(true, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.EDIT_POLISCHEDULE, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PoliSchedule");

                            CommandLog(false, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.EDIT_POLISCHEDULE, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PoliSchedule");

                        CommandLog(false, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.EDIT_POLISCHEDULE, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var regEntity = Mapper.Map<PoliScheduleModel, PoliSchedule>(request.Data);
                    regEntity.CreatedBy = request.Data.Account.UserName;
                    regEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.PoliScheduleRepository.Insert(regEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded2, "PoliSchedule", regEntity.ID);

                        CommandLog(true, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.ADD_NEW_POLISCHEDULE, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PoliSchedule");

                        CommandLog(false, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.ADD_NEW_POLISCHEDULE, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null)
                {
                    if (request.Data.Id > 0)
                        CommandLog(false, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.EDIT_POLISCHEDULE, request.Data.Account, request.Data);
                    else
                        CommandLog(false, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.ADD_NEW_POLISCHEDULE, request.Data.Account, request.Data);
                }
            }

            return response;
        }

        /// <summary>
        /// Get klinik ID
        /// </summary>
        /// <param name="organizationCode"></param>
        /// <returns></returns>
        private long? GetClinicID(string organizationCode)
        {
            Organization organization = _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == organizationCode);
            return organization.KlinikID;
        }

        /// <summary>
        /// Get all PoliSchedule
        /// </summary>
        /// <returns></returns>
        public IList<PoliScheduleModel> GetAllPoliSchedule()
        {
            var qry = _unitOfWork.PoliScheduleRepository.Get();
            IList<PoliScheduleModel> PoliScheduleList = new List<PoliScheduleModel>();
            foreach (var item in qry)
            {
                var _PoliSchedule = Mapper.Map<PoliSchedule, PoliScheduleModel>(item);
                PoliScheduleList.Add(_PoliSchedule);
            }

            return PoliScheduleList;
        }

        /// <summary>
        /// Get detail of PoliSchedule
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliScheduleResponse GetDetail(PoliScheduleRequest request)
        {
            PoliScheduleResponse response = new PoliScheduleResponse();

            var qry = _unitOfWork.PoliScheduleRepository.Query(x => x.ID == request.Data.Id);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<PoliSchedule, PoliScheduleModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get PoliSchedule list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliScheduleResponse GetListData(PoliScheduleRequest request)
        {
            List<PoliScheduleModel> lists = new List<PoliScheduleModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<PoliSchedule>(true);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Doctor.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "doctorname":
                            qry = _unitOfWork.PoliScheduleRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Doctor.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliScheduleRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "doctorname":
                            qry = _unitOfWork.PoliScheduleRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Doctor.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliScheduleRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PoliScheduleRepository.Get(searchPredicate, null);
            }

            // get status master
            var statusList = new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.PoliScheduleStatus.ToString()).ToList();

            foreach (var item in qry)
            {
                PoliScheduleModel prData = Mapper.Map<PoliSchedule, PoliScheduleModel>(item);
                var status = statusList.FirstOrDefault(x => x.Value == prData.Status.ToString());
                prData.StatusStr = status == null ? prData.Status.ToString() : status.Name;
                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PoliScheduleResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove certain PoliSchedule data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliScheduleResponse RemoveData(PoliScheduleRequest request)
        {
            PoliScheduleResponse response = new PoliScheduleResponse();
            try
            {
                var isExist = _unitOfWork.PoliScheduleRepository.GetById(request.Data.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.PoliScheduleRepository.Delete(isExist.ID);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved2, "PoliSchedule", isExist.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PoliSchedule");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PoliSchedule");
                }
            }
            catch
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }

            return response;
        }
    }
}
