using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.MasterData.LookupCategory
{
    public class LookUpCategoryHandler : BaseFeatures, IBaseFeatures<LookUpCategoryResponse, LookUpCategoryRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LookUpCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create or Edit LookUp Category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LookUpCategoryResponse CreateOrEdit(LookUpCategoryRequest request)
        {
            LookUpCategoryResponse response = new LookUpCategoryResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.LookUpCategoryRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        // save the old data
                        var _oldentity = Mapper.Map<Data.DataRepository.LookupCategory, LookUpCategoryModel>(qry);
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        // update data
                        qry.LookUpName = request.Data.LookUpName;
                        qry.LookUpCode = request.Data.LookUpCode;
                        qry.LookUpContent = request.Data.LookUpContent;

                        _unitOfWork.LookUpCategoryRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "LookUpCategory", qry.LookUpName, qry.ID);

                            CommandLog(true, ClinicEnums.Module.MASTER_LOOKUPCATEGORY, Constants.Command.EDIT_LOOKUP_CATEGORY, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "LookUpCategory");

                            CommandLog(false, ClinicEnums.Module.MASTER_LOOKUPCATEGORY, Constants.Command.EDIT_LOOKUP_CATEGORY, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "LookUpCategory");

                        CommandLog(false, ClinicEnums.Module.MASTER_LOOKUPCATEGORY, Constants.Command.EDIT_LOOKUP_CATEGORY, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var lookupEntity = Mapper.Map<LookUpCategoryModel, Data.DataRepository.LookupCategory>(request.Data);
                    lookupEntity.CreatedBy = request.Data.Account.UserCode;
                    lookupEntity.CreatedDate = DateTime.Now;

                    _unitOfWork.LookUpCategoryRepository.Insert(lookupEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "LookUpCategory", lookupEntity.LookUpName, lookupEntity.ID);

                        CommandLog(true, ClinicEnums.Module.MASTER_LOOKUPCATEGORY, Constants.Command.ADD_LOOKUP_CATEGORY, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "LookUpCategory");

                        CommandLog(false, ClinicEnums.Module.MASTER_LOOKUPCATEGORY, Constants.Command.ADD_LOOKUP_CATEGORY, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_LOOKUPCATEGORY, Constants.Command.EDIT_LOOKUP_CATEGORY, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_LOOKUPCATEGORY, Constants.Command.ADD_LOOKUP_CATEGORY, request.Data.Account, ex);
            }

            return response;

        }

        /// <summary>
        /// Get Detail of Lookup Category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LookUpCategoryResponse GetDetail(LookUpCategoryRequest request)
        {
             LookUpCategoryResponse response = new LookUpCategoryResponse();

            var qry = _unitOfWork.LookUpCategoryRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Data.DataRepository.LookupCategory, LookUpCategoryModel>(qry.FirstOrDefault());
            }

            return response;
        }

        /// <summary>
        /// Get List Data of Lookup Category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LookUpCategoryResponse GetListData(LookUpCategoryRequest request)
        {
            List<LookUpCategoryModel> lists = new List<LookUpCategoryModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Data.DataRepository.LookupCategory>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.LookUpName .Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "lookupname":
                            qry = _unitOfWork.LookUpCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.LookUpName));
                            break;

                        default:
                            qry = _unitOfWork.LookUpCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "lookupname":
                            qry = _unitOfWork.LookUpCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.LookUpName));
                            break;

                        default:
                            qry = _unitOfWork.LookUpCategoryRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.LookUpCategoryRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = Mapper.Map<Data.DataRepository.LookupCategory,LookUpCategoryModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new LookUpCategoryResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        /// <summary>
        /// Remove Data Look Up Category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LookUpCategoryResponse RemoveData(LookUpCategoryRequest request)
        {
            LookUpCategoryResponse response = new LookUpCategoryResponse();

            try
            {
                var lookupCategory = _unitOfWork.LookUpCategoryRepository.GetById(request.Data.Id);
                if (lookupCategory.ID > 0)
                {
                    lookupCategory.RowStatus = -1;
                    lookupCategory.ModifiedBy = request.Data.Account.UserCode;
                    lookupCategory.ModifiedDate = DateTime.Now;

                    _unitOfWork.LookUpCategoryRepository.Update(lookupCategory);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "LookUpCategory", lookupCategory.LookUpName, lookupCategory.ID);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "LookUpCategory");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "LookUpCategory");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_LOOKUPCATEGORY, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;

        }
    }
}
