using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.DAL.Interfaces
{
    public interface IDepartment
    {
        List<Department> GetAllDepartment();
        Department GetById(int id);
        Int32 Add(Department dept);
        Int32 Update(Department dept);
        Int32 Delete(int id);
    }
}
