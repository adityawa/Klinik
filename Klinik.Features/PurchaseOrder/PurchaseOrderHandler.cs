using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.PurchaseOrder;
using Klinik.Entities.PurchaseOrderDetail;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseOrderHandler : BaseFeatures, IBaseFeatures<PurchaseOrderResponse, PurchaseOrderRequest>
    {
        public PurchaseOrderHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseOrderResponse CreateOrEdit(PurchaseOrderRequest request)
        {
            PurchaseOrderResponse response = new PurchaseOrderResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PurchaseOrderRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new PurchaseOrder
                        {
                            PurchaseRequestId = qry.PurchaseRequestId,
                            ponumber = qry.ponumber,
                            podate = qry.podate,
                            approve_by = qry.approve_by,
                            approve = qry.approve,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.PurchaseRequestId = qry.PurchaseRequestId;
                        qry.ponumber = request.Data.ponumber;
                        qry.podate = request.Data.podate;
                        qry.request_by = request.Data.request_by;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;
                        qry.RowStatus = 0;

                        _unitOfWork.PurchaseOrderRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new PurchaseOrderModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PurchaseOrder", qry.ponumber, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEORDER, Constants.Command.EDIT_PURCHASE_ORDER, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseOrder");

                            CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDER, Constants.Command.EDIT_PURCHASE_ORDER, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseOrder");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDER, Constants.Command.EDIT_PURCHASE_ORDER, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var purhcaseorderEntity = new PurchaseOrder
                    {
                        PurchaseRequestId = request.Data.PurchaseRequestId,
                        ponumber = request.Data.ponumber,
                        podate = request.Data.podate,
                        request_by = request.Data.request_by,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        RowStatus = 0
                    };

                    _unitOfWork.PurchaseOrderRepository.Insert(purhcaseorderEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new PurchaseOrderModel
                    {
                        Id = purhcaseorderEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PurchaseOrder", purhcaseorderEntity.ponumber, purhcaseorderEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEORDER, Constants.Command.ADD_PURCHASE_ORDER, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PurchaseOrder");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDER, Constants.Command.ADD_PURCHASE_ORDER, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDER, Constants.Command.EDIT_PURCHASE_ORDER, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDER, Constants.Command.EDIT_PURCHASE_ORDER, request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseOrderResponse GetDetail(PurchaseOrderRequest request)
        {
            PurchaseOrderResponse response = new PurchaseOrderResponse();

            var qry = _unitOfWork.PurchaseOrderRepository.GetById(request.Data.Id);
            //DeliveryOrderDetailModel newdeliveryOrderdetailModel = new DeliveryOrderDetailModel();
            if (qry != null)
            {
                response.Entity = new PurchaseOrderModel
                {
                    Id = qry.id,
                    PurchaseRequestId = qry.PurchaseRequestId,
                    ponumber = qry.ponumber,
                    podate = qry.podate,
                    approve_by = qry.approve_by,
                    request_by = qry.request_by,
                    approve = qry.approve,
                    ModifiedBy = qry.ModifiedBy,
                    CreatedBy = qry.CreatedBy,
                    ModifiedDate = qry.ModifiedDate,
                };

                foreach (var item in qry.PurchaseOrderDetails)
                {
                    var newpurchaseOrderdetailModel = new PurchaseOrderDetailModel
                    {
                        Id = item.id,
                        PurchaseOrderId = qry.id,
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

                    response.Entity.purchaseOrderdetailModels.Add(newpurchaseOrderdetailModel);
                }
            }

            return response;
        }

        public PurchaseOrderResponse GetListData(PurchaseOrderRequest request)
        {
            List<PurchaseOrderModel> lists = new List<PurchaseOrderModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<PurchaseOrder>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.ponumber.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "ponumber":
                            qry = _unitOfWork.PurchaseOrderRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ponumber));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseOrderRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "ponumber":
                            qry = _unitOfWork.PurchaseOrderRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ponumber));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseOrderRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PurchaseOrderRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = new PurchaseOrderModel
                {
                    Id = item.id,
                    PurchaseRequestId = item.PurchaseRequestId,
                    ponumber = item.ponumber,
                    podate = item.podate,
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

            var response = new PurchaseOrderResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public PurchaseOrderResponse RemoveData(PurchaseOrderRequest request)
        {
            PurchaseOrderResponse response = new PurchaseOrderResponse();

            try
            {
                var deliveryoder = _unitOfWork.PurchaseOrderRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.RowStatus = -1;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseOrderRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseOrder", deliveryoder.ponumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseOrder");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseOrder");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDER, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseOrderResponse ApproveData(PurchaseOrderRequest request)
        {
            PurchaseOrderResponse response = new PurchaseOrderResponse();

            try
            {
                var deliveryoder = _unitOfWork.PurchaseOrderRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.approve = 1;
                    deliveryoder.approve_by = request.Data.Account.UserCode;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseOrderRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseOrder", deliveryoder.ponumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseOrder");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseOrder");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDER, ClinicEnums.Action.APPROVE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
