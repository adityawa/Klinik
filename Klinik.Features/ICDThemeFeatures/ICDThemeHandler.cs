using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.ICDThemeFeatures
{
    public class ICDThemeHandler : BaseFeatures
    {
        public ICDThemeHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public List<ICDThemeModel> GetAll()
        {
            List<ICDThemeModel> IcdThems = new List<ICDThemeModel>();
            var _qry = _unitOfWork.ICDThemeRepository.Get(x => x.RowStatus == 0);
            foreach(var item in _qry)
            {
                IcdThems.Add(new ICDThemeModel
                {
                    Id = item.Id,
                    Name=item.Name
                });
            }
            return IcdThems;
        }

        public List<ICDThemeModel> Get(string prefix, long id1, long id2)
        {
            List<ICDThemeModel> IcdThems = new List<ICDThemeModel>();
            List<long> excludeID = new List<long>();
            if (id1 > 0)
                excludeID.Add(id1);
            if (id2 > 0)
                excludeID.Add(id2);

            var _qry = _unitOfWork.ICDThemeRepository.Get(x => x.RowStatus == 0 && x.Name.Contains(prefix) && !excludeID.Contains(x.Id));
            foreach (var item in _qry)
            {
                IcdThems.Add(new ICDThemeModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Code=item.Code
                });
            }
            return IcdThems;
        }
    }
}
