using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Klinik.Web.Models.MasterData;
using Klinik.Web.DataAccess.DataRepository;

using Klinik.Web.Models.MappingMaster;
using Klinik.Web.DataAccess;

namespace Klinik.Web.Infrastructure
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {

            CreateMap<Organization, OrganizationModel>();
            CreateMap<OrganizationModel, Organization>();
            CreateMap<Clinic, ClinicModel>();
            CreateMap<Organization, Klinik.Web.Features.MasterData.Organization.OrganizationData>()
                .ForMember(m => m.Klinik, map => map.MapFrom(p => p.Clinic.Name));
            CreateMap<Privilege, PrivilegeModel>()
                .ForMember(m => m.Privilige_Name, map => map.MapFrom(p => p.Privilege_Name));
            CreateMap<PrivilegeModel, Privilege>()
                .ForMember(m => m.Privilege_Name, map => map.MapFrom(p => p.Privilige_Name));
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
                .ForMember(x=>x.PrivileveName, map=>map.MapFrom(p=>p.Privilege.Privilege_Name))
                .ForMember(x=>x.PrivilegeDesc, map=>map.MapFrom(p=>p.Privilege.Privilege_Desc));

            CreateMap<OrganizationPrivilegeModel, OrganizationPrivilege>();

            CreateMap<RolePrivilege, RolePrivilegeModel>()
               .ForMember(x => x.RoleDesc, map => map.MapFrom(p => p.OrganizationRole.RoleName))
               .ForMember(x => x.PrivilegeDesc, map => map.MapFrom(p => p.Privilege.Privilege_Name));
              

            CreateMap<RolePrivilegeModel, RolePrivilege>();

        }


    }
}