using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.DAL.Interfaces
{
    public interface IParameter
    {
        List<Parameter> GetParameterBasedOnCode(string code);
        Parameter GetById(int id);
    }
}
