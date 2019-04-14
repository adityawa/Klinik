using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Loket;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Klinik.Features
{
    public class LoketHandler : BaseFeatures, IBaseFeatures<LoketResponse, LoketRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LoketHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit registration
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LoketResponse CreateOrEdit(LoketRequest request)
        {
            LoketResponse response = new LoketResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.RegistrationRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<QueuePoli, LoketModel>(qry);

                        // update data
                        qry.Status = request.Data.Status;
                        qry.PoliFrom = request.Data.PoliFromID;
                        qry.PoliTo = request.Data.PoliToID;
                        qry.DoctorID = request.Data.DoctorID;
                        qry.Type = (short)request.Data.Type;
                        qry.ReffID = request.Data.ReffID;
                        qry.Remark = request.Data.Remark;
                        qry.DoctorID = request.Data.DoctorID == 0 ? (int?)null : request.Data.DoctorID;
                        qry.FormMedical.Necessity = request.Data.NecessityType.ToString();
                        qry.FormMedical.PaymentType = request.Data.PaymentType.ToString();
                        qry.FormMedical.Number = request.Data.PaymentNumber;

                        _unitOfWork.RegistrationRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated2, "Registration", qry.ID);

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
                    var queueEntity = Mapper.Map<LoketModel, QueuePoli>(request.Data);
                    queueEntity.CreatedBy = request.Data.Account.UserCode;
                    queueEntity.CreatedDate = DateTime.Now;
                    queueEntity.TransactionDate = DateTime.Now;
                    queueEntity.Status = (int)RegistrationStatusEnum.New;
                    queueEntity.ClinicID = GetClinicID(request.Data.Account.Organization);
                    queueEntity.PoliFrom = 1;
                    queueEntity.SortNumber = GenerateSortNumber(request.Data.PoliToID, request.Data.DoctorID);
                    queueEntity.DoctorID = request.Data.DoctorID == 0 ? (int?)null : request.Data.DoctorID;

                    // create form medical
                    var formMedical = new FormMedical
                    {
                        ClinicID = request.Data.Account.ClinicID,
                        PatientID = request.Data.PatientID,
                        PaymentType = request.Data.PaymentType.ToString(),
                        Number = request.Data.PaymentNumber,
                        Necessity = request.Data.NecessityType.ToString(),
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now
                    };

                    // reference to queue
                    queueEntity.FormMedical = formMedical;

                    _unitOfWork.RegistrationRepository.Insert(queueEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Registration", queueEntity.PatientID, queueEntity.ID);

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

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.REGISTRATION, Constants.Command.EDIT_REGISTRATION, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.REGISTRATION, Constants.Command.ADD_NEW_REGISTRATION, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get klinik ID
        /// </summary>
        /// <param name="organizationCode"></param>
        /// <returns></returns>
        private long GetClinicID(string organizationCode)
        {
            Organization organization = _unitOfWork.OrganizationRepository.GetFirstOrDefault(x => x.OrgCode == organizationCode);
            return organization.KlinikID.Value;
        }

        /// <summary>
        /// Generate the sort number
        /// </summary>
        /// <param name="poliID"></param>
        /// <returns></returns>
        private int GenerateSortNumber(int poliID, int doctorID)
        {
            List<QueuePoli> currentQueueList = _unitOfWork.RegistrationRepository.Get(x => x.PoliTo == poliID &&
            x.TransactionDate.Year == DateTime.Today.Year &&
            x.TransactionDate.Month == DateTime.Today.Month &&
            x.TransactionDate.Day == DateTime.Today.Day);

            if (doctorID > 0 && currentQueueList.Count > 0)
                currentQueueList = currentQueueList.Where(x => x.DoctorID == doctorID).ToList();

            int sortNumber = currentQueueList.Count + 1;

            return sortNumber;
        }

        /// <summary>
        /// Get all registration
        /// </summary>
        /// <returns></returns>
        public IList<LoketModel> GetAllRegistration()
        {
            var qry = _unitOfWork.RegistrationRepository.Get();
            IList<LoketModel> registrationList = new List<LoketModel>();
            foreach (var item in qry)
            {
                var _registration = Mapper.Map<QueuePoli, LoketModel>(item);
                registrationList.Add(_registration);
            }

            return registrationList;
        }

        /// <summary>
        /// Get detail of registration
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LoketResponse GetDetail(LoketRequest request)
        {
            LoketResponse response = new LoketResponse();

            QueuePoli qry = _unitOfWork.RegistrationRepository.GetFirstOrDefault(x => x.ID == request.Data.Id);
            if (qry != null)
            {
                response.Entity = Mapper.Map<QueuePoli, LoketModel>(qry);

                // reformat gender
                response.Entity.PatientGender = response.Entity.PatientGender == "M" ? Messages.Male : Messages.Female;
                response.Entity.PatientType = response.Entity.PatientType == "2" ? Messages.Company : Messages.General;

                FormMedical formMedical = _unitOfWork.FormMedicalRepository.GetFirstOrDefault(x => x.ID == qry.FormMedicalID);
                if (formMedical != null)
                {
                    response.Entity.NecessityType = int.Parse(formMedical.Necessity);
                    response.Entity.PaymentType = int.Parse(formMedical.PaymentType);
                    response.Entity.PaymentNumber = formMedical.Number;
                }
            }

            return response;
        }

        /// <summary>
        /// Get registration list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LoketResponse GetListData(LoketRequest request)
        {
            return GetListData(request, 1);
        }

        /// <summary>
        /// Get registration list
        /// </summary>
        /// <param name="request"></param>
        /// <param name="poliID"></param>
        /// <returns></returns>
        public LoketResponse GetListData(LoketRequest request, int poliID = 1)
        {
            List<LoketModel> lists = new List<LoketModel>();
            List<QueuePoli> qry = null;
            var searchPredicate = PredicateBuilder.New<QueuePoli>(true);

            // add default filter to show today queue only
            if (poliID == 1)
            {
                searchPredicate = searchPredicate.And(p => p.TransactionDate.Year == DateTime.Today.Year &&
                                                           p.TransactionDate.Month == DateTime.Today.Month &&
                                                           p.TransactionDate.Day == DateTime.Today.Day);
            }
            else
            {
                searchPredicate = searchPredicate.And(p => p.TransactionDate.Year == DateTime.Today.Year &&
                                           p.TransactionDate.Month == DateTime.Today.Month &&
                                           p.TransactionDate.Day == DateTime.Today.Day &&
                                           p.PoliTo == poliID);
            }

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
                LoketModel prData = Mapper.Map<QueuePoli, LoketModel>(item);

                // get the doctor name
                if (prData.DoctorID != 0)
                {
                    Doctor doctor = _unitOfWork.DoctorRepository.GetById(prData.DoctorID);
                    if (doctor != null)
                        prData.DoctorStr = doctor.Name;
                }

                // format the registration type
                prData.TypeStr = prData.TypeStr.Replace("WalkIn", "Walk-In");

                // format the queue code
                if (item.Type == (int)RegistrationTypeEnum.MCU)
                {
                    prData.SortNumberCode = "M-" + string.Format("{0:D3}", item.SortNumber);
                }
                else
                {
                    prData.SortNumberCode = item.Poli1.Code.Trim() + "-" + string.Format("{0:D3}", item.SortNumber);
                }

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            int take = request.PageSize == 0 ? totalRequest : request.PageSize;
            var data = lists.Skip(request.Skip).Take(take).ToList();

            var response = new LoketResponse
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
        public LoketResponse RemoveData(LoketRequest request)
        {
            LoketResponse response = new LoketResponse();
            try
            {
                var registration = _unitOfWork.RegistrationRepository.GetById(request.Data.Id);
                if (registration.ID > 0)
                {
                    registration.RowStatus = -1;
                    registration.ModifiedBy = request.Data.Account.UserCode;
                    registration.ModifiedDate = DateTime.Now;

                    _unitOfWork.RegistrationRepository.Update(registration);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved2, "Registration", registration.ID);
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
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.REGISTRATION, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Process the selected registration queue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LoketResponse ProcessRegistration(LoketRequest request)
        {
            LoketResponse response = new LoketResponse();
            try
            {
                var currentRegistration = _unitOfWork.RegistrationRepository.GetById(request.Data.Id);
                if (currentRegistration != null)
                {
                    // get previous registration with status new
                    var previousRegistrationList = _unitOfWork.RegistrationRepository.Get(x => x.ID != currentRegistration.ID &&
                                        x.TransactionDate.Year == currentRegistration.TransactionDate.Year &&
                                        x.TransactionDate.Month == currentRegistration.TransactionDate.Month &&
                                        x.TransactionDate.Day == currentRegistration.TransactionDate.Day &&
                                        x.ClinicID == currentRegistration.ClinicID &&
                                        x.PoliTo == currentRegistration.PoliTo &&
                                        x.DoctorID == currentRegistration.DoctorID &&
                                        x.Status == 0 &&
                                        x.SortNumber < currentRegistration.SortNumber);

                    foreach (var item in previousRegistrationList)
                    {
                        item.Status = (int)RegistrationStatusEnum.Hold;

                        _unitOfWork.RegistrationRepository.Update(item);

                        _unitOfWork.Save();
                    }

                    currentRegistration.Status = (int)RegistrationStatusEnum.Process;
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Registration", Messages.Patient, currentRegistration.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");
                }
            }
            catch
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }

            return response;
        }

        /// <summary>
        /// Hold the selected registration queue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LoketResponse HoldRegistration(LoketRequest request)
        {
            LoketResponse response = new LoketResponse();
            try
            {
                var currentRegistration = _unitOfWork.RegistrationRepository.GetById(request.Data.Id);
                if (currentRegistration != null)
                {
                    currentRegistration.Status = (int)RegistrationStatusEnum.Hold;
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Registration", Messages.Patient, currentRegistration.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");
                }
            }
            catch
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }

            return response;
        }

        /// <summary>
        /// Finish the selected registration queue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LoketResponse FinishRegistration(LoketRequest request)
        {
            LoketResponse response = new LoketResponse();
            try
            {
                var currentRegistration = _unitOfWork.RegistrationRepository.GetById(request.Data.Id);
                if (currentRegistration != null)
                {
                    currentRegistration.Status = (int)RegistrationStatusEnum.Finish;
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Registration", Messages.Patient, currentRegistration.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");
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
