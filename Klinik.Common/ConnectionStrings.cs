using System.Configuration;

namespace Klinik.Common
{
    public static class ConnectionStrings
    {
        public static string ReportConnectionString
            {
                    get { return ConfigurationManager.ConnectionStrings[Constants.ConnectionStringName.ReportDBConnString].ConnectionString; }
            }
    }
}
