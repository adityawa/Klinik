using Klinik.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.Data.DataRepository;
using AutoMapper;
using Klinik.Entities.Form;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using Klinik.Entities.Letter;
using Klinik.Common.Enumerations;

namespace Klinik.Features.SuratReferensi.SuratIzinSakit
{
    public class SuratSakitHandler:BaseFeatures
    {

        public SuratSakitHandler(IUnitOfWork unitOfWork, KlinikDBEntities context=null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public SuratSakitResponse SaveSuratSakit(SuratSakitRequest request)
        {
            int resultAffected = 0;
            var response = new SuratSakitResponse();
            request.Data.LetterType = LetterEnum.SuratIzinSakit.ToString();
            request.Data.AutoNumber = GetLatestAutoNoSurat(LetterEnum.SuratIzinSakit.ToString(), request.Data.Account.ClinicID) + 1;
            request.Data.Year = DateTime.Now.Year;
            try
            {
                var entity = Mapper.Map<SuratIzinSakitModel, Letter>(request.Data);
                entity.CreatedBy = request.Data.Account.UserName;
                entity.CreatedDate = DateTime.Now;
                _unitOfWork.LetterRepository.Insert(entity);

                resultAffected = _unitOfWork.Save();

                if (resultAffected > 0)
                {
                    response.Status = true;
                   
                }
                else
                {
                    response.Status = false;
                }
            }
            catch(Exception )
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }
            return response;
        }
        public SuratSakitResponse GetSuratIzinSakitData(long formMedID)
        {
            var response = new SuratSakitResponse();
            var formExamineData = _unitOfWork.FormExamineRepository.GetFirstOrDefault(x => x.FormMedicalID == formMedID);
            var letterData = _unitOfWork.LetterRepository.Get(x => x.FormMedicalID == formMedID, orderBy: q => q.OrderByDescending(x => x.CreatedDate)).FirstOrDefault();
            if (formExamineData != null)
            {
                long _doctorId = formExamineData.DoctorID ?? 0;
                string _dokterName = _unitOfWork.DoctorRepository.GetFirstOrDefault(x => x.ID == _doctorId).Name;
                if (formExamineData.NeedSuratSakit == true)
                {
                    //get data patient
                    var loketData = _unitOfWork.RegistrationRepository.GetFirstOrDefault(x=>x.FormMedicalID==formMedID);
                    if (loketData != null)
                    {
                        var qryPatient = _unitOfWork.PatientRepository.GetById(loketData.PatientID);
                        response.Entity = new Entities.Letter.SuratIzinSakitModel
                        {
                            NamaDokter = _dokterName,
                            ExamineData = Mapper.Map<FormExamine, FormExamineModel>(formExamineData),
                            patientData = Mapper.Map<Patient, PatientModel>(qryPatient),
                            NoSurat = letterData==null?"": $"{letterData.AutoNumber}/SKIS/{DateTime.Now.Month}/{DateTime.Now.Year}",
                            strSelesaiIstirahat = formExamineData.Sampai.Value.ToString("dd/MM/yyyy"),
                            strStartIstirahat = formExamineData.TransDate.Value.ToString("dd/MM/yyyy"),
                            Pekerjaan=letterData==null?"":letterData.Pekerjaan,
                        
                        };
                    }
                }
            }
            return response;
        }
    }
}
