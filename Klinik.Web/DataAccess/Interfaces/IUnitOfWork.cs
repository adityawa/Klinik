using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Web.DataAccess.Interfaces
{
    public interface IUnitOfWork<TEntity> where TEntity:class
    {
        IGenericRepository<TEntity> ModelRepository { get; }
        void Save();
    }
}
