using Klinik.Web.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Klinik.Web.DataAccess.Concrete
{
    public class UnitOfWork<Model> : IUnitOfWork<Model>, IDisposable where Model:class
    {
        private readonly KlinikDBEntities _context;
        private IGenericRepository<Model> _modelRepository;
        private bool disposed = false;

        public UnitOfWork(KlinikDBEntities context)
        {
            _context = context;
        }
       
        public IGenericRepository<Model> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<Model>(_context)); }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}