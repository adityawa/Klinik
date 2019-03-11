namespace Klinik.Common
{
    /// <summary>
    /// Class contains collection of enumeration
    /// </summary>
    public class ClinicEnums
    {
        public enum enumStatus
        {
            SUCCESS,
            ERROR
        }

        public enum enumMasterTypes
        {
            EmploymentType,
            Department,
            City,
            ClinicType
        }

        public enum enumAuthResult
        {
            SUCCESS,
            UNRECOGNIZED
        }

        public enum enumModule
        {
          LOGIN,
          MASTER_ORGANIZATION
        }

        public enum enumAction
        {
            Add,
            Edit,
            DELETE
        }
    }
}