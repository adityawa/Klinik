using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Web.DataAccess
{
    public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<Web.Clinic> ClinicRepository { get; }
        IGenericRepository<Web.Employee> EmployeeRepository { get; }
        IGenericRepository<Web.Organization> OrganizationRepository { get; }
        IGenericRepository<Web.Privilege> PrivilegeRepository { get; }
        IGenericRepository<Web.OrganizationRole> RoleRepository { get; }
        IGenericRepository<Web.User> UserRepository { get; }
        IGenericRepository<Web.GeneralMaster> MasterRepository { get; }
        IGenericRepository<Web.OrganizationPrivilege> OrgPrivRepository { get; }
        int Save();
    }
}
