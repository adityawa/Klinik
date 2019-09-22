using AutoMapper;
using Klinik.Data;
using repository= Klinik.Data.DataRepository;
using Klinik.Entities.ProductCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Features.ProductCategory
{
    public class ProductCategoryHandler: BaseFeatures
    {
        public ProductCategoryHandler(IUnitOfWork unitOfWork, repository.KlinikDBEntities context=null)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public List<ProductCategoryModel> GetAllProductCategory()
        {
            List<ProductCategoryModel> productCategories = new List<ProductCategoryModel>();
            var qry = _unitOfWork.ProductCategoryRepository.Get(x=>x.RowStatus==0);
            foreach(var item in qry)
            {
                productCategories.Add(new ProductCategoryModel
                {
                    Id=item.ID,
                    Name=item.Name
                });
            }

            return productCategories;
        }
    }
}
