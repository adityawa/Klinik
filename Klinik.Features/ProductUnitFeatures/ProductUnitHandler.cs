using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.ProductUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.ProductUnitFeatures
{
    public class ProductUnitHandler : BaseFeatures
    {
        public ProductUnitHandler(IUnitOfWork unitOfWork, KlinikDBEntities context = null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public List<ProductUnitModel> GetAllProductUnit()
        {
            List<ProductUnitModel> productUnits = new List<ProductUnitModel>();
            var qry = _unitOfWork.ProductUnitRepository.Get(x => x.RowStatus == 0);
            foreach(var item in qry)
            {
                productUnits.Add(new ProductUnitModel
                {
                    Id=item.ID,
                    Name=item.Name
                });
            }

            return productUnits;
        }
    }
}
