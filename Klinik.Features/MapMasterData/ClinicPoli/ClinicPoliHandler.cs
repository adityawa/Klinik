using AutoMapper;
using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MappingMaster;
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
                    foreach (int _polid in request.Data.ListPoliId)
                    {
                        var clinicPoli = new PoliClinic
                        {
                            ClinicID = request.Data.ClinicID,
                            PoliID = _polid,
                            CreatedBy = request.Data.Account.UserCode,
                            ModifiedDate = DateTime.Now
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
                _model.ListPoliId.Add(item.PoliID);
            }

            var response = new ClinicPoliResponse
            {
                Entity = _model
            };

            return response;
        }
    }
}
