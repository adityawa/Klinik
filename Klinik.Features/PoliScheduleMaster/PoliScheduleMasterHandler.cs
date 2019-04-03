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
    public class PoliScheduleMasterHandler : BaseFeatures, IBaseFeatures<PoliScheduleMasterResponse, PoliScheduleMasterRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public PoliScheduleMasterHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit PoliSchedule Master
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliScheduleMasterResponse CreateOrEdit(PoliScheduleMasterRequest request)
        {
            PoliScheduleMasterResponse response = new PoliScheduleMasterResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PoliScheduleMasterRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<PoliScheduleMaster, PoliScheduleMasterModel>(qry);

                        // update data                        
                        qry.ClinicID = request.Data.ClinicID;
                        qry.PoliID = request.Data.PoliID;
                        qry.DoctorID = request.Data.DoctorID;
                        qry.StartTime = request.Data.StartTime;
                        qry.EndTime = request.Data.EndTime;
                        qry.Day = request.Data.Day;

                        _unitOfWork.PoliScheduleMasterRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated2, "PoliScheduleMaster", qry.ID);

                            CommandLog(true, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.EDIT_POLISCHEDULE, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PoliScheduleMaster");

                            CommandLog(false, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.EDIT_POLISCHEDULE, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PoliScheduleMaster");

                        CommandLog(false, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.EDIT_POLISCHEDULE, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var regEntity = Mapper.Map<PoliScheduleMasterModel, PoliScheduleMaster>(request.Data);
                    regEntity.CreatedBy = request.Data.Account.UserCode;
                    regEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.PoliScheduleMasterRepository.Insert(regEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded2, "PoliScheduleMaster", regEntity.ID);

                        CommandLog(true, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.ADD_NEW_POLISCHEDULE, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PoliScheduleMaster");

                        CommandLog(false, ClinicEnums.Module.POLI_SCHEDULE, Constants.Command.ADD_NEW_POLISCHEDULE, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.POLI_SCHEDULE_MASTER, Constants.Command.EDIT_POLISCHEDULE, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.POLI_SCHEDULE_MASTER, Constants.Command.ADD_NEW_POLISCHEDULE, request.Data.Account, ex);
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
        /// Get all PoliSchedule Master
        /// </summary>
        /// <returns></returns>
        public IList<PoliScheduleMasterModel> GetAllPoliSchedule()
        {
            var qry = _unitOfWork.PoliScheduleMasterRepository.Get();
            IList<PoliScheduleMasterModel> PoliScheduleList = new List<PoliScheduleMasterModel>();
            foreach (var item in qry)
            {
                var _poliSchedule = Mapper.Map<PoliScheduleMaster, PoliScheduleMasterModel>(item);
                PoliScheduleList.Add(_poliSchedule);
            }

            return PoliScheduleList;
        }

        /// <summary>
        /// Get detail of PoliSchedule Master
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliScheduleMasterResponse GetDetail(PoliScheduleMasterRequest request)
        {
            PoliScheduleMasterResponse response = new PoliScheduleMasterResponse();

            var qry = _unitOfWork.PoliScheduleMasterRepository.Query(x => x.ID == request.Data.Id);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<PoliScheduleMaster, PoliScheduleMasterModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get PoliSchedule list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliScheduleMasterResponse GetListData(PoliScheduleMasterRequest request)
        {
            List<PoliScheduleMasterModel> lists = new List<PoliScheduleMasterModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<PoliScheduleMaster>(true);

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
                            qry = _unitOfWork.PoliScheduleMasterRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Doctor.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliScheduleMasterRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "doctorname":
                            qry = _unitOfWork.PoliScheduleMasterRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Doctor.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliScheduleMasterRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PoliScheduleMasterRepository.Get(searchPredicate, null);
            }

            // get day master
            var dayList = new MasterHandler(_unitOfWork).GetMasterDataByType(ClinicEnums.MasterTypes.Day.ToString()).ToList();

            foreach (var item in qry)
            {
                PoliScheduleMasterModel prData = Mapper.Map<PoliScheduleMaster, PoliScheduleMasterModel>(item);
                var day = dayList.FirstOrDefault(x => x.Value == prData.Day.ToString());
                prData.DayName = day == null ? prData.Day.ToString() : day.Name;
                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PoliScheduleMasterResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove certain PoliSchedule Master data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PoliScheduleMasterResponse RemoveData(PoliScheduleMasterRequest request)
        {
            PoliScheduleMasterResponse response = new PoliScheduleMasterResponse();
            try
            {
                var isExist = _unitOfWork.PoliScheduleMasterRepository.GetById(request.Data.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.PoliScheduleMasterRepository.Delete(isExist.ID);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved2, "PoliScheduleMaster", isExist.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PoliScheduleMaster");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PoliScheduleMaster");
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
