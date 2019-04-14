using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using System.Collections.Generic;

namespace Klinik.Features
{
    public class EmployeeStatusHandler : BaseFeatures
    {
        public EmployeeStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<EmployeeStatusModel> GetAllEmployeeStatus()
        {
            var qry = _unitOfWork.EmployeeStatusRepository.Get();
            IList<EmployeeStatusModel> empstatus = new List<EmployeeStatusModel>();
            foreach (var item in qry)
            {
                var _status = Mapper.Map<EmployeeStatu, EmployeeStatusModel>(item);
                empstatus.Add(_status);
            }

            return empstatus;
        }
    }
}
