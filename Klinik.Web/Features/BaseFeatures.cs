using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Klinik.Web.DataAccess;
using Klinik.Web.DataAccess.DataRepository;

namespace Klinik.Web.Features
{
    public abstract class BaseFeatures
    {
        public IUnitOfWork _unitOfWork;
        public KlinikDBEntities _context;
        public IList<string> errorFields = new List<string>();

       
    }
}