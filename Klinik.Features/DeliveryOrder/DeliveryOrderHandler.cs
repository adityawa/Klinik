using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.DeliveryOrder;
using Klinik.Entities.DeliveryOrderDetail;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class DeliveryOrderHandler : BaseFeatures, IBaseFeatures<DeliveryOrderResponse, DeliveryOrderRequest>
    {
        public DeliveryOrderHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DeliveryOrderResponse CreateOrEdit(DeliveryOrderRequest request)
        {
            DeliveryOrderResponse response = new DeliveryOrderResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.DeliveryOrderRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new DeliveryOrder
                        { 
                            poid = qry.poid,
                            donumber = qry.donumber,
                            dodate = qry.dodate,
                            dodest = qry.dodest,
                            approve_by = qry.approve_by,
                            approve = qry.approve,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.poid = qry.id;
                        qry.donumber = request.Data.donumber;
                        qry.dodate = request.Data.dodate;
                        qry.dodest = request.Data.dodest;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;
                        qry.RowStatus = 0;

                        _unitOfWork.DeliveryOrderRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new DeliveryOrderModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "DeliveryOrder", qry.donumber, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.EDIT_DELIVERY_ORDER, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "DeliveryOrder");

                            CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.EDIT_DELIVERY_ORDER, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "DeliveryOrder");

                        CommandLog(false, ClinicEnums.Module.MASTER_GUDANG, Constants.Command.EDIT_GUDANG, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var deliveryorderEntity = new DeliveryOrder
                    {
                        poid = request.Data.poid,
                        donumber = request.Data.donumber,
                        dodate = request.Data.dodate,
                        dodest = request.Data.dodest,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        RowStatus = 0
                    };
                    
                    _unitOfWork.DeliveryOrderRepository.Insert(deliveryorderEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new DeliveryOrderModel
                    {
                        Id = deliveryorderEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "DeliveryOrder", deliveryorderEntity.donumber, deliveryorderEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.ADD_DELIVERY_ORDER, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "DeliveryOrder");

                        CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.ADD_DELIVERY_ORDER, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.EDIT_DELIVERY_ORDER, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.EDIT_DELIVERY_ORDER, request.Data.Account, ex);
            }

            return response;
        }

        public DeliveryOrderResponse GetDetail(DeliveryOrderRequest request)
        {
            DeliveryOrderResponse response = new DeliveryOrderResponse();

            var qry = _unitOfWork.DeliveryOrderRepository.GetById(request.Data.Id);
            //DeliveryOrderDetailModel newdeliveryOrderdetailModel = new DeliveryOrderDetailModel();
            if (qry != null)
            {
                response.Entity = new DeliveryOrderModel
                {
                    Id = qry.id,
                    poid = qry.poid,
                    donumber = qry.donumber,
                    dodate = qry.dodate,
                    dodest = qry.dodest,
                    approve_by = qry.approve_by,
                    approve = qry.approve,
                    ModifiedBy = qry.ModifiedBy,
                    CreatedBy = qry.CreatedBy,
                    ModifiedDate = qry.ModifiedDate,
                };

                foreach (var item in qry.DeliveryOrderDetails)
                {
                    var newdeliveryOrderdetailModel = new DeliveryOrderDetailModel
                    {
                        Id = item.id,
                        DeliveryOderId = qry.id,
                        ProductId_Po = item.ProductId_Po,
                        namabarang_po = item.namabarang_po,
                        qty_po = item.qty_po,
                        qty_po_final = item.qty_po_final,
                        ProductId = item.ProductId,
                        namabarang = item.namabarang,
                        GudangId = item.GudangId,
                        ClinicId = item.ClinicId,
                        qty_do = item.qty_do,
                        remark_do = item.remark_do,
                        qty_adj = item.qty_adj,
                        remark_adj = item.remark_adj,
                        namagudang = item.Gudang.name,
                        namaklinik = item.Clinic.Name,
                };

                    response.Entity.deliveryOrderDetailModels.Add(newdeliveryOrderdetailModel);
                }
            }
            
            return response;
        }

        public DeliveryOrderResponse GetListData(DeliveryOrderRequest request)
        {
            List<DeliveryOrderModel> lists = new List<DeliveryOrderModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<DeliveryOrder>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.donumber.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "donumber":
                            qry = _unitOfWork.DeliveryOrderRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.donumber));
                            break;

                        default:
                            qry = _unitOfWork.DeliveryOrderRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "donumber":
                            qry = _unitOfWork.DeliveryOrderRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.donumber));
                            break;

                        default:
                            qry = _unitOfWork.DeliveryOrderRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.DeliveryOrderRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = new DeliveryOrderModel
                {
                    Id = item.id,
                    poid = item.poid,
                    donumber = item.donumber,
                    dodate = item.dodate,
                    dodest = item.dodest,
                    approve_by = item.approve_by,
                    approve = item.approve,
                    ModifiedBy = item.ModifiedBy,
                    CreatedBy = item.CreatedBy,
                    ModifiedDate = item.ModifiedDate,
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new DeliveryOrderResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public DeliveryOrderResponse RemoveData(DeliveryOrderRequest request)
        {
            DeliveryOrderResponse response = new DeliveryOrderResponse();

            try
            {
                var deliveryoder = _unitOfWork.DeliveryOrderRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.RowStatus = -1;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.DeliveryOrderRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "DeliveryOrder", deliveryoder.donumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "DeliveryOrder");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "DeliveryOrder");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDER, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        public DeliveryOrderResponse ApproveData(DeliveryOrderRequest request)
        {
            DeliveryOrderResponse response = new DeliveryOrderResponse();

            try
            {
                var deliveryoder = _unitOfWork.DeliveryOrderRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.approve = 1;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.DeliveryOrderRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "DeliveryOrder", deliveryoder.donumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "DeliveryOrder");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "DeliveryOrder");
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
