using AutoMapper;
using Klinik.Common;
using Klinik.Common.Enumerations;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Letter;
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Klinik.Features.SuratReferensi.SuratPersetujuanTindakan
{
    public class PersetujuanTindakanHandler : BaseFeatures
    {
        public PersetujuanTindakanHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public EmployeeModel GetPenanggungJawab(long employeeId)
        {
            var employee = _unitOfWork.EmployeeRepository.GetById(employeeId);
            var employeeData = Mapper.Map<Employee, EmployeeModel>(employee);
            return employeeData;
        }

        public PersetujuanTindakanResponse GetPatientPenjaminData(long patientId)
        {
            var response = new PersetujuanTindakanResponse { };
            var patient = _unitOfWork.PatientRepository.GetById(patientId);
            var patientData = Mapper.Map<Patient, PatientModel>(patient);
            response.Entity = new PersetujuanTindakanModel();
            response.Entity.EmployeeData = new EmployeeModel();
            response.Entity.PatientData = patientData;
            var umurPasien = CommonUtils.GetPatientAge(response.Entity.PatientData.BirthDate);
            var SAPNoPatient = new PersetujuanTindakanHandler(_unitOfWork).GetSAPBasedEmpId(response.Entity.PatientData.EmployeeID);
            response.Entity.SAPPatient = SAPNoPatient;
            response.Entity.UmurPatient = umurPasien;

            //get penjamin
            var employeePenjamin = new EmployeeModel { };
            if (patientData.EmployeeID != 0)
            {
                var qryPenjamin = _unitOfWork.EmployeeRepository.GetById(patientData.EmployeeID);
                employeePenjamin = Mapper.Map<Employee, EmployeeModel>(qryPenjamin);
                response.Entity.EmployeeData = employeePenjamin;
                response.Entity.UmurPenjamin = CommonUtils.GetPatientAge(employeePenjamin.Birthdate);
            }
            return response;

        }

        public PersetujuanTindakanResponse SavePersetujuanTindakan(PersetujuanTindakanRequest request)
        {
            int _resultAffected = 0;
            request.Data.LetterType = LetterEnum.MedicalAcceptanceLetter.ToString();
            request.Data.AutoNumber = GetLatestAutoNoSurat(LetterEnum.MedicalAcceptanceLetter.ToString()) + 1;
            request.Data.Year = DateTime.Now.Year;

            var response = new PersetujuanTindakanResponse { };

            try
            {

                var _entity = Mapper.Map<PersetujuanTindakanModel, Letter>(request.Data);
                _entity.ResponsiblePerson = JsonConvert.SerializeObject(request.Data.PenjaminData);
                _entity.CreatedBy = request.Data.Account.UserName;
                _entity.CreatedDate = DateTime.Now;
                _unitOfWork.LetterRepository.Insert(_entity);
                _resultAffected = _unitOfWork.Save();
                var letterId = _entity.Id;

                //get detail patient
                var _pasien = _unitOfWork.PatientRepository.GetById(request.Data.ForPatient);

                response.Entity = new PersetujuanTindakanModel();
                response.Entity.PatientData = new PatientModel();
                response.Entity.PatientData = Mapper.Map<Patient, PatientModel>(_pasien);


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

        public PersetujuanTindakanResponse GetDetailPersetujuanTindakan(long idLetter)
        {
            var _qryLetter = _unitOfWork.LetterRepository.GetById(idLetter);
            var response = new PersetujuanTindakanResponse();
            if (response.Entity == null)
                response.Entity = new PersetujuanTindakanModel();
            if (_qryLetter != null)
            {
                response.Entity.Action = _qryLetter.Action;
                response.Entity.PenjaminData = JsonConvert.DeserializeObject<PenjaminModel>(_qryLetter.ResponsiblePerson);
                response.Entity.Treatment = _qryLetter.Treatment;
                response.Entity.Decision = _qryLetter.Decision;
                response.Entity.NoSurat = $"{ _qryLetter.AutoNumber}/Klinik/{DateTime.Now.Year.ToString()}/{DateTime.Now.Month.ToString()}";
            }

            var _qryPatient = _unitOfWork.PatientRepository.GetById(_qryLetter.ForPatient);
            if (_qryPatient != null)
            {
                response.Entity.PatientData = Mapper.Map<Patient, PatientModel>(_qryPatient);
                var umurPasien = CommonUtils.GetPatientAge(response.Entity.PatientData.BirthDate);
                var SAPNoPatient = new PersetujuanTindakanHandler(_unitOfWork).GetSAPBasedEmpId(response.Entity.PatientData.EmployeeID);
                response.Entity.SAPPatient = SAPNoPatient;
                response.Entity.UmurPatient = umurPasien;
            }

            return response;
        }
        public string GetEmployeeSAPno(long idEmp)
        {
            return GetSAPBasedEmpId(idEmp);
        }

    }
}
