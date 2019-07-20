using AutoMapper;
using Klinik.Common;
using Klinik.Common.Enumerations;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Letter;
using Klinik.Entities.Loket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.SuratReferensi.SuratLabReferensi
{
    public class RujukanLabHandler : BaseFeatures
    {
        public RujukanLabHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public List<LoketModel> GetFormMedicalIds(int patientId)
        {
            List<LoketModel> lokets = new List<LoketModel>();
            var qry = _unitOfWork.RegistrationRepository.Get(x => x.PatientID == patientId && x.FormMedicalID > 0)
                .Select(x => new
                {
                    x.TransactionDate,
                    x.FormMedicalID
                }).Distinct();

            foreach (var item in qry)
            {
                lokets.Add(new LoketModel
                {
                    FormMedicalID = item.FormMedicalID ?? 0,
                    TransactionDateStr = item.TransactionDate.ToString("yyyy-MM-dd")
                });
            }

            return lokets;

        }
        public RujukanLabResponse SaveSuratRujukanLab(RujukanLabRequest request)
        {
            int _resultAffected = 0;
            request.Data.LetterType = LetterEnum.LabReferenceLetter.ToString();
            request.Data.AutoNumber = GetLatestAutoNoSurat(LetterEnum.LabReferenceLetter.ToString()) + 1;
            request.Data.Year = DateTime.Now.Year;

            var _dob = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);
            if (_dob != null)
            {
                request.Data.PatientAge = CommonUtils.GetPatientAge(_dob.BirthDate);
            }

            var response = new RujukanLabResponse { };

            try
            {
                var cekExist = _unitOfWork.LetterRepository.Get(x => x.FormMedicalID == request.Data.FormMedicalID);
                if (!cekExist.Any())
                {
                    var _entity = Mapper.Map<LabReferenceLetterModel, Letter>(request.Data);
                    _unitOfWork.LetterRepository.Insert(_entity);
                    _resultAffected = _unitOfWork.Save();
                }

                //get detail patient
                var _pasien = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);
                response.Patient.Name = _pasien.Name;
                response.Patient.EmployeeID = _pasien.EmployeeID ?? 0;
               
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
