using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.Models;
namespace Klinik.DAL.Interfaces
{
    public interface IEmployee
    {
        List<Employee> GetAllEmployee();
        Employee GetEmployeeById(int id);
        Employee GetEmployeeByEmployeeId(string empid);
        Int32? AddEmployee(Employee emp);
        Int32? UpdateEmployee(Employee emp);
        int DeleteEmployee(int id);
        bool CheckEmployeeIdExist(string empid);
        
    }
}
