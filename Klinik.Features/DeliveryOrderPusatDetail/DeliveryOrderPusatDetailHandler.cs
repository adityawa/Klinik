using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
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
                        var _oldentity = new DeliveryOrderPusatDetail
                        {
                            ProductId = qry.ProductId,
                            namabarang = qry.namabarang,
                            //ClinicId = qry.ClinicId,
                            //DeliveryOderPusatId = qry.DeliveryOderPusatId,
                            //ProductId_Po = qry.ProductId_Po,
                            //GudangId = qry.GudangId,
                            //qty_po = qry.qty_po,
                            //qty_po_final = qry.qty_po_final,
                            //qty_do = qry.qty_do,
                            //qty_adj = qry.qty_adj,
                            //remark_adj = qry.remark_adj,
                            //remark_do = qry.remark_do,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            //namabarang_po = qry.namabarang_po,
                            ModifiedBy = qry.ModifiedBy,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.ProductId = request.Data.ProductId;
                        qry.namabarang = request.Data.namabarang;
                        //qry.namabarang_po = request.Data.namabarang_po;
                        //qry.DeliveryOderPusatId = request.Data.DeliveryOderPusatId;
                        //qry.ClinicId = request.Data.ClinicId;
                        //qry.GudangId = request.Data.GudangId;
                        //qry.ProductId_Po = request.Data.ProductId_Po;
                        //qry.qty_po = request.Data.qty_po;
                        //qry.qty_po_final = request.Data.qty_po_final;
                        //qry.qty_do = request.Data.qty_do;
                        //qry.qty_adj = request.Data.qty_adj;
                        //qry.remark_adj = request.Data.remark_adj;
                        //qry.remark_do = request.Data.remark_do;
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
                    var deliveryorderdetailEntity = new DeliveryOrderPusatDetail
                    {
                        ProductId = request.Data.ProductId,
                        //DeliveryOderPusatId = request.Data.DeliveryOderPusatId,
                        namabarang = request.Data.namabarang,
                        //namabarang_po = request.Data.namabarang_po,
                        //GudangId = request.Data.GudangId,
                        //ClinicId = request.Data.ClinicId,
                        //ProductId_Po = request.Data.ProductId_Po,
                        //qty_po = request.Data.qty_po,
                        //qty_po_final = request.Data.qty_po_final,
                        //qty_do = request.Data.qty_do,
                        //qty_adj = request.Data.qty_adj,
                        //remark_adj = request.Data.remark_adj,
                        //remark_do = request.Data.remark_do,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                    };

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
