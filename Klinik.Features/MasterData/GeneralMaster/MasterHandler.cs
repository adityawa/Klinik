using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class MasterHandler : BaseFeatures
    {
        public MasterHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<GeneralMaster> GetMasterDataByType(string type)
        {
            return _unitOfWork.MasterRepository.Query(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        public IQueryable<LookupCategory> GetLookupCategoryByName(string name)
        {
            return _unitOfWork.LookUpCategoryRepository.Query(x => x.TypeName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public List<string> GetRujukans()
        {
            return _unitOfWork.LetterRepository.Get().Select(x => x.OtherInfo).Distinct().ToList();
        }
    }
}