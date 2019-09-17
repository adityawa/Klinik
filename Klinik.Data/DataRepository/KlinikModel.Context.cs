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
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Clinic> Clinics { get; set; }
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
        public virtual DbSet<GeneralMaster> GeneralMasters { get; set; }
        public virtual DbSet<Gudang> Gudangs { get; set; }
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
        public virtual DbSet<PatientClinic> PatientClinics { get; set; }
        public virtual DbSet<Poli> Polis { get; set; }
        public virtual DbSet<PoliClinic> PoliClinics { get; set; }
        public virtual DbSet<PoliFlowTemplate> PoliFlowTemplates { get; set; }
        public virtual DbSet<PoliSchedule> PoliSchedules { get; set; }
        public virtual DbSet<PoliScheduleMaster> PoliScheduleMasters { get; set; }
        public virtual DbSet<PoliService> PoliServices { get; set; }
        public virtual DbSet<Privilege> Privileges { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductInGudang> ProductInGudangs { get; set; }
        public virtual DbSet<ProductMedicine> ProductMedicines { get; set; }
        public virtual DbSet<ProductUnit> ProductUnits { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<PurchaseOrderPusat> PurchaseOrderPusats { get; set; }
        public virtual DbSet<PurchaseOrderPusatDetail> PurchaseOrderPusatDetails { get; set; }
        public virtual DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public virtual DbSet<PurchaseRequestConfig> PurchaseRequestConfigs { get; set; }
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
        public virtual DbSet<LookupCategory> LookupCategories { get; set; }
        public virtual DbSet<PatientAge> PatientAges { get; set; }
    }
}
