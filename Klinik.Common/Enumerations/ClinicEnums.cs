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
            Department
        }

        public enum enumAuthResult
        {
            SUCCESS,
            UNRECOGNIZED
        }

        public enum enumAction
        {
            Add,
            Edit,
            DELETE
        }
    }
}