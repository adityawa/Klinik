using Klinik.Data;
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.MasterData.Hospital
{
    public class HospitalHandler : BaseFeatures
    {
        public HospitalHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public HospitalResponse GetHospital()
        {
            List<HospitalModel> hospitals = new List<HospitalModel>();
            var _qry = _unitOfWork.HospitalRepository.Get(x => x.RowStatus == 0);
            foreach(var item in _qry)
            {
                hospitals.Add(new HospitalModel
                {
                    Id=item.Id,
                    Name=item.Name,
                    Phone=item.Phone
                });
            }

            var response = new HospitalResponse
            {
                Data = hospitals
            };

            return response;
        }
    }
}
