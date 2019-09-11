using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.DeliveryOrderPusat;
using Klinik.Entities.DeliveryOrderPusatDetail;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features
{
    public class DeliveryOrderPusatHandler : BaseFeatures, IBaseFeatures<DeliveryOrderPusatResponse, DeliveryOrderPusatRequest>
    {
        public DeliveryOrderPusatHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DeliveryOrderPusatResponse CreateOrEdit(DeliveryOrderPusatRequest request)
        {
            DeliveryOrderPusatResponse response = new DeliveryOrderPusatResponse();

            try
            {
                if (request.Data.Id > 0)
                {
                    var qry = _unitOfWork.DeliveryOrderPusatRepository.GetById(request.Data.Id);
                    if (qry != null)
                    {
                        var _oldentity = new DeliveryOrderPusat
                        {
                            poid = qry.poid,
                            donumber = qry.donumber,
                            dodate = qry.dodate,
                            dodest = qry.dodest,
                            approve_by = qry.approve_by,
                            approve = qry.approve,
                            ModifiedBy = qry.ModifiedBy,
                            CreatedBy = qry.CreatedBy,
                            CreatedDate = qry.CreatedDate,
                            ModifiedDate = qry.ModifiedDate,
                            RowStatus = qry.RowStatus
                        };

                        // update data
                        qry.poid = request.Data.poid > 0 ? request.Data.poid : qry.poid;
                        qry.donumber = request.Data.donumber;
                        qry.dodate = request.Data.dodate;
                        qry.dodest = request.Data.dodest;
                        qry.ModifiedBy = request.Data.Account.UserCode;
                        qry.ModifiedDate = DateTime.Now;
                        qry.RowStatus = 0;

                        _unitOfWork.DeliveryOrderPusatRepository.Update(qry);
                        int resultAffected = _unitOfWork.Save();
                        response.Entity = new DeliveryOrderPusatModel
                        {
                            Id = request.Data.Id
                        };
                        if (resultAffected > 0)
                        {
                            response.Message = string.Format(Messages.ObjectHasBeenUpdated, "DeliveryOrderPusat", qry.donumber, qry.id);

                            CommandLog(true, ClinicEnums.Module.MASTER_DELIVERYORDERPUSAT, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = string.Format(Messages.UpdateObjectFailed, "DeliveryOrderPusat");

                            CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT_DETAIL, request.Data.Account, request.Data, _oldentity);
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.UpdateObjectFailed, "DeliveryOrderPusat");

                        CommandLog(false, ClinicEnums.Module.MASTER_GUDANG, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT, request.Data.Account, request.Data);
                    }
                }
                else
                {
                    var deliveryorderpusatEntity = new DeliveryOrderPusat
                    {
                        poid = request.Data.poid,
                        donumber = request.Data.donumber,
                        dodate = request.Data.dodate,
                        dodest = request.Data.dodest,
                        CreatedBy = request.Data.Account.UserCode,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        RowStatus = 0
                    };

                    _unitOfWork.DeliveryOrderPusatRepository.Insert(deliveryorderpusatEntity);
                    int resultAffected = _unitOfWork.Save();
                    response.Entity = new DeliveryOrderPusatModel
                    {
                        Id = deliveryorderpusatEntity.id
                    };
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenAdded, "DeliveryOrderPusat", deliveryorderpusatEntity.donumber, deliveryorderpusatEntity.id);

                        CommandLog(true, ClinicEnums.Module.MASTER_DELIVERYORDER, Constants.Command.ADD_DELIVERY_ORDER, request.Data.Account, request.Data);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.AddObjectFailed, "DeliveryOrderPusat");

                        CommandLog(false, ClinicEnums.Module.MASTER_DELIVERYORDERPUSAT, Constants.Command.ADD_DELIVERY_ORDER_PUSAT, request.Data.Account, request.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                if (request.Data != null && request.Data.Id > 0)
                    ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDERPUSAT, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT, request.Data.Account, ex);
                else
                    ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDERPUSAT, Constants.Command.EDIT_DELIVERY_ORDER_PUSAT, request.Data.Account, ex);
            }

            return response;
        }

        public DeliveryOrderPusatResponse GetDetail(DeliveryOrderPusatRequest request)
        {
            DeliveryOrderPusatResponse response = new DeliveryOrderPusatResponse();

            var qry = _unitOfWork.DeliveryOrderPusatRepository.GetById(request.Data.Id);

            if (qry != null)
            {
                response.Entity = new DeliveryOrderPusatModel
                {
                    Id = qry.id,
                    poid = qry.poid,
                    donumber = qry.donumber,
                    dodate = qry.dodate,
                    dodest = qry.dodest,
                    approve_by = qry.approve_by,
                    approve = qry.approve,
                    ModifiedBy = qry.ModifiedBy,
                    CreatedBy = qry.CreatedBy,
                    ModifiedDate = qry.ModifiedDate,
                };

                foreach (var item in qry.DeliveryOrderPusatDetails)
                {
                    var newdeliveryOrderdetailpusatModel = Mapper.Map<DeliveryOrderPusatDetail, DeliveryOrderPusatDetailModel>(item);

                    response.Entity.deliveryOrderDetailpusatModels.Add(newdeliveryOrderdetailpusatModel);
                }
            }

            return response;
        }

        public DeliveryOrderPusatResponse GetListData(DeliveryOrderPusatRequest request)
        {
            List<DeliveryOrderPusatModel> lists = new List<DeliveryOrderPusatModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<DeliveryOrderPusat>(true);

            // add default filter to show the active data only
            searchPredicate = searchPredicate.And(x => x.RowStatus == 0);

            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.donumber.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "donumber":
                            qry = _unitOfWork.DeliveryOrderPusatRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.donumber));
                            break;

                        default:
                            qry = _unitOfWork.DeliveryOrderPusatRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.id));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "donumber":
                            qry = _unitOfWork.DeliveryOrderPusatRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.donumber));
                            break;

                        default:
                            qry = _unitOfWork.DeliveryOrderPusatRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.id));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.DeliveryOrderPusatRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = new DeliveryOrderPusatModel
                {
                    Id = item.id,
                    poid = item.poid,
                    donumber = item.donumber,
                    dodate = item.dodate,
                    dodest = item.dodest,
                    approve_by = item.approve_by,
                    approve = item.approve,
                    ModifiedBy = item.ModifiedBy,
                    CreatedBy = item.CreatedBy,
                    ModifiedDate = item.ModifiedDate,
                    createformat = GeneralHandler.FormatDate(item.CreatedDate),
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new DeliveryOrderPusatResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public DeliveryOrderPusatResponse RemoveData(DeliveryOrderPusatRequest request)
        {
            DeliveryOrderPusatResponse response = new DeliveryOrderPusatResponse();

            try
            {
                var deliveryoder = _unitOfWork.DeliveryOrderPusatRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.RowStatus = -1;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.DeliveryOrderPusatRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "DeliveryOrderPusat", deliveryoder.donumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "DeliveryOrderPusat");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "DeliveryOrderPusat");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDERPUSAT, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
            }

            return response;
        }

        public DeliveryOrderPusatResponse ApproveData(DeliveryOrderPusatRequest request)
        {
            DeliveryOrderPusatResponse response = new DeliveryOrderPusatResponse();

            try
            {
                var deliveryoder = _unitOfWork.DeliveryOrderPusatRepository.GetById(request.Data.Id);
                if (deliveryoder.id > 0)
                {
                    deliveryoder.approve = 1;
                    deliveryoder.ModifiedBy = request.Data.Account.UserCode;
                    deliveryoder.ModifiedDate = DateTime.Now;

                    _unitOfWork.DeliveryOrderPusatRepository.Update(deliveryoder);
                    int resultAffected = _unitOfWork.Save();
                    if (resultAffected > 0)
                    {
                        response.Message = string.Format(Messages.ObjectHasBeenRemoved, "DeliveryOrderPusat", deliveryoder.donumber, deliveryoder.id);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = string.Format(Messages.RemoveObjectFailed, "DeliveryOrderPusat");
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = string.Format(Messages.RemoveObjectFailed, "DeliveryOrderPusat");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = Messages.GeneralError;

                ErrorLog(ClinicEnums.Module.MASTER_DELIVERYORDERPUSAT, ClinicEnums.Action.APPROVE.ToString(), request.Data.Account, ex);
            }

            return response;
        }
    }
}
