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
            Day,
            PoliType
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
            MASTER_ORGANIZATION_PRIVILEGE,
            MASTER_CLINIC,
            MASTER_EMPLOYEE,
            MASTER_PRIVILEGE,
            MASTER_ROLE,
            MASTER_ROLE_PRIVILEGE,
            MASTER_USER,
            MASTER_USER_ROLE,
            MASTER_DOCTOR,
            EMPLOYEE_ASSIGNMENT,
            REGISTRATION,
            POLI_SCHEDULE,
            POLI_SCHEDULE_MASTER,
            MASTER_PARAMEDIC,
            Patient,
            FormPreExamine,
            MASTER_POLI,
            MASTER_POLI_CLINIC,
            FORM_EXAMINE,
            MASTER_PRODUCT,
            MASTER_PRODUCT_CATEGORY,
            MASTER_PRODUCT_MEDICINE,
            MASTER_PRODUCT_UNIT,
            MASTER_LAB_ITEM,
            MASTER_LAB_ITEM_CATEGORY,
            MASTER_MEDICINE,
            MASTER_MENU,
            MASTER_SERVICE,
            MASTER_POLI_SERVICE,
            MASTER_GUDANG
        }

        public enum Action
        {
            Add,
            Edit,
            DELETE,
            Process,
            Hold,
            Finish,
            Reschedule
        }

        public enum SourceTable
        {
            PATIENT
        }

        public enum polyType
        {
            Type1 = 1,
            Type2 = 2,
            Type3 = 3,
            Type4 = 4
        }

    }
}