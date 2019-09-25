using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.PurchaseRequestPusatDetail;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseRequestPusatDetailHandler : BaseFeatures, IBaseFeatures<PurchaseRequestPusatDetailResponse, PurchaseRequestPusatDetailRequest>
    {
        public PurchaseRequestPusatDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseRequestPusatDetailResponse CreateOrEdit(PurchaseRequestPusatDetailRequest request)
        {
            PurchaseRequestPusatDetailResponse response = new PurchaseRequestPusatDetailResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    PurchaseRequestPusatDetail qry = _unitOfWork.PurchaseRequestPusatDetailRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        PurchaseRequestPusatDetail _oldentity = new PurchaseRequestPusatDetail
                        {
                            PurchaseRequestPusatId = qry.PurchaseRequestPusatId,
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
                            total = qry.total,
                            qty_unit = qry.qty_unit,
                            qty_box = qry.qty_box,
                            qty_final = qry.qty_final,
                            remark = qry.remark,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.PurchaseRequestPusatId = request.Data.PurchaseRequestPusatId;
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
                        qry.total = request.Data.total;
                        qry.qty_unit = request.Data.qty_unit;
                        qry.qty_box = request.Data.qty_box;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;
                        qry.qty_final = request.Data.qty_final;
                        qry.remark = request.Data.remark;

                        _unitOfWork.PurchaseRequestPusatDetailRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new PurchaseRequestPusatDetailModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PurchaseRequestPusatDetail", qry.namabarang, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSATDETAIL, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseRequestPusatDetail");

                            CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSATDETAIL, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseRequestPusatDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSATDETAIL, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    PurchaseRequestPusatDetail purhcaserequestpusatdetailEntity = new PurchaseRequestPusatDetail
                    {
                        PurchaseRequestPusatId = request.Data.PurchaseRequestPusatId,
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
                        total = request.Data.total,
                        qty_final = request.Data.qty_final,
                        remark = request.Data.remark,
                        qty_unit = request.Data.qty_unit,
                        qty_box = request.Data.qty_box,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        RowStatus = 0
                    };

                    _unitOfWork.PurchaseRequestPusatDetailRepository.Insert(purhcaserequestpusatdetailEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new PurchaseRequestPusatDetailModel
                    {
                        Id = purhcaserequestpusatdetailEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PurchaseRequestPusatDetail", purhcaserequestpusatdetailEntity.namabarang, purhcaserequestpusatdetailEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSATDETAIL, Constants.Command.ADD_PURCHASE_REQUEST_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PurchaseOrderPusatDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSATDETAIL, Constants.Command.ADD_PURCHASE_REQUEST_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                {
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSATDETAIL, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT_DETAIL, request.Data.Account, ex);
                }
                else
                {
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSATDETAIL, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT_DETAIL, request.Data.Account, ex);
                }
            }

            return response;
        }

        public PurchaseRequestPusatDetailResponse GetDetail(PurchaseRequestPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public PurchaseRequestPusatDetailResponse GetListData(PurchaseRequestPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public PurchaseRequestPusatDetailResponse RemoveData(PurchaseRequestPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
