using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.Entities.Letter;
using Klinik.Entities.MasterData;
using Klinik.Entities.PreExamine;
using AutoMapper;
using Klinik.Common.Enumerations;

namespace Klinik.Features.SuratReferensi.SuratBadanSehat
{
    public class HealthBodyHandler:BaseFeatures
    {
        public HealthBodyHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public HealthBodyResponse GetPatientAndPreExamineData(HealthBodyRequest request)
        {
            var response = new HealthBodyResponse();
            try
            {
                if (request.Data.ForPatient == 0)
                    request.Data.ForPatient = _unitOfWork.RegistrationRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault().PatientID;
                var _patientData = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);
                var _preExamineData = _unitOfWork.FormPreExamineRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault();
                var _letterData = _unitOfWork.LetterRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID).FirstOrDefault();
                if (response.Entity == null)
                    response.Entity = new HealthBodyLetterModel();
                response.Entity.PatientData = new PatientModel();
                response.Entity.PreExamineData = new PreExamineModel();
                response.Entity.Keperluan = _letterData==null?"":_letterData.Keperluan;
                response.Entity.Pekerjaan = _letterData == null ? "" : _letterData.Pekerjaan;
                response.Entity.Decision = _letterData==null?"":_letterData.Decision;
                response.Entity.NoSurat =_letterData==null?"": $"{_letterData.AutoNumber}/SKKBS/{DateTime.Now.Month}/{DateTime.Now.Year}";
                response.Entity.PatientData = Mapper.Map<Patient, PatientModel>(_patientData);
                response.Entity.PreExamineData = Mapper.Map<FormPreExamine, PreExamineModel>(_preExamineData);
                response.Status = true;
            }
            catch(Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }
           
            return response;
        }

        public HealthBodyResponse SaveSuratBadanSehat(HealthBodyRequest request)
        {
            int _resultAffected = 0;
            request.Data.LetterType = LetterEnum.HealthBodyLetter.ToString();
            request.Data.AutoNumber = GetLatestAutoNoSurat(LetterEnum.HealthBodyLetter.ToString()) + 1;
            request.Data.Year = DateTime.Now.Year;
            
            var response = new HealthBodyResponse { };

            try
            {
                var cekExist = _unitOfWork.LetterRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID && x.LetterType==request.Data.LetterType && x.ForPatient==request.Data.ForPatient);
                if (!cekExist.Any())
                {
                    var _entity = Mapper.Map<HealthBodyLetterModel, Letter>(request.Data);
                    _entity.CreatedBy = request.Data.Account.UserName;
                    _entity.CreatedDate = DateTime.Now;
                    _unitOfWork.LetterRepository.Insert(_entity);
                    _resultAffected = _unitOfWork.Save();
                }

                //get detail patient
                var _pasien = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);
                response.PatientData = Mapper.Map<Patient, PatientModel>(_pasien);
                if (response.Entity == null)
                    response.Entity = new HealthBodyLetterModel();
             
                response.Entity.FormMedicalID = request.Data.FormMedicalID;
                response.Status = true;

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message.ToString();
            }
            return response;
        }
    }
}
