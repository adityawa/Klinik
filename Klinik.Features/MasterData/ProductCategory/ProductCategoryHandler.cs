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
    public class ProductCategoryHandler : BaseFeatures, IBaseFeatures<ProductCategoryResponse, ProductCategoryRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ProductCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit productCategory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductCategoryResponse CreateOrEdit(ProductCategoryRequest request)
        {
            ProductCategoryResponse response = new ProductCategoryResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.ProductCategoryRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<ProductCategory, ProductCategoryModel>(qry);

                        // update data
                        qry.Name = request.Data.Name;

                        _unitOfWork.ProductCategoryRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "ProductCategory", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_PRODUCT_CATEGORY, Constants.Command.EDIT_PRODUCT_CATEGORY, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "ProductCategory");

                            CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_CATEGORY, Constants.Command.EDIT_PRODUCT_CATEGORY, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "ProductCategory");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_CATEGORY, Constants.Command.EDIT_PRODUCT_CATEGORY, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var productCategoryCatEntity = Mapper.Map<ProductCategoryModel, ProductCategory>(request.Data);
                    productCategoryCatEntity.CreatedBy = request.Data.Account.UserCode;
                    productCategoryCatEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.ProductCategoryRepository.Insert(productCategoryCatEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "ProductCategory", productCategoryCatEntity.Name, productCategoryCatEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_PRODUCT_CATEGORY, Constants.Command.ADD_PRODUCT_CATEGORY, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "ProductCategory");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_CATEGORY, Constants.Command.ADD_PRODUCT_CATEGORY, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_CATEGORY, Constants.Command.EDIT_PRODUCT_CATEGORY, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_CATEGORY, Constants.Command.ADD_PRODUCT_CATEGORY, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get product category details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductCategoryResponse GetDetail(ProductCategoryRequest request)
        {
            ProductCategoryResponse response = new ProductCategoryResponse();

            var qry = _unitOfWork.ProductCategoryRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<ProductCategory, ProductCategoryModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get product category list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductCategoryResponse GetListData(ProductCategoryRequest request)
        {
            List<ProductCategoryModel> lists = new List<ProductCategoryModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<ProductCategory>(true);

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
                            qry = _unitOfWork.ProductCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.ProductCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.ProductCategoryRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<ProductCategory, ProductCategoryModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ProductCategoryResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove product category data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductCategoryResponse RemoveData(ProductCategoryRequest request)
        {
            ProductCategoryResponse response = new ProductCategoryResponse();

            try
            {
                var productCategory = _unitOfWork.ProductCategoryRepository.GetById(request.Data.Id);
                if (productCategory.ID > 0)
                {
                    productCategory.RowStatus = -1;
                    productCategory.ModifiedBy = request.Data.Account.UserCode;
                    productCategory.ModifiedDate = DateTime.Now;

                    _unitOfWork.ProductCategoryRepository.Update(productCategory);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "ProductCategory", productCategory.Name, productCategory.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "ProductCategory");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "ProductCategory");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_CATEGORY, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}