using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Registration;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Klinik.Features.Registration
{
    public class RegistrationHandler : BaseFeatures, IBaseFeatures<RegistrationResponse, RegistrationRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public RegistrationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit registration
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RegistrationResponse CreateOrEdit(RegistrationRequest request)
        {
            RegistrationResponse response = new RegistrationResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.RegistrationRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<QueuePoli, RegistrationModel>(qry);

                        // update data
                        qry.Status = request.Data.Status;
                        qry.PoliFrom = request.Data.PoliFromID;
                        qry.PoliTo = request.Data.PoliToID;
                        qry.Doctor = request.Data.Doctor;
                        qry.Type = (short)request.Data.Type;
                        qry.ReffID = request.Data.ReffID;
                        qry.Remark = request.Data.Remark;

                        _unitOfWork.RegistrationRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Registration", Messages.Patient, qry.ID);

                            CommandLog(true, ClinicEnums.Module.REGISTRATION, Constants.Command.EDIT_REGISTRATION, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");

                            CommandLog(false, ClinicEnums.Module.REGISTRATION, Constants.Command.EDIT_REGISTRATION, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");

                        CommandLog(false, ClinicEnums.Module.REGISTRATION, Constants.Command.EDIT_REGISTRATION, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var regEntity = Mapper.Map<RegistrationModel, QueuePoli>(request.Data);
                    regEntity.CreatedBy = request.Data.CreatedBy ?? "SYSTEM";
                    regEntity.CreatedDate = DateTime.Now;
                    regEntity.TransactionDate = DateTime.Now;
                    regEntity.Status = (int)RegistrationStatusEnum.New;
                    regEntity.ClinicID = GetClinicID(request.Data.Account.Organization);
                    regEntity.PoliFrom = 1;
                    regEntity.SortNumber = GenerateSortNumber(request.Data.PoliToID);

                    _unitOfWork.RegistrationRepository.Insert(regEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Registration", regEntity.PatientID, regEntity.ID);

                        CommandLog(true, ClinicEnums.Module.REGISTRATION, Constants.Command.ADD_NEW_REGISTRATION, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Registration");

                        CommandLog(false, ClinicEnums.Module.REGISTRATION, Constants.Command.ADD_NEW_REGISTRATION, request.Data.Account, request.Data);
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
                        CommandLog(false, ClinicEnums.Module.MASTER_ROLE, Constants.Command.EDIT_REGISTRATION, request.Data.Account, request.Data);
                    else
                        CommandLog(false, ClinicEnums.Module.MASTER_PRIVILEGE, Constants.Command.ADD_NEW_REGISTRATION, request.Data.Account, request.Data);
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
        /// Generate the sort number
        /// </summary>
        /// <param name="poliID"></param>
        /// <returns></returns>
        private int? GenerateSortNumber(int poliID)
        {
            var currentQueueList = _unitOfWork.RegistrationRepository.Get(x => x.PoliTo.Value == poliID &&
            x.TransactionDate.Value.Year == DateTime.Today.Year &&
            x.TransactionDate.Value.Month == DateTime.Today.Month &&
            x.TransactionDate.Value.Day == DateTime.Today.Day);

            int sortNumber = currentQueueList.Count + 1;

            return sortNumber;
        }

        /// <summary>
        /// Get all registration
        /// </summary>
        /// <returns></returns>
        public IList<RegistrationModel> GetAllRegistration()
        {
            var qry = _unitOfWork.RegistrationRepository.Get();
            IList<RegistrationModel> registrationList = new List<RegistrationModel>();
            foreach (var item in qry)
            {
                var _registration = Mapper.Map<QueuePoli, RegistrationModel>(item);
                registrationList.Add(_registration);
            }

            return registrationList;
        }

        /// <summary>
        /// Get detail of registration
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RegistrationResponse GetDetail(RegistrationRequest request)
        {
            RegistrationResponse response = new RegistrationResponse();

            var qry = _unitOfWork.RegistrationRepository.Query(x => x.ID == request.Data.Id);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<QueuePoli, RegistrationModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get registration list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RegistrationResponse GetListData(RegistrationRequest request)
        {
            List<RegistrationModel> lists = new List<RegistrationModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<QueuePoli>(true);
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Patient.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "patientname":
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Patient.Name));
                            break;

                        default:
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "patientname":
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Patient.Name));
                            break;

                        default:
                            qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.RegistrationRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                RegistrationModel prData = Mapper.Map<QueuePoli, RegistrationModel>(item);
                prData.DoctorStr = prData.Doctor == 1 ? "Dr Robert" : "Dr Boyke";

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new RegistrationResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove certain registration data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RegistrationResponse RemoveData(RegistrationRequest request)
        {
            RegistrationResponse response = new RegistrationResponse();
            try
            {
                var isExist = _unitOfWork.RegistrationRepository.GetById(request.Data.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.RegistrationRepository.Delete(isExist.ID);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Registration", Messages.Patient, isExist.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Registration");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Registration");
                }
            }
            catch
            {
                response.Status = false;
                response.Message = Messages.GeneralError; ;
            }

            return response;
        }
    }
}
