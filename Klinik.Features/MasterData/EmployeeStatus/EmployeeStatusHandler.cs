using Klinik.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.MasterData.EmployeeStatus
{
    public class EmployeeStatusHandler : BaseFeatures
    {
        public EmployeeStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
    }
}
