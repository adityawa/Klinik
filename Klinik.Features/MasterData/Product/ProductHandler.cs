using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class ProductHandler : BaseFeatures, IBaseFeatures<ProductResponse, ProductRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductResponse CreateOrEdit(ProductRequest request)
        {
            ProductResponse response = new ProductResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.ProductRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Product, ProductModel>(qry);
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        // update data
                        qry.Name = request.Data.Name;
                        qry.ProductCategoryID = request.Data.ProductCategoryID;
                        qry.ProductUnitID = request.Data.ProductUnitID;
                        qry.RetailPrice = request.Data.RetailPrice;

                        _unitOfWork.ProductRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Product", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_PRODUCT, Constants.Command.EDIT_PRODUCT, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Product");

                            CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT, Constants.Command.EDIT_PRODUCT, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Product");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT, Constants.Command.EDIT_PRODUCT, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var productEntity = Mapper.Map<ProductModel, Product>(request.Data);
                    productEntity.CreatedBy = request.Data.Account.UserCode;
                    productEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.ProductRepository.Insert(productEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Entity = new ProductModel
                        {
                            Id = productEntity.ID
                        };
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Product", productEntity.Name, productEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_PRODUCT, Constants.Command.ADD_PRODUCT, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Product");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT, Constants.Command.ADD_PRODUCT, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCT, Constants.Command.EDIT_PRODUCT, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCT, Constants.Command.ADD_PRODUCT, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get product details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductResponse GetDetail(ProductRequest request)
        {
            ProductResponse response = new ProductResponse();

            var qry = _unitOfWork.ProductRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Product, ProductModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get product list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductResponse GetListData(ProductRequest request)
        {
            List<ProductModel> lists = new List<ProductModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Product>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Name.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.ProductRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.ProductRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.ProductRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Product, ProductModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ProductResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove product data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductResponse RemoveData(ProductRequest request)
        {
            ProductResponse response = new ProductResponse();

            try
            {
                var product = _unitOfWork.ProductRepository.GetById(request.Data.Id);
                if (product.ID > 0)
                {
                    product.RowStatus = -1;
                    product.ModifiedBy = request.Data.Account.UserCode;
                    product.ModifiedDate = DateTime.Now;

                    _unitOfWork.ProductRepository.Update(product);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Product", product.Name, product.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Product");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Product");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PRODUCT, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}