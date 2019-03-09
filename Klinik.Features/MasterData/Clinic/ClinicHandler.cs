using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using System.Collections.Generic;

namespace Klinik.Features
{
    public class ClinicHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ClinicHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all clinic
        /// </summary>
        /// <returns></returns>
        public IList<ClinicModel> GetAllClinic()
        {
            var qry = _unitOfWork.ClinicRepository.Get();
            IList<ClinicModel> clinics = new List<ClinicModel>();
            foreach (var item in qry)
            {
                var _clinic = Mapper.Map<Clinic, ClinicModel>(item);
                clinics.Add(_clinic);
            }

            return clinics;
        }
    }
}