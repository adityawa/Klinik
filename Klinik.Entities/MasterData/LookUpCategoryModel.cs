﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.MasterData
{
    public class LookUpCategoryModel:BaseModel
    {
        public string LookupName { get; set; }

        public string LookupContent { get; set; }
    }
}