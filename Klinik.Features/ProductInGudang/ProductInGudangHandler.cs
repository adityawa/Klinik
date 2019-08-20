using Klinik.Common;
using Klinik.Data;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class ProductInGudangHandler : BaseFeatures, IBaseFeatures<ProductInGudangResponse, ProductInGudangRequest>
    {
        public ProductInGudangHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ProductInGudangResponse CreateOrEdit(ProductInGudangRequest request)
        {
            ProductInGudangResponse response = new ProductInGudangResponse();

            var get = _unitOfWork.ProductInGudangRepository.Query(x => x.ProductId == request.Data.ProductId && x.GudangId == request.Data.GudangId, null);
            var testing = get.FirstOrDefault().id;
            var qry = new Data.DataRepository.ProductInGudang();
            if (get.Count() > 0)
            {
                qry = _unitOfWork.ProductInGudangRepository.GetById(get.FirstOrDefault().id);
            }
            
            try
            {
                if (qry.id > 0)
                {
                    // update data
                    qry.ModifiedBy = request.Data.Account.UserCode;
                    qry.ModifiedDate = DateTime.Now;
                    qry.stock = qry.stock + request.Data.stock;

                    _unitOfWork.ProductInGudangRepository.Update(qry);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenUpdated, "ProductInGudang", qry.ProductId, qry.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PRODUCTINGUDANG, Constants.Command.EDIT_PRODCUTINGUDANG, request.Data.Account, request.Data, "");
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "ProductInGudang");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCTINGUDANG, Constants.Command.EDIT_PRODCUTINGUDANG, request.Data.Account, request.Data, "");
                    }
                }
                else
                {
                    var productingudangEntity = new Data.DataRepository.ProductInGudang
                    {
                        ProductId = request.Data.ProductId,
                        GudangId = request.Data.GudangId,
                        stock = request.Data.stock,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        RowStatus = 0,
                    };

                    _unitOfWork.ProductInGudangRepository.Insert(productingudangEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "ProductInGudang", productingudangEntity.ProductId, productingudangEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PRODUCTINGUDANG, Constants.Command.ADD_PRODCUTINGUDANG, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "ProductInGudang");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCTINGUDANG, Constants.Command.ADD_PRODCUTINGUDANG, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCTINGUDANG, Constants.Command.EDIT_PRODCUTINGUDANG, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCTINGUDANG, Constants.Command.EDIT_PRODCUTINGUDANG, request.Data.Account, ex);
            }

            return response;
        }

        public ProductInGudangResponse GetDetail(ProductInGudangRequest request)
        {
            throw new NotImplementedException();
        }

        public ProductInGudangResponse GetListData(ProductInGudangRequest request)
        {
            throw new NotImplementedException();
        }

        public ProductInGudangResponse RemoveData(ProductInGudangRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
