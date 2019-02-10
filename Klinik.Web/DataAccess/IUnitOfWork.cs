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
        IGenericRepository<Web.Organization> OrganizationRepository { get; }
        IGenericRepository<Web.Privilege> PrivilegeRepository { get; }
        int Save();
    }
}
