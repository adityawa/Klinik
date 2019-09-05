using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseOrder;
using Klinik.Entities.PurchaseOrderDetail;
using Klinik.Entities.PurchaseRequest;
using Klinik.Entities.PurchaseRequestDetail;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class CreatePoByPr : BaseFeatures
    {
        public CreatePoByPr(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(PurchaseRequestResponse _response)
        {
            var searchPredicate = PredicateBuilder.New<PurchaseOrder>(true);

            var purchaseorderrequest = new PurchaseOrderRequest
            {
                Data = Mapper.Map<PurchaseRequestModel, PurchaseOrderModel>(_response.Entity)
            };

            purchaseorderrequest.Data.approve = null;
            purchaseorderrequest.Data.podate = DateTime.Now;
            purchaseorderrequest.Data.Validasi = null;
            purchaseorderrequest.Data.approveby = null;
            purchaseorderrequest.Data.PurchaseRequestId = Convert.ToInt32(_response.Entity.Id);
            purchaseorderrequest.Data.Id = 0;

            var lastponumber = _unitOfWork.PurchaseOrderRepository.Get(searchPredicate, orderBy: a => a.OrderByDescending(x => x.CreatedDate)).Select(a => a.ponumber).FirstOrDefault();
            DateTime? getmonth = _unitOfWork.PurchaseOrderRepository.Get(searchPredicate, orderBy: a => a.OrderByDescending(x => x.CreatedDate)).Select(a => a.podate).FirstOrDefault();
            DateTime? month = getmonth != null ? getmonth : DateTime.Now;
            string ponumber = lastponumber != null ? GeneralHandler.stringincrement(lastponumber, Convert.ToDateTime(month)) : "00001";

            purchaseorderrequest.Data.ponumber = "PO" + _response.Entity.Account.Organization + DateTime.Now.Year + DateTime.Now.Month + ponumber;
            purchaseorderrequest.Data.Account = _response.Entity.Account;

            PurchaseOrderResponse purchaseorderresponse = new PurchaseOrderResponse();

            new PurchaseOrderValidator(_unitOfWork).Validate(purchaseorderrequest, out purchaseorderresponse);

            if (_response.Entity.purchaserequestdetailModels != null)
            {
                int i = 0;
                foreach (var item in _response.Entity.purchaserequestdetailModels)
                {
                    var purchaseorderdetailrequest = new PurchaseOrderDetailRequest
                    {
                        Data = Mapper.Map<PurchaseRequestDetailModel, PurchaseOrderDetailModel>(item)
                    };
                    purchaseorderdetailrequest.Data.PurchaseOrderId = Convert.ToInt32(purchaseorderresponse.Entity.Id);
                    purchaseorderdetailrequest.Data.Account = _response.Entity.Account;
                    purchaseorderdetailrequest.Data.Id = 0;
                    //
                    var requestnamabarang = new ProductRequest
                    {
                        Data = new ProductModel
                        {
                            Id = Convert.ToInt32(item.ProductId)
                        }
                    };

                    ProductResponse namabarang = new ProductHandler(_unitOfWork).GetDetail(requestnamabarang);
                    purchaseorderdetailrequest.Data.namabarang = namabarang.Entity.Name;
                    PurchaseOrderDetailResponse _purchaseorderdetailresponse = new PurchaseOrderDetailResponse();
                    new PurchaseOrderDetailValidator(_unitOfWork).Validate(purchaseorderdetailrequest, out _purchaseorderdetailresponse);
                    i++;
                }
            }
        }
    }
}
