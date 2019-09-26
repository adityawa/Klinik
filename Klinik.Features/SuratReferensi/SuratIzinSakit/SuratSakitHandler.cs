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

namespace Klinik.Features.SuratReferensi.SuratIzinSakit
{
    public class SuratSakitHandler:BaseFeatures
    {

        public SuratSakitHandler(IUnitOfWork unitOfWork, KlinikDBEntities context=null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public SuratSakitResponse GetSuratIzinSakitData(long formMedID)
        {
            var response = new SuratSakitResponse();
            var formExamineData = _unitOfWork.FormExamineRepository.GetFirstOrDefault(x => x.FormMedicalID == formMedID);
         
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
                            NoSurat = "",
                            strSelesaiIstirahat = formExamineData.Sampai.Value.ToString("dd/MM/yyyy"),
                            strStartIstirahat = formExamineData.TransDate.Value.ToString("dd/MM/yyyy")
                        
                        };
                    }
                }
            }
            return response;
        }
    }
}
