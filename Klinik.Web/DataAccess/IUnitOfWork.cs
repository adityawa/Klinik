using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.Web.DataAccess.DataRepository;
namespace Klinik.Web.DataAccess
{
    public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<Clinic> ClinicRepository { get; }
        IGenericRepository<Employee> EmployeeRepository { get; }
        IGenericRepository<Organization> OrganizationRepository { get; }
        IGenericRepository<Privilege> PrivilegeRepository { get; }
        IGenericRepository<OrganizationRole> RoleRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<GeneralMaster> MasterRepository { get; }
        IGenericRepository<OrganizationPrivilege> OrgPrivRepository { get; }
        IGenericRepository<RolePrivilege> RolePrivRepository { get; }
        IGenericRepository<UserRole> UserRoleRepository { get; }
        int Save();
    }
}
