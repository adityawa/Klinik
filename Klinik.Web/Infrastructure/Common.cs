using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
namespace Klinik.Web.Infrastructure
{
    public static class Common
    {
        public static string GetGeneralErrorMesg()
        {
            return System.Configuration.ConfigurationManager.AppSettings["GeneralError"].ToString();
        }
    }
}