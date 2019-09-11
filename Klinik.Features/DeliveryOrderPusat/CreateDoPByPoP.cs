using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.DeliveryOrderPusat;
using Klinik.Entities.DeliveryOrderPusatDetail;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseOrderPusat;
using Klinik.Entities.PurchaseOrderPusatDetail;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class CreateDoPByPoP : BaseFeatures
    {
        public CreateDoPByPoP(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(PurchaseOrderPusatResponse _response)
        {
            var searchPredicate = PredicateBuilder.New<DeliveryOrderPusat>(true);

            var deliveryorderpusatrequest = new DeliveryOrderPusatRequest
            {
                Data = Mapper.Map<PurchaseOrderPusatModel, DeliveryOrderPusatModel>(_response.Entity)
            };

            deliveryorderpusatrequest.Data.approve = null;
            deliveryorderpusatrequest.Data.podate = DateTime.Now;
            deliveryorderpusatrequest.Data.Validasi = null;
            deliveryorderpusatrequest.Data.approve_by = null;
            deliveryorderpusatrequest.Data.poid = Convert.ToInt32(_response.Entity.Id);
            deliveryorderpusatrequest.Data.Id = 0;

            var lastponumber = _unitOfWork.DeliveryOrderPusatRepository.Get(searchPredicate, orderBy: a => a.OrderByDescending(x => x.CreatedDate)).Select(a => a.donumber).FirstOrDefault();
            DateTime? getmonth = _unitOfWork.DeliveryOrderPusatRepository.Get(searchPredicate, orderBy: a => a.OrderByDescending(x => x.CreatedDate)).Select(a => a.dodate).FirstOrDefault();
            DateTime? month = getmonth != null ? getmonth : DateTime.Now;
            string ponumber = lastponumber != null ? GeneralHandler.stringincrement(lastponumber, Convert.ToDateTime(month)) : "00001";

            deliveryorderpusatrequest.Data.donumber = "DO" + _response.Entity.Account.Organization + DateTime.Now.Year + DateTime.Now.Month + ponumber;
            deliveryorderpusatrequest.Data.Account = _response.Entity.Account;

            DeliveryOrderPusatResponse purchaseorderresponse = new DeliveryOrderPusatResponse();

            new DeliveryOrderPusatValidator(_unitOfWork).Validate(deliveryorderpusatrequest, out purchaseorderresponse);

            if (_response.Entity.purchaseOrderdetailpusatModels != null)
            {
                int i = 0;
                foreach (var item in _response.Entity.purchaseOrderdetailpusatModels)
                {
                    var deliveryorderdetailrequest = new DeliveryOrderPusatDetailRequest
                    {
                        Data = Mapper.Map<PurchaseOrderPusatDetailModel, DeliveryOrderPusatDetailModel>(item)
                    };
                    deliveryorderdetailrequest.Data.DeliveryOrderPusatId = Convert.ToInt32(purchaseorderresponse.Entity.Id);
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
                    DeliveryOrderPusatDetailResponse _purchaseorderdetailresponse = new DeliveryOrderPusatDetailResponse();
                    new DeliveryOrderPusatDetailValidator(_unitOfWork).Validate(deliveryorderdetailrequest, out _purchaseorderdetailresponse);
                    i++;
                }
            }
        }
    }
}
