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
    public class ProductMedicineHandler : BaseFeatures, IBaseFeatures<ProductMedicineResponse, ProductMedicineRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ProductMedicineHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or edit productMedicine
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductMedicineResponse CreateOrEdit(ProductMedicineRequest request)
        {
            ProductMedicineResponse response = new ProductMedicineResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.ProductMedicineRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<ProductMedicine, ProductMedicineModel>(qry);

                        _oldentity.ProductID = request.Data.ProductID;
                        _oldentity.MedicineID = request.Data.MedicineID;
                        _oldentity.Amount = request.Data.Amount;

                        _unitOfWork.ProductMedicineRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated2, "ProductMedicine", qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_PRODUCT_MEDICINE, Constants.Command.EDIT_PRODUCT_MEDICINE, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "ProductMedicine");

                            CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_MEDICINE, Constants.Command.EDIT_PRODUCT_MEDICINE, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "ProductMedicine");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_MEDICINE, Constants.Command.EDIT_PRODUCT_MEDICINE, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var productMedicineEntity = Mapper.Map<ProductMedicineModel, ProductMedicine>(request.Data);
                    productMedicineEntity.CreatedBy = request.Data.Account.UserCode;
                    productMedicineEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.ProductMedicineRepository.Insert(productMedicineEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded2, "ProductMedicine", productMedicineEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_PRODUCT_MEDICINE, Constants.Command.ADD_PRODUCT_MEDICINE, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "ProductMedicine");

                        CommandLog(false, ClinicEnums.Module.MASTER_PRODUCT_MEDICINE, Constants.Command.ADD_PRODUCT_MEDICINE, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_MEDICINE, Constants.Command.EDIT_PRODUCT_MEDICINE, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_MEDICINE, Constants.Command.ADD_PRODUCT_MEDICINE, request.Data.Account, ex);
            }

            return response;
        }

        /// <summary>
        /// Get product medicine details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductMedicineResponse GetDetail(ProductMedicineRequest request)
        {
            ProductMedicineResponse response = new ProductMedicineResponse();

            var qry = _unitOfWork.ProductMedicineRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<ProductMedicine, ProductMedicineModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get product medicine list of data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductMedicineResponse GetListData(ProductMedicineRequest request)
        {
            List<ProductMedicineModel> lists = new List<ProductMedicineModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<ProductMedicine>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

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
                        case "name":
                            qry = _unitOfWork.ProductMedicineRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Product.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductMedicineRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.ProductMedicineRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Product.Name));
                            break;

                        default:
                            qry = _unitOfWork.ProductMedicineRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.ProductMedicineRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<ProductMedicine, ProductMedicineModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ProductMedicineResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove product medicine data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProductMedicineResponse RemoveData(ProductMedicineRequest request)
        {
            ProductMedicineResponse response = new ProductMedicineResponse();

            try
            {
                var productMedicine = _unitOfWork.ProductMedicineRepository.GetById(request.Data.Id);
                if (productMedicine.ID > 0)
                {
                    productMedicine.RowStatus = -1;
                    productMedicine.ModifiedBy = request.Data.Account.UserCode;
                    productMedicine.ModifiedDate = DateTime.Now;

                    _unitOfWork.ProductMedicineRepository.Update(productMedicine);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "ProductMedicine", productMedicine.Product.Name, productMedicine.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "ProductMedicine");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "ProductMedicine");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PRODUCT_MEDICINE, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}