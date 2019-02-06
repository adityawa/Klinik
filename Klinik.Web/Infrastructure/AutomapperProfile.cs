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
            CreateMap<Organization, Klinik.Web.Features.MasterData.Organization.OrganizationData>()
                .ForMember(m => m.Klinik, map => map.MapFrom(p => p.Clinic.Name));
                
            

        }


    }
}