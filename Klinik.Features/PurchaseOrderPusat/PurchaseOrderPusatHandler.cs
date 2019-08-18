using Klinik.Common;
using Klinik.Data;
using Klinik.Entities.PurchaseOrderPusat;
using Klinik.Entities.PurchaseOrderPusatDetail;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class PurchaseOrderPusatHandler : BaseFeatures, IBaseFeatures<PurchaseOrderPusatResponse, PurchaseOrderPusatRequest>
    {
        public PurchaseOrderPusatHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseOrderPusatResponse CreateOrEdit(PurchaseOrderPusatRequest request)
        {
            PurchaseOrderPusatResponse response = new PurchaseOrderPusatResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PurchaseOrderPusatRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new Data.DataRepository.PurchaseOrderPusat 
                        {
                            PurchaseRequestId = qry.PurchaseRequestId,
                            ponumber = qry.ponumber,
                            podate = qry.podate,
                            approve_by = qry.approve_by,
                            approve = qry.approve,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.PurchaseRequestId = qry.PurchaseRequestId;
                        qry.ponumber = request.Data.ponumber;
                        qry.podate = request.Data.podate;
                        qry.request_by = request.Data.request_by;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;
                        qry.RowStatus = 0;

                        _unitOfWork.PurchaseOrderPusatRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new PurchaseOrderPusatModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PurchaseOrderPusat", qry.ponumber, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseOrderPusat");

                            CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseOrderPusat");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var purhcaseorderpusatEntity = new Data.DataRepository.PurchaseOrderPusat
                    {
                        PurchaseRequestId = request.Data.PurchaseRequestId,
                        ponumber = request.Data.ponumber,
                        podate = request.Data.podate,
                        request_by = request.Data.request_by,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        RowStatus = 0
                    };

                    _unitOfWork.PurchaseOrderPusatRepository.Insert(purhcaseorderpusatEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new PurchaseOrderPusatModel
                    {
                        Id = purhcaseorderpusatEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PurchaseOrderPusat", purhcaseorderpusatEntity.ponumber, purhcaseorderpusatEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, Constants.Command.ADD_PURCHASE_ORDER_PUSAT, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PurchaseOrderPusat");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, Constants.Command.ADD_PURCHASE_ORDER_PUSAT, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, Constants.Command.EDIT_PURCHASE_ORDER_PUSAT, request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseOrderPusatResponse GetDetail(PurchaseOrderPusatRequest request)
        {
            PurchaseOrderPusatResponse response = new PurchaseOrderPusatResponse();

            var qry = _unitOfWork.PurchaseOrderPusatRepository.GetById(request.Data.Id);
            //DeliveryOrderDetailModel newdeliveryOrderdetailModel = new DeliveryOrderDetailModel();
            if (qry != null)
            {
                response.Entity = new PurchaseOrderPusatModel
                {
                    Id = qry.id,
                    PurchaseRequestId = qry.PurchaseRequestId,
                    ponumber = qry.ponumber,
                    podate = qry.podate,
                    approve_by = qry.approve_by,
                    request_by = qry.request_by,
                    approve = qry.approve,
                    ModifiedBy = qry.ModifiedBy,
                    CreatedBy = qry.CreatedBy,
                    ModifiedDate = qry.ModifiedDate,
                };

                foreach (var item in qry.PurchaseOrderPusatDetails)
                {
                    var newpurchaseOrderpusatdetailModel = new PurchaseOrderPusatDetailModel
                    {
                        Id = item.id,
                        PurchaseOrderPusatId = qry.id,
                        ProductId = item.ProductId,
                        namabarang = item.namabarang,
                        VendorId = item.VendorId,
                        namavendor = item.namavendor,
                        satuan = item.satuan,
                        harga = item.harga,
                        stok_prev = item.stok_prev,
                        total_req = item.total_req,
                        total_dist = item.total_dist,
                        sisa_stok = item.sisa_stok,
                        qty = item.qty,
                        qty_add = item.qty_add,
                        reason_add = item.reason_add,
                        total = item.total,
                        qty_unit = item.qty_unit,
                        qty_box = item.qty_box,
                    };

                    response.Entity.purchaseOrderdetailpusatModels.Add(newpurchaseOrderpusatdetailModel);
                }
            }

            return response;
        }

        public PurchaseOrderPusatResponse GetListData(PurchaseOrderPusatRequest request)
        {
            List<PurchaseOrderPusatModel> lists = new List<PurchaseOrderPusatModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Data.DataRepository.PurchaseOrderPusat>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.ponumber.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "ponumber":
                            qry = _unitOfWork.PurchaseOrderPusatRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ponumber));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseOrderPusatRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "ponumber":
                            qry = _unitOfWork.PurchaseOrderPusatRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ponumber));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseOrderPusatRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PurchaseOrderPusatRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = new PurchaseOrderPusatModel
                {
                    Id = item.id,
                    PurchaseRequestId = item.PurchaseRequestId,
                    ponumber = item.ponumber,
                    podate = item.podate,
                    approve_by = item.approve_by,
                    approve = item.approve,
                    request_by = item.request_by,
                    ModifiedBy = item.ModifiedBy,
                    CreatedBy = item.CreatedBy,
                    ModifiedDate = item.ModifiedDate,
                    createformat = GeneralHandler.FormatDate(item.CreatedDate)
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PurchaseOrderPusatResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public PurchaseOrderPusatResponse RemoveData(PurchaseOrderPusatRequest request)
        {
            PurchaseOrderPusatResponse response = new PurchaseOrderPusatResponse();

            try
            {
                var purchaseoderpusat = _unitOfWork.PurchaseOrderPusatRepository.GetById(request.Data.Id);
                if (purchaseoderpusat.id > 0)
                {
                    purchaseoderpusat.RowStatus = -1;
                    purchaseoderpusat.ModifiedBy = request.Data.Account.UserCode;
                    purchaseoderpusat.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseOrderPusatRepository.Update(purchaseoderpusat);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseOrderPusat", purchaseoderpusat.ponumber, purchaseoderpusat.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseOrderPusat");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseOrderPusat");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseOrderPusatResponse ApproveData(PurchaseOrderPusatRequest request)
        {
            PurchaseOrderPusatResponse response = new PurchaseOrderPusatResponse();

            try
            {
                var purchaseoderpusat = _unitOfWork.PurchaseOrderPusatRepository.GetById(request.Data.Id);
                if (purchaseoderpusat.id > 0)
                {
                    purchaseoderpusat.approve = 1;
                    purchaseoderpusat.approve_by = request.Data.Account.UserCode;
                    purchaseoderpusat.ModifiedBy = request.Data.Account.UserCode;
                    purchaseoderpusat.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseOrderPusatRepository.Update(purchaseoderpusat);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseOrderPusat", purchaseoderpusat.ponumber, purchaseoderpusat.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseOrderPusat");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseOrderPusat");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEORDERPUSAT, ClinicEnums.Action.APPROVE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
