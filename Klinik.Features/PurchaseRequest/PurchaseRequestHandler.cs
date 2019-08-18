﻿using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.PurchaseRequest;
using Klinik.Entities.PurchaseRequestDetail;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseRequestHandler : BaseFeatures, IBaseFeatures<PurchaseRequestResponse, PurchaseRequestRequest>
    {
        public PurchaseRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseRequestResponse CreateOrEdit(PurchaseRequestRequest request)
        {
            PurchaseRequestResponse response = new PurchaseRequestResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PurchaseRequestRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new Data.DataRepository.PurchaseRequest
                        {
                            prnumber = qry.prnumber,
                            prdate = qry.prdate,
                            approve_by = qry.approve_by,
                            approve = qry.approve,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.prnumber = request.Data.prnumber;
                        qry.prdate = request.Data.prdate;
                        qry.request_by = request.Data.request_by;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;
                        qry.RowStatus = 0;

                        _unitOfWork.PurchaseRequestRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new PurchaseRequestModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PurchaseRequest", qry.prnumber, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.EDIT_PURCHASE_REQUEST, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseRequest");

                            CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.EDIT_PURCHASE_REQUEST, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseRequest");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.EDIT_PURCHASE_REQUEST, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var purhcaserequestEntity = new Data.DataRepository.PurchaseRequest
                    {
                        prnumber = request.Data.prnumber,
                        prdate = request.Data.prdate,
                        request_by = request.Data.request_by,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        RowStatus = 0
                    };

                    _unitOfWork.PurchaseRequestRepository.Insert(purhcaserequestEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new PurchaseRequestModel
                    {
                        Id = purhcaserequestEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PurchaseRequest", purhcaserequestEntity.prnumber, purhcaserequestEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.ADD_PURCHASE_REQUEST, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PurchaseRequest");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.ADD_PURCHASE_REQUEST, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.EDIT_PURCHASE_REQUEST, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.EDIT_PURCHASE_REQUEST, request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseRequestResponse GetDetail(PurchaseRequestRequest request)
        {
            PurchaseRequestResponse response = new PurchaseRequestResponse();

            var qry = _unitOfWork.PurchaseRequestRepository.GetById(request.Data.Id);
            //DeliveryOrderDetailModel newdeliveryOrderdetailModel = new DeliveryOrderDetailModel();
            if (qry != null)
            {
                response.Entity = new PurchaseRequestModel
                {
                    Id = qry.id,
                    prnumber = qry.prnumber,
                    prdate = qry.prdate,
                    approve_by = qry.approve_by,
                    request_by = qry.request_by,
                    approve = qry.approve,
                    ModifiedBy = qry.ModifiedBy,
                    CreatedBy = qry.CreatedBy,
                    ModifiedDate = qry.ModifiedDate,
                };

                foreach (var item in qry.PurchaseRequestDetails)
                {
                    var newpurchaserequestdetailModel = new PurchaseRequestDetailModel
                    {
                        Id = item.id,
                        PurchaseRequestId = qry.id,
                        ProductId = item.ProductId,
                        namabarang = item.namabarang,
                        tot_pemakaian = item.tot_pemakaian,
                        sisa_stok = item.sisa_stok,
                        qty = item.qty,
                        qty_add = item.qty_add,
                        reason_add = item.reason_add,
                        total = item.total,
                        nama_by_ho = item.nama_by_ho,
                        qty_by_ho = item.qty_by_ho,
                        remark_by_ho = item.remark_by_ho,
                    };

                    response.Entity.purchaserequestdetailModels.Add(newpurchaserequestdetailModel);
                }
            }

            return response;
        }

        public PurchaseRequestResponse GetListData(PurchaseRequestRequest request)
        {
            List<PurchaseRequestModel> lists = new List<PurchaseRequestModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Data.DataRepository.PurchaseRequest>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.prnumber.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "prnumber":
                            qry = _unitOfWork.PurchaseRequestRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.prnumber));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseRequestRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "prnumber":
                            qry = _unitOfWork.PurchaseRequestRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.prnumber));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseRequestRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PurchaseRequestRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = new PurchaseRequestModel
                {
                    Id = item.id,
                    prnumber = item.prnumber,
                    prdate = item.prdate,
                    approve_by = item.approve_by,
                    approve = item.approve,
                    request_by = item.request_by,
                    ModifiedBy = item.ModifiedBy,
                    CreatedBy = item.CreatedBy,
                    ModifiedDate = item.ModifiedDate,
                    createformat = GeneralHandler.FormatDate(item.CreatedDate)
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PurchaseRequestResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public PurchaseRequestResponse RemoveData(PurchaseRequestRequest request)
        {
            PurchaseRequestResponse response = new PurchaseRequestResponse();

            try
            {
                var deliveryoder = _unitOfWork.PurchaseRequestRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.RowStatus = -1;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseRequestRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseRequest", deliveryoder.prnumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequest");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequest");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUEST, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseRequestResponse ApproveData(PurchaseRequestRequest request)
        {
            PurchaseRequestResponse response = new PurchaseRequestResponse();

            try
            {
                var deliveryoder = _unitOfWork.PurchaseRequestRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.approve = 1;
                    deliveryoder.approve_by = request.Data.Account.UserCode;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseRequestRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseRequest", deliveryoder.prnumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequest");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequest");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUEST, ClinicEnums.Action.APPROVE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}