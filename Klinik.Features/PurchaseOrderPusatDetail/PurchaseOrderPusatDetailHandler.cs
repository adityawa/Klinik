using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.PurchaseOrderPusatDetail;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseOrderPusatDetailHandler : BaseFeatures, IBaseFeatures<PurchaseOrderPusatDetailResponse, PurchaseOrderPusatDetailRequest>
    {
        public PurchaseOrderPusatDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseOrderPusatDetailResponse CreateOrEdit(PurchaseOrderPusatDetailRequest request)
        {
            PurchaseOrderPusatDetailResponse response = new PurchaseOrderPusatDetailResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    PurchaseOrderPusatDetail qry = _unitOfWork.PurchaseOrderPusatDetailRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        PurchaseOrderPusatDetail _oldentity = new PurchaseOrderPusatDetail
                        {
                            PurchaseOrderPusatId = qry.PurchaseOrderPusatId,
                            ProductId = qry.ProductId,
                            namabarang = qry.namabarang,
                            VendorId = qry.VendorId,
                            namavendor = qry.namavendor,
                            satuan = qry.satuan,
                            harga = qry.harga,
                            stok_prev = qry.stok_prev,
                            total_req = qry.total_req,
                            total_dist = qry.total_dist,
                            sisa_stok = qry.sisa_stok,
                            qty = qry.qty,
                            qty_add = qry.qty_add,
                            reason_add = qry.reason_add,
                            qty_final = qry.qty_final,
                            remark = qry.remark,
                            total = qry.total,
                            qty_unit = qry.qty_unit,
                            qty_box = qry.qty_box,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.PurchaseOrderPusatId = request.Data.PurchaseOrderPusatId;
                        qry.ProductId = request.Data.ProductId;
                        qry.namabarang = request.Data.namabarang;
                        qry.VendorId = request.Data.VendorId;
                        qry.namavendor = request.Data.namavendor;
                        qry.satuan = request.Data.satuan;
                        qry.harga = request.Data.harga;
                        qry.stok_prev = request.Data.stok_prev;
                        qry.total_req = request.Data.total_req;
                        qry.total_dist = request.Data.total_dist;
                        qry.sisa_stok = request.Data.sisa_stok;
                        qry.qty = request.Data.qty;
                        qry.qty_add = request.Data.qty_add;
                        qry.reason_add = request.Data.reason_add;
                        qry.qty_final = request.Data.qty_final;
                        qry.remark = request.Data.remark;
                        qry.total = request.Data.total;
                        qry.qty_unit = request.Data.qty_unit;
                        qry.qty_box = request.Data.qty_box;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.PurchaseOrderPusatDetailRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new PurchaseOrderPusatDetailModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PurchaseOrderPusatDetail", qry.namabarang, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseOrderPusatDetail");

                            CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERPUSATDETAIL, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseOrderPusatDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERDETAIL, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    PurchaseOrderPusatDetail purhcaseorderpusatdetailEntity = new PurchaseOrderPusatDetail
                    {
                        PurchaseOrderPusatId = request.Data.PurchaseOrderPusatId,
                        ProductId = request.Data.ProductId,
                        namabarang = request.Data.namabarang,
                        VendorId = request.Data.VendorId,
                        namavendor = request.Data.namavendor,
                        satuan = request.Data.satuan,
                        harga = request.Data.harga,
                        stok_prev = request.Data.stok_prev,
                        qty = request.Data.qty,
                        total_req = request.Data.total_req,
                        total_dist = request.Data.total_dist,
                        sisa_stok = request.Data.sisa_stok,
                        qty_add = request.Data.qty_add,
                        reason_add = request.Data.reason_add,
                        qty_final = request.Data.qty_final,
                        remark = request.Data.remark,
                        total = request.Data.total,
                        qty_unit = request.Data.qty_unit,
                        qty_box = request.Data.qty_box,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        RowStatus = 0
                    };

                    _unitOfWork.PurchaseOrderPusatDetailRepository.Insert(purhcaseorderpusatdetailEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new PurchaseOrderPusatDetailModel
                    {
                        Id = purhcaseorderpusatdetailEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PurchaseOrderPusatDetail", purhcaseorderpusatdetailEntity.namabarang, purhcaseorderpusatdetailEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEORDERPUSATDETAIL, Constants.Command.ADD_PURCHASE_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PurchaseOrderPusatDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERPUSATDETAIL, Constants.Command.ADD_PURCHASE_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                {
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDERPUSATDETAIL, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT_DETAIL, request.Data.Account, ex);
                }
                else
                {
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDERPUSATDETAIL, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT_DETAIL, request.Data.Account, ex);
                }
            }

            return response;
        }

        public PurchaseOrderPusatDetailResponse GetDetail(PurchaseOrderPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public PurchaseOrderPusatDetailResponse GetListData(PurchaseOrderPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public PurchaseOrderPusatDetailResponse RemoveData(PurchaseOrderPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
