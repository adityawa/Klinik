﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.Models;
using Klinik.Web.Models.MasterData;
namespace Klinik.Web.Features.MasterData.User
{
    public class UserRequest : BaseGetRequest
    {
        public UserModel RequestUserData { get; set; }
    }
}