using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class FormExamineHandler : BaseFeatures, IBaseFeatures<FormExamineResponse, FormExamineRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public FormExamineHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit PoliFormExamine
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FormExamineResponse CreateOrEdit(FormExamineRequest request)
        {
            FormExamineResponse response = new FormExamineResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.FormExamineRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<FormExamine, FormExamineModel>(qry);

                        // update data

                        _unitOfWork.FormExamineRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated2, "PoliFormExamine", qry.ID);

                            CommandLog(true, ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PoliFormExamine");

                            CommandLog(false, ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PoliFormExamine");

                        CommandLog(false, ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    List<FormExamineMedicine> formExamineMedicine = new List<FormExamineMedicine>();
                    foreach (var item in request.Data.MedicineDataList)
                    {
                        var medicine = Mapper.Map<FormExamineMedicineModel, FormExamineMedicine>(item);
                        medicine.CreatedBy = request.Data.Account.UserCode;
                        medicine.CreatedDate = DateTime.Now;

                        formExamineMedicine.Add(medicine);
                    }

                    List<FormExamineLab> formExamineLab = new List<FormExamineLab>();
                    foreach (var item in request.Data.LabDataList)
                    {
                        var lab = Mapper.Map<FormExamineLabModel, FormExamineLab>(item);
                        lab.CreatedBy = request.Data.Account.UserCode;
                        lab.CreatedDate = DateTime.Now;

                        formExamineLab.Add(lab);
                    }

                    List<FormExamineService> formExamineService = new List<FormExamineService>();
                    foreach (var item in request.Data.ServiceDataList)
                    {
                        var service = Mapper.Map<FormExamineServiceModel, FormExamineService>(item);
                        service.CreatedBy = request.Data.Account.UserCode;
                        service.CreatedDate = DateTime.Now;

                        formExamineService.Add(service);
                    }

                    // update status the old registration to process
                    QueuePoli oldRegistration = _unitOfWork.RegistrationRepository.GetById(request.Data.LoketData.Id);
                    oldRegistration.Status = (int)RegistrationStatusEnum.Process;
                    oldRegistration.ModifiedBy = request.Data.Account.UserCode;
                    oldRegistration.ModifiedDate = DateTime.Now;

                    _unitOfWork.RegistrationRepository.Update(oldRegistration);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected < 0)
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Registration");
                        CommandLog(false, ClinicEnums.Module.REGISTRATION, Constants.Command.EDIT_REGISTRATION, request.Data.Account, request.Data);

                        return response;
                    }

                    // create a new registration
                    QueuePoli queue = Mapper.Map<LoketModel, QueuePoli>(request.Data.LoketData);
                    queue.ID = 0; // reset                    
                    queue.DoctorID = request.Data.DoctorToID == 0 ? (int?)null : request.Data.DoctorToID;
                    queue.PoliFrom = queue.PoliTo;
                    queue.PoliTo = request.Data.PoliToID;
                    queue.CreatedBy = request.Data.Account.UserCode;
                    queue.CreatedDate = DateTime.Now;
                    queue.TransactionDate = DateTime.Now;
                    queue.Status = (int)RegistrationStatusEnum.New;
                    queue.SortNumber = GenerateSortNumber(request.Data.PoliToID, request.Data.DoctorToID);

                    _unitOfWork.RegistrationRepository.Insert(queue);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected < 0)
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Registration");

                        CommandLog(false, ClinicEnums.Module.REGISTRATION, Constants.Command.ADD_NEW_REGISTRATION, request.Data.Account, request.Data);

                        return response;
                    }

                    FormExamine formExamine = Mapper.Map<FormExamineModel, FormExamine>(request.Data.ExamineData);
                    formExamine.CreatedBy = request.Data.Account.UserCode;
                    formExamine.CreatedDate = DateTime.Now;
                    formExamine.FormExamineMedicines = formExamineMedicine;
                   // formExamine.FormExamineLabs = formExamineLab;

                    // save the form examine data
                    _unitOfWork.FormExamineRepository.Insert(formExamine);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded2, "FormExamine", formExamine.ID);

                        CommandLog(true, ClinicEnums.Module.FORM_EXAMINE, Constants.Command.ADD_NEW_REGISTRATION, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "FormExamine");

                        CommandLog(false, ClinicEnums.Module.FORM_EXAMINE, Constants.Command.ADD_NEW_REGISTRATION, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.ADD_FORM_EXAMINE, request.Data.Account, ex);
            }

            return response;
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

        public FormExamineResponse GetDetail(FormExamineRequest request)
        {
            throw new NotImplementedException();
        }

        public FormExamineResponse GetListData(FormExamineRequest request)
        {
            throw new NotImplementedException();
        }

        public FormExamineResponse RemoveData(FormExamineRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
