using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.PurchaseRequestConfig;
using Klinik.Features.Account;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseRequestConfigHandler : BaseFeatures, IBaseFeatures<PurchaseRequestConfigResponse, PurchaseRequestConfigRequest>
    {
        public PurchaseRequestConfigHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = new KlinikDBEntities();
        }

        public PurchaseRequestConfigResponse CreateOrEdit(PurchaseRequestConfigRequest request)
        {
            PurchaseRequestConfigResponse response = new PurchaseRequestConfigResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PurchaseRequestConfigRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = Mapper.Map<Data.DataRepository.PurchaseRequestConfig, PurchaseRequestConfigModel>(qry);
                        qry.StartDate = request.Data.StartDate;
                        qry.ModifiedDate = request.Data.ModifiedDate;
                        qry.ModifiedBy = OneLoginSession.Account.UserCode;

                        _unitOfWork.PurchaseRequestConfigRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            
                        }
                        else
                        {
                            
                        }
                    }
                }
                else
                {
                    var insertdata = Mapper.Map<PurchaseRequestConfigModel, Data.DataRepository.PurchaseRequestConfig>(request.Data);
                    insertdata.CreatedBy = request.Data.Account.UserCode;
                    insertdata.CreatedDate = DateTime.Now;
                    insertdata.GudangId = OneLoginSession.Account.GudangID;

                    _unitOfWork.PurchaseRequestConfigRepository.Insert(insertdata);

                    int resultAffected = _unitOfWork.Save();
                }
            }
            catch (Exception )
            {

            }

            return response;
        }
        public PurchaseRequestConfigResponse GetDetail(PurchaseRequestConfigRequest request)
        {
            PurchaseRequestConfigResponse response = new PurchaseRequestConfigResponse();

            var qry = _unitOfWork.PurchaseRequestConfigRepository.Query(x => x.id == request.Data.Id);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Data.DataRepository.PurchaseRequestConfig, PurchaseRequestConfigModel>(qry.FirstOrDefault());
            }

            return response;
        }

        public PurchaseRequestConfigResponse GetListData(PurchaseRequestConfigRequest request)
        {
            List<PurchaseRequestConfigModel> lists = new List<PurchaseRequestConfigModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Data.DataRepository.PurchaseRequestConfig>(true);

            searchPredicate = searchPredicate.And(x => x.RowStatus == 0 && x.GudangId == OneLoginSession.Account.GudangID);

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "startdate":
                            qry = _unitOfWork.PurchaseRequestConfigRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.StartDate));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseRequestConfigRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "startdate":
                            qry = _unitOfWork.PurchaseRequestConfigRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.StartDate));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseRequestConfigRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PurchaseRequestConfigRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prc = Mapper.Map<Data.DataRepository.PurchaseRequestConfig, PurchaseRequestConfigModel>(item);
                lists.Add(prc);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PurchaseRequestConfigResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public PurchaseRequestConfigResponse RemoveData(PurchaseRequestConfigRequest request)
        {
            PurchaseRequestConfigResponse response = new PurchaseRequestConfigResponse();

            try
            {
                var purchaseorderconfig = _unitOfWork.PurchaseRequestConfigRepository.GetById(request.Data.Id);
                if (purchaseorderconfig.id > 0)
                {
                    purchaseorderconfig.RowStatus = -1;
                    purchaseorderconfig.ModifiedBy = request.Data.Account.UserCode;
                    purchaseorderconfig.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseRequestConfigRepository.Update(purchaseorderconfig);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Gudang", purchaseorderconfig.Gudang.name, purchaseorderconfig.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Config");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Config");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDER, ClinicEnums.Action.APPROVE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
