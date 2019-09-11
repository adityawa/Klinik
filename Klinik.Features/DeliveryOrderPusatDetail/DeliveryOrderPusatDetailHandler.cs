using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.DeliveryOrderPusatDetail;
using Klinik.Features.Account;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class DeliveryOrderPusatDetailHandler : BaseFeatures, IBaseFeatures<DeliveryOrderPusatDetailResponse, DeliveryOrderPusatDetailRequest>
    {
        public DeliveryOrderPusatDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DeliveryOrderPusatDetailResponse CreateOrEdit(DeliveryOrderPusatDetailRequest request)
        {
            DeliveryOrderPusatDetailResponse response = new DeliveryOrderPusatDetailResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.DeliveryOrderPusatDetailRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = Mapper.Map<DeliveryOrderPusatDetail, DeliveryOrderPusatDetailModel>(qry);

                        // update data
                        qry.ProductId = request.Data.ProductId;
                        qry.namabarang = request.Data.namabarang;
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
                        qry.Recived = request.Data.Recived;
                        qry.No_Do_Vendor = request.Data.No_Do_Vendor;
                        qry.Tgl_Do_Vendor = request.Data.Tgl_Do_Vendor;
                        qry.SendBy = request.Data.SendBy;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.DeliveryOrderPusatDetailRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "DeliveryOrderPusatDetail", qry.namabarang, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_DELIVERYORDERPUSATDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "DeliveryOrderPusatDetail");

                            CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDERDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "DeliveryOrderPusatDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDERPUSATDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var deliveryorderdetailEntity = Mapper.Map<DeliveryOrderPusatDetailModel, DeliveryOrderPusatDetail>(request.Data);
                    deliveryorderdetailEntity.CreatedBy = OneLoginSession.Account.UserCode;
                    deliveryorderdetailEntity.CreatedDate = DateTime.Now;
                    _unitOfWork.DeliveryOrderPusatDetailRepository.Insert(deliveryorderdetailEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "DeliveryOrderPusatDetail", deliveryorderdetailEntity.namabarang, deliveryorderdetailEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_DELIVERYORDERPUSATDETAIL, Constants.Command.ADD_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "DeliveryOrderPusatDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDERPUSATDETAIL, Constants.Command.ADD_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDERPUSATDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDERPUSATDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, ex);
            }

            return response;
        }

        public DeliveryOrderPusatDetailResponse GetDetail(DeliveryOrderPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public DeliveryOrderPusatDetailResponse GetListData(DeliveryOrderPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public DeliveryOrderPusatDetailResponse RemoveData(DeliveryOrderPusatDetailRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
