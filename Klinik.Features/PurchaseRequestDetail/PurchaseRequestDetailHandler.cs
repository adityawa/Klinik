using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.PurchaseRequestDetail;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseRequestDetailHandler : BaseFeatures, IBaseFeatures<PurchaseRequestDetailResponse, PurchaseRequestDetailRequest>
    {
        public PurchaseRequestDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseRequestDetailResponse CreateOrEdit(PurchaseRequestDetailRequest request)
        {
            PurchaseRequestDetailResponse response = new PurchaseRequestDetailResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    Data.DataRepository.PurchaseRequestDetail qry = _unitOfWork.PurchaseRequestDetailRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        Data.DataRepository.PurchaseRequestDetail _oldentity = new Data.DataRepository.PurchaseRequestDetail
                        {
                            ProductId = qry.ProductId,
                            PurchaseRequestId = qry.PurchaseRequestId,
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
                        qry.ProductId = request.Data.ProductId;
                        qry.PurchaseRequestId = request.Data.PurchaseRequestId;
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

                        _unitOfWork.PurchaseRequestDetailRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new PurchaseRequestDetailModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PurchaseRequestDetail", qry.namabarang, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEREQUESTDETAIL, Constants.Command.EDIT_PURCHASE_REQUEST_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseRequestDetail");

                            CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUESTDETAIL, Constants.Command.EDIT_PURCHASE_REQUEST_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseRequestDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUESTDETAIL, Constants.Command.EDIT_PURCHASE_REQUEST_DETAIL, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    Data.DataRepository.PurchaseRequestDetail purhcaserequestdetailEntity = new Data.DataRepository.PurchaseRequestDetail
                    {
                        ProductId = request.Data.ProductId,
                        PurchaseRequestId = request.Data.PurchaseRequestId,
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
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                    };

                    _unitOfWork.PurchaseRequestDetailRepository.Insert(purhcaserequestdetailEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new PurchaseRequestDetailModel
                    {
                        Id = purhcaserequestdetailEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PurchaseRequestDetail", purhcaserequestdetailEntity.namabarang, purhcaserequestdetailEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.ADD_PURCHASE_REQUEST_DETAIL, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PurchaseRequestDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.ADD_PURCHASE_REQUEST_DETAIL, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                {
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.EDIT_PURCHASE_REQUEST_DETAIL, request.Data.Account, ex);
                }
                else
                {
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUEST, Constants.Command.EDIT_PURCHASE_REQUEST_DETAIL, request.Data.Account, ex);
                }
            }

            return response;
        }

        public PurchaseRequestDetailResponse GetDetail(PurchaseRequestDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public PurchaseRequestDetailResponse GetListData(PurchaseRequestDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public PurchaseRequestDetailResponse RemoveData(PurchaseRequestDetailRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
