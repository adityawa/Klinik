using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Features.MasterData;
using Klinik.Features.MasterData.GeneralMaster;
using Klinik.Features.MasterData.LookupCategory;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace Klinik.Features
{
    public class MasterHandler : BaseFeatures, IBaseFeatures<MasterResponse, MasterRequest>
    {
        public MasterHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<GeneralMaster> GetMasterDataByType(string type)
        {
            return _unitOfWork.MasterRepository.Query(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        public IQueryable<LookupCategory> GetLookupCategoryByName(string name)
        {
            return _unitOfWork.LookUpCategoryRepository.Query(x => x.TypeName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public List<string> GetRujukans()
        {
            return _unitOfWork.LetterRepository.Get().Select(x => x.OtherInfo).Distinct().ToList();
        }

        public List<LookupCategory> GetLookupCategories()
        {
            return _unitOfWork.LookUpCategoryRepository.GetAll();
        }

        public MasterResponse GetListData(MasterRequest request)
        {
            List<MasterModel> lists = new List<MasterModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Data.DataRepository.GeneralMaster>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.MasterRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.MasterRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.MasterRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.MasterRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.MasterRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<GeneralMaster, MasterModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            data = data.OrderBy(x => x.CategoryId).ToList();

            var response = new MasterResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public MasterResponse CreateOrEdit(MasterRequest request)
        {
            MasterResponse response = new MasterResponse();

            try
            {
                request.Data.Type = _unitOfWork.LookUpCategoryRepository.GetFirstOrDefault(x => x.ID == request.Data.CategoryId).TypeName;
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.MasterRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<GeneralMaster, MasterModel>(qry);
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        // update data
                        qry.CategoryId = request.Data.CategoryId;
                        qry.Type = request.Data.Type;
                        qry.Name = request.Data.Name;
                        qry.Value = request.Data.Value;

                        _unitOfWork.MasterRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "GeneralMaster", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_GENERAL, Constants.Command.EDIT_GENERAL_MASTER, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "GeneralMaster");

                            CommandLog(false, ClinicEnums.Module.MASTER_GENERAL, Constants.Command.EDIT_GENERAL_MASTER, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "GeneralMaster");

                        CommandLog(false, ClinicEnums.Module.MASTER_GENERAL, Constants.Command.EDIT_GENERAL_MASTER, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var lookupEntity = Mapper.Map<MasterModel, GeneralMaster>(request.Data);
                    lookupEntity.CreatedBy = request.Data.Account.UserCode;
                    lookupEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.MasterRepository.Insert(lookupEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "GeneralMaster", lookupEntity.Name, lookupEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_GENERAL, Constants.Command.ADD_GENERAL_MASTER, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "GeneralMaster");

                        CommandLog(false, ClinicEnums.Module.MASTER_GENERAL, Constants.Command.ADD_GENERAL_MASTER, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_GENERAL, Constants.Command.EDIT_GENERAL_MASTER, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_GENERAL, Constants.Command.ADD_GENERAL_MASTER, request.Data.Account, ex);
            }

            return response;
        }

        public MasterResponse GetDetail(MasterRequest request)
        {
            MasterResponse response = new MasterResponse();

            var qry = _unitOfWork.MasterRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<GeneralMaster, MasterModel>(qry.FirstOrDefault());
            }

            return response;
        }

        public MasterResponse RemoveData(MasterRequest request)
        {
            MasterResponse response = new MasterResponse();

            try
            {
                var generalMaster = _unitOfWork.MasterRepository.GetById(request.Data.Id);
                if (generalMaster.ID > 0)
                {
                    generalMaster.RowStatus = -1;
                    generalMaster.ModifiedBy = request.Data.Account.UserCode;
                    generalMaster.ModifiedDate = DateTime.Now;

                    _unitOfWork.MasterRepository.Update(generalMaster);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "GeneralMaster", generalMaster.Name, generalMaster.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "GeneralMaster");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "GeneralMaster");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_GENERAL, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;

        }
    }
    
}