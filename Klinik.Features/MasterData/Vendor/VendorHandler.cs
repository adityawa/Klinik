using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class VendorHandler  : BaseFeatures, IBaseFeatures<VendorResponse, VendorRequest>
    {
        public VendorHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public VendorResponse CreateOrEdit(VendorRequest request)
        {
            VendorResponse response = new VendorResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.VendorRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new Vendor
                        {
                            namavendor = qry.namavendor,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedBy = qry.ModifiedBy,
                            ModifiedDate = qry.ModifiedDate,
                        };

                        // update data
                        qry.namavendor = request.Data.namavendor;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;

                        _unitOfWork.VendorRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "Vendor", qry.namavendor, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_VENDOR, Constants.Command.EDIT_VENDOR, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "Vendor");

                            CommandLog(false, ClinicEnums.Module.MASTER_VENDOR, Constants.Command.EDIT_GUDANG, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "Vendor");

                        CommandLog(false, ClinicEnums.Module.MASTER_VENDOR, Constants.Command.EDIT_VENDOR, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var vendorEntity = new Vendor
                    {
                        namavendor = request.Data.namavendor,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now
                };

                    _unitOfWork.VendorRepository.Insert(vendorEntity);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "Vendor", vendorEntity.namavendor, vendorEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_VENDOR, Constants.Command.ADD_VENDOR, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "Vendor");

                        CommandLog(false, ClinicEnums.Module.MASTER_VENDOR, Constants.Command.ADD_VENDOR, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_VENDOR, Constants.Command.EDIT_VENDOR, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_VENDOR, Constants.Command.EDIT_VENDOR, request.Data.Account, ex);
            }

            return response;
        }

        public VendorResponse GetDetail(VendorRequest request)
        {
            VendorResponse response = new VendorResponse();

            var qry = _unitOfWork.VendorRepository.Query(x => x.id == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = new VendorModel
                {
                    namavendor = qry.FirstOrDefault().namavendor,
                    CreatedBy = qry.FirstOrDefault().CreatedBy,
                    CreatedDate = Convert.ToDateTime(qry.FirstOrDefault().CreatedDate),
                    ModifiedBy = qry.FirstOrDefault().ModifiedBy,
                    ModifiedDate = qry.FirstOrDefault().ModifiedDate,
                };
            }

            return response;
        }

        public VendorResponse GetListData(VendorRequest request)
        {
            List<VendorModel> lists = new List<VendorModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Vendor>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.namavendor.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "namavendor":
                            qry = _unitOfWork.VendorRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.namavendor));
                            break;

                        default:
                            qry = _unitOfWork.VendorRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "namavendor":
                            qry = _unitOfWork.VendorRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.namavendor));
                            break;

                        default:
                            qry = _unitOfWork.VendorRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.VendorRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = new VendorModel
                {
                    namavendor = item.namavendor,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new VendorResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public VendorResponse RemoveData(VendorRequest request)
        {
            VendorResponse response = new VendorResponse();

            try
            {
                var vendor = _unitOfWork.VendorRepository.GetById(request.Data.Id);
                if (vendor.id > 0)
                {
                    vendor.RowStatus = -1;
                    vendor.ModifiedBy = request.Data.Account.UserCode;
                    vendor.ModifiedDate = DateTime.Now;

                    _unitOfWork.VendorRepository.Update(vendor);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "Vendor", vendor.namavendor, vendor.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "Vendor");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "Gudang");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_VENDOR, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
