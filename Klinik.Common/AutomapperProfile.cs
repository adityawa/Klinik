using AutoMapper;
using Klinik.Data.DataRepository;
using Klinik.Entities.MappingMaster;
using Klinik.Entities.MasterData;

namespace Klinik.Common
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Organization, OrganizationModel>();
            CreateMap<OrganizationModel, Organization>();
            CreateMap<Clinic, ClinicModel>()
                .ForMember(m => m.LegalDateDesc, map => map.MapFrom(p => p.LegalDate == null ? "" : p.LegalDate.Value.ToString("MM/dd/yyyy")));
                

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
                .ForMember(x => x.ExpiredDateStr, map => map.MapFrom(p => p.ExpiredDate == null ? "" : p.ExpiredDate.Value.ToString("MM/dd/yyyy")));
            CreateMap<UserModel, User>()
                .ForMember(x => x.OrganizationID, map => map.MapFrom(p => p.OrgID));

            CreateMap<Employee, EmployeeModel>()
                .ForMember(x => x.BirthdateStr, map => map.MapFrom(p => p.BirthDate == null ? "" : p.BirthDate.Value.ToString("MM/dd/yyyy")))
                .ForMember(x => x.EmpTypeDesc, map => map.MapFrom(p => p.GeneralMaster.Name));
            CreateMap<EmployeeModel, Employee>();

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
        }
    }
}