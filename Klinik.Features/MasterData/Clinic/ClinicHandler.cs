using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Features.MasterData.Clinic;
using System.Collections.Generic;
using System;
using System.Linq;
using LinqKit;

namespace Klinik.Features
{
    public class ClinicHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ClinicHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all clinic
        /// </summary>
        /// <returns></returns>
        public IList<ClinicModel> GetAllClinic()
        {
            var qry = _unitOfWork.ClinicRepository.Get();
            IList<ClinicModel> clinics = new List<ClinicModel>();
            foreach (var item in qry)
            {
                var _clinic = Mapper.Map<Clinic, ClinicModel>(item);
                clinics.Add(_clinic);
            }

            return clinics;
        }

        /// <summary>
        /// Create or edit a clinic
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ClinicResponse CreateOrEdit(ClinicRequest request)
        {
            int resultAffected = 0;

            ClinicResponse response = new ClinicResponse();
            try
            {
                if (request.RequestClinicModel.Id > 0)
                {
                    var qry = _unitOfWork.ClinicRepository.GetById(request.RequestClinicModel.Id);
                    if (qry != null)
                    {
                        qry.Name = request.RequestClinicModel.Name;
                        qry.Address = request.RequestClinicModel.Address;
                        qry.LegalNumber = request.RequestClinicModel.LegalNumber;
                        qry.LegalDate = request.RequestClinicModel.LegalDate;
                        qry.ContactNumber = request.RequestClinicModel.ContactNumber;
                        qry.Email = request.RequestClinicModel.Email;
                        qry.Lat = request.RequestClinicModel.Lat;
                        qry.Long = request.RequestClinicModel.Long;
                        qry.CityID = request.RequestClinicModel.CityId;
                        qry.ClinicType = request.RequestClinicModel.ClinicType;
                        qry.DateModified = DateTime.Now;
                        qry.LastUpdateBy = request.RequestClinicModel.ModifiedBy ?? "SYSTEM";
                        _unitOfWork.ClinicRepository.Update(qry);
                        resultAffected = _unitOfWork.Save();

                        if (resultAffected > 0)
                        {
                            response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                            response.Message = $"Success Update Clinic {qry.Name} with Id {qry.Code}";
                        }
                        else
                        {
                            response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                            response.Message = "Update Data Failed";
                        }
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = "Update Data Failed";
                    }
                }
                else
                {
                    var clinicEntity = Mapper.Map<ClinicModel, Clinic>(request.RequestClinicModel);
                    clinicEntity.CreatedBy = request.RequestClinicModel.CreatedBy ?? "SYSTEM";
                    clinicEntity.DateCreated = DateTime.Now;
                    _unitOfWork.ClinicRepository.Insert(clinicEntity);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                        response.Message = $"Success Add new Clinic {clinicEntity.Name} with Id {clinicEntity.Code}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = "Add Data Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg();
            }

            return response;
        }

        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ClinicResponse GetDetail(ClinicRequest request)
        {
            ClinicResponse response = new ClinicResponse();

            var qry = _unitOfWork.ClinicRepository.Query(x => x.ID == request.RequestClinicModel.Id);
            if (qry.FirstOrDefault() != null)
            {

                response.Entity = Mapper.Map<Clinic, ClinicModel>(qry.FirstOrDefault());
            }
            return response;
        }

        /// <summary>
        /// Get employee list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ClinicResponse GetListData(ClinicRequest request)
        {
            List<ClinicModel> lists = new List<ClinicModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Clinic>(true);
            if (!String.IsNullOrEmpty(request.searchValue) && !String.IsNullOrWhiteSpace(request.searchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Code.Contains(request.searchValue) || p.Name.Contains(request.searchValue));
            }

            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDir)))
            {
                if (request.sortColumnDir == "asc")
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "code":
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Code), includes: x => x.GeneralMaster);
                            break;
                        case "name":
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name), includes: x => x.GeneralMaster);
                            break;

                        default:
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID), includes: x => x.GeneralMaster);
                            break;
                    }
                }
                else
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "code":
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Code), includes: x => x.GeneralMaster);
                            break;
                        case "name":
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name), includes: x => x.GeneralMaster);
                            break;

                        default:
                            qry = _unitOfWork.ClinicRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID), includes: x => x.GeneralMaster);
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.ClinicRepository.Get(searchPredicate, null, includes: x => x.GeneralMaster);
            }
            foreach (var item in qry)
            {
                var clinicData = Mapper.Map<Clinic, ClinicModel>(item);
                long _cityId = clinicData.CityId ?? 0;
                long _clinicTypeId = clinicData.ClinicType ?? 0;
                var getCityDesc = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Id == _cityId && x.Type == ClinicEnums.enumMasterTypes.City.ToString());
                var getClinicTypeDesc = _unitOfWork.MasterRepository.GetFirstOrDefault(x => x.Id == _clinicTypeId && x.Type == ClinicEnums.enumMasterTypes.ClinicType.ToString());
                if (getCityDesc != null)
                    clinicData.CityDesc = getCityDesc.Name ?? "";
                if (getClinicTypeDesc != null)
                    clinicData.ClinicTypeDesc = getClinicTypeDesc.Name ?? "";
                lists.Add(clinicData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.skip).Take(request.pageSize).ToList();

            var response = new ClinicResponse
            {
                draw = request.draw,
                recordsFiltered = totalRequest,
                recordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove employee data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ClinicResponse RemoveData(ClinicRequest request)
        {
            ClinicResponse response = new ClinicResponse();
            int resultAffected = 0;
            try
            {
                var isExist = _unitOfWork.ClinicRepository.GetById(request.RequestClinicModel.Id);
                if (isExist.ID > 0)
                {
                    _unitOfWork.ClinicRepository.Delete(isExist.ID);
                    resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Status = ClinicEnums.enumStatus.SUCCESS.ToString();
                        response.Message = $"Success remove Clinic {isExist.Name} with Id {isExist.Code}";
                    }
                    else
                    {
                        response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                        response.Message = $"Remove Clinic Failed!";
                    }
                }
                else
                {
                    response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                    response.Message = $"Remove Clinic Failed!";
                }
            }
            catch (Exception ex)
            {
                response.Status = ClinicEnums.enumStatus.ERROR.ToString();
                response.Message = CommonUtils.GetGeneralErrorMesg(); ;
            }

            return response;
        }
    }
}