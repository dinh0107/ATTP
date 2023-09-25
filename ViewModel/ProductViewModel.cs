using ATTP.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ATTP.ViewModel
{
    public class InsertCategoryViewModel
    {
        public ProductCategory ProductCategory { get; set; }
        [Display(Name = "Danh mục sản phẩm")]
        public int? ParentId { get; set; }
        [Display(Name = "Danh mục sản phẩm")]
        public int? CategoryId { get; set; }

        public SelectList ChildCategoryList { get; set; }
        public SelectList ProductCategoryList { get; set; }

        public InsertCategoryViewModel()
        {
            ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "Name");
        }
    }

    public class ListProductViewModel
    {
        public PagedList.IPagedList<Product> Products { get; set; }
        public SelectList SelectCategories { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public SelectList ThirdCategoryList { get; set; }
        public int? ParentId { get; set; }
        public int? CatId { get; set; }
        public int? ThirdId { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }
        public int? Type { get; set; }

        public ListProductViewModel()
        {
            ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
            ThirdCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
        }
    }

    public class InsertProductViewModel
    {
        public Product Product { get; set; }
        [Display(Name = "Danh mục sản phẩm con"), Required(ErrorMessage = "Hãy chọn danh mục sản phẩm")]
        public int ParentId { get; set; }
        [Display(Name = "Danh mục sản phẩm cha")]
        public int? CategoryId { get; set; }
        [Display(Name = "Thương hiệu")]
        public int? BrandId { get; set; }
        public int CountFilter { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public SelectList SelectCategories { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public SelectList ProductCategoryList { get; set; }

        [Display(Name = "Giá niêm yết"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string Price { get; set; }
        [Display(Name = "Giá khuyến mãi"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceSale { get; set; }

        public InsertProductViewModel()
        {
            ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
        }
    }
}