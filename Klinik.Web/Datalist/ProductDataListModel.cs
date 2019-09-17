using Datalist;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Web.Datalist
{
	public class ProductDataListModel
	{
		[Key]
		public long Id { get; set; }

		[DatalistColumn]
		[Display(Name = "Name")]
		public string Name { get; set; }

		[DatalistColumn]
		[Display(Name = "Code")]
		public string Code { get; set; }

		[DatalistColumn]
		[Display(Name = "Retail Price")]
		public decimal RetailPrice { get; set; }

		[DatalistColumn]
		[Display(Name = "Product Category")]
		public string ProductCategoryName { get; set; }

		[DatalistColumn]
		[Display(Name = "Product Unit")]
		public string ProductUnitName { get; set; }
	}
}