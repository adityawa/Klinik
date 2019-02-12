
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Klinik.Web.DataAccess
{
    public class UnitOfWork :IUnitOfWork, IDisposable
    {
        private readonly KlinikDBEntities _context;
        private IGenericRepository<Clinic> _clinicRepository;
        private IGenericRepository<Organization> _organizationRepository;
        private IGenericRepository<Privilege> _privilegeRepository;
        private IGenericRepository<OrganizationRole> _roleRepository;
        private bool disposed = false;

        
        public UnitOfWork(KlinikDBEntities context)
        {
            _context = context;
        }

        public IGenericRepository<Organization> OrganizationRepository
        {
            get
            {

                if (_organizationRepository == null)
                    _organizationRepository = new GenericRepository<Organization>(_context);
                return _organizationRepository;
            }
        }

        public IGenericRepository<Clinic> ClinicRepository
        {
            get
            {

                if (_clinicRepository == null)
                    _clinicRepository = new GenericRepository<Clinic>(_context);
                return _clinicRepository;
            }
        }

        public IGenericRepository<Privilege> PrivilegeRepository
        {
            get
            {
                if (_privilegeRepository == null)
                    _privilegeRepository = new GenericRepository<Privilege>(_context);
                return _privilegeRepository;
            }
        }

        public IGenericRepository<OrganizationRole> RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                    _roleRepository = new GenericRepository<OrganizationRole>(_context);
                return _roleRepository;
            }
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

      
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        int IUnitOfWork.Save()
        {
            return _context.SaveChanges();
        }
    }
}