using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class GeneralHandler
    {
        public static string FormatDate(DateTime dateTime)
        {
           return dateTime.ToString("MM/dd/yy");
        }
    }
}
