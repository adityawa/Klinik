using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.Models;
using Klinik.Web.Models.MasterData;
namespace Klinik.Web.Features.MasterData.Organization
{
    public class OrganizationResponse: BaseResponse<OrganizationData>
    {
        
    }

    public class OrganizationData : OrganizationModel
    {
        public string Klinik { get; set; }
    }

}