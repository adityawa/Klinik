using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Entities.PurchaseOrderPusat;
using Klinik.Entities.PurchaseOrderPusatDetail;
using Klinik.Entities.PurchaseRequestPusat;
using Klinik.Entities.PurchaseRequestPusatDetail;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class CreatePOPByPRP : BaseFeatures
    {
        public CreatePOPByPRP(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(PurchaseRequestPusatResponse _response)
        {
            var searchPredicate = PredicateBuilder.New<PurchaseOrderPusat>(true);

            var purchaseorderpusatrequest = new PurchaseOrderPusatRequest
            {
                Data = Mapper.Map<PurchaseRequestPusatModel, PurchaseOrderPusatModel>(_response.Entity)
            };

            purchaseorderpusatrequest.Data.approve = null;
            purchaseorderpusatrequest.Data.podate = DateTime.Now;
            purchaseorderpusatrequest.Data.Validasi = null;
            purchaseorderpusatrequest.Data.approve_by = null;
            purchaseorderpusatrequest.Data.PurchaseRequestId = Convert.ToInt32(_response.Entity.Id);
            purchaseorderpusatrequest.Data.Id = 0;

            var lastponumber = _unitOfWork.PurchaseOrderPusatRepository.Get(searchPredicate, orderBy: a => a.OrderByDescending(x => x.CreatedDate)).Select(a => a.ponumber).FirstOrDefault();
            DateTime? getmonth = _unitOfWork.PurchaseOrderPusatRepository.Get(searchPredicate, orderBy: a => a.OrderByDescending(x => x.CreatedDate)).Select(a => a.podate).FirstOrDefault();
            DateTime? month = getmonth != null ? getmonth : DateTime.Now;
            string ponumber = lastponumber != null ? GeneralHandler.stringincrement(lastponumber, Convert.ToDateTime(month)) : "00001";

            purchaseorderpusatrequest.Data.ponumber = "PR" + _response.Entity.Account.Organization + DateTime.Now.Year + DateTime.Now.Month + ponumber;
            purchaseorderpusatrequest.Data.Account = _response.Entity.Account;

            PurchaseOrderPusatResponse purchaseorderresponse = new PurchaseOrderPusatResponse();

            new PurchaseOrderPusatValidator(_unitOfWork).Validate(purchaseorderpusatrequest, out purchaseorderresponse);

            if (_response.Entity.purchaserequestPusatdetailModels != null)
            {
                int i = 0;
                foreach (var item in _response.Entity.purchaserequestPusatdetailModels)
                {
                    var purchaseorderdetailrequest = new PurchaseOrderPusatDetailRequest
                    {
                        Data = Mapper.Map<PurchaseRequestPusatDetailModel, PurchaseOrderPusatDetailModel>(item)
                    };
                    purchaseorderdetailrequest.Data.PurchaseOrderPusatId = Convert.ToInt32(purchaseorderresponse.Entity.Id);
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
                    PurchaseOrderPusatDetailResponse _purchaseorderdetailresponse = new PurchaseOrderPusatDetailResponse();
                    new PurchaseOrderPusatDetailValidator(_unitOfWork).Validate(purchaseorderdetailrequest, out _purchaseorderdetailresponse);
                    i++;
                }
            }
        }
    }
}
