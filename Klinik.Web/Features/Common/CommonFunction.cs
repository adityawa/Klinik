using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess.Interfaces;
using Klinik.Web.DataAccess.Concrete;
namespace Klinik.Web.Features.Common
{
    public class CommonFunction<TEntity> where TEntity:class
    {
        private KlinikDBEntities _clinicContext;
        private IUnitOfWork<TEntity> _unitOfWork;
        public CommonFunction(KlinikDBEntities clinicContext, UnitOfWork<TEntity> unitOfWork)
        {
            _clinicContext = clinicContext;
            _unitOfWork = unitOfWork;
        }

        public List<Clinic> GetClinic()
        {
            var qry
        }
    }
}