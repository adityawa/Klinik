using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.DAL.Interfaces;
using System.Data.Entity;
namespace Klinik.DAL.Implementor
{
    public class EmployeeConcrete : BaseConcrete, IEmployee
    {
        public EmployeeConcrete(DB_Klinik_2Entities context)
        {
            this._context = context;
        }
        public List<Employee> GetAllEmployee()
        {
            try
            {
                return _context.Employees.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Employee GetEmployeeById(int id)
        {
            try
            {
                return _context.Employees.SingleOrDefault(x => x.id == id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Employee GetEmployeeByEmployeeId(string empid)
        {
            try
            {
                return _context.Employees.SingleOrDefault(x => x.EmpID == empid);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Int32? AddEmployee(Employee emp)
        {
            Int32? retId = 0;
            try
            {
                emp.DateCreated = DateTime.Now;
                _context.Employees.Add(emp);
                _context.SaveChanges();
                retId = emp.id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retId;
        }

        public Int32? UpdateEmployee(Employee emp)
        {
            int retId = 0;
            try
            {
                emp.DateModified = DateTime.Now;
                _context.Entry(emp).State = EntityState.Modified;
                _context.SaveChanges();
                retId = emp.id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retId;
        }

        public int DeleteEmployee(int id)
        {
            int result_affected = 0;
            var toRemove = _context.Employees.SingleOrDefault(x => x.id == id);
            _context.Employees.Remove(toRemove);
            result_affected = _context.SaveChanges();

            return result_affected;
        }

        public bool CheckEmployeeIdExist(string empid)
        {
            bool isExist = true;
            try
            {
                var result = _context.Employees.SingleOrDefault(x => x.EmpID == empid);
                if (result == null)
                {
                    isExist = false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return isExist;
        }

       
    }
}
