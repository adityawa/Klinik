using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Loket;
using Klinik.Entities.PreExamine;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Klinik.Features
{
    public class PreExamineHandler : BaseFeatures
    {
        public PreExamineHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public LoketResponse GetListData(LoketRequest request)
        {
            var _loketId = _unitOfWork.PoliRepository.GetFirstOrDefault(x => x.Name == Constants.NameConstant.Loket);
            Expression<Func<QueuePoli, bool>> _serachCriteria = x=>x.PoliFrom==_loketId.ID;

            List<LoketModel> lists = base.GetbaseLoketData(request, _serachCriteria);
            int totalRequest = lists.Count();
            var response = new LoketResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = lists
            };

            return response;
        }

        public PreExamineResponse GetDetailNotPreExamine(PreExamineRequest request)
        {
            var _getdetailQueuePoli = _unitOfWork.RegistrationRepository.GetById(request.Data.LoketData.Id);
            long formMedicalID = _getdetailQueuePoli.FormMedicalID.Value;

            var _preexmodel = new PreExamineModel
            {
                DoctorID = _getdetailQueuePoli.DoctorID ?? 0,
                FormMedicalID = formMedicalID,
                strTransDate = _getdetailQueuePoli.TransactionDate.ToString("dd/MM/yyyy"),
            };

            //cek data preexamine
            var _preExamineData = _unitOfWork.FormPreExamineRepository.GetFirstOrDefault(x => x.FormMedicalID == formMedicalID);
            if (_preExamineData != null)
            {
                _preexmodel = Mapper.Map<FormPreExamine, PreExamineModel>(_preExamineData);
                if (_preexmodel.strTransDate == "" || _preexmodel.strTransDate == null)
                {
                    _preexmodel.strTransDate = _getdetailQueuePoli.TransactionDate.ToString("dd/MM/yyyy");
                }

                if (_preExamineData.MenstrualDate != null)
                {
                    _preexmodel.strMenstrualDate = _preExamineData.MenstrualDate.Value.ToString("dd/MM/yyyy").Contains("1900") ? "" : _preExamineData.MenstrualDate.Value.ToString("dd/MM/yyyy");
                }

                if (_preExamineData.KBDate != null)
                {
                    _preexmodel.strKBDate = _preExamineData.KBDate.Value.ToString("dd/MM/yyyy").Contains("1900") ? "" : _preExamineData.KBDate.Value.ToString("dd/MM/yyyy");
                }
            }

            if (_preexmodel.LoketData == null)
                _preexmodel.LoketData = new LoketModel();

            _preexmodel.LoketData.Id = _getdetailQueuePoli.ID;

            var response = new PreExamineResponse
            {
                Entity = _preexmodel
            };

            return response;
        }

        public PreExamineResponse CreateOrEdit(PreExamineRequest request)
        {
            int resultAffected = 0;
            PreExamineResponse response = new PreExamineResponse();
            try
            {
                var _cekExistById = _unitOfWork.FormPreExamineRepository.GetById(request.Data.Id);
                if (_cekExistById != null)
                {
                    _cekExistById.FormMedicalID = request.Data.FormMedicalID;
                    _cekExistById.TransDate = reformatDate(request.Data.strTransDate);
                    _cekExistById.DoctorID = request.Data.DoctorID;
                    _cekExistById.Temperature = request.Data.Temperature;
                    _cekExistById.Weight = request.Data.Weight;
                    _cekExistById.Height = request.Data.Height;
                    _cekExistById.Respiratory = request.Data.Respitory;
                    _cekExistById.Pulse = request.Data.Pulse;
                    _cekExistById.Systolic = request.Data.Systolic;
                    _cekExistById.Diastolic = request.Data.Diastolic;
                    _cekExistById.Others = request.Data.Others;
                    _cekExistById.RightEye = request.Data.RightEye;
                    _cekExistById.LeftEye = request.Data.LeftEye;
                    _cekExistById.ColorBlind = request.Data.ColorBlind;
                    if (!String.IsNullOrEmpty(request.Data.strMenstrualDate))
                    {
                        _cekExistById.MenstrualDate = reformatDate(request.Data.strMenstrualDate);
                    }

                    if (!String.IsNullOrEmpty(request.Data.strKBDate))
                    {
                        _cekExistById.KBDate = reformatDate(request.Data.strKBDate);
                    }

                    _cekExistById.DailyGlasses = request.Data.DailyGlasses;
                    _cekExistById.ExamineGlasses = request.Data.ExamineGlasses;
                    _cekExistById.ModifiedBy = request.Data.Account.UserName;
                    _cekExistById.ModifiedDate = DateTime.Now;

                    _unitOfWork.FormPreExamineRepository.Update(_cekExistById);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected <= 0)
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Pre examine Data");
                    }
                    else
                    {
                        response.Status = true;
                        response.Message = string.Format(Messages.ObjectHasBeenUpdated2, "Update Pre Examine Data", request.Data.Id);
                    }
                }
                else
                {
                    var entiti = Mapper.Map<PreExamineModel, FormPreExamine>(request.Data);
                    entiti.TransDate = reformatDate(request.Data.strTransDate);
                    if (!String.IsNullOrEmpty(request.Data.strMenstrualDate))
                    {
                        entiti.MenstrualDate = reformatDate(request.Data.strMenstrualDate);
                    }

                    if (!String.IsNullOrEmpty(request.Data.strKBDate))
                    {
                        entiti.KBDate = reformatDate(request.Data.strKBDate);
                    }
                    else
                    {
                        entiti.KBDate = Convert.ToDateTime("1900-01-01");
                    }
                    entiti.CreatedBy = request.Data.Account.UserName;
                    entiti.CreatedDate = DateTime.Now;
                    _unitOfWork.FormPreExamineRepository.Insert(entiti);
                    var _updatePoli = _unitOfWork.RegistrationRepository.GetById(request.Data.LoketData.Id);
                    if (_updatePoli != null)
                    {
                        _updatePoli.IsPreExamine = true;
                        _unitOfWork.RegistrationRepository.Update(_updatePoli);

                    }
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected <= 0)
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Pre examine Data");
                    }
                    else
                    {
                        response.Status = true;
                        response.Message = string.Format(Messages.ObjectPreExamineDataAdded, request.Data.LoketData.PatientName, entiti.ID.ToString());
                    }
                }


            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.FormPreExamine, Constants.Command.EDIT_FORM_PREEXAMINE, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.ADD_NEW_FORM_PREEXAMINE, request.Data.Account, ex);
            }
            return response;
        }
    }
}
