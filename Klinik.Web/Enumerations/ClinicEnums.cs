using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.Enumerations
{
    public class ClinicEnums
    {
        public enum enumStatus
        {
            SUCCESS,
            ERROR
        }

        public enum enumMasterTypes
        {
            EmploymentType,
            Department
        }

        public enum enumAuthResult
        {
            SUCCESS,
            UNRECOGNIZED
        }

        public enum enumAction
        {
            DELETE
        }
    }
}