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
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeAssignment> EmployeeAssignments { get; set; }
        public virtual DbSet<EmployeeStatu> EmployeeStatus { get; set; }
        public virtual DbSet<FamilyRelationship> FamilyRelationships { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<MCUPackage> MCUPackages { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationPrivilege> OrganizationPrivileges { get; set; }
        public virtual DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public virtual DbSet<PasswordHistory> PasswordHistories { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientClinic> PatientClinics { get; set; }
        public virtual DbSet<Poli> Polis { get; set; }
        public virtual DbSet<PoliClinic> PoliClinics { get; set; }
        public virtual DbSet<PoliFlowTemplate> PoliFlowTemplates { get; set; }
        public virtual DbSet<PoliSchedule> PoliSchedules { get; set; }
        public virtual DbSet<Privilege> Privileges { get; set; }
        public virtual DbSet<QueuePoli> QueuePolis { get; set; }
        public virtual DbSet<RolePrivilege> RolePrivileges { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<GeneralMaster> GeneralMasters { get; set; }
        public virtual DbSet<Clinic> Clinics { get; set; }
    }
}
