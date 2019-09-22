using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.ProductInGudang;
using Klinik.Features.Account;
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
            ProductInGudangResponse response = new ProductInGudangResponse();

            var qry = _unitOfWork.ProductInGudangRepository.GetById(request.Data.Id);
            if (qry != null)
            {
                response.Entity = new ProductInGudangModel
                {
                   ProductId = qry.ProductId,
                   GudangId = qry.GudangId,
                   ProductName = qry.Product.Name,
                   GudangName = qry.Gudang.name,
                   stock = qry.stock
                };
            }

            return response;
        }

        public ProductInGudangResponse GetListData(ProductInGudangRequest request)
        {
            List<ProductInGudangModel> lists = new List<ProductInGudangModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Data.DataRepository.ProductInGudang>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0 && x.GudangId == OneLoginSession.Account.GudangID);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Product.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "ProductName":
                            qry = _unitOfWork.ProductInGudangRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Product.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductInGudangRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "ProductName":
                            qry = _unitOfWork.ProductInGudangRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Product.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductInGudangRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.ProductInGudangRepository.Get(searchPredicate, null);
            }
            List<ProductInGudang> newdata = new List<ProductInGudang>();
            newdata = qry;
            foreach (var item in newdata)
            {
                var prData = new ProductInGudangModel
                {
                    Id = item.id,
                    ProductName = item.Product.Name,
                    GudangName = item.Gudang.name,
                    stock = item.stock
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ProductInGudangResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public ProductInGudangResponse RemoveData(ProductInGudangRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
