using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class DoctorHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public DoctorHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all Doctor
        /// </summary>
        /// <returns></returns>
        public IList<DoctorModel> GetAllDoctor()
        {
            var qry = _unitOfWork.DoctorRepository.Get();
            IList<DoctorModel> Doctors = new List<DoctorModel>();
            foreach (var item in qry)
            {
                var _Doctor = Mapper.Map<Doctor, DoctorModel>(item);
                Doctors.Add(_Doctor);
            }

            return Doctors;
        }

        /// <summary>
        /// Create or edit a Doctor
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DoctorResponse CreateOrEdit(DoctorRequest request)
        {
            DoctorResponse response = new DoctorResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.DoctorRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Doctor, DoctorModel>(qry);

                        // update data
                        qry.Name = request.Data.Name;
                        qry.Address = request.Data.Address;
                        qry.KTPNumber = request.Data.KTPNumber;
                        qry.STRNumber = request.Data.STRNumber;
                        qry.STRValidFrom = request.Data.STRValidFrom;
                        qry.STRValidTo = request.Data.STRValidTo;
                        qry.Email = request.Data.Email;
                        qry.HPNumber = request.Data.HPNumber;
                        qry.Remark = request.Data.Remark;
                        qry.SpesialisID = request.Data.SpesialisID;
                        qry.ModifiedDate = DateTime.Now;
                        qry.ModifiedBy = request.Data.ModifiedBy ?? "SYSTEM";

                        _unitOfWork.DoctorRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Doctor", qry.Name, qry.Code);

                            CommandLog(true, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.EDIT_DOCTOR, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Doctor");

                            CommandLog(false, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.EDIT_DOCTOR, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Doctor");

                        CommandLog(false, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.EDIT_DOCTOR, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var doctorEntity = Mapper.Map<DoctorModel, Doctor>(request.Data);
                    doctorEntity.CreatedBy = request.Data.CreatedBy ?? "SYSTEM";
                    doctorEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.DoctorRepository.Insert(doctorEntity);

                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Doctor", doctorEntity.Name, doctorEntity.Code);

                        CommandLog(true, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.ADD_NEW_DOCTOR, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Doctor");

                        CommandLog(false, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.ADD_NEW_DOCTOR, request.Data.Account, request.Data);
                    }
                }
            }
            catch
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null)
                {
                    if (request.Data.Id > 0)
                        CommandLog(false, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.EDIT_DOCTOR, request.Data.Account, request.Data);
                    else
                        CommandLog(false, ClinicEnums.Module.MASTER_DOCTOR, Constants.Command.ADD_NEW_DOCTOR, request.Data.Account, request.Data);
                }
            }

            return response;
        }

        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DoctorResponse GetDetail(DoctorRequest request)
        {
            DoctorResponse response = new DoctorResponse();

            var qry = _unitOfWork.DoctorRepository.Query(x => x.ID == request.Data.Id);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Doctor, DoctorModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get employee list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DoctorResponse GetListData(DoctorRequest request)
        {
            List<DoctorModel> lists = new List<DoctorModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Doctor>(true);
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Code.Contains(request.SearchValue) || p.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "code":
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Code));
                            break;
                        case "name":
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "code":
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Code));
                            break;
                        case "name":
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.DoctorRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.DoctorRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                DoctorModel doctorData = Mapper.Map<Doctor, DoctorModel>(item);
                bool isDoctor = doctorData.TypeID == (int)ParamedicTypeEnum.Doctor;
                doctorData.TypeName = isDoctor ? Messages.Doctor : Messages.Paramedic;
                if (isDoctor)
                {
                    long _doctorTypeId = doctorData.SpesialisID ?? 0;
                    var getDoctorTypeDesc = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Id == _doctorTypeId && x.Type == ClinicEnums.MasterTypes.DoctorType.ToString());
                    if (getDoctorTypeDesc != null)
                        doctorData.SpesialisName = getDoctorTypeDesc.Name ?? "";
                }
                else
                {
                    doctorData.SpesialisName = "-";
                }

                lists.Add(doctorData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new DoctorResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove employee data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DoctorResponse RemoveData(DoctorRequest request)
        {
            DoctorResponse response = new DoctorResponse();
            try
            {
                var isExist = _unitOfWork.DoctorRepository.GetById(request.Data.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.DoctorRepository.Delete(isExist.ID);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Doctor", isExist.Name, isExist.Code);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Doctor");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Doctor");
                }
            }
            catch
            {
                response.Status = false;
                response.Message = Messages.GeneralError; ;
            }

            return response;
        }
    }
}