using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.Models.MasterData;
using AutoMapper;
namespace Klinik.Web.Features.MasterData.Clinic
{
    public class ClinicHandler:BaseFeatures
    {
      
        public ClinicHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<ClinicModel> GetAllClinic()
        {
            var qry = _unitOfWork.ClinicRepository.Get();
            IList<ClinicModel> clinics = new List<ClinicModel>();
            foreach(var item in qry)
            {
                var _clinic = Mapper.Map<Web.Clinic, ClinicModel>(item);
                clinics.Add(_clinic);
            }

            return clinics;
        }
    }
}