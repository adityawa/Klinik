using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.PurchaseOrderDetail;
using Klinik.Resources;
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
                    Data.DataRepository.PurchaseOrderDetail qry = _unitOfWork.PurchaseOrderDetailRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        Data.DataRepository.PurchaseOrderDetail _oldentity = new Data.DataRepository.PurchaseOrderDetail
                        {
                            PurchaseOrderId = qry.PurchaseOrderId,
                            ProductId = qry.ProductId,
                            namabarang = qry.namabarang,
                            tot_pemakaian = qry.tot_pemakaian,
                            sisa_stok = qry.sisa_stok,
                            qty = qry.qty,
                            qty_add = qry.qty_add,
                            reason_add = qry.reason_add,
                            total = qry.total,
                            nama_by_ho = qry.nama_by_ho,
                            qty_by_ho = qry.qty_by_ho,
                            remark_by_ho = qry.remark_by_ho,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.PurchaseOrderId = request.Data.PurchaseOrderId;
                        qry.ProductId = request.Data.ProductId;
                        qry.namabarang = request.Data.namabarang;
                        qry.tot_pemakaian = request.Data.tot_pemakaian;
                        qry.sisa_stok = request.Data.sisa_stok;
                        qry.qty = request.Data.qty;
                        qry.qty_add = request.Data.qty_add;
                        qry.reason_add = request.Data.reason_add;
                        qry.total = request.Data.total;
                        qry.nama_by_ho = request.Data.nama_by_ho;
                        qry.qty_by_ho = request.Data.qty_by_ho;
                        qry.remark_by_ho = request.Data.remark_by_ho;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.PurchaseOrderDetailRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new PurchaseOrderDetailModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PurchaseOrderDetail", qry.namabarang, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEORDER, Constants.Command.EDIT_PURCHASE_ORDER_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseOrderDetail");

                            CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERDETAIL, Constants.Command.EDIT_PURCHASE_ORDER_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseOrderDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERDETAIL, Constants.Command.EDIT_PURCHASE_ORDER_DETAIL, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    Data.DataRepository.PurchaseOrderDetail purhcaseorderdetailEntity = new Data.DataRepository.PurchaseOrderDetail
                    {
                        PurchaseOrderId = request.Data.PurchaseOrderId,
                        ProductId = request.Data.ProductId,
                        namabarang = request.Data.namabarang,
                        tot_pemakaian = request.Data.tot_pemakaian,
                        sisa_stok = request.Data.sisa_stok,
                        qty = request.Data.qty,
                        qty_add = request.Data.qty_add,
                        reason_add = request.Data.reason_add,
                        total = request.Data.total,
                        nama_by_ho = request.Data.nama_by_ho,
                        qty_by_ho = request.Data.qty_by_ho,
                        remark_by_ho = request.Data.remark_by_ho,
                        ModifiedBy = request.Data.Account.UserCode,
                        ModifiedDate = DateTime.Now,
                    };

                    _unitOfWork.PurchaseOrderDetailRepository.Insert(purhcaseorderdetailEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new PurchaseOrderDetailModel
                    {
                        Id = purhcaseorderdetailEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PurchaseOrderDetail", purhcaseorderdetailEntity.namabarang, purhcaseorderdetailEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEORDERDETAIL, Constants.Command.ADD_PURCHASE_ORDER_DETAIL, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PurchaseOrderDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERDETAIL, Constants.Command.ADD_PURCHASE_ORDER_DETAIL, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                {
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDERDETAIL, Constants.Command.EDIT_PURCHASE_ORDER_DETAIL, request.Data.Account, ex);
                }
                else
                {
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDERDETAIL, Constants.Command.EDIT_PURCHASE_ORDER_DETAIL, request.Data.Account, ex);
                }
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
