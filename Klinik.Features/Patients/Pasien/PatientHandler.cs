using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Document;
using Klinik.Entities.MasterData;
using Klinik.Entities.Patient;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Klinik.Features
{
    public class PatientHandler : BaseFeatures, IBaseFeatures<PatientResponse, PatientRequest>
    {
        public PatientHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }


        public List<PatientModel> GetAll()
        {
            var patients = new List<PatientModel>();

            var _qry = _unitOfWork.PatientRepository.Get(x => x.RowStatus == 0);
            foreach (var item in _qry)
            {
                patients.Add(new PatientModel { Id = item.ID, Name = item.Name });
            }

            return patients;
        }

        public PatientResponse CreateOrEdit(PatientRequest request)
        {
            var response = new PatientResponse();
            int result = 0;
            long _photoID = 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var newPatientKey = $"{request.Data.Name.Trim().Replace(" ", "")}_{request.Data.BirthDateStr.Replace("/", "")}";
                    var cekIsPatientKeyExist = _unitOfWork.PatientRepository.GetFirstOrDefault(x => x.PatientKey == newPatientKey);
                    if (cekIsPatientKeyExist == null && request.Data.Id == 0)
                    {
                        //handle photo
                        if (request.Data.file != null && request.Data.file.ContentLength > 0)
                        {
                            if (request.Data.Photo == null)
                                request.Data.Photo = new DocumentModel();

                            var uploadDir = "~/fileDoc/Photo";
                            request.Data.Photo.ActualPath = Path.Combine(HttpContext.Current.Server.MapPath(uploadDir), request.Data.file.FileName);
                            request.Data.Photo.ActualName = request.Data.file.FileName;
                            request.Data.Photo.TypeDoc = request.Data.file.ContentType;
                        }

                        var _entityPatient = Mapper.Map<PatientModel, Patient>(request.Data);
                        _entityPatient.BirthDate = reformatDate(request.Data.BirthDateStr);
                        var _month = DateTime.Now.Month.ToString().Length == 1 ? string.Format("0{0}", DateTime.Now.Month.ToString()) : DateTime.Now.Month.ToString();
                        _entityPatient.MRNumber = $"{_month}{DateTime.Now.Year.ToString().Substring(2, 2)}{ProvideMRNo()}";
                        _entityPatient.CreatedBy = request.Data.Account.UserName;
                        _entityPatient.CreatedDate = DateTime.Now;
                        _context.Patients.Add(_entityPatient);
                        result = _context.SaveChanges();

                        if (request.Data.Photo != null && !String.IsNullOrEmpty(request.Data.Photo.ActualPath))
                        {
                            var _entityPhoto = Mapper.Map<DocumentModel, FileArchieve>(request.Data.Photo);
                            _entityPhoto.SourceTable = ClinicEnums.SourceTable.PATIENT.ToString();
                            _entityPhoto.CreatedBy = request.Data.Account.UserName;
                            _entityPhoto.CreatedDate = DateTime.Now;
                            _context.FileArchieves.Add(_entityPhoto);
                            result = _context.SaveChanges();
                            _photoID = _entityPhoto.ID;

                            request.Data.file.SaveAs(request.Data.Photo.ActualPath);
                        }

                        //GetClinicID for current admin login

                        var _entityPatientClinic = new PatientClinic
                        {
                            ClinicID = request.Data.Account.ClinicID,
                            PhotoID = _photoID,
                            PatientID = _entityPatient.ID,
                            TempAddress = request.Data.PatientClinic.TempAddress,
                            TempCityID = request.Data.PatientClinic.TempCityId,
                            RefferencePerson = request.Data.PatientClinic.RefferencePerson,
                            RefferenceNumber = request.Data.PatientClinic.RefferenceNumber,
                            RefferenceRelation = request.Data.PatientClinic.RefferenceRelation,
                            OldMRNumber = request.Data.PatientClinic.OldMRNumber,
                            CreatedBy = request.Data.Account.UserName,
                            CreatedDate = DateTime.Now
                        };

                        _context.PatientClinics.Add(_entityPatientClinic);
                        result = _context.SaveChanges();
                        transaction.Commit();

                        CommandLog(true, ClinicEnums.Module.Patient, Constants.Command.ADD_NEW_PATIENT, request.Data.Account, request.Data);

                        response.Status = true;
                        response.Message = string.Format(Messages.ObjectHasBeenAdded2, request.Data.Name, _entityPatient.ID);
                        response.Entity = new PatientModel { Id = _entityPatient.ID };
                    }
                    else
                    {
                        //edit data biasa
                        if (request.Data.Id != 0 && cekIsPatientKeyExist != null)
                        {
                            int resultUpdated = 0;
                            var willBeEdit = _context.Patients.SingleOrDefault(x => x.ID == request.Data.Id);
                            if (willBeEdit != null)
                            {
                                willBeEdit.EmployeeID = request.Data.EmployeeID;
                                willBeEdit.FamilyRelationshipID = (short)request.Data.familyRelationshipID;
                                willBeEdit.Name = request.Data.Name;
                                willBeEdit.Gender = request.Data.Gender;
                                willBeEdit.MaritalStatus = request.Data.MaritalStatus;
                                willBeEdit.BirthDate = reformatDate(request.Data.BirthDateStr);
                                willBeEdit.KTPNumber = request.Data.KTPNumber;
                                willBeEdit.Address = request.Data.Address;
                                willBeEdit.BPJSNumber = request.Data.BPJSNumber;
                                willBeEdit.CityID = request.Data.CityID;
                                willBeEdit.Type = request.Data.Type;
                                willBeEdit.BloodType = request.Data.BloodType;
                                willBeEdit.ModifiedBy = request.Data.Account.UserName;
                                willBeEdit.ModifiedDate = DateTime.Now;
                                willBeEdit.Birthplace = request.Data.Birthplace;
                                resultUpdated = _context.SaveChanges();
                            }

                            var _patientclinicWillBeEdit = _context.PatientClinics.SingleOrDefault(x => x.PatientID == request.Data.Id && x.ClinicID == request.Data.Account.ClinicID);
                            if (_patientclinicWillBeEdit != null)
                            {
                                long _idPhoto = _patientclinicWillBeEdit.PhotoID ?? 0;
                                _patientclinicWillBeEdit.TempAddress = request.Data.PatientClinic.TempAddress;
                                _patientclinicWillBeEdit.TempCityID = request.Data.PatientClinic.TempCityId;
                                _patientclinicWillBeEdit.RefferenceNumber = request.Data.PatientClinic.RefferenceNumber;
                                _patientclinicWillBeEdit.RefferencePerson = request.Data.PatientClinic.RefferencePerson;
                                _patientclinicWillBeEdit.RefferenceRelation = request.Data.PatientClinic.RefferenceRelation;
                                _patientclinicWillBeEdit.ModifiedBy = request.Data.Account.UserName;
                                _patientclinicWillBeEdit.ModifiedDate = DateTime.Now;

                                resultUpdated = _context.SaveChanges();

                                var _existingPhotoId = _context.FileArchieves.SingleOrDefault(x => x.ID == _idPhoto && x.SourceTable == ClinicEnums.SourceTable.PATIENT.ToString());
                                if (_existingPhotoId != null)
                                {
                                    if (request.Data.file != null)//&& _existingPhotoId.ActualName != request.Data.file.FileName)
                                    {
                                        //need update
                                        if (request.Data.Photo == null)
                                            request.Data.Photo = new DocumentModel();
                                        var uploadDir = "~/fileDoc/Photo";
                                        request.Data.Photo.ActualPath = Path.Combine(HttpContext.Current.Server.MapPath(uploadDir), request.Data.file.FileName);
                                        request.Data.Photo.ActualName = request.Data.file.FileName;
                                        request.Data.Photo.TypeDoc = request.Data.file.ContentType;

                                        _existingPhotoId.ActualName = request.Data.Photo.ActualName;
                                        _existingPhotoId.ActualPath = request.Data.Photo.ActualPath;
                                        _existingPhotoId.TypeDoc = request.Data.Photo.TypeDoc;
                                        _existingPhotoId.ModifiedBy = request.Data.Account.UserName;
                                        _existingPhotoId.ModifiedDate = DateTime.Now;
                                        resultUpdated = _context.SaveChanges();
                                        request.Data.file.SaveAs(request.Data.Photo.ActualPath);
                                    }
                                }
                            }

                            transaction.Commit();
                            response.Status = true;
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated2, "Patient", willBeEdit.ID);
                        }
                        else if (request.Data.Id == 0 && cekIsPatientKeyExist != null && request.Data.IsUseExistingData == null)
                        {
                            //data pernah terdaftar di klinik lain
                            response.IsNeedConfirmation = true;
                            response.Message = "This Patient already registered in other clinic. Do you want to use an existing data or replace with new ones?";
                        }

                        else if (request.Data.Id == 0 && cekIsPatientKeyExist != null && request.Data.IsUseExistingData == false)
                        {
                            //insert new with different clinic
                            var patientKey = $"{request.Data.Name.Trim().Replace(" ", "")}_{request.Data.BirthDateStr.Replace("/", "")}";
                            var _cekIsExist = _unitOfWork.PatientRepository.GetFirstOrDefault(x => x.PatientKey == newPatientKey);
                            if (_cekIsExist != null)
                            {
                                var _tempPatientId = _cekIsExist.ID;

                                //handle photo
                                if (request.Data.file != null && request.Data.file.ContentLength > 0)
                                {
                                    if (request.Data.Photo == null)
                                        request.Data.Photo = new DocumentModel();

                                    var uploadDir = "~/fileDoc/Photo";
                                    request.Data.Photo.ActualPath = Path.Combine(HttpContext.Current.Server.MapPath(uploadDir), request.Data.file.FileName);
                                    request.Data.Photo.ActualName = request.Data.file.FileName;
                                    request.Data.Photo.TypeDoc = request.Data.file.ContentType;
                                }

                                if (request.Data.Photo != null)
                                {
                                    if (!String.IsNullOrEmpty(request.Data.Photo.ActualPath))
                                    {
                                        var _entityPhoto = Mapper.Map<DocumentModel, FileArchieve>(request.Data.Photo);
                                        _entityPhoto.SourceTable = ClinicEnums.SourceTable.PATIENT.ToString();
                                        _entityPhoto.CreatedBy = request.Data.Account.UserName;
                                        _entityPhoto.CreatedDate = DateTime.Now;
                                        _context.FileArchieves.Add(_entityPhoto);
                                        result = _context.SaveChanges();
                                        _photoID = _entityPhoto.ID;
                                    }
                                }

                                var _patientClinicEntity = new PatientClinic
                                {
                                    PatientID = _tempPatientId,
                                    ClinicID = request.Data.Account.ClinicID,
                                    OldMRNumber = request.Data.PatientClinic.OldMRNumber,
                                    TempAddress = request.Data.PatientClinic.TempAddress,
                                    TempCityID = request.Data.PatientClinic.TempCityId,
                                    RefferencePerson = request.Data.PatientClinic.RefferencePerson,
                                    RefferenceNumber = request.Data.PatientClinic.RefferenceNumber,
                                    RefferenceRelation = request.Data.PatientClinic.RefferenceRelation,
                                    CreatedBy = request.Data.Account.UserName,
                                    CreatedDate = DateTime.Now,
                                    PhotoID = _photoID
                                };

                                _context.PatientClinics.Add(_patientClinicEntity);
                                result = _context.SaveChanges();
                            }

                            transaction.Commit();
                            response.Status = true;
                            response.Message = string.Format(Messages.ObjectPatientUpdated, request.Data.Name, request.Data.Account.ClinicID);
                        }
                        else if (request.Data.Id == 0 && cekIsPatientKeyExist != null && request.Data.IsUseExistingData == true)
                        {
                            //insert new clinic in Patient Clinic
                            var patientKey = $"{request.Data.Name.Trim().Replace(" ", "")}_{request.Data.BirthDateStr.Replace("/", "")}";
                            var _cekIsExist = _unitOfWork.PatientRepository.GetFirstOrDefault(x => x.PatientKey == newPatientKey);
                            if (_cekIsExist != null)
                            {
                                var _tempPatientId = _cekIsExist.ID;
                                var _willCopyPatientClinic = _unitOfWork.PatientClinicRepository.GetFirstOrDefault(x => x.PatientID == _tempPatientId);
                                var _patientClinicEntity = new PatientClinic
                                {
                                    PatientID = _tempPatientId,
                                    ClinicID = request.Data.Account.ClinicID,
                                    OldMRNumber = _willCopyPatientClinic.OldMRNumber,
                                    TempAddress = _willCopyPatientClinic.TempAddress,
                                    TempCityID = _willCopyPatientClinic.TempCityID,
                                    RefferencePerson = _willCopyPatientClinic.RefferencePerson,
                                    RefferenceNumber = _willCopyPatientClinic.RefferenceNumber,
                                    RefferenceRelation = _willCopyPatientClinic.RefferenceRelation,
                                    CreatedBy = request.Data.Account.UserName,
                                    CreatedDate = DateTime.Now,
                                    PhotoID = _willCopyPatientClinic.PhotoID
                                };

                                _context.PatientClinics.Add(_patientClinicEntity);
                                result = _context.SaveChanges();
                                transaction.Commit();
                                response.Status = true;
                                response.Message = string.Format(Messages.ObjectPatientAdded, request.Data.Name, request.Data.Account.ClinicID);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    response.Status = false;
                    response.Message = Messages.GeneralError;

                    ErrorLog(ClinicEnums.Module.Patient, Constants.Command.ADD_EDIT_PATIENT, request.Data.Account, ex);
                }
            }

            return response;
        }

        public PatientResponse GetDetail(PatientRequest request)
        {
            PatientResponse _response = new PatientResponse();
            var _patientDetail = _unitOfWork.PatientRepository.GetFirstOrDefault(x => x.ID == request.Data.Id);
            if (_patientDetail != null)
            {
                _response.Entity = Mapper.Map<Patient, PatientModel>(_patientDetail);
            }

            if (_response.Entity.PatientClinic == null)
                _response.Entity.PatientClinic = new Entities.Patient.PatientClinicModel();
            if (_response.Entity.Photo == null)
                _response.Entity.Photo = new DocumentModel();

            var _patientClinicEntity = _unitOfWork.PatientClinicRepository.GetFirstOrDefault(x => x.ClinicID == request.Data.Account.ClinicID && x.PatientID == _patientDetail.ID);
            if (_patientClinicEntity != null)
            {
                _response.Entity.PatientClinic = Mapper.Map<PatientClinic, PatientClinicModel>(_patientClinicEntity);

                var _filePhotoEntity = _unitOfWork.FileArchiveRepository.GetFirstOrDefault(x => x.SourceTable == ClinicEnums.SourceTable.PATIENT.ToString() && x.ID == _patientClinicEntity.PhotoID);
                _response.Entity.Photo = Mapper.Map<FileArchieve, DocumentModel>(_filePhotoEntity);
            }

            return _response;
        }

        public PatientResponse GetListData(PatientRequest request)
        {
            long _clinicID = request.Data.Account.ClinicID;
            IList<long> clinicsID = _unitOfWork.PatientClinicRepository.Get(x => x.ClinicID == _clinicID).Select(x => x.PatientID).ToList();
            List<PatientModel> lists = new List<PatientModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Patient>(true);
            if (clinicsID.Count() > 0)
            {
                searchPredicate = searchPredicate.And(p => clinicsID.Contains(p.ID) && p.RowStatus == 0);
            }
            else
            {
                searchPredicate = searchPredicate.And(p => p.RowStatus == 0);
            }
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Name.Contains(request.SearchValue) ||
                p.Employee.EmpName.Contains(request.SearchValue) || p.KTPNumber.Contains(request.SearchValue) || p.Address.Contains(request.SearchValue) || p.Birthplace.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.PatientRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;
                        case "employee":
                            qry = _unitOfWork.PatientRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Employee.EmpName));
                            break;
                        case "birthdatestr":
                            qry = _unitOfWork.PatientRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.BirthDate));
                            break;

                        default:
                            qry = _unitOfWork.PatientRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.PatientRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;
                        case "employee":
                            qry = _unitOfWork.PatientRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Employee.EmpName));
                            break;
                        case "birthdatestr":
                            qry = _unitOfWork.PatientRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.BirthDate));
                            break;

                        default:
                            qry = _unitOfWork.PatientRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PatientRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                PatientModel psData = Mapper.Map<Patient, PatientModel>(item);
                if (psData.familyRelationshipID != 0)
                {
                    var employeeRelation = _unitOfWork.FamilyRelationshipRepository.GetById(psData.familyRelationshipID);
                    if (employeeRelation != null)
                        psData.familyRelationshipDesc = employeeRelation.Name;
                }
                var cityNm = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Type == Constants.MasterType.CITY && x.Value == psData.CityID.ToString());
                if (cityNm != null)
                    psData.CityNm = cityNm.Name;
                var patientType = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Type == Constants.MasterType.PATIENT_TYPE && x.Value == psData.Type.ToString());
                if (patientType != null)
                    psData.TypeDesc = patientType.Name;
                lists.Add(psData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PatientResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public PatientResponse RemoveData(PatientRequest request)
        {
            PatientResponse response = new PatientResponse();
            try
            {
                var patient = _unitOfWork.PatientRepository.GetById(request.Data.Id);
                var patientClinic = _unitOfWork.PatientClinicRepository.GetFirstOrDefault(x => x.PatientID == patient.ID && x.ClinicID == request.Data.Account.ClinicID);
                var fileArchive = _unitOfWork.FileArchiveRepository.GetById(patientClinic.PhotoID);
                if (patient.ID > 0)
                {
                    patient.RowStatus = -1;
                    _unitOfWork.PatientRepository.Update(patient);
                    if (patientClinic != null)
                    {
                        patientClinic.RowStatus = -1;
                        _unitOfWork.PatientClinicRepository.Update(patientClinic);
                    }

                    if (fileArchive != null)
                    {
                        fileArchive.RowStatus = -1;
                        _unitOfWork.FileArchiveRepository.Update(fileArchive);
                    }

                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved2, "Patient", patient.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Patient");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Patient");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.Patient, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        private string ProvideMRNo()
        {
            string _mrNumber = string.Empty;
            var countPatientInaMonth = _unitOfWork.PatientRepository.Get(x => x.CreatedDate.Year == DateTime.Now.Year && x.CreatedDate.Month == DateTime.Now.Month);
            if (countPatientInaMonth.Any())
            {
                var ctr = countPatientInaMonth.Count;
                switch (ctr.ToString().Length)
                {
                    case 1:
                        _mrNumber = $"00{ ++ctr}";
                        break;
                    case 2:
                        _mrNumber = $"0{ ++ctr}";
                        break;
                    case 3:
                        _mrNumber = $"{ ++ctr}";
                        break;
                }

                return _mrNumber;
            }
            else
            {
                return "001";
            }
        }
    }
}
