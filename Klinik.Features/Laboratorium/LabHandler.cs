using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Form;
using Klinik.Entities.Loket;
using Klinik.Entities.MasterData;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.Laboratorium
{
    public class LabHandler :BaseFeatures
    {
        public LabHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public LabResponse CreateOrEdit(LabRequest request)
        {
            throw new NotImplementedException();
        }

        public LabResponse GetDetail(LabRequest request)
        {
            throw new NotImplementedException();
        }

        public LabResponse GetDetailPatient(long IdQueuePoli)
        {
            var qry_poli = _unitOfWork.RegistrationRepository.GetById(IdQueuePoli);
            var LabResponse = new LabResponse { };

            if (qry_poli != null)
            {
                if (LabResponse.Entity == null)
                    LabResponse.Entity = new FormExamineLabModel();
                LabResponse.Entity.PatientData = Mapper.Map<Patient, PatientModel>(qry_poli.Patient);
                LabResponse.Entity.FormMedicalID = qry_poli.FormMedicalID.Value;
            }
            return LabResponse;
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

        public LabResponse RemoveData(LabRequest request)
        {
            throw new NotImplementedException();
        }

        #region ::Binding::
        public IList<LabItemCategoryModel> GetLaboratoriumCategory(string poliName)
        {
            var _qryPoli = _unitOfWork.PoliRepository.GetFirstOrDefault(x => x.Name == poliName);
            IList<LabItemCategoryModel> labCategories = new List<LabItemCategoryModel>();
            var qry = _unitOfWork.LabItemCategoryRepository.Get(x => x.PoliID == _qryPoli.ID);
            foreach(var item in qry)
            {
                var _item = Mapper.Map<LabItemCategory, LabItemCategoryModel>(item);
                labCategories.Add(_item);
            }
            return labCategories;
        }
        #endregion
    }
}
