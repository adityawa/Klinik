using AutoMapper;
using Klinik.Data.DataRepository;
using Klinik.Entities;
using Klinik.Entities.Administration;
using Klinik.Entities.Document;
using Klinik.Entities.MappingMaster;
using Klinik.Entities.MasterData;
using Klinik.Entities.Patient;
using Klinik.Entities.PoliSchedules;
using Klinik.Entities.Loket;
using Klinik.Entities.Form;
using Klinik.Entities.PreExamine;
using Klinik.Entities.Poli;
using Klinik.Entities.Letter;
using Klinik.Entities.PurchaseOrder;
using Klinik.Entities.PurchaseOrderDetail;
using Klinik.Entities.PurchaseRequestDetail;
using Klinik.Entities.PurchaseRequest;
using Klinik.Entities.DeliveryOrder;
using Klinik.Entities.DeliveryOrderDetail;

namespace Klinik.Common
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Organization, OrganizationModel>();
            CreateMap<OrganizationModel, Organization>();

            CreateMap<Clinic, ClinicModel>()
                .ForMember(m => m.LegalDateDesc, map => map.MapFrom(p => p.LegalDate == null ? "" : p.LegalDate.Value.ToString("dd/MM/yyyy")));
            CreateMap<ClinicModel, Clinic>()
                .ForMember(m => m.CreatedDate, map => map.MapFrom(p => p.CreatedDate))
                .ForMember(m => m.ModifiedDate, map => map.MapFrom(p => p.ModifiedDate))
                .ForMember(m => m.ModifiedBy, map => map.MapFrom(p => p.ModifiedBy));

            CreateMap<Organization, OrganizationData>()
                .ForMember(m => m.Klinik, map => map.MapFrom(p => p.Clinic.Name));

            CreateMap<Privilege, PrivilegeModel>()
                .ForMember(m => m.Privilige_Name, map => map.MapFrom(p => p.Privilege_Name))
                .ForMember(m => m.MenuDesc, map => map.MapFrom(p => p.Menu.Description))
                .ForMember(m => m.MenuID, map => map.MapFrom(p => p.MenuID ?? 0));
            CreateMap<PrivilegeModel, Privilege>()
                .ForMember(m => m.Privilege_Name, map => map.MapFrom(p => p.Privilige_Name))
                .ForMember(m => m.MenuID, map => map.MapFrom(p => p.MenuID));

            CreateMap<OrganizationRole, RoleModel>()
                .ForMember(m => m.OrganizationName, map => map.MapFrom(p => p.Organization.OrgName));
            CreateMap<RoleModel, OrganizationRole>();

            CreateMap<User, UserModel>()
                .ForMember(x => x.EmployeeName, map => map.MapFrom(p => p.Employee.EmpName))
                .ForMember(x => x.OrganizationName, map => map.MapFrom(p => p.Organization.OrgName))
                .ForMember(x => x.StatusDesc, map => map.MapFrom(p => p.Status == true ? "Active" : "Inactive"))
                .ForMember(x => x.ExpiredDateStr, map => map.MapFrom(p => p.ExpiredDate == null ? "" : p.ExpiredDate.Value.ToString("dd/MM/yyyy")));
            CreateMap<UserModel, User>()
                .ForMember(x => x.OrganizationID, map => map.MapFrom(p => p.OrgID));

            CreateMap<Employee, EmployeeModel>()
                .ForMember(x => x.BirthdateStr, map => map.MapFrom(p => p.BirthDate == null ? "" : p.BirthDate.Value.ToString("dd/MM/yyyy")))
                .ForMember(x => x.EmpTypeDesc, map => map.MapFrom(p => p.FamilyRelationship.Name))
                .ForMember(x => x.EmpStatusDesc, map => map.MapFrom(p => p.EmployeeStatu.Name))
                .ForMember(x => x.EmpTypeDesc, map => map.MapFrom(p => p.FamilyRelationship.Name))
                .ForMember(x => x.Email, map => map.MapFrom(p => p.Email == null ? "" : CommonUtils.Decryptor(p.Email, CommonUtils.KeyEncryptor)))
                .ForMember(x => x.HPNumber, map => map.MapFrom(p => p.HPNumber == null ? "" : CommonUtils.Decryptor(p.HPNumber, CommonUtils.KeyEncryptor)));

            CreateMap<EmployeeModel, Employee>()
                .ForMember(x => x.EmpType, map => map.MapFrom(p => p.EmpType))
                .ForMember(x => x.Status, map => map.MapFrom(p => p.EmpStatus))
                .ForMember(x => x.HPNumber, map => map.MapFrom(p => p.HPNumber == null ? "" : CommonUtils.Encryptor(p.HPNumber, CommonUtils.KeyEncryptor)))
                .ForMember(x => x.Email, map => map.MapFrom(p => p.Email == null ? "" : CommonUtils.Encryptor(p.Email, CommonUtils.KeyEncryptor)));

            CreateMap<OrganizationPrivilege, OrganizationPrivilegeModel>()
                .ForMember(x => x.OrganizationName, map => map.MapFrom(p => p.Organization.OrgName))
                .ForMember(x => x.PrivileveName, map => map.MapFrom(p => p.Privilege.Privilege_Name))
                .ForMember(x => x.PrivilegeDesc, map => map.MapFrom(p => p.Privilege.Privilege_Desc));
            CreateMap<OrganizationPrivilegeModel, OrganizationPrivilege>();

            CreateMap<RolePrivilege, RolePrivilegeModel>()
               .ForMember(x => x.RoleDesc, map => map.MapFrom(p => p.OrganizationRole.RoleName))
               .ForMember(x => x.PrivilegeDesc, map => map.MapFrom(p => p.Privilege.Privilege_Name));
            CreateMap<RolePrivilegeModel, RolePrivilege>();

            CreateMap<UserRole, UserRoleModel>()
                .ForMember(x => x.UserName, map => map.MapFrom(p => p.User.UserName))
                .ForMember(x => x.RoleName, map => map.MapFrom(p => p.OrganizationRole.RoleName));
            CreateMap<UserRoleModel, UserRole>();

            CreateMap<Menu, MenuModel>();
            CreateMap<MenuModel, Menu>();

            CreateMap<Log, LogModel>()
                .ForMember(x => x.StartStr, map => map.MapFrom(p => p.Start.ToString("dd/MM/yyyy hh:mm:ss")));
            CreateMap<LogModel, Log>();

            CreateMap<EmployeeStatu, EmployeeStatusModel>()
               .ForMember(x => x.Description, map => map.MapFrom(p => p.Name + " - " + p.Status));

            CreateMap<FamilyRelationship, FamilyRelationshipModel>();

            CreateMap<LoketModel, QueuePoli>()
                .ForMember(m => m.PoliFrom, map => map.MapFrom(p => p.PoliFromID))
                .ForMember(m => m.PoliTo, map => map.MapFrom(p => p.PoliToID));
            CreateMap<QueuePoli, LoketModel>()
                .ForMember(m => m.PoliFromID, map => map.MapFrom(p => p.PoliFrom))
                .ForMember(m => m.PoliToID, map => map.MapFrom(p => p.PoliTo))
                .ForMember(m => m.PoliFromName, map => map.MapFrom(p => p.Poli.Name))
                .ForMember(m => m.PoliToName, map => map.MapFrom(p => p.Poli1.Name))
                .ForMember(m => m.PatientID, map => map.MapFrom(p => p.PatientID))
                .ForMember(m => m.PatientName, map => map.MapFrom(p => p.Patient.Name))
                .ForMember(m => m.PatientAddress, map => map.MapFrom(p => p.Patient.Address))
                .ForMember(m => m.PatientBirthDateStr, map => map.MapFrom(p => p.Patient.BirthDate.ToString("dd/MM/yyyy")))
                .ForMember(m => m.PatientAddress, map => map.MapFrom(p => p.Patient.Address))
                .ForMember(m => m.PatientBloodType, map => map.MapFrom(p => p.Patient.BloodType))
                .ForMember(m => m.PatientBPJSNumber, map => map.MapFrom(p => p.Patient.BPJSNumber))
                .ForMember(m => m.PatientGender, map => map.MapFrom(p => p.Patient.Gender))
                .ForMember(m => m.PatientHPNumber, map => map.MapFrom(p => p.Patient.HPNumber))
                .ForMember(m => m.FormMedicalID, map => map.MapFrom(p => p.FormMedical.ID))
                .ForMember(m => m.PatientKTPNumber, map => map.MapFrom(p => p.Patient.KTPNumber))
                .ForMember(m => m.PatientType, map => map.MapFrom(p => p.Patient.Type))
                .ForMember(m => m.StatusStr, map => map.MapFrom(p => ((RegistrationStatusEnum)p.Status.Value).ToString()))
                .ForMember(m => m.TypeStr, map => map.MapFrom(p => ((RegistrationTypeEnum)p.Type).ToString()))
                .ForMember(m => m.TransactionDateStr, map => map.MapFrom(p => p.TransactionDate.ToString("dd/MM/yyyy hh:mm:ss")))
                .ForMember(m => m.TransactionDate, map => map.MapFrom(p => p.TransactionDate))
                .ForMember(m => m.DoctorStr, map => map.MapFrom(p => p.Doctor.Name))
                .ForMember(m => m.ClinicName, map => map.MapFrom(p => p.Clinic.Name))
                .ForMember(m => m.MRNumber, map => map.MapFrom(p => p.Patient.MRNumber))
                .ForMember(m => m.strIsPreExamine, map => map.MapFrom(p => p.IsPreExamine == true ? "Yes" : "No"));


            CreateMap<PoliModel, Poli>();
            CreateMap<Poli, PoliModel>();

            CreateMap<PatientModel, Patient>()
                .ForMember(x => x.PatientKey, map => map.MapFrom(p => p.Name.Trim().Replace(" ", "") + "_" + p.BirthDateStr.Replace("/", "")));

            CreateMap<Patient, PatientModel>()
                .ForMember(x => x.BirthDateStr, map => map.MapFrom(p => p.BirthDate.ToString("dd/MM/yyyy")))
                .ForMember(x => x.EmployeeName, map => map.MapFrom(p => p.Employee.EmpName))
                .ForMember(x => x.SAP, map => map.MapFrom(p => p.Employee.EmpID))
                .ForMember(x => x.Umur, map => map.MapFrom(p => CommonUtils.GetPatientAge(p.BirthDate)));

            CreateMap<PoliFlowTemplateModel, PoliFlowTemplate>();
            CreateMap<PoliFlowTemplate, PoliFlowTemplateModel>()
                .ForMember(m => m.PoliTypeIDTo, map => map.MapFrom(p => p.PoliTypeIDTo))
                .ForMember(m => m.PoliTypeID, map => map.MapFrom(p => p.PoliTypeID));

            CreateMap<DoctorModel, Doctor>();
            CreateMap<Doctor, DoctorModel>()
                .ForMember(m => m.STRValidFromStr, map => map.MapFrom(p => p.STRValidFrom.HasValue ? p.STRValidFrom.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(m => m.STRValidToStr, map => map.MapFrom(p => p.STRValidTo.HasValue ? p.STRValidTo.Value.ToString("dd/MM/yyyy") : string.Empty));

            CreateMap<PoliScheduleModel, PoliSchedule>();
            CreateMap<PoliSchedule, PoliScheduleModel>()
                .ForMember(m => m.StartDateStr, map => map.MapFrom(p => p.StartDate.ToString("dd/MM/yyyy hh:mm:ss")))
                .ForMember(m => m.EndDateStr, map => map.MapFrom(p => p.EndDate.ToString("dd/MM/yyyy hh:mm:ss")));

            CreateMap<PoliScheduleMasterModel, PoliScheduleMaster>();
            CreateMap<PoliScheduleMaster, PoliScheduleMasterModel>()
                .ForMember(m => m.StartTimeStr, map => map.MapFrom(p => p.StartTime.ToString(@"hh\:mm")))
                .ForMember(m => m.EndTimeStr, map => map.MapFrom(p => p.EndTime.ToString(@"hh\:mm")));

            CreateMap<PatientClinicModel, PatientClinic>();
            CreateMap<PatientClinic, PatientClinicModel>();

            CreateMap<DocumentModel, FileArchieve>();
            CreateMap<FileArchieve, DocumentModel>();

            CreateMap<DoctorModel, User>();
            CreateMap<DoctorModel, Employee>()
                .ForMember(m => m.EmpName, map => map.MapFrom(p => p.Name));

            CreateMap<City, CityModel>()
                .ForMember(x => x.City, map => map.MapFrom(p => p.City1));
            CreateMap<CityModel, City>();

            CreateMap<FormMedicalModel, FormMedical>();
            CreateMap<FormMedical, FormMedicalModel>();

            CreateMap<PreExamineModel, FormPreExamine>()
                .ForMember(x => x.KBDate, map => map.MapFrom(p => CommonUtils.ConvertStringDate2Datetime(p.strKBDate)))
                .ForMember(x => x.MenstrualDate, map => map.MapFrom(p => CommonUtils.ConvertStringDate2Datetime(p.strMenstrualDate)));
            CreateMap<FormPreExamine, PreExamineModel>()
                .ForMember(x => x.strTransDate, map => map.MapFrom(p => p.TransDate.ToString("dd/MM/yyyy")))
                .ForMember(m => m.strKBDate, map => map.MapFrom(p => p.KBDate.HasValue ? p.KBDate.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(m => m.strMenstrualDate, map => map.MapFrom(p => p.MenstrualDate.HasValue ? p.MenstrualDate.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(x => x.DoctorName, map => map.MapFrom(p => p.Doctor.Name));

            CreateMap<FormExamineModel, FormExamine>();
            CreateMap<FormExamine, FormExamineModel>();

            CreateMap<FormExamineAttachmentModel, FormExamineAttachment>();
            CreateMap<FormExamineAttachment, FormExamineAttachmentModel>();

            CreateMap<FormExamineLabModel, FormExamineLab>();
            CreateMap<FormExamineLab, FormExamineLabModel>()
                .ForMember(x => x.LabItemDesc, map => map.MapFrom(p => p.LabItem.Name));

            CreateMap<FormExamineServiceModel, FormExamineService>();
            CreateMap<FormExamineService, FormExamineServiceModel>();

            CreateMap<FormExamineMedicineModel, FormExamineMedicine>();
            CreateMap<FormExamineMedicine, FormExamineMedicineModel>()
                .ForMember(x => x.ProductName, map => map.MapFrom(p => p.Product.Name));

            CreateMap<LoketModel, PoliExamineModel>();


            CreateMap<LabItemCategory, LabItemCategoryModel>()
                .ForMember(x => x.PoliName, map => map.MapFrom(p => p.Poli.Name));


            CreateMap<ProductModel, Product>();
            CreateMap<Product, ProductModel>();

            CreateMap<ProductUnitModel, ProductUnit>();
            CreateMap<ProductUnit, ProductUnitModel>();

            CreateMap<ProductCategoryModel, ProductCategory>();
            CreateMap<ProductCategory, ProductCategoryModel>();

            CreateMap<MedicineModel, Medicine>();
            CreateMap<Medicine, MedicineModel>();

            CreateMap<ProductMedicineModel, ProductMedicine>();
            CreateMap<ProductMedicine, ProductMedicineModel>()
                .ForMember(m => m.ProductName, map => map.MapFrom(p => p.Product.Name))
                .ForMember(m => m.MedicineName, map => map.MapFrom(p => p.Medicine.Name));

            CreateMap<LabItemCategoryModel, LabItemCategory>();
            CreateMap<LabItemCategory, LabItemCategoryModel>()
                .ForMember(m => m.PoliName, map => map.MapFrom(p => p.Poli.Name));

            CreateMap<LabItemModel, LabItem>();
            CreateMap<LabItem, LabItemModel>()
                .ForMember(m => m.LabItemCategoryName, map => map.MapFrom(p => p.LabItemCategory.Name));

            CreateMap<ServiceModel, Service>();
            CreateMap<Service, ServiceModel>();

            CreateMap<PoliServiceModel, PoliService>();
            CreateMap<PoliService, PoliServiceModel>()
                .ForMember(m => m.ClinicName, map => map.MapFrom(p => p.Clinic.Name))
                .ForMember(m => m.PoliName, map => map.MapFrom(p => p.Poli.Name))
                .ForMember(m => m.ServicesName, map => map.MapFrom(p => p.Service.Name));

            CreateMap<Letter, LabReferenceLetterModel>();
            CreateMap<LabReferenceLetterModel, Letter>()
                .ForMember(m => m.CreatedBy, map => map.MapFrom(p => p.Account.UserName));

            CreateMap<SuratRujukanLabKeluar, SuratRujukanKeluarModel>();
            CreateMap<SuratRujukanKeluarModel, SuratRujukanLabKeluar>();

            CreateMap<HealthBodyLetterModel, Letter>();
            CreateMap<Letter, HealthBodyLetterModel>();

            CreateMap<RujukanBerobatModel, Letter>().ForMember(x => x.Pekerjaan, map => map.MapFrom(p => p.Perusahaan));
            CreateMap<Letter, RujukanBerobatModel>().ForMember(x => x.Perusahaan, map => map.MapFrom(p => p.Pekerjaan));

            CreateMap<PersetujuanTindakanModel, Letter>();
            CreateMap<Letter, PersetujuanTindakanModel>();
            CreateMap<PanggilanPoliModel, PanggilanPoli>();
            CreateMap<PanggilanPoli, PanggilanPoliModel>();

			CreateMap<FormExamineMedicineDetail, FormExamineMedicineDetailModel>();				
			CreateMap<FormExamineMedicineDetailModel, FormExamineMedicineDetail>();

            CreateMap<PurchaseOrderModel, PurchaseOrder>();
            CreateMap<PurchaseOrderDetailModel, PurchaseOrderDetail>();

            CreateMap<PurchaseRequestModel, PurchaseRequest>();
            CreateMap<PurchaseRequestDetailModel, PurchaseRequestDetail>();

            CreateMap<PurchaseRequest, PurchaseRequestModel>();
            CreateMap<PurchaseRequestDetail, PurchaseRequestDetailModel> ();

            CreateMap<PurchaseRequestModel, PurchaseOrderModel>();
            CreateMap<PurchaseRequestDetailModel, PurchaseOrderDetailModel>();

            CreateMap<PurchaseOrder, PurchaseOrderModel>();
            CreateMap<PurchaseOrderDetail, PurchaseOrderDetailModel>();

            CreateMap<PurchaseOrderModel, DeliveryOrderModel>();
            CreateMap<PurchaseOrderDetailModel, DeliveryOrderDetailModel>()
                .ForMember(m => m.qty_request, map => map.MapFrom(p => p.total))
                .ForMember(m => m.qty_by_HP, map => map.MapFrom(p => p.qty_by_ho));

            CreateMap<DeliveryOrderDetail, DeliveryOrderDetailModel>();
            CreateMap<DeliveryOrderDetailModel, DeliveryOrderDetail>();

            CreateMap<DeliveryOrder, DeliveryOrderModel>()
                .ForMember(m => m.ponumber, map => map.MapFrom(p => p.PurchaseOrder.ponumber))
                .ForMember(m => m.podate, map => map.MapFrom(p => p.PurchaseOrder.podate))
                .ForMember(m => m.prnumber, map => map.MapFrom(p => p.PurchaseOrder.PurchaseRequest.prnumber))
                .ForMember(m => m.prdate, map => map.MapFrom(p => p.PurchaseOrder.PurchaseRequest.prdate))
                .ForMember(m => m.prrequestby, map => map.MapFrom(p => p.PurchaseOrder.PurchaseRequest.request_by));
        }
    }
}