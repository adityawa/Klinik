using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;

namespace Klinik.Features
{
    public class GudangHandler : BaseFeatures, IBaseFeatures<GudangResponse, GudangRequest>
    {
        public GudangHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public GudangResponse CreateOrEdit(GudangRequest request)
        {
            GudangResponse response = new GudangResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.GudangRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new Gudang
                        {
                            name = qry.name,
                            ClinicId = qry.ClinicId,
                            ModifiedBy = request.Data.Account.UserCode,
                            ModifiedDate = DateTime.Now,
                        };

                        // update data
                        qry.name = request.Data.name;
                        qry.ClinicId = request.Data.ClinicId;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.GudangRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Gudang", qry.name, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_GUDANG, Constants.Command.EDIT_GUDANG, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Gudang");

                            CommandLog(false, ClinicEnums.Module.MASTER_GUDANG, Constants.Command.EDIT_GUDANG, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Gudang");

                        CommandLog(false, ClinicEnums.Module.MASTER_GUDANG, Constants.Command.EDIT_GUDANG, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var gudangEntity = new Gudang
                    {
                        name = request.Data.name,
                        ClinicId = request.Data.ClinicId,
                        CreatedBy = request.Data.Account.UserCode,
                        RowStatus = 0,
                        CreatedDate = DateTime.Now,
                    };

                    _unitOfWork.GudangRepository.Insert(gudangEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Product", gudangEntity.name, gudangEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_GUDANG, Constants.Command.ADD_GUDANG, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "gudang");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT, Constants.Command.ADD_GUDANG, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_GUDANG, Constants.Command.EDIT_GUDANG, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_GUDANG, Constants.Command.EDIT_GUDANG, request.Data.Account, ex);
            }

            return response;
        }

        public GudangResponse GetDetail(GudangRequest request)
        {
            GudangResponse response = new GudangResponse();

            var qry = _unitOfWork.GudangRepository.Query(x => x.id == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = new GudangModel
                {
                    ClinicId = qry.FirstOrDefault().ClinicId,
                    Id = qry.FirstOrDefault().id,
                    name = qry.FirstOrDefault().name,
                    ClinicName = qry.FirstOrDefault().Clinic.Name,
                    IsGudangPusat = qry.FirstOrDefault().IsGudangPusat
                };
            }

            return response;
        }

        public GudangResponse GetListData(GudangRequest request)
        {
            List<GudangModel> lists = new List<GudangModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Gudang>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.GudangRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.name));
                            break;

                        default:
                            qry = _unitOfWork.GudangRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.GudangRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.name));
                            break;

                        default:
                            qry = _unitOfWork.GudangRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.GudangRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = new GudangModel {
                    ClinicId = item.ClinicId,
                    Id = item.id,
                    name = item.name,
                    ClinicName = item.Clinic.Name
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new GudangResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public GudangResponse RemoveData(GudangRequest request)
        {
            GudangResponse response = new GudangResponse();

            try
            {
                var gudang = _unitOfWork.GudangRepository.GetById(request.Data.Id);
                if (gudang.id > 0)
                {
                    gudang.RowStatus = -1;
                    gudang.ModifiedBy = request.Data.Account.UserCode;
                    gudang.ModifiedDate = DateTime.Now;

                    _unitOfWork.GudangRepository.Update(gudang);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Gudang", gudang.name, gudang.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Gudang");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Gudang");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_GUDANG, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
