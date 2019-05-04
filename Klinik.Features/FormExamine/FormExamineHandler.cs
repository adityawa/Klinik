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
        /// <param name="context"></param>
        public FormExamineHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        /// <summary>
        /// Create or edit PoliFormExamine
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FormExamineResponse CreateOrEdit(FormExamineRequest request)
        {
            FormExamineResponse response = new FormExamineResponse();

            if (request.Data.Id > 0)
            {
                try
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
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = Messages.GeneralError;

                    if (request.Data != null && request.Data.Id > 0)
                        ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, ex);
                    else
                        ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.ADD_FORM_EXAMINE, request.Data.Account, ex);
                }
            }
            else
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        List<FormExamineMedicine> formExamineMedicine = new List<FormExamineMedicine>();
                        foreach (var item in request.Data.MedicineDataList)
                        {
                            var medicine = Mapper.Map<FormExamineMedicineModel, FormExamineMedicine>(item);
                            medicine.CreatedBy = request.Data.Account.UserCode;
                            medicine.CreatedDate = DateTime.Now;

                            formExamineMedicine.Add(medicine);
                        }

                        foreach (var item in request.Data.LabDataList)
                        {
                            var lab = Mapper.Map<FormExamineLabModel, FormExamineLab>(item);
                            lab.FormMedicalID = request.Data.LoketData.FormMedicalID;
                            lab.CreatedBy = request.Data.Account.UserCode;
                            lab.CreatedDate = DateTime.Now;

                            _context.FormExamineLabs.Add(lab);
                            _context.SaveChanges();
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
                        QueuePoli oldRegistration = _context.QueuePolis.FirstOrDefault(x => x.ID == request.Data.LoketData.Id);
                        oldRegistration.Status = (int)RegistrationStatusEnum.Process;

                        _context.SaveChanges();

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

                        _context.QueuePolis.Add(queue);
                        _context.SaveChanges();

                        FormExamine formExamine = Mapper.Map<FormExamineModel, FormExamine>(request.Data.ExamineData);
                        formExamine.CreatedBy = request.Data.Account.UserCode;
                        formExamine.CreatedDate = DateTime.Now;
                        formExamine.FormExamineMedicines = formExamineMedicine;
                        formExamine.FormExamineServices = formExamineService;

                        // save the form examine data
                        _context.FormExamines.Add(formExamine);
                        _context.SaveChanges();

                        transaction.Commit();

                        response.Message = Messages.DataSaved;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        response.Status = false;
                        response.Message = Messages.GeneralError;

                        if (request.Data != null && request.Data.Id > 0)
                            ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.EDIT_FORM_EXAMINE, request.Data.Account, ex);
                        else
                            ErrorLog(ClinicEnums.Module.FORM_EXAMINE, Constants.Command.ADD_FORM_EXAMINE, request.Data.Account, ex);
                    }
                }
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
