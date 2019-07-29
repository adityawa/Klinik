using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Klinik.Features.Farmasi
{
    public class FarmasiHandler : BaseFeatures
    {
        public FarmasiHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public FarmasiResponse CreateOrEdit(FarmasiRequest request)
        {
            FarmasiResponse response = new FarmasiResponse();

            return response;
        }

        public static List<Int32> GetSelectedFarmasiItem(long IdQueue)
        {
            List<Int32> farmasiItemIds = new List<Int32>();
            var _getFormMedical = _unitOfWork.RegistrationRepository.GetById(IdQueue);
            if (_getFormMedical != null)
            {
                var qryLabItems = _unitOfWork.FormExamineMedicineRepository.Get(x => x.FormExamine.FormMedicalID == _getFormMedical.FormMedicalID);
                foreach (var item in qryLabItems)
                {
                    farmasiItemIds.Add((int)item.ID);
                }
            }
            return farmasiItemIds;
        }

        public LoketResponse GetListData(LoketRequest request)
        {
            var _laboratoriumId = _unitOfWork.PoliRepository.GetFirstOrDefault(x => x.Name == Constants.NameConstant.Laboratorium);
            Expression<Func<QueuePoli, bool>> _serachCriteria = x => x.PoliTo == _laboratoriumId.ID;

            List<LoketModel> lists = base.GetbaseLoketData(request, _serachCriteria);
            int totalRequest = lists.Count();
            var response = new LoketResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = lists
            };

            return response;
        }

        public FarmasiResponse GetFarmasiForInput(FarmasiRequest request)
        {
            List<FormExamineMedicineModel> lists = new List<FormExamineMedicineModel>();
            dynamic qry = null;
            var searchPredicate = PredicateBuilder.New<FormExamineMedicine>(true);

            searchPredicate = searchPredicate.And(x => x.FormExamine.FormMedicalID == request.Data.LoketData.FormMedicalID);
            qry = _unitOfWork.FormExamineMedicineRepository.Get(searchPredicate, null);

            foreach (var item in qry)
            {
                var prData = Mapper.Map<FormExamineLab, FormExamineLabModel>(item);

                lists.Add(prData);
            }

            int totalRequest = lists.Count();
            var data = lists;

            var response = new FarmasiResponse
            {
                Draw = request.Draw,
                RecordsFiltered = totalRequest,
                RecordsTotal = totalRequest,
                Data = data
            };

            return response;
        }
    }
}
