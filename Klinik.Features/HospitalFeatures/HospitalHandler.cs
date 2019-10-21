using Klinik.Data;
using Klinik.Data.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.HospitalFeatures
{
    public class HospitalHandler : BaseFeatures
    {
        public HospitalHandler(IUnitOfWork unitOfWork, KlinikDBEntities context=null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public string GetHospitalName(string strId)
        {
            int iId = Convert.ToInt16(strId);
            string _hospitalName = "";
            var qry = _unitOfWork.HospitalRepository.GetById(iId);
            if (qry != null)
                _hospitalName = qry.Name;
            return _hospitalName;
        }
    }
}
