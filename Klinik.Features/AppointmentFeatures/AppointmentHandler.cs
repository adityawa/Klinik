using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.AppointmentEntities;
using Klinik.Entities.MasterData;
using Klinik.Entities.MCUPackageEntities;
using Klinik.Entities.PoliSchedules;
using Klinik.Resources;
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
    }
}
