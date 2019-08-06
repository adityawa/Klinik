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

namespace Klinik.Features.Pharmacy
{
    public class PharmacyHandler : BaseFeatures
    {
        public PharmacyHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public PharmacyResponse CreateOrEdit(PharmacyRequest request)
        {
            PharmacyResponse response = new PharmacyResponse();

            return response;
        }

        public static List<Int32> GetSelectedPharmacyItem(long IdQueue)
        {
            List<Int32> PharmacyItemIds = new List<Int32>();
            var _getFormMedical = _unitOfWork.RegistrationRepository.GetById(IdQueue);
            if (_getFormMedical != null)
            {
                var qryLabItems = _unitOfWork.FormExamineMedicineRepository.Get(x => x.FormExamine.FormMedicalID == _getFormMedical.FormMedicalID);
                foreach (var item in qryLabItems)
                {
                    PharmacyItemIds.Add((int)item.ID);
                }
            }
            return PharmacyItemIds;
        }

        public LoketResponse GetListData(LoketRequest request)
        {
            Expression<Func<QueuePoli, bool>> _serachCriteria = x => x.PoliTo == request.Data.PoliToID;

            List<LoketModel> lists = base.GetFarmasiBaseLoketData(request, _serachCriteria);
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

        public PharmacyResponse GetPharmacyForInput(PharmacyRequest request)
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

            var response = new PharmacyResponse
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
