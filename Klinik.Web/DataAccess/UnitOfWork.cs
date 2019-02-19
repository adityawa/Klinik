﻿
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
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Employee> _employeeRepository;
        private IGenericRepository<GeneralMaster> _masterRepository;
        private IGenericRepository<OrganizationPrivilege> _orgprivRepository;
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

        public IGenericRepository<User> UserRepository 
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new GenericRepository<User>(_context);
                return _userRepository;
            }
        }

        public IGenericRepository<Employee> EmployeeRepository
        {
            get
            {
                if (_employeeRepository == null)
                    _employeeRepository = new GenericRepository<Employee>(_context);

                return _employeeRepository;
            }
        }

        public IGenericRepository<GeneralMaster> MasterRepository
        {
            get
            {
                if (_masterRepository == null)
                    _masterRepository = new GenericRepository<GeneralMaster>(_context);

                return _masterRepository;
            }
        }

        public IGenericRepository<OrganizationPrivilege> OrgPrivRepository
        {
            get
            {
                if (_orgprivRepository == null)
                    _orgprivRepository = new GenericRepository<OrganizationPrivilege>(_context);

                return _orgprivRepository;
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