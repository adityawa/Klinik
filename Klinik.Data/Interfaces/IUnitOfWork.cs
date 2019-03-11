using Klinik.Data.DataRepository;
using System;

namespace Klinik.Data
{
    /// <summary>
    /// Interface of Unity of Work
    /// </summary>
    public interface IUnitOfWork : IDisposable
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
        IGenericRepository<Menu> MenuRepository { get; }
        IGenericRepository<PasswordHistory> PasswordHistoryRepository { get; }

        IGenericRepository<Log> LogRepository { get; }
        int Save();
    }
}
