using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.Models.MasterData;
using Klinik.Web.Infrastructure;
using AutoMapper;
namespace Klinik.Web.Features.MasterData.Employee
{
    public class EmployeeHandler:BaseFeatures
    {
        public EmployeeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<EmployeeModel> GetAllEmployee()
        {
            var qry = _unitOfWork.EmployeeRepository.Get();
            IList<EmployeeModel> employees = new List<EmployeeModel>();
            foreach (var item in qry)
            {
                var _clinic = Mapper.Map<Web.Employee, EmployeeModel>(item);
                employees.Add(_clinic);
            }

            return employees;
        }
    }
}