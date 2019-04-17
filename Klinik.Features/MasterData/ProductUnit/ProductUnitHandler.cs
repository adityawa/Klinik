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
    public class ProductUnitHandler : BaseFeatures, IBaseFeatures<ProductUnitResponse, ProductUnitRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ProductUnitHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductUnitResponse CreateOrEdit(ProductUnitRequest request)
        {
            ProductUnitResponse response = new ProductUnitResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.ProductUnitRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<ProductUnit, ProductUnitModel>(qry);

                        // update data
                        qry.Name = request.Data.Name;

                        _unitOfWork.ProductUnitRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "ProductUnit", qry.Name, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_PRODUCT_UNIT, Constants.Command.EDIT_PRODUCT_UNIT, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "ProductUnit");

                            CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_UNIT, Constants.Command.EDIT_PRODUCT_UNIT, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "ProductUnit");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_UNIT, Constants.Command.EDIT_PRODUCT_UNIT, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var productUnitEntity = Mapper.Map<ProductUnitModel, ProductUnit>(request.Data);
                    productUnitEntity.CreatedBy = request.Data.Account.UserCode;
                    productUnitEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.ProductUnitRepository.Insert(productUnitEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "ProductUnit", productUnitEntity.Name, productUnitEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_PRODUCT_UNIT, Constants.Command.ADD_PRODUCT_UNIT, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "ProductUnit");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_UNIT, Constants.Command.ADD_PRODUCT_UNIT, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_UNIT, Constants.Command.EDIT_PRODUCT_UNIT, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_UNIT, Constants.Command.ADD_PRODUCT_UNIT, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get product unit details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductUnitResponse GetDetail(ProductUnitRequest request)
        {
            ProductUnitResponse response = new ProductUnitResponse();

            var qry = _unitOfWork.ProductUnitRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<ProductUnit, ProductUnitModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get product unit list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductUnitResponse GetListData(ProductUnitRequest request)
        {
            List<ProductUnitModel> lists = new List<ProductUnitModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<ProductUnit>(true);

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
                            qry = _unitOfWork.ProductUnitRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductUnitRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.ProductUnitRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductUnitRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.ProductUnitRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<ProductUnit, ProductUnitModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ProductUnitResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove product unit data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductUnitResponse RemoveData(ProductUnitRequest request)
        {
            ProductUnitResponse response = new ProductUnitResponse();

            try
            {
                var product = _unitOfWork.ProductUnitRepository.GetById(request.Data.Id);
                if (product.ID > 0)
                {
                    product.RowStatus = -1;
                    product.ModifiedBy = request.Data.Account.UserCode;
                    product.ModifiedDate = DateTime.Now;

                    _unitOfWork.ProductUnitRepository.Update(product);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "ProductUnit", product.Name, product.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "ProductUnit");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "ProductUnit");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_UNIT, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}