using Klinik.Data.DataRepository;
using System;

namespace Klinik.Data
{
    /// <summary>
    /// Interface of Unity of Work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Clinic> ClinicRepository { get; }
        IGenericRepository<Employee> EmployeeRepository { get; }
        IGenericRepository<Organization> OrganizationRepository { get; }
        IGenericRepository<Privilege> PrivilegeRepository { get; }
        IGenericRepository<OrganizationRole> RoleRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<GeneralMaster> MasterRepository { get; }
        IGenericRepository<OrganizationPrivilege> OrgPrivRepository { get; }
        IGenericRepository<RolePrivilege> RolePrivRepository { get; }
        IGenericRepository<UserRole> UserRoleRepository { get; }
        IGenericRepository<Menu> MenuRepository { get; }
        IGenericRepository<PasswordHistory> PasswordHistoryRepository { get; }
        IGenericRepository<EmployeeAssignment> EmployeeAssignmentRepository { get; }
        IGenericRepository<EmployeeStatu> EmployeeStatusRepository { get; }
        IGenericRepository<FamilyRelationship> FamilyRelationshipRepository { get; }
        IGenericRepository<QueuePoli> RegistrationRepository { get; }
        IGenericRepository<Poli> PoliRepository { get; }
        IGenericRepository<PoliClinic> PoliClinicRepository { get; }
        IGenericRepository<Log> LogRepository { get; }
        IGenericRepository<Patient> PatientRepository { get; }
        IGenericRepository<PoliFlowTemplate> PoliFlowTemplateRepository { get; }
        IGenericRepository<Doctor> DoctorRepository { get; }
        IGenericRepository<PoliSchedule> PoliScheduleRepository { get; }
        IGenericRepository<PoliScheduleMaster> PoliScheduleMasterRepository { get; }
        IGenericRepository<PatientClinic> PatientClinicRepository { get; }
        IGenericRepository<FileArchieve> FileArchiveRepository { get; }
        IGenericRepository<City> CityRepository { get; }
        IGenericRepository<FormMedical> FormMedicalRepository { get; }
        IGenericRepository<FormPreExamine> FormPreExamineRepository { get; }
        IGenericRepository<FormExamineAttachment> FormExamineAttachmentRepository { get; }
        IGenericRepository<FormExamineLab> FormExamineLabRepository { get; }
        IGenericRepository<FormExamineMedicine> FormExamineMedicineRepository { get; }
        IGenericRepository<FormExamineService> FormExamineServiceRepository { get; }
        IGenericRepository<FormExamine> FormExamineRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<ProductCategory> ProductCategoryRepository { get; }
        IGenericRepository<ProductMedicine> ProductMedicineRepository { get; }
        IGenericRepository<ProductUnit> ProductUnitRepository { get; }
        IGenericRepository<Medicine> MedicineRepository { get; }
        IGenericRepository<LabItem> LabItemRepository { get; }
        IGenericRepository<LabItemCategory> LabItemCategoryRepository { get; }
        IGenericRepository<Service> ServicesRepository { get; }
        IGenericRepository<PoliService> PoliServicesRepository { get; }
        IGenericRepository<Letter> LetterRepository { get; }
        IGenericRepository<SuratRujukanLabKeluar> SuratRujukanLabKeluarRepository { get; }
        int Save();
    }
}
