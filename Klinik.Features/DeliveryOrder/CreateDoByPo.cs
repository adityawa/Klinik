using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.DeliveryOrder;
using Klinik.Entities.DeliveryOrderDetail;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseOrder;
using Klinik.Entities.PurchaseOrderDetail;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class CreateDoByPo : BaseFeatures
    {
        public CreateDoByPo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(PurchaseOrderResponse _response)
        {
            var searchPredicate = PredicateBuilder.New<Data.DataRepository.DeliveryOrder>(true);
            _response.Entity.approve = null;
            _response.Entity.approveby = null;

            var request = new DeliveryOrderRequest
            {
                Data = Mapper.Map<PurchaseOrderModel, DeliveryOrderModel>(_response.Entity)
            };
            request.Data.dodate = DateTime.Now;
            request.Data.poid = Convert.ToInt32(_response.Entity.Id);
            request.Data.Id = 0;

            var lastponumber = _unitOfWork.DeliveryOrderRepository.Get(searchPredicate, orderBy: a => a.OrderByDescending(x => x.CreatedDate)).Select(a => a.donumber).FirstOrDefault();
            DateTime? getmonth = _unitOfWork.DeliveryOrderRepository.Get(searchPredicate, orderBy: a => a.OrderByDescending(x => x.CreatedDate)).Select(a => a.dodate).FirstOrDefault();
            DateTime? month = getmonth != null ? getmonth : DateTime.Now;
            string ponumber = lastponumber != null ? GeneralHandler.stringincrement(lastponumber, Convert.ToDateTime(month)) : "00001";

            request.Data.donumber = "DO" + _response.Entity.Account.Organization + DateTime.Now.Year + DateTime.Now.Month + ponumber;
            request.Data.Account = _response.Entity.Account;

            DeliveryOrderResponse deliveryorderresponse = new DeliveryOrderResponse();

            new DeliveryOrderValidator(_unitOfWork).Validate(request, out deliveryorderresponse);

            if (_response.Entity.PurchaseOrderDetails != null)
            {
                int i = 0;
                foreach (var item in _response.Entity.PurchaseOrderDetails)
                {
                    var deliveryorderdetailrequest = new DeliveryOrderDetailRequest
                    {
                        Data = Mapper.Map<PurchaseOrderDetailModel, DeliveryOrderDetailModel>(item)
                    };
                    deliveryorderdetailrequest.Data.DeliveryOderId = Convert.ToInt32(deliveryorderresponse.Entity.Id);
                    deliveryorderdetailrequest.Data.Account = _response.Entity.Account;
                    deliveryorderdetailrequest.Data.Id = 0;
                    //
                    var requestnamabarang = new ProductRequest
                    {
                        Data = new ProductModel
                        {
                            Id = Convert.ToInt32(item.ProductId)
                        }
                    };

                    ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                    deliveryorderdetailrequest.Data.namabarang = namabarang.Entity.Name;
                    DeliveryOrderDetailResponse _deliveryorderdetailresponse = new DeliveryOrderDetailResponse();
                    new DeliveryOrderDetailValidator(_unitOfWork).Validate(deliveryorderdetailrequest, out _deliveryorderdetailresponse);
                    i++;
                }
            }
        }
    }
}
