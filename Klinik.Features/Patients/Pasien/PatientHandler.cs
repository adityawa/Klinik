using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Document;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Patients.Pasien
{
    public class PatientHandler:BaseFeatures, IBaseFeatures<PatientResponse, PatientRequest>
    {
        public PatientHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
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
                    if (!request.Data.IsaSamePerson)
                    {
                        var _entityPatient = Mapper.Map<PatientModel, Patient>(request.Data);
                        _entityPatient.MRNumber = $"{ DateTime.Now.Month}{DateTime.Now.Year.ToString().Substring(2, 2)}_{ProvideMRNo()}";
                        _entityPatient.CreatedBy = request.Data.Account.UserName;
                        _entityPatient.CreatedDate = DateTime.Now;
                        _context.Patients.Add(_entityPatient);
                        result = _context.SaveChanges();

                        if(!String.IsNullOrEmpty(request.Data.Photo.ActualPath))
                        {
                            var _entityPhoto = Mapper.Map<DocumentModel, FileArchieve>(request.Data.Photo);
                            _entityPhoto.CreatedBy = request.Data.Account.UserName;
                            _entityPhoto.CreatedDate = DateTime.Now;
                            _context.FileArchieves.Add(_entityPhoto);
                            result = _context.SaveChanges();
                            _photoID = _entityPhoto.ID;
                        }

                        //GetClinicID for current admin login

                        var _entityPatientClinic = new PatientClinic
                        {
                            ClinicID = request.Data.Account.clinicID,
                            PhotoID = _photoID,
                            PatientID = _entityPatient.ID,
                            TempAddress=request.Data.PatientClinic.TempAddress,
                            TempCityID=request.Data.PatientClinic.TempCityId,
                            RefferencePerson=request.Data.PatientClinic.RefferencePerson,
                            RefferenceNumber=request.Data.PatientClinic.RefferenceNumber,
                            RefferenceRelation=request.Data.PatientClinic.RefferenceRelation,
                            OldMRNumber=request.Data.PatientClinic.OldMRNumber,
                            CreatedBy=request.Data.Account.UserName,
                            CreatedDate=DateTime.Now
                        };

                        _context.PatientClinics.Add(_entityPatientClinic);
                        result = _context.SaveChanges();
                    }
                    transaction.Commit();

                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    response.Status = false;
                    response.Message = Messages.GeneralError;
                }
            }
            return response;
        }

        public PatientResponse GetDetail(PatientRequest request)
        {
            throw new NotImplementedException();
        }

        public PatientResponse GetListData(PatientRequest request)
        {
            throw new NotImplementedException();
        }

        public PatientResponse RemoveData(PatientRequest request)
        {
            throw new NotImplementedException();
        }

        private string ProvideMRNo()
        {
            string _mrNumber = string.Empty;
            var countPatientInaMonth = _unitOfWork.PatientRepository.Get(x => x.CreatedDate.Year == DateTime.Now.Year && x.CreatedDate.Month == DateTime.Now.Month);
            if(countPatientInaMonth.Any())
            {
                var ctr = countPatientInaMonth.Count;
                switch (ctr.ToString().Length)
                {
                    case 1:
                        _mrNumber= $"00{ ++ctr}";
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
