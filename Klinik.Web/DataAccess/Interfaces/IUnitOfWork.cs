using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Web.DataAccess.Interfaces
{
    public interface IUnitOfWork<Model>
    {
        IGenericRepository<Model> ModelRepository { get; }
        void Save();
    }
}
