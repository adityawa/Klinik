﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Klinik.Data.DataRepository
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class KlinikDBEntities : DbContext
    {
        public KlinikDBEntities()
            : base("name=KlinikDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AppConfig> AppConfigs { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<DeliveryOrder> DeliveryOrders { get; set; }
        public virtual DbSet<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }
        public virtual DbSet<DeliveryOrderPusat> DeliveryOrderPusats { get; set; }
        public virtual DbSet<DeliveryOrderPusatDetail> DeliveryOrderPusatDetails { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<DoctorClinic> DoctorClinics { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeAssignment> EmployeeAssignments { get; set; }
        public virtual DbSet<EmployeeStatu> EmployeeStatus { get; set; }
        public virtual DbSet<FamilyRelationship> FamilyRelationships { get; set; }
        public virtual DbSet<FileArchieve> FileArchieves { get; set; }
        public virtual DbSet<FormExamine> FormExamines { get; set; }
        public virtual DbSet<FormExamineAttachment> FormExamineAttachments { get; set; }
        public virtual DbSet<FormExamineLab> FormExamineLabs { get; set; }
        public virtual DbSet<FormExamineMedicine> FormExamineMedicines { get; set; }
        public virtual DbSet<FormExamineMedicineDetail> FormExamineMedicineDetails { get; set; }
        public virtual DbSet<FormExamineService> FormExamineServices { get; set; }
        public virtual DbSet<FormMedical> FormMedicals { get; set; }
        public virtual DbSet<FormPreExamine> FormPreExamines { get; set; }
        public virtual DbSet<HistoryProductInGudang> HistoryProductInGudangs { get; set; }
        public virtual DbSet<ICDTheme> ICDThemes { get; set; }
        public virtual DbSet<LabItem> LabItems { get; set; }
        public virtual DbSet<LabItemCategory> LabItemCategories { get; set; }
        public virtual DbSet<Letter> Letters { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<MCUPackage> MCUPackages { get; set; }
        public virtual DbSet<Medicine> Medicines { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationPrivilege> OrganizationPrivileges { get; set; }
        public virtual DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public virtual DbSet<PanggilanPoli> PanggilanPolis { get; set; }
        public virtual DbSet<PasswordHistory> PasswordHistories { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientAge> PatientAges { get; set; }
        public virtual DbSet<PatientClinic> PatientClinics { get; set; }
        public virtual DbSet<Poli> Polis { get; set; }
        public virtual DbSet<PoliClinic> PoliClinics { get; set; }
        public virtual DbSet<PoliFlowTemplate> PoliFlowTemplates { get; set; }
        public virtual DbSet<PoliSchedule> PoliSchedules { get; set; }
        public virtual DbSet<PoliScheduleMaster> PoliScheduleMasters { get; set; }
        public virtual DbSet<PoliService> PoliServices { get; set; }
        public virtual DbSet<Privilege> Privileges { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductMedicine> ProductMedicines { get; set; }
        public virtual DbSet<ProductUnit> ProductUnits { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<PurchaseOrderPusat> PurchaseOrderPusats { get; set; }
        public virtual DbSet<PurchaseOrderPusatDetail> PurchaseOrderPusatDetails { get; set; }
        public virtual DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public virtual DbSet<PurchaseRequestDetail> PurchaseRequestDetails { get; set; }
        public virtual DbSet<PurchaseRequestPusat> PurchaseRequestPusats { get; set; }
        public virtual DbSet<PurchaseRequestPusatDetail> PurchaseRequestPusatDetails { get; set; }
        public virtual DbSet<QueuePoli> QueuePolis { get; set; }
        public virtual DbSet<RolePrivilege> RolePrivileges { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<stok> stoks { get; set; }
        public virtual DbSet<stok_bulanan> stok_bulanan { get; set; }
        public virtual DbSet<stok1> stoks1 { get; set; }
        public virtual DbSet<substitute> substitutes { get; set; }
        public virtual DbSet<SuratRujukanLabKeluar> SuratRujukanLabKeluars { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<PurchaseRequestConfig> PurchaseRequestConfigs { get; set; }
        public virtual DbSet<MCURegistrationInterface> MCURegistrationInterfaces { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<FormExamineICDInfo> FormExamineICDInfoes { get; set; }
        public virtual DbSet<GeneralMaster> GeneralMasters { get; set; }
        public virtual DbSet<LookupCategory> LookupCategories { get; set; }
        public virtual DbSet<Clinic> Clinics { get; set; }
        public virtual DbSet<Gudang> Gudangs { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductInGudang> ProductInGudangs { get; set; }
        public virtual DbSet<VendorProduct> VendorProducts { get; set; }
    
        [DbFunction("KlinikDBEntities1", "fusp_registrations_get_by_status")]
        public virtual IQueryable<fusp_registrations_get_by_status_Result> fusp_registrations_get_by_status(string documentStatus, string isTransferred)
        {
            var documentStatusParameter = documentStatus != null ?
                new ObjectParameter("DocumentStatus", documentStatus) :
                new ObjectParameter("DocumentStatus", typeof(string));
    
            var isTransferredParameter = isTransferred != null ?
                new ObjectParameter("IsTransferred", isTransferred) :
                new ObjectParameter("IsTransferred", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fusp_registrations_get_by_status_Result>("[KlinikDBEntities1].[fusp_registrations_get_by_status](@DocumentStatus, @IsTransferred)", documentStatusParameter, isTransferredParameter);
        }
    
        public virtual int SP_EmployeeSync(Nullable<System.DateTime> lastUpdateTime, Nullable<int> rangeMinute, string lastupdateby)
        {
            var lastUpdateTimeParameter = lastUpdateTime.HasValue ?
                new ObjectParameter("LastUpdateTime", lastUpdateTime) :
                new ObjectParameter("LastUpdateTime", typeof(System.DateTime));
    
            var rangeMinuteParameter = rangeMinute.HasValue ?
                new ObjectParameter("RangeMinute", rangeMinute) :
                new ObjectParameter("RangeMinute", typeof(int));
    
            var lastupdatebyParameter = lastupdateby != null ?
                new ObjectParameter("lastupdateby", lastupdateby) :
                new ObjectParameter("lastupdateby", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_EmployeeSync", lastUpdateTimeParameter, rangeMinuteParameter, lastupdatebyParameter);
        }
    
        public virtual ObjectResult<string> usp_MCU_registration()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("usp_MCU_registration");
        }
    
        public virtual ObjectResult<usp_registrations_get_by_status_Result> usp_registrations_get_by_status(string documentStatus, string isTransferred)
        {
            var documentStatusParameter = documentStatus != null ?
                new ObjectParameter("DocumentStatus", documentStatus) :
                new ObjectParameter("DocumentStatus", typeof(string));
    
            var isTransferredParameter = isTransferred != null ?
                new ObjectParameter("IsTransferred", isTransferred) :
                new ObjectParameter("IsTransferred", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_registrations_get_by_status_Result>("usp_registrations_get_by_status", documentStatusParameter, isTransferredParameter);
        }
    
        public virtual int usp_registrations_update_by_regnumber(string regNumber)
        {
            var regNumberParameter = regNumber != null ?
                new ObjectParameter("RegNumber", regNumber) :
                new ObjectParameter("RegNumber", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_registrations_update_by_regnumber", regNumberParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> SP_EmployeeInsert(string empID, string empName, Nullable<System.DateTime> birthDate, string gender, string empType, string kTPNumber, string hPNumber, string email, string lastEmpID, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, string department, string businessUnit, string region, string grade, string empStatus, Nullable<System.DateTime> lastUpdateTime, string lastupdateby)
        {
            var empIDParameter = empID != null ?
                new ObjectParameter("EmpID", empID) :
                new ObjectParameter("EmpID", typeof(string));
    
            var empNameParameter = empName != null ?
                new ObjectParameter("EmpName", empName) :
                new ObjectParameter("EmpName", typeof(string));
    
            var birthDateParameter = birthDate.HasValue ?
                new ObjectParameter("BirthDate", birthDate) :
                new ObjectParameter("BirthDate", typeof(System.DateTime));
    
            var genderParameter = gender != null ?
                new ObjectParameter("Gender", gender) :
                new ObjectParameter("Gender", typeof(string));
    
            var empTypeParameter = empType != null ?
                new ObjectParameter("EmpType", empType) :
                new ObjectParameter("EmpType", typeof(string));
    
            var kTPNumberParameter = kTPNumber != null ?
                new ObjectParameter("KTPNumber", kTPNumber) :
                new ObjectParameter("KTPNumber", typeof(string));
    
            var hPNumberParameter = hPNumber != null ?
                new ObjectParameter("HPNumber", hPNumber) :
                new ObjectParameter("HPNumber", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("Email", email) :
                new ObjectParameter("Email", typeof(string));
    
            var lastEmpIDParameter = lastEmpID != null ?
                new ObjectParameter("LastEmpID", lastEmpID) :
                new ObjectParameter("LastEmpID", typeof(string));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var departmentParameter = department != null ?
                new ObjectParameter("Department", department) :
                new ObjectParameter("Department", typeof(string));
    
            var businessUnitParameter = businessUnit != null ?
                new ObjectParameter("BusinessUnit", businessUnit) :
                new ObjectParameter("BusinessUnit", typeof(string));
    
            var regionParameter = region != null ?
                new ObjectParameter("Region", region) :
                new ObjectParameter("Region", typeof(string));
    
            var gradeParameter = grade != null ?
                new ObjectParameter("Grade", grade) :
                new ObjectParameter("Grade", typeof(string));
    
            var empStatusParameter = empStatus != null ?
                new ObjectParameter("EmpStatus", empStatus) :
                new ObjectParameter("EmpStatus", typeof(string));
    
            var lastUpdateTimeParameter = lastUpdateTime.HasValue ?
                new ObjectParameter("LastUpdateTime", lastUpdateTime) :
                new ObjectParameter("LastUpdateTime", typeof(System.DateTime));
    
            var lastupdatebyParameter = lastupdateby != null ?
                new ObjectParameter("lastupdateby", lastupdateby) :
                new ObjectParameter("lastupdateby", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("SP_EmployeeInsert", empIDParameter, empNameParameter, birthDateParameter, genderParameter, empTypeParameter, kTPNumberParameter, hPNumberParameter, emailParameter, lastEmpIDParameter, startDateParameter, endDateParameter, departmentParameter, businessUnitParameter, regionParameter, gradeParameter, empStatusParameter, lastUpdateTimeParameter, lastupdatebyParameter);
        }
    
        public virtual int SP_GeneratePoliSchedule(Nullable<System.DateTime> startDate, Nullable<int> range, string lastupdateby)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var rangeParameter = range.HasValue ?
                new ObjectParameter("Range", range) :
                new ObjectParameter("Range", typeof(int));
    
            var lastupdatebyParameter = lastupdateby != null ?
                new ObjectParameter("lastupdateby", lastupdateby) :
                new ObjectParameter("lastupdateby", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GeneratePoliSchedule", startDateParameter, rangeParameter, lastupdatebyParameter);
        }
    }
}
