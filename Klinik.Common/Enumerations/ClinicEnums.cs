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
            ERROR
        }

        public enum MasterTypes
        {
            EmploymentType,
            Department,
            City,
            ClinicType
        }

        public enum AuthResult
        {
            SUCCESS,
            UNRECOGNIZED
        }

        public enum Module
        {
            LOGIN,
            MASTER_ORGANIZATION
        }

        public enum Action
        {
            Add,
            Edit,
            DELETE
        }
    }
}