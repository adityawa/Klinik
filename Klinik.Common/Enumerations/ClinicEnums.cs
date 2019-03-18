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
            DoctorType
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
            REGISTRATION
        }

        public enum Action
        {
            Add,
            Edit,
            DELETE
        }
    }
}