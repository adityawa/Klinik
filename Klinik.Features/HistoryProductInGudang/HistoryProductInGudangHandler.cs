using Klinik.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class HistoryProductInGudangHandler : BaseFeatures, IBaseFeatures<HistoryProductInGudangResponse, HistoryProductInGudangRequest>
    {
        public HistoryProductInGudangHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public HistoryProductInGudangResponse CreateOrEdit(HistoryProductInGudangRequest request)
        {
            var productingudangEntity = new Data.DataRepository.HistoryProductInGudang
            {
                ProductId = request.Data.ProductId,
                GudangId = request.Data.GudangId,
                value = request.Data.value,
                CreatedBy = request.Data.Account.UserCode,
                CreatedDate = DateTime.Now,
            };

            _unitOfWork.HistoryProductInGudangRepository.Insert(productingudangEntity);
            int resultAffected = _unitOfWork.Save();
            return new HistoryProductInGudangResponse();
        }

        public HistoryProductInGudangResponse GetDetail(HistoryProductInGudangRequest request)
        {
            throw new NotImplementedException();
        }

        public HistoryProductInGudangResponse GetListData(HistoryProductInGudangRequest request)
        {
            throw new NotImplementedException();
        }

        public HistoryProductInGudangResponse RemoveData(HistoryProductInGudangRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
