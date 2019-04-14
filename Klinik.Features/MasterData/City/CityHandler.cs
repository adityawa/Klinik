using AutoMapper;
using Klinik.Data;
using Klinik.Entities.MasterData;
using System.Collections.Generic;

namespace Klinik.Features
{
    public class CityHandler : BaseFeatures
    {
        public CityHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<CityModel> GetAllCity()
        {
            var qry = _unitOfWork.CityRepository.Get();
            IList<CityModel> cities = new List<CityModel>();
            foreach (var item in qry)
            {
                var _citi = Mapper.Map<Klinik.Data.DataRepository.City, CityModel>(item);
                cities.Add(_citi);
            }

            return cities;
        }
    }
}
