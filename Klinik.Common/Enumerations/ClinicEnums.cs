namespace Klinik.Common
{
    /// <summary>
    /// Class contains collection of enumeration
    /// </summary>
    public class ClinicEnums
    {
        public enum Status
        {
            SUCCESS,
            ERROR,
            UNRECOGNIZED
        }

        public enum MasterTypes
        {
            EmploymentType,
            Department,
            City,
            ClinicType,
            DoctorType,
            PoliScheduleStatus,
            ParamedicType,
            Day
        }

        public enum AuthResult
        {
            SUCCESS,
            UNRECOGNIZED
        }

        public enum Module
        {
            LOGIN,
            MASTER_ORGANIZATION,
            MASTER_CLINIC,
            MASTER_EMPLOYEE,
            MASTER_PRIVILEGE,
            MASTER_ROLE,
            MASTER_USER,
            MASTER_DOCTOR,
            EMPLOYEE_ASSIGNMENT,
            REGISTRATION,
            POLI_SCHEDULE,
            MASTER_PARAMEDIC
        }

        public enum Action
        {
            Add,
            Edit,
            DELETE,
            Process,
            Hold,
            Finish
        }
    }
}