﻿using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities;
using Klinik.Entities.PurchaseRequestPusat;
using Klinik.Entities.PurchaseRequestPusatDetail;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class PurchaseRequestPusatHandler : BaseFeatures, IBaseFeatures<PurchaseRequestPusatResponse, PurchaseRequestPusatRequest>
    {
        public PurchaseRequestPusatHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseRequestPusatResponse CreateOrEdit(PurchaseRequestPusatRequest request)
        {
            PurchaseRequestPusatResponse response = new PurchaseRequestPusatResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.PurchaseRequestPusatRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new Data.DataRepository.PurchaseRequestPusat
                        {
                            prnumber = qry.prnumber,
                            prdate = qry.prdate,
                            approve_by = qry.approve_by,
                            approve = qry.approve,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus,
                            GudangId = qry.GudangId,
                        };

                        // update data
                        qry.prnumber = request.Data.prnumber;
                        qry.prdate = request.Data.prdate;
                        qry.request_by = request.Data.request_by;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;
                        qry.RowStatus = 0;
                        qry.GudangId = request.Data.GudangId;

                        _unitOfWork.PurchaseRequestPusatRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new PurchaseRequestPusatModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "PurchaseRequestPusat", qry.prnumber, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseRequestPusat");

                            CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "PurchaseRequestPusat");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var purhcaserequestEntity = new Data.DataRepository.PurchaseRequestPusat
                    {
                        prnumber = request.Data.prnumber,
                        prdate = request.Data.prdate,
                        request_by = request.Data.request_by,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        GudangId = request.Data.GudangId,
                        RowStatus = 0
                    };

                    _unitOfWork.PurchaseRequestPusatRepository.Insert(purhcaserequestEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new PurchaseRequestPusatModel
                    {
                        Id = purhcaserequestEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "PurchaseRequestPusat", purhcaserequestEntity.prnumber, purhcaserequestEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, Constants.Command.ADD_PURCHASE_REQUEST_PUSAT, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "PurchaseRequestPusat");

                        CommandLog(false, ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, Constants.Command.ADD_PURCHASE_REQUEST_PUSAT, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, Constants.Command.EDIT_PURCHASE_REQUEST_PUSAT, request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseRequestPusatResponse GetDetail(PurchaseRequestPusatRequest request)
        {
            PurchaseRequestPusatResponse response = new PurchaseRequestPusatResponse();

            var qry = _unitOfWork.PurchaseRequestPusatRepository.GetById(request.Data.Id);

            if (qry != null)
            {
                response.Entity = new PurchaseRequestPusatModel
                {
                    Id = qry.id,
                    prnumber = qry.prnumber,
                    prdate = qry.prdate,
                    approve_by = qry.approve_by,
                    request_by = qry.request_by,
                    approve = qry.approve,
                    ModifiedBy = qry.ModifiedBy,
                    CreatedBy = qry.CreatedBy,
                    ModifiedDate = qry.ModifiedDate,
                    Validasi = qry.Validasi
                };

                foreach (var item in qry.PurchaseRequestPusatDetails)
                {
                    var newpurchaserequestdetailModel = new PurchaseRequestPusatDetailModel
                    {
                        Id = item.id,
                        PurchaseRequestPusatId = qry.id,
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
                        qty_final = item.qty_final,
                        remark = item.remark,
                        total = item.total,
                        qty_unit = item.qty_unit,
                        qty_box = item.qty_box,
                    };

                    response.Entity.purchaserequestPusatdetailModels.Add(newpurchaserequestdetailModel);
                }
            }

            return response;
        }

        public PurchaseRequestPusatResponse GetListData(PurchaseRequestPusatRequest request)
        {
            List<PurchaseRequestPusatModel> lists = new List<PurchaseRequestPusatModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<Data.DataRepository.PurchaseRequestPusat>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);
            if ((GeneralHandler.authorized("APPROVE_M_PURCHASEREQUESTPUSAT") == "false"))
            {
                searchPredicate.And(x => x.approve >= 1);
            }
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.prnumber.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "prnumber":
                            qry = _unitOfWork.PurchaseRequestPusatRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.prnumber));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseRequestPusatRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "prnumber":
                            qry = _unitOfWork.PurchaseRequestPusatRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.prnumber));
                            break;

                        default:
                            qry = _unitOfWork.PurchaseRequestPusatRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PurchaseRequestPusatRepository.Get(searchPredicate, null);
            }
            List<Data.DataRepository.PurchaseRequestPusat> purchaseRequests = new List<Data.DataRepository.PurchaseRequestPusat>();
            purchaseRequests = qry;
            foreach (var item in purchaseRequests)
            {
                var prData = new PurchaseRequestPusatModel
                {
                    Id = item.id,
                    prnumber = item.prnumber,
                    prdate = item.prdate,
                    approve_by = item.approve_by,
                    approve = item.approve,
                    request_by = item.request_by,
                    ModifiedBy = item.ModifiedBy,
                    CreatedBy = item.CreatedBy,
                    ModifiedDate = item.ModifiedDate,
                    createformat = GeneralHandler.FormatDate(Convert.ToDateTime(item.CreatedDate)),
                    Validasi = item.Validasi,
                    poid = item.PurchaseOrderPusats.Count > 0 ? item.PurchaseOrderPusats.FirstOrDefault().id : 0,
                    ponumber = item.PurchaseOrderPusats.Count > 0 ? item.PurchaseOrderPusats.FirstOrDefault().ponumber : "",
                    createpo = item.PurchaseOrderPusats.Count > 0 ? GeneralHandler.FormatDate(Convert.ToDateTime(item.PurchaseOrderPusats.FirstOrDefault().podate)) : null,
                    doid = item.PurchaseOrderPusats.Count > 0 ? item.PurchaseOrderPusats.FirstOrDefault().DeliveryOrderPusats.Count > 0 ? item.PurchaseOrderPusats.FirstOrDefault().DeliveryOrderPusats.FirstOrDefault().id : 0 : 0,
                    donumber = item.PurchaseOrderPusats.Count > 0 ? item.PurchaseOrderPusats.FirstOrDefault().DeliveryOrderPusats.Count > 0 ? item.PurchaseOrderPusats.FirstOrDefault().DeliveryOrderPusats.FirstOrDefault().donumber : "" : "",
                    createdo = item.PurchaseOrderPusats.Count > 0 ? item.PurchaseOrderPusats.FirstOrDefault().DeliveryOrderPusats.Count > 0 ? GeneralHandler.FormatDate(Convert.ToDateTime(item.PurchaseOrderPusats.FirstOrDefault().DeliveryOrderPusats.FirstOrDefault().dodate)) : null : null,
                    status = GeneralHandler.PurchaseRequestPusatStatus(item.id),
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new PurchaseRequestPusatResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public PurchaseRequestPusatResponse RemoveData(PurchaseRequestPusatRequest request)
        {
            PurchaseRequestPusatResponse response = new PurchaseRequestPusatResponse();

            try
            {
                var deliveryoder = _unitOfWork.PurchaseRequestPusatRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.RowStatus = -1;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseRequestPusatRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseRequestPusat", deliveryoder.prnumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequestPusat");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequestPusat");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseRequestPusatResponse ApproveData(PurchaseRequestPusatRequest request)
        {
            PurchaseRequestPusatResponse response = new PurchaseRequestPusatResponse();

            try
            {
                var deliveryoder = _unitOfWork.PurchaseRequestPusatRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.approve = 1;
                    deliveryoder.approve_by = request.Data.Account.UserCode;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseRequestPusatRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseRequestPusat", deliveryoder.prnumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequestPusat");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequestPusat");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, ClinicEnums.Action.APPROVE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        public PurchaseRequestPusatResponse ValidasiData(PurchaseRequestPusatRequest request)
        {
            PurchaseRequestPusatResponse response = new PurchaseRequestPusatResponse();

            try
            {
                var purchaserequestpusat = _unitOfWork.PurchaseRequestPusatRepository.GetById(request.Data.Id);
                if (purchaserequestpusat.id > 0)
                {
                    purchaserequestpusat.Validasi = 1;
                    purchaserequestpusat.approve_by = request.Data.Account.UserCode;
                    purchaserequestpusat.ModifiedBy = request.Data.Account.UserCode;
                    purchaserequestpusat.ModifiedDate = DateTime.Now;

                    _unitOfWork.PurchaseRequestPusatRepository.Update(purchaserequestpusat);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        var purchaseorderdetail = _unitOfWork.PurchaseRequestPusatDetailRepository.Query(a => a.PurchaseRequestPusatId == purchaserequestpusat.id).ToList();
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "PurchaseRequestPusat", purchaserequestpusat.prnumber, purchaserequestpusat.id);
                        response.Entity = Mapper.Map<Data.DataRepository.PurchaseRequestPusat, PurchaseRequestPusatModel>(purchaserequestpusat);
                        foreach (var item in purchaseorderdetail)
                        {
                            var _purchaseRequestpusatDetail = Mapper.Map<Data.DataRepository.PurchaseRequestPusatDetail, PurchaseRequestPusatDetailModel>(item);
                            response.Entity.purchaserequestPusatdetailModels.Add(_purchaseRequestpusatDetail);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequest");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "PurchaseRequest");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_PURCHASEREQUESTPUSAT, ClinicEnums.Action.VALIDASI.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
