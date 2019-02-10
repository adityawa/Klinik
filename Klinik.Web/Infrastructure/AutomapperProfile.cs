using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Klinik.Web.Models.MasterData;
using Klinik.Web.Features.MasterData.Organization;

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

        }


    }
}