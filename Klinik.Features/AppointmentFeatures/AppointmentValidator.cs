using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.AppointmentFeatures
{
    public class AppointmentValidator : BaseFeatures
    {

        private const string ADD_PRIVILEGE_NAME = "ADD_APPOINTMENT";
        public AppointmentValidator(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public AppointmentResponse Validate(AppointmentRequest request)
        {
            var response = new AppointmentResponse();
            bool isHavePrivilege = true;
            if (request.Data == null)
            {
                response.Status = false;
                response.Message = "Request cannot be Null";
            }

            if (request.Data.AppointmentDate == null)
            {
                errorFields.Add("Appointment Date");
            }

            if (request.Data.ClinicID == 0)
                errorFields.Add("Clinic");

            if (request.Data.PoliID == 0)
                errorFields.Add("Poli");

            if (request.Data.DoctorID == 0)
                errorFields.Add("Doctor");

            if (request.Data.PatientID == 0)
                errorFields.Add("Employee");

            if (request.Data.RequirementID == 0)
                errorFields.Add("Necesity");
            if (request.Data.Jam == null)
                errorFields.Add("Time Booking");

            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }

            isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            //validasi jam praktek
            if (request.Data.Jam != null)
            {
                var isExist = _unitOfWork.PoliScheduleRepository.GetFirstOrDefault(x => x.StartDate <= request.Data.Jam && x.EndDate >= request.Data.Jam);
                if (isExist == null)
                {
                    response.Status = false;
                    response.Message = "Time is not Available";
                }
            }

            //validasi is exist with appointmentDate Poli ID Patient ID DoctorID Clinic ID
            var isAppointmentExist = _unitOfWork.AppointmentRepository.GetFirstOrDefault(x => x.ClinicID == request.Data.ClinicID
              && x.PoliID == request.Data.PoliID
              && x.DoctorID == request.Data.DoctorID
              && x.PatientID == request.Data.PatientID
              && x.AppointmentDate == request.Data.AppointmentDate);

            if (isAppointmentExist != null)
            {
                response.Status = false;
                response.Message = $"Appointment is already exist with ID {isAppointmentExist.ID} And Patient Name {isAppointmentExist.Patient.Name}";
            }

            if (response.Status)
            {
                response = new AppointmentHandler(_unitOfWork, _context).SaveAppointment(request);
            }

            return response;
        }

        public AppointmentResponse ValidateBeforeDelete(AppointmentRequest request)
        {
            var response = new AppointmentResponse();
            bool isHavePrivilege = true;
            if (request.Data == null)
            {
                response.Status = false;
                response.Message = "Request cannot be Null";
            }

            if (request.Data.Id == 0)
            {
                errorFields.Add("ID not recognize");
            }
            if (errorFields.Any())
            {
                response.Status = false;
                response.Message = string.Format(Messages.ValidationErrorFields, String.Join(",", errorFields));
            }
            //cek is exist in Registration
            var regExist = _unitOfWork.RegistrationRepository.Get(x => x.AppointmentID == request.Data.Id);
            if (regExist.Count > 0)
            {
                response.Status = false;
                response.Message = "Appointment Id was exist in Registration";
            }

            isHavePrivilege = IsHaveAuthorization(ADD_PRIVILEGE_NAME, request.Data.Account.Privileges.PrivilegeIDs);
            if (!isHavePrivilege)
            {
                response.Status = false;
                response.Message = Messages.UnauthorizedAccess;
            }

            if (response.Status)
            {
                response = new AppointmentHandler(_unitOfWork).RemoveAppointment(request.Data.Id);
            }
            return response;
        }
    }
}
