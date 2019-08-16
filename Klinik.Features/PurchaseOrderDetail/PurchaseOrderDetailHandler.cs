using Klinik.Data;
using Klinik.Data.DataRepository;
using System;

namespace Klinik.Features
{
    public class PurchaseOrderDetailHandler : BaseFeatures, IBaseFeatures<PurchaseOrderDetailResponse, PurchaseOrderDetailRequest>
    {
        public PurchaseOrderDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseOrderDetailResponse CreateOrEdit(PurchaseOrderDetailRequest request)
        {
            PurchaseOrderDetailResponse response = new PurchaseOrderDetailResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PurchaseOrderDetailRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new Data.DataRepository.PurchaseOrderDetail
                        {
                            PurchaseOrderId = qry.PurchaseOrderId,
                            ProductId = qry.ProductId,
                            namabarang = qry.namabarang,
                            tot_pemakaian = qry.tot_pemakaian,
                            sisa_stok = qry.sisa_stok,
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

        public PurchaseOrderDetailResponse GetDetail(PurchaseOrderDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public PurchaseOrderDetailResponse GetListData(PurchaseOrderDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public PurchaseOrderDetailResponse RemoveData(PurchaseOrderDetailRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
