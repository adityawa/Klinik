using AutoMapper;
using Klinik.Data.DataRepository;
using Klinik.Entities;
using Klinik.Entities.Administration;
using Klinik.Entities.MappingMaster;
using Klinik.Entities.MasterData;
using Klinik.Entities.PoliSchedules;
using Klinik.Entities.Registration;

namespace Klinik.Common
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Organization, OrganizationModel>();
            CreateMap<OrganizationModel, Organization>();
            CreateMap<Clinic, ClinicModel>()
                .ForMember(m => m.LegalDateDesc, map => map.MapFrom(p => p.LegalDate == null ? "" : p.LegalDate.Value.ToString("dd/MM/yyyy")));


            CreateMap<ClinicModel, Clinic>()
                .ForMember(m => m.DateCreated, map => map.MapFrom(p => p.CreatedDate))
                .ForMember(m => m.DateModified, map => map.MapFrom(p => p.ModifiedDate))
                .ForMember(m => m.LastUpdateBy, map => map.MapFrom(p => p.ModifiedBy));

            CreateMap<Organization, OrganizationData>()
                .ForMember(m => m.Klinik, map => map.MapFrom(p => p.Clinic.Name));

            CreateMap<Privilege, PrivilegeModel>()
                .ForMember(m => m.Privilige_Name, map => map.MapFrom(p => p.Privilege_Name))
                .ForMember(m => m.MenuDesc, map => map.MapFrom(p => p.Menu.Description))
            .ForMember(m => m.MenuID, map => map.MapFrom(p => p.MenuID ?? 0));

            CreateMap<PrivilegeModel, Privilege>()
                .ForMember(m => m.Privilege_Name, map => map.MapFrom(p => p.Privilige_Name))
                .ForMember(m => m.MenuID, map => map.MapFrom(p => p.MenuID));
            CreateMap<OrganizationRole, RoleModel>()
                .ForMember(m => m.OrganizationName, map => map.MapFrom(p => p.Organization.OrgName));
            CreateMap<RoleModel, OrganizationRole>();

            CreateMap<User, UserModel>()
                .ForMember(x => x.EmployeeName, map => map.MapFrom(p => p.Employee.EmpName))
                .ForMember(x => x.OrganizationName, map => map.MapFrom(p => p.Organization.OrgName))
                .ForMember(x => x.StatusDesc, map => map.MapFrom(p => p.Status == true ? "Active" : "Inactive"))
                .ForMember(x => x.ExpiredDateStr, map => map.MapFrom(p => p.ExpiredDate == null ? "" : p.ExpiredDate.Value.ToString("dd/MM/yyyy")));
            CreateMap<UserModel, User>()
                .ForMember(x => x.OrganizationID, map => map.MapFrom(p => p.OrgID));

            CreateMap<Employee, EmployeeModel>()
                .ForMember(x => x.BirthdateStr, map => map.MapFrom(p => p.BirthDate == null ? "" : p.BirthDate.Value.ToString("dd/MM/yyyy")))
                .ForMember(x => x.EmpTypeDesc, map => map.MapFrom(p => p.FamilyRelationship.Name))
                .ForMember(x => x.EmpStatusDesc, map => map.MapFrom(p => p.EmployeeStatu.Name))
                .ForMember(x => x.EmpTypeDesc, map => map.MapFrom(p => p.FamilyRelationship.Name));

            CreateMap<EmployeeModel, Employee>()
                .ForMember(x => x.EmpType, map => map.MapFrom(p => p.EmpType))
                .ForMember(x => x.Status, map => map.MapFrom(p => p.EmpStatus));

            CreateMap<OrganizationPrivilege, OrganizationPrivilegeModel>()
                .ForMember(x => x.OrganizationName, map => map.MapFrom(p => p.Organization.OrgName))
                .ForMember(x => x.PrivileveName, map => map.MapFrom(p => p.Privilege.Privilege_Name))
                .ForMember(x => x.PrivilegeDesc, map => map.MapFrom(p => p.Privilege.Privilege_Desc));

            CreateMap<OrganizationPrivilegeModel, OrganizationPrivilege>();

            CreateMap<RolePrivilege, RolePrivilegeModel>()
               .ForMember(x => x.RoleDesc, map => map.MapFrom(p => p.OrganizationRole.RoleName))
               .ForMember(x => x.PrivilegeDesc, map => map.MapFrom(p => p.Privilege.Privilege_Name));

            CreateMap<RolePrivilegeModel, RolePrivilege>();

            CreateMap<UserRole, UserRoleModel>()
                .ForMember(x => x.UserName, map => map.MapFrom(p => p.User.UserName))
                .ForMember(x => x.RoleName, map => map.MapFrom(p => p.OrganizationRole.RoleName));

            CreateMap<UserRoleModel, UserRole>();

            CreateMap<Menu, MenuModel>();

            CreateMap<Log, LogModel>()
                .ForMember(x => x.strStart, map => map.MapFrom(p => p.Start.ToString("dd/MM/yyyy")));
            CreateMap<LogModel, Log>();

            CreateMap<EmployeeStatu, EmployeeStatusModel>()
               .ForMember(x => x.Description, map => map.MapFrom(p => p.Name + " - " + p.Status));

            CreateMap<FamilyRelationship, FamilyRelationshipModel>();

            CreateMap<RegistrationModel, QueuePoli>()
                .ForMember(m => m.PoliFrom, map => map.MapFrom(p => p.PoliFromID))
                .ForMember(m => m.PoliTo, map => map.MapFrom(p => p.PoliToID));
            CreateMap<QueuePoli, RegistrationModel>()
                .ForMember(m => m.PoliFromID, map => map.MapFrom(p => p.PoliFrom))
                .ForMember(m => m.PoliToID, map => map.MapFrom(p => p.PoliTo))
                .ForMember(m => m.PatientName, map => map.MapFrom(p => p.Patient.Name))
                .ForMember(m => m.PoliFromName, map => map.MapFrom(p => p.Poli.Name))
                .ForMember(m => m.PoliToName, map => map.MapFrom(p => p.Poli1.Name))
                .ForMember(m => m.StatusStr, map => map.MapFrom(p => ((RegistrationStatusEnum)p.Status.Value).ToString()))
                .ForMember(m => m.TypeStr, map => map.MapFrom(p => ((RegistrationTypeEnum)p.Type.Value).ToString()))
                .ForMember(m => m.TransactionDateStr, map => map.MapFrom(p => p.TransactionDate.Value.ToString("dd/MM/yyyy hh:mm:ss")))
                .ForMember(m => m.TransactionDate, map => map.MapFrom(p => p.TransactionDate.Value));

            CreateMap<PoliModel, Poli>();
            CreateMap<Poli, PoliModel>();

            CreateMap<PatientModel, Patient>();
            CreateMap<Patient, PatientModel>();

            CreateMap<PoliFlowTemplateModel, PoliFlowTemplate>();
            CreateMap<PoliFlowTemplate, PoliFlowTemplateModel>()
                .ForMember(m => m.PoliTypeIDTo, map => map.MapFrom(p => p.PoliTypeIDTo.Value))
                .ForMember(m => m.PoliTypeID, map => map.MapFrom(p => p.PoliTypeID.Value));

            CreateMap<DoctorModel, Doctor>();
            CreateMap<Doctor, DoctorModel>()
                .ForMember(m => m.STRValidFromStr, map => map.MapFrom(p => p.STRValidFrom.Value.ToString("dd/MM/yyyy")))
                .ForMember(m => m.STRValidToStr, map => map.MapFrom(p => p.STRValidTo.Value.ToString("dd/MM/yyyy")));

            CreateMap<PoliScheduleModel, PoliSchedule>();
            CreateMap<PoliSchedule, PoliScheduleModel>()
                .ForMember(m => m.StartDateStr, map => map.MapFrom(p => p.StartDate.Value.ToString("dd/MM/yyyy hh:mm:ss")))
                .ForMember(m => m.EndDateStr, map => map.MapFrom(p => p.EndDate.Value.ToString("dd/MM/yyyy hh:mm:ss")));
        }
    }
}