using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klinik.DAL.Interfaces;
namespace Klinik.DAL.Implementor
{
    public class DepartmentConcrete:BaseConcrete, IDepartment
    {

        public List<Department> GetAllDepartment()
        {
            try
            {
                return _context.Departments.Where(x => x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Department GetById(int id)
        {
            throw new NotImplementedException();
        }

        public int Add(Department dept)
        {
            throw new NotImplementedException();
        }

        public int Update(Department dept)
        {
            throw new NotImplementedException();
        }

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
