﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.MasterData
{
    public class GudangModel : BaseModel
    {
        public string name { get; set; }
        public long ClinicId { get; set; }
        public string ClinicName { get; set; }
        public Nullable<bool> IsGudangPusat { get; set; }
        public Nullable<long> OrganizationId { get; set; }
    }
}
