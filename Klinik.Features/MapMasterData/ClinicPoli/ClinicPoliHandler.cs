using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MappingMaster;
using Klinik.Entities.MasterData;
using Klinik.Resources;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class ClinicPoliHandler : BaseFeatures
    {
        public ClinicPoliHandler(IUnitOfWork unitOfWork, KlinikDBEntities context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public ClinicPoliResponse CreateOrEdit(ClinicPoliRequest request)
        {
            ClinicPoliResponse response = new ClinicPoliResponse();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var toberemove = _context.PoliClinics.Where(x => x.ClinicID == request.Data.ClinicID);
                    _context.PoliClinics.RemoveRange(toberemove);
                    _context.SaveChanges();

                    //insert new
                    foreach (int _polid in request.Data.PoliIDs)
                    {
                        var clinicPoli = new PoliClinic
                        {
                            ClinicID = request.Data.ClinicID,
                            PoliID = _polid,
                            RowStatus = 0,
                            CreatedBy = request.Data.Account.UserCode,
                            Createddate = DateTime.Now
                        };

                        _context.PoliClinics.Add(clinicPoli);
                    }

                    int resultAffected = _context.SaveChanges();

                    transaction.Commit();
                    response.Status = true;
                    response.Message = Messages.DataSaved;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    response.Status = false;
                    response.Message = Messages.GeneralError;

                    ErrorLog(ClinicEnums.Module.MASTER_POLI_CLINIC, ClinicEnums.Action.DELETE.ToString(), request.Data.Account, ex);
                }
            }

            return response;
        }

        public ClinicPoliResponse GetListData(ClinicPoliRequest request)
        {
            var qry = _unitOfWork.PoliClinicRepository.Get(x => x.ClinicID == request.Data.ClinicID && x.RowStatus == 0);
            ClinicPoliModel _model = new ClinicPoliModel();

            if (qry.Count > 0)
                _model.ClinicID = qry.FirstOrDefault().ClinicID;

            foreach (var item in qry)
            {
                _model.PoliIDs.Add(item.PoliID);
            }

            var response = new ClinicPoliResponse
            {
                Entity = _model
            };

            return response;
        }

        public ClinicPoliResponse GetPoliBasedOnOrClinic(ClinicPoliRequest request)
        {
            var _clinicId = _unitOfWork.ClinicRepository.GetById(request.Data.ClinicID) == null ? 0 : _unitOfWork.ClinicRepository.GetById(request.Data.ClinicID).ID;

            List<ClinicPoliModel> lists = new List<ClinicPoliModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<PoliClinic>(true);
            searchPredicate = searchPredicate.And(x => x.ClinicID == _clinicId);
            if (!String.IsNullOrEmpty(request.SearchValue) && !String.IsNullOrWhiteSpace(request.SearchValue))
            {
                searchPredicate = searchPredicate.And(p => p.Poli.Name.Contains(request.SearchValue) || p.Poli.Code.Contains(request.SearchValue));
            }

            if (!(string.IsNullOrEmpty(request.SortColumn) && string.IsNullOrEmpty(request.SortColumnDir)))
            {
                if (request.SortColumnDir == "asc")
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.PoliClinicRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.Poli.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliClinicRepository.Get(searchPredicate, orderBy: q => q.OrderBy(x => x.ID));
                            break;
                    }
                }
                else
                {
                    switch (request.SortColumn.ToLower())
                    {
                        case "name":
                            qry = _unitOfWork.PoliClinicRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.Poli.Name));
                            break;

                        default:
                            qry = _unitOfWork.PoliClinicRepository.Get(searchPredicate, orderBy: q => q.OrderByDescending(x => x.ID));
                            break;
                    }
                }
            }
            else
            {
                qry = _unitOfWork.PoliClinicRepository.Get(searchPredicate, null);
            }

            foreach (var item in qry)
            {
                var prData = new ClinicPoliModel
                {
                    Id = item.ID,
                    ClinicID = item.ClinicID,
                    PoliID = item.PoliID,
                    PoliName = item.Poli.Name,
                    ClinicName = item.Clinic.Name,
                    PoliCode = item.Poli.Code,
                };

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists.Skip(request.Skip).Take(request.PageSize).ToList();

            var response = new ClinicPoliResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }

        public ClinicResponse GetDetail(ClinicRequest request)
        {
            ClinicResponse response = new ClinicResponse();

            var qry = _unitOfWork.ClinicRepository.Query(x => x.ID == request.Data.Id, null);
            if (qry.FirstOrDefault() != null)
            {
                response.Entity = Mapper.Map<Klinik.Data.DataRepository.Clinic, ClinicModel>(qry.FirstOrDefault());
            }

            return response;
        }
    }
}
