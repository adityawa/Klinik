using AutoMapper;
using Klinik.Common.Enumerations;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Letter;
using Klinik.Entities.MasterData;
using Klinik.Entities.PreExamine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratRujukanBerobat
{
    public class RujukanBerobatHandler : BaseFeatures
    {
        public RujukanBerobatHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public RujukanBerobatResponse GetDetailDataForRujukan(RujukanBerobatRequest request)
        {
            var response = new RujukanBerobatResponse();
            try
            {
                //get data patient
                var _patientData = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);
                //get data form examine
                var _ExamineData = _unitOfWork.FormExamineRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault();
                //get data PreExamine
                var _preExamineData = _unitOfWork.FormPreExamineRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault();

                response.Entity = new RujukanBerobatModel();
                response.Entity.PatientData = new PatientModel();
                response.Entity.PreExamineData = new PreExamineModel();
                response.Entity.FormExamineData = new FormExamineModel();
                if (_patientData != null)
                {

                    response.Entity.PatientData = Mapper.Map<Patient, PatientModel>(_patientData);
                    if (response.Entity.PatientData.familyRelationshipID != 0)
                        response.Entity.PatientData.familyRelationshipDesc = _unitOfWork.FamilyRelationshipRepository.GetById(response.Entity.PatientData.familyRelationshipID).Code;
                }
                if (_ExamineData != null)
                {

                    response.Entity.FormExamineData = Mapper.Map<FormExamine, FormExamineModel>(_ExamineData);
                }
                if (_preExamineData != null)
                {

                    response.Entity.PreExamineData = Mapper.Map<FormPreExamine, PreExamineModel>(_preExamineData);
                }

                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }


            return response;
        }

        public RujukanBerobatResponse SaveSuratRujukanBerobat(RujukanBerobatRequest request)
        {
            int _resultAffected = 0;
            request.Data.LetterType = LetterEnum.MedicalReferenceLetter.ToString();
            request.Data.AutoNumber = GetLatestAutoNoSurat(LetterEnum.MedicalReferenceLetter.ToString(), request.Data.Account.ClinicID) + 1;
            request.Data.Year = DateTime.Now.Year;

            var response = new RujukanBerobatResponse { };

            try
            {

                var _entity = Mapper.Map<RujukanBerobatModel, Letter>(request.Data);
                _entity.OtherInfo = JsonConvert.SerializeObject(request.Data.InfoRujukanData);
                _entity.CreatedBy = request.Data.Account.UserName;
                _entity.CreatedDate = DateTime.Now;
                _unitOfWork.LetterRepository.Insert(_entity);
                _resultAffected = _unitOfWork.Save();
                var letterId = _entity.Id;

                //get detail patient
                var _pasien = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);
                var _ExamineData = _unitOfWork.FormExamineRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault();
                var _preExamineData = _unitOfWork.FormPreExamineRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault();

                response.Entity = new RujukanBerobatModel();
                response.Entity.PatientData = new PatientModel();
                response.Entity.PreExamineData = new PreExamineModel();
                response.Entity.FormExamineData = new FormExamineModel();

                if (_pasien != null)
                    response.Entity.PatientData = Mapper.Map<Patient, PatientModel>(_pasien);
                if (_ExamineData != null)
                    response.Entity.FormExamineData = Mapper.Map<FormExamine, FormExamineModel>(_ExamineData);
                if (_preExamineData != null)
                    response.Entity.PreExamineData = Mapper.Map<FormPreExamine, PreExamineModel>(_preExamineData);

                response.Entity.NoSurat = $"{_entity.AutoNumber}/klinik/{DateTime.Now.Year}/{DateTime.Now.Month}";
                response.Entity.FormMedicalID = _entity.FormMedicalID ?? 0;
                response.Entity.Id = letterId;
                response.Status = true;

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message.ToString();
            }
            return response;
        }

        public RujukanBerobatResponse PreparePrintSuratRujukanBerobat(long LetterId)
        {
            long formMedicalId = 0;
            var response = new RujukanBerobatResponse();
            try
            {
                var _letterData = _unitOfWork.LetterRepository.GetById(LetterId);
                if (_letterData != null)
                    formMedicalId = _letterData.FormMedicalID ?? 0;
                //get data patient
                var _patientData = _unitOfWork.PatientRepository.GetById(_letterData.ForPatient??0);
                //get data form examine
                var _ExamineData = _unitOfWork.FormExamineRepository.Get(x => x.FormMedicalID == formMedicalId).FirstOrDefault();
                //get data PreExamine
                var _preExamineData = _unitOfWork.FormPreExamineRepository.Get(x => x.FormMedicalID == formMedicalId).FirstOrDefault();

                response.Entity = new RujukanBerobatModel();
                response.Entity.FormMedicalID = formMedicalId;
                response.Entity.Perusahaan = _letterData.Pekerjaan;
                response.Entity.PatientData = new PatientModel();
                response.Entity.InfoRujukanData = JsonConvert.DeserializeObject<InfoRujukan>(_letterData.OtherInfo);
                response.Entity.PreExamineData = new PreExamineModel();
                response.Entity.FormExamineData = new FormExamineModel();
                if (_patientData != null)
                {

                    response.Entity.PatientData = Mapper.Map<Patient, PatientModel>(_patientData);
                    if (response.Entity.PatientData.familyRelationshipID != 0)
                        response.Entity.PatientData.familyRelationshipDesc = _unitOfWork.FamilyRelationshipRepository.GetById(response.Entity.PatientData.familyRelationshipID).Code;
                }
                if (_ExamineData != null)
                {

                    response.Entity.FormExamineData = Mapper.Map<FormExamine, FormExamineModel>(_ExamineData);
                }
                if (_preExamineData != null)
                {

                    response.Entity.PreExamineData = Mapper.Map<FormPreExamine, PreExamineModel>(_preExamineData);
                }

                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }


            return response;
        }
    }
}
