using Datalist;
using Klinik.Data.DataRepository;
using Klinik.Web.Datalist;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Web
{
	public class ProductDataList : MvcDatalist<ProductDataListModel>
	{
		private KlinikDBEntities _context { get; }
		private long _clinicID { get; }

		public ProductDataList(KlinikDBEntities context, long clinicID)
		{
			_context = context;
			_clinicID = clinicID;
		}

		public ProductDataList()
		{
			Url = "AllProduct";
			Title = "Product";

			Filter.Sort = "Name";
			Filter.Order = DatalistSortOrder.Asc;
		}

		public override IQueryable<ProductDataListModel> GetModels()
		{
			List<ProductDataListModel> result = new List<ProductDataListModel>();
			List<Product> productClinicList = _context.Products.Where(x => x.ClinicID == _clinicID).ToList();

			foreach (var product in productClinicList)
			{
				ProductDataListModel productModel = MapFrom(product);
				result.Add(productModel);
			}

			return result.AsQueryable();
		}

		private ProductDataListModel MapFrom(Product product)
		{
			ProductDataListModel productModel = new ProductDataListModel
			{
				Id = product.ID,
				Name = product.Name,
				RetailPrice = product.RetailPrice,
				ProductCategoryName = product.ProductCategory.Name,
				ProductUnitName = product.ProductUnit.Name
			};

			return productModel;
		}
	}
}