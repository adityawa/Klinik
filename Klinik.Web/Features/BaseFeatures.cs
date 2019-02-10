using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
namespace Klinik.Web.Features
{
    public abstract class BaseFeatures
    {
        public IUnitOfWork _unitOfWork;

        //public void DisposeUnitOfWork(ref IUnitOfWork unitOfWork)
        //{
        //    unitOfWork.Dispose();

        //    unitOfWork = new UnitOfWork(new KlinikDBEntities());
        //}
    }
}