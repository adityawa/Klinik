using AutoMapper;
using Klinik.Data;
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.MasterData.Poli
{
    public class PoliHandler : BaseFeatures
    {
        public PoliHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IList<PoliModel> GetAllPoli(int exId)
        {
            var qry = _unitOfWork.PoliRepository.Get(x => x.ID != exId);
            IList<PoliModel> polies = new List<PoliModel>();
            foreach (var item in qry)
            {
                var _poli = Mapper.Map<Klinik.Data.DataRepository.Poli, PoliModel>(item);
                polies.Add(_poli);
            }

            return polies;
        }

    }
}
