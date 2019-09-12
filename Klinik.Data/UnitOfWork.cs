using Klinik.Data.DataRepository;
using System;

namespace Klinik.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
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
        private IGenericRepository<RolePrivilege> _roleprivRepository;
        private IGenericRepository<UserRole> _userroleRepository;
        private IGenericRepository<Menu> _menuRepository;
        private IGenericRepository<PasswordHistory> _passwordHistRepostiory;
        private IGenericRepository<Log> _logRepository;
        private IGenericRepository<EmployeeAssignment> _employeeAssignmentRepository;
        private IGenericRepository<EmployeeStatu> _employeeStatusRepository;
        public IGenericRepository<FamilyRelationship> _familyRelationshipRepository;
        private IGenericRepository<QueuePoli> _registrationRepository;
        private IGenericRepository<Poli> _poliRepository;
        private IGenericRepository<Patient> _patientRepository;
        private IGenericRepository<PoliFlowTemplate> _poliFlowTemplateRepository;
        private IGenericRepository<Doctor> _doctorRepository;
        private IGenericRepository<PoliSchedule> _poliScheduleRepository;
        private IGenericRepository<PoliScheduleMaster> _poliScheduleMasterRepository;
        private IGenericRepository<PatientClinic> _patientClinicRepository;
        private IGenericRepository<FileArchieve> _fileArchiveRepository;
        private IGenericRepository<City> _cityRepository;
        private IGenericRepository<FormMedical> _formMedicalRepository;
        private IGenericRepository<FormPreExamine> _formPreExamineRepository;
        private IGenericRepository<FormExamine> _formExamineRepository;
        private IGenericRepository<FormExamineAttachment> _formExamineAttachmentRepository;
        private IGenericRepository<FormExamineLab> _formExamineLabRepository;
        private IGenericRepository<FormExamineMedicine> _formExamineMedicineRepository;
        private IGenericRepository<FormExamineService> _formExamineServiceRepository;
        private IGenericRepository<PoliClinic> _poliClinicRepository;
        private IGenericRepository<Product> _productRepository;
        private IGenericRepository<ProductCategory> _productCategoryRepository;
        private IGenericRepository<ProductMedicine> _productMedicineRepository;
        private IGenericRepository<ProductUnit> _productUnitRepository;
        private IGenericRepository<Medicine> _medicineRepository;
        private IGenericRepository<LabItem> _labItemRepository;
        private IGenericRepository<LabItemCategory> _labItemCategoryRepository;
        private IGenericRepository<Service> _serviceRepository;
        private IGenericRepository<PoliService> _poliServiceRepository;
        private IGenericRepository<Letter> _letterRepository;
        private IGenericRepository<SuratRujukanLabKeluar> _suratRujukanLabKeluarRepository;
        private IGenericRepository<Gudang> _gudangRepository;
        private IGenericRepository<DeliveryOrder> _deliveryorderRepository;
        private IGenericRepository<DeliveryOrderDetail> _deliveryorderdetailRepository;
        private IGenericRepository<DeliveryOrderPusat> _deliveryorderpusatRepository;
        private IGenericRepository<DeliveryOrderPusatDetail> _deliveryorderpusatdetailRepository;
        private IGenericRepository<PurchaseOrder> _purchaseorderRepository;
        private IGenericRepository<PurchaseOrderDetail> _purchaseorderdetailRepository;
        private IGenericRepository<Vendor> _vendorRepository;
        private IGenericRepository<PurchaseOrderPusat> _purchaseorderpusatRepository;
        private IGenericRepository<PurchaseOrderPusatDetail> _purchaseorderpusatdetailRepository;
        private IGenericRepository<PurchaseRequest> _purchaserequestRepository;
        private IGenericRepository<PurchaseRequestDetail> _purchaserequestdetailRepository;
        private IGenericRepository<PurchaseRequestPusat> _purchaserequestpusatRepository;
        private IGenericRepository<PurchaseRequestPusatDetail> _purchaserequestpusatdetailRepository;
        private IGenericRepository<ProductInGudang> _productingudangRepository;
        private IGenericRepository<HistoryProductInGudang> _historyprodcutingudangRepository;
        private IGenericRepository<PanggilanPoli> _panggilanPoliRepository;
		private IGenericRepository<FormExamineMedicineDetail> _formExamineMedicineDetailRepository;
        private IGenericRepository<PurchaseRequestConfig> _purchaseRequestConfigRepository;

        private IGenericRepository<LookupCategory> _lookupCategoryRepository;

        private IGenericRepository<ICDTheme> _icdThemeRepository;
        private IGenericRepository<Appointment> _appointmentRepository;
        private IGenericRepository<MCUPackage> _mcuPackageRepository;
        private bool disposed = false;

        public UnitOfWork(KlinikDBEntities context)
        {
            _context = context;
        }

        public IGenericRepository<Service> ServicesRepository
        {
            get
            {
                if (_serviceRepository == null)
                    _serviceRepository = new GenericRepository<Service>(_context);
                return _serviceRepository;
            }
        }

        public IGenericRepository<PoliService> PoliServicesRepository
        {
            get
            {
                if (_poliServiceRepository == null)
                    _poliServiceRepository = new GenericRepository<PoliService>(_context);
                return _poliServiceRepository;
            }
        }

        public IGenericRepository<Product> ProductRepository
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new GenericRepository<Product>(_context);
                return _productRepository;
            }
        }

        public IGenericRepository<ProductCategory> ProductCategoryRepository
        {
            get
            {
                if (_productCategoryRepository == null)
                    _productCategoryRepository = new GenericRepository<ProductCategory>(_context);
                return _productCategoryRepository;
            }
        }

        public IGenericRepository<ProductMedicine> ProductMedicineRepository
        {
            get
            {
                if (_productMedicineRepository == null)
                    _productMedicineRepository = new GenericRepository<ProductMedicine>(_context);
                return _productMedicineRepository;
            }
        }

        public IGenericRepository<ProductUnit> ProductUnitRepository
        {
            get
            {
                if (_productUnitRepository == null)
                    _productUnitRepository = new GenericRepository<ProductUnit>(_context);
                return _productUnitRepository;
            }
        }

        public IGenericRepository<Medicine> MedicineRepository
        {
            get
            {
                if (_medicineRepository == null)
                    _medicineRepository = new GenericRepository<Medicine>(_context);
                return _medicineRepository;
            }
        }

        public IGenericRepository<LabItem> LabItemRepository
        {
            get
            {
                if (_labItemRepository == null)
                    _labItemRepository = new GenericRepository<LabItem>(_context);
                return _labItemRepository;
            }
        }

        public IGenericRepository<FormMedical> FormMedicalRepository
        {
            get
            {
                if (_formMedicalRepository == null)
                    _formMedicalRepository = new GenericRepository<FormMedical>(_context);
                return _formMedicalRepository;
            }
        }

        public IGenericRepository<FormPreExamine> FormPreExamineRepository
        {
            get
            {
                if (_formPreExamineRepository == null)
                    _formPreExamineRepository = new GenericRepository<FormPreExamine>(_context);
                return _formPreExamineRepository;
            }
        }

        public IGenericRepository<FormExamine> FormExamineRepository
        {
            get
            {
                if (_formExamineRepository == null)
                    _formExamineRepository = new GenericRepository<FormExamine>(_context);
                return _formExamineRepository;
            }
        }

        public IGenericRepository<FormExamineAttachment> FormExamineAttachmentRepository
        {
            get
            {
                if (_formExamineAttachmentRepository == null)
                    _formExamineAttachmentRepository = new GenericRepository<FormExamineAttachment>(_context);
                return _formExamineAttachmentRepository;
            }
        }

        public IGenericRepository<FormExamineLab> FormExamineLabRepository
        {
            get
            {
                if (_formExamineLabRepository == null)
                    _formExamineLabRepository = new GenericRepository<FormExamineLab>(_context);
                return _formExamineLabRepository;
            }
        }

        public IGenericRepository<FormExamineMedicine> FormExamineMedicineRepository
        {
            get
            {
                if (_formExamineMedicineRepository == null)
                    _formExamineMedicineRepository = new GenericRepository<FormExamineMedicine>(_context);
                return _formExamineMedicineRepository;
            }
        }

        public IGenericRepository<FormExamineService> FormExamineServiceRepository
        {
            get
            {
                if (_formExamineServiceRepository == null)
                    _formExamineServiceRepository = new GenericRepository<FormExamineService>(_context);
                return _formExamineServiceRepository;
            }
        }

        public IGenericRepository<PoliScheduleMaster> PoliScheduleMasterRepository
        {
            get
            {
                if (_poliScheduleMasterRepository == null)
                    _poliScheduleMasterRepository = new GenericRepository<PoliScheduleMaster>(_context);
                return _poliScheduleMasterRepository;
            }
        }

        public IGenericRepository<PoliSchedule> PoliScheduleRepository
        {
            get
            {
                if (_poliScheduleRepository == null)
                    _poliScheduleRepository = new GenericRepository<PoliSchedule>(_context);
                return _poliScheduleRepository;
            }
        }

        public IGenericRepository<Doctor> DoctorRepository
        {
            get
            {
                if (_doctorRepository == null)
                    _doctorRepository = new GenericRepository<Doctor>(_context);
                return _doctorRepository;
            }
        }

        public IGenericRepository<PoliFlowTemplate> PoliFlowTemplateRepository
        {
            get
            {
                if (_poliFlowTemplateRepository == null)
                    _poliFlowTemplateRepository = new GenericRepository<PoliFlowTemplate>(_context);
                return _poliFlowTemplateRepository;
            }
        }

        public IGenericRepository<Patient> PatientRepository
        {
            get
            {
                if (_patientRepository == null)
                    _patientRepository = new GenericRepository<Patient>(_context);
                return _patientRepository;
            }
        }

        public IGenericRepository<QueuePoli> RegistrationRepository
        {
            get
            {
                if (_registrationRepository == null)
                    _registrationRepository = new GenericRepository<QueuePoli>(_context);

                return _registrationRepository;
            }
        }

        public IGenericRepository<Poli> PoliRepository
        {
            get
            {
                if (_poliRepository == null)
                    _poliRepository = new GenericRepository<Poli>(_context);

                return _poliRepository;
            }
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

        public IGenericRepository<City> CityRepository
        {
            get
            {
                if (_cityRepository == null)
                    _cityRepository = new GenericRepository<City>(_context);

                return _cityRepository;
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

        public IGenericRepository<RolePrivilege> RolePrivRepository
        {
            get
            {
                if (_roleprivRepository == null)
                    _roleprivRepository = new GenericRepository<RolePrivilege>(_context);

                return _roleprivRepository;
            }
        }

        public IGenericRepository<UserRole> UserRoleRepository
        {
            get
            {
                if (_userroleRepository == null)
                    _userroleRepository = new GenericRepository<UserRole>(_context);

                return _userroleRepository;
            }
        }

        public IGenericRepository<Menu> MenuRepository
        {
            get
            {
                if (_menuRepository == null)
                    _menuRepository = new GenericRepository<Menu>(_context);

                return _menuRepository;
            }
        }

        public IGenericRepository<PasswordHistory> PasswordHistoryRepository
        {
            get
            {
                if (_passwordHistRepostiory == null)
                    _passwordHistRepostiory = new GenericRepository<PasswordHistory>(_context);

                return _passwordHistRepostiory;
            }
        }

        public IGenericRepository<Log> LogRepository
        {
            get
            {
                if (_logRepository == null)
                    _logRepository = new GenericRepository<Log>(_context);

                return _logRepository;
            }
        }

        public IGenericRepository<EmployeeAssignment> EmployeeAssignmentRepository
        {
            get
            {
                if (_employeeAssignmentRepository == null)
                    _employeeAssignmentRepository = new GenericRepository<EmployeeAssignment>(_context);

                return _employeeAssignmentRepository;
            }
        }

        public IGenericRepository<EmployeeStatu> EmployeeStatusRepository
        {
            get
            {
                if (_employeeStatusRepository == null)
                    _employeeStatusRepository = new GenericRepository<EmployeeStatu>(_context);

                return _employeeStatusRepository;
            }
        }

        public IGenericRepository<FamilyRelationship> FamilyRelationshipRepository
        {
            get
            {
                if (_familyRelationshipRepository == null)
                    _familyRelationshipRepository = new GenericRepository<FamilyRelationship>(_context);

                return _familyRelationshipRepository;
            }
        }

        public IGenericRepository<PatientClinic> PatientClinicRepository
        {
            get
            {
                if (_patientClinicRepository == null)
                    _patientClinicRepository = new GenericRepository<PatientClinic>(_context);

                return _patientClinicRepository;
            }
        }

        public IGenericRepository<FileArchieve> FileArchiveRepository
        {
            get
            {
                if (_fileArchiveRepository == null)
                    _fileArchiveRepository = new GenericRepository<FileArchieve>(_context);

                return _fileArchiveRepository;
            }
        }

        public IGenericRepository<PoliClinic> PoliClinicRepository
        {
            get
            {
                if (_poliClinicRepository == null)
                    _poliClinicRepository = new GenericRepository<PoliClinic>(_context);
                return _poliClinicRepository;
            }
        }

        public IGenericRepository<LabItemCategory> LabItemCategoryRepository
        {
            get
            {
                if (_labItemCategoryRepository == null)
                    _labItemCategoryRepository = new GenericRepository<LabItemCategory>(_context);
                return _labItemCategoryRepository;
            }
        }

        public IGenericRepository<Letter> LetterRepository
        {
            get
            {
                if (_letterRepository == null)
                    _letterRepository = new GenericRepository<Letter>(_context);
                return _letterRepository;
            }
        }

        public IGenericRepository<SuratRujukanLabKeluar> SuratRujukanLabKeluarRepository
        {
            get
            {
                if (_suratRujukanLabKeluarRepository == null)
                    _suratRujukanLabKeluarRepository = new GenericRepository<SuratRujukanLabKeluar>(_context);
                return _suratRujukanLabKeluarRepository;
            }
        }

        public IGenericRepository<Gudang> GudangRepository
        {
            get
            {
                if (_gudangRepository == null)
                    _gudangRepository = new GenericRepository<Gudang>(_context);
                return _gudangRepository;
            }
        }

        public IGenericRepository<DeliveryOrder> DeliveryOrderRepository
        {
            get
            {
                if (_deliveryorderRepository == null)
                    _deliveryorderRepository = new GenericRepository<DeliveryOrder>(_context);
                return _deliveryorderRepository;
            }
        }

        public IGenericRepository<DeliveryOrderDetail> DeliveryOrderDetailRepository
        {
            get
            {
                if (_deliveryorderdetailRepository == null)
                    _deliveryorderdetailRepository = new GenericRepository<DeliveryOrderDetail>(_context);
                return _deliveryorderdetailRepository;
            }
        }

        public IGenericRepository<DeliveryOrderPusat> DeliveryOrderPusatRepository
        {
            get
            {
                if (_deliveryorderpusatRepository == null)
                    _deliveryorderpusatRepository = new GenericRepository<DeliveryOrderPusat>(_context);
                return _deliveryorderpusatRepository;
            }
        }

        public IGenericRepository<DeliveryOrderPusatDetail> DeliveryOrderPusatDetailRepository
        {
            get
            {
                if (_deliveryorderpusatdetailRepository == null)
                    _deliveryorderpusatdetailRepository = new GenericRepository<DeliveryOrderPusatDetail>(_context);
                return _deliveryorderpusatdetailRepository;
            }
        }

        public IGenericRepository<PurchaseOrder> PurchaseOrderRepository
        {
            get
            {
                if (_purchaseorderRepository == null)
                    _purchaseorderRepository = new GenericRepository<PurchaseOrder>(_context);
                return _purchaseorderRepository;
            }
        }

        public IGenericRepository<PurchaseOrderDetail> PurchaseOrderDetailRepository
        {
            get
            {
                if (_purchaseorderdetailRepository == null)
                    _purchaseorderdetailRepository = new GenericRepository<PurchaseOrderDetail>(_context);
                return _purchaseorderdetailRepository;
            }
        }

        public IGenericRepository<PanggilanPoli> PanggilanPoliRepository
        {
            get
            {
                if (_panggilanPoliRepository == null)
                    _panggilanPoliRepository = new GenericRepository<PanggilanPoli>(_context);
                return _panggilanPoliRepository;
            }
        }

        public IGenericRepository<Vendor> VendorRepository
        {
            get
            {
                if (_vendorRepository == null)
                    _vendorRepository = new GenericRepository<Vendor>(_context);
                return _vendorRepository;
            }
        }

        public IGenericRepository<PurchaseOrderPusat> PurchaseOrderPusatRepository
        {
            get
            {
                if (_purchaseorderpusatRepository == null)
                    _purchaseorderpusatRepository = new GenericRepository<PurchaseOrderPusat>(_context);
                return _purchaseorderpusatRepository;
            }
        }

        public IGenericRepository<PurchaseOrderPusatDetail> PurchaseOrderPusatDetailRepository
        {
            get
            {
                if (_purchaseorderpusatdetailRepository == null)
                    _purchaseorderpusatdetailRepository = new GenericRepository<PurchaseOrderPusatDetail>(_context);
                return _purchaseorderpusatdetailRepository;
            }
        }

        public IGenericRepository<PurchaseRequest> PurchaseRequestRepository
        {
            get
            {
                if (_purchaserequestRepository == null)
                    _purchaserequestRepository = new GenericRepository<PurchaseRequest>(_context);
                return _purchaserequestRepository;
            }
        }

        public IGenericRepository<PurchaseRequestDetail> PurchaseRequestDetailRepository
        {
            get
            {
                if (_purchaserequestdetailRepository == null)
                    _purchaserequestdetailRepository = new GenericRepository<PurchaseRequestDetail>(_context);
                return _purchaserequestdetailRepository;
            }
        }

        public IGenericRepository<PurchaseRequestPusat> PurchaseRequestPusatRepository
        {
            get
            {
                if (_purchaserequestpusatRepository == null)
                    _purchaserequestpusatRepository = new GenericRepository<PurchaseRequestPusat>(_context);
                return _purchaserequestpusatRepository;
            }
        }

        public IGenericRepository<PurchaseRequestPusatDetail> PurchaseRequestPusatDetailRepository
        {
            get
            {
                if (_purchaserequestpusatdetailRepository == null)
                    _purchaserequestpusatdetailRepository = new GenericRepository<PurchaseRequestPusatDetail>(_context);
                return _purchaserequestpusatdetailRepository;
            }
        }

		public IGenericRepository<FormExamineMedicineDetail> FormExamineMedicineDetailRepository
		{
			get
			{
				if (_formExamineMedicineDetailRepository == null)
					_formExamineMedicineDetailRepository = new GenericRepository<FormExamineMedicineDetail>(_context);
				return _formExamineMedicineDetailRepository;
			}
		}

        public IGenericRepository<ProductInGudang> ProductInGudangRepository
        {
            get
            {
                if (_productingudangRepository == null)
                    _productingudangRepository = new GenericRepository<ProductInGudang>(_context);
                return _productingudangRepository;
            }
        }

        public IGenericRepository<HistoryProductInGudang> HistoryProductInGudangRepository
        {
            get
            {
                if (_historyprodcutingudangRepository == null)
                    _historyprodcutingudangRepository = new GenericRepository<HistoryProductInGudang>(_context);
                return _historyprodcutingudangRepository;
            }
        }

        public IGenericRepository<PurchaseRequestConfig> PurchaseRequestConfigRepository
        {
            get
            {
                if (_purchaseRequestConfigRepository == null)
                    _purchaseRequestConfigRepository = new GenericRepository<PurchaseRequestConfig>(_context);
                return _purchaseRequestConfigRepository;
            }
        }

        public IGenericRepository<LookupCategory> LookUpCategoryRepository
        {
            get
            {
                if (_lookupCategoryRepository == null)
                    _lookupCategoryRepository = new GenericRepository<LookupCategory>(_context);
                return _lookupCategoryRepository;
            }
        }

        public IGenericRepository<ICDTheme> ICDThemeRepository
        {
            get
            {
                if (_icdThemeRepository == null)
                    _icdThemeRepository = new GenericRepository<ICDTheme>(_context);
                return _icdThemeRepository;
            }
        }

        public IGenericRepository<Appointment> AppointmentRepository
        {
            get
            {
                if (_appointmentRepository == null)
                    _appointmentRepository = new GenericRepository<Appointment>(_context);
                return _appointmentRepository;
            }
        }

        public IGenericRepository<MCUPackage> MCUpackageRepository
        {
            get
            {
                if (_mcuPackageRepository == null)
                    _mcuPackageRepository = new GenericRepository<MCUPackage>(_context);
                return _mcuPackageRepository;
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
