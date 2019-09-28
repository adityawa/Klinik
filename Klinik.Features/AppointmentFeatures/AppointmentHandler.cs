using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.AppointmentEntities;
using Klinik.Entities.MasterData;
using Klinik.Entities.MCUPackageEntities;
using Klinik.Entities.PoliSchedules;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.AppointmentFeatures
{
    public class AppointmentHandler : BaseFeatures
    {

        public AppointmentHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public List<PoliModel> GetAllPoli()
        {
            List<PoliModel> polis = new List<PoliModel>();
            var _qry = _unitOfWork.PoliRepository.Get(x => x.RowStatus == 0);
            foreach (var item in _qry)
            {
                polis.Add(Mapper.Map<Poli, PoliModel>(item));
            }
            return polis;
        }

        public List<MCUPackageModel> GetMCUPackage()
        {
            List<MCUPackageModel> McuPakages = new List<MCUPackageModel>();
            var _qry = _unitOfWork.MCUpackageRepository.Get(x => x.RowStatus == 0);
            McuPakages.Insert(0, new MCUPackageModel
            {
                Id = 0,
                Name = "-"
            });

            foreach (var item in _qry)
            {
                McuPakages.Add(new MCUPackageModel
                {
                    Id = item.ID,
                    Name = item.Name
                });
            }
            return McuPakages;
        }

        public AppointmentResponse GetDoctorBasedOnPoli(AppointmentRequest request)
        {
            var response = new AppointmentResponse();
            List<PoliScheduleModel> availableSchedules = new List<PoliScheduleModel>();
            // DateTime start=new DateTime(request.Data.AppointmentDate.Year, request.Data.AppointmentDate.Month, request.Data.AppointmentDate.Day, )
            var qry = _unitOfWork.PoliScheduleRepository.Get(x => x.PoliID == request.Data.PoliID && (DbFunctions.TruncateTime(x.StartDate) <= request.Data.AppointmentDate && DbFunctions.TruncateTime(x.EndDate) >= request.Data.AppointmentDate));
            foreach (var item in qry)
            {
                availableSchedules.Add(Mapper.Map<PoliSchedule, PoliScheduleModel>(item));
            }

            var data = availableSchedules.Skip(request.Skip).Take(request.PageSize).ToList();

            response.schedules = data;
            response.RecordsTotal = availableSchedules.Count;
            response.RecordsFiltered = availableSchedules.Count;
            return response;
        }

        public AppointmentResponse SaveAppointment(AppointmentRequest request)
        {
            var response = new AppointmentResponse();
            int resultAffected = 0;
            try
            {
                var entity = Mapper.Map<AppointmentModel, Appointment>(request.Data);
                entity.CreatedBy = request.Data.Account.UserName;
                entity.CreatedDate = DateTime.Now;
                entity.Status = 1;

                _unitOfWork.AppointmentRepository.Insert(entity);
                resultAffected = _unitOfWork.Save();
                response.Status = true;
                response.Message =Messages.DataSaved;
            }
            catch(Exception )
            {
                response.Status = false;
                response.Message= Messages.GeneralError;
            }
            return response;
        }

        public AppointmentResponse GetListAppointment(AppointmentRequest request)
        {
            List<AppointmentModel> appointments = new List<AppointmentModel>();
            IQueryable<GeneralMaster> tempMaster = new MasterHandler(_unitOfWork).GetMasterDataByType(Constants.MasterType.NECESSITY_TYPE);
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Appointment>(true);
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Patient.Name.Contains(request.SearchValue) || p.Poli.Name.Contains(request.SearchValue) || p.Doctor.Name.Contains(request.SearchValue) || p.Clinic.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "EmpName":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Patient.Name));
                            break;
                        case "DoctorName":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Doctor.Name));
                            break;
                        case "PoliName":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Poli.Name));
                            break;
                        case "ClinicName":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Clinic.Name));
                            break;
                        case "AppointmentDate":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.AppointmentDate));
                            break;
                        default:
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "EmpName":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Patient.Name));
                            break;
                        case "DoctorName":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Doctor.Name));
                            break;
                        case "PoliName":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Poli.Name));
                            break;
                        case "ClinicName":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Clinic.Name));
                            break;
                        case "AppointmentDate":
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.AppointmentDate));
                            break;
                        default:
                            qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.AppointmentRepository.Get(searchPredicate, null);
            }

            foreach(var item in qry)
            {
                var appointmentData = Mapper.Map<Appointment, AppointmentModel>(item);
                int _requirementId = item.RequirementID;
                appointmentData.RequirementName = tempMaster.FirstOrDefault(x => x.Value ==  _requirementId.ToString())==null?"": tempMaster.FirstOrDefault(x => x.Value == _requirementId.ToString()).Name;
                appointments.Add(appointmentData);
            }

            int totalRequest = appointments.Count();
            var data = appointments.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new AppointmentResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public AppointmentResponse EditAppointment(AppointmentRequest request)
        {
            var response = new AppointmentResponse();
            int resultAffected = 0;
            try
            {
                var toBeUpdate = _unitOfWork.AppointmentRepository.GetById(request.Data.Id);

                if (toBeUpdate != null)
                {
                    toBeUpdate.PoliID = (Int32)request.Data.PoliID;
                    toBeUpdate.ClinicID = request.Data.ClinicID;
                    toBeUpdate.RequirementID = request.Data.RequirementID;
                    toBeUpdate.DoctorID = (Int32)request.Data.DoctorID;
                    toBeUpdate.ModifiedBy = request.Data.Account.UserName;
                    toBeUpdate.ModifiedDate = DateTime.Now;

                    resultAffected = _unitOfWork.Save();
                    response.Status = true;
                    response.Message = Messages.DataSaved;
                }
                else
                {
                    response.Status = false;
                    response.Message = Messages.NoDataUpdated;
                }
               
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }
            return response;
        }

        public AppointmentResponse RemoveAppointment(long AppointmentID)
        {
            int resultAffected = 0;
            var response = new AppointmentResponse();
            try
            {
              
                    _unitOfWork.AppointmentRepository.Delete(AppointmentID);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = true;
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved2, "Appointment", AppointmentID.ToString());
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = $"Appointment with Id {AppointmentID} failed to remove";
                    }
               
            }
            catch(Exception)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;
            }
            return response;
        }

        public AppointmentResponse GetDetailAppointment(long idAppointment)
        {
            var response = new AppointmentResponse();
            if (idAppointment > 0)
            {
                var _qryDetail = _unitOfWork.AppointmentRepository.GetById(idAppointment);
                if (_qryDetail != null)
                {
                    var _appointmentModel = Mapper.Map<Appointment, AppointmentModel>(_qryDetail);
                    response.Entity = _appointmentModel;
                }
            }
            return response;
        }
    }
}
