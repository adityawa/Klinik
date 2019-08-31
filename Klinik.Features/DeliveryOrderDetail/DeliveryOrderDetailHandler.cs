using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.DeliveryOrderDetail;
using Klinik.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class DeliveryOrderDetailHandler : BaseFeatures, IBaseFeatures<DeliveryOrderDetailResponse, DeliveryOrderDetailRequest>
    {
        public DeliveryOrderDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DeliveryOrderDetailResponse CreateOrEdit(DeliveryOrderDetailRequest request)
        {
            DeliveryOrderDetailResponse response = new DeliveryOrderDetailResponse();
            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.DeliveryOrderDetailRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = Mapper.Map<DeliveryOrderDetail, DeliveryOrderDetailModel>(qry);

                        // update data
                        qry.DeliveryOderId = request.Data.DeliveryOderId;
                        qry.ProductId = request.Data.ProductId;
                        qry.namabarang = request.Data.namabarang;
                        qry.qty_request = request.Data.qty_request;
                        qry.nama_by_ho = request.Data.nama_by_ho;
                        qry.qty_by_HP = request.Data.qty_by_HP;
                        qry.remark_by_ho = request.Data.remark_by_ho;
                        qry.qty_adj = request.Data.qty_adj;
                        qry.remark_adj = request.Data.remark_adj;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.DeliveryOrderDetailRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "DeliveryOrderDetail", qry.namabarang, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.EDIT_DELIVERY_ORDER_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "DeliveryOrderDetail");

                            CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDERDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "DeliveryOrderDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDERDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_DETAIL, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var deliveryorderdetailEntity = Mapper.Map<DeliveryOrderDetailModel, DeliveryOrderDetail>(request.Data);

                    _unitOfWork.DeliveryOrderDetailRepository.Insert(deliveryorderdetailEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "DeliveryOrderDetail", deliveryorderdetailEntity.namabarang, deliveryorderdetailEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_DELIVERYORDERDETAIL, Constants.Command.ADD_DELIVERY_ORDER_DETAIL, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "DeliveryOrderDetail");

                        CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDERDETAIL, Constants.Command.ADD_DELIVERY_ORDER_DETAIL, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDERDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_DETAIL, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDERDETAIL, Constants.Command.EDIT_DELIVERY_ORDER_DETAIL, request.Data.Account, ex);
            }

            return response;
        }

        public DeliveryOrderDetailResponse GetDetail(DeliveryOrderDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public DeliveryOrderDetailResponse GetListData(DeliveryOrderDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public DeliveryOrderDetailResponse RemoveData(DeliveryOrderDetailRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
