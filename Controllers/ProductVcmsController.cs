using ATTP.DAL;
using ATTP.Models;
using ATTP.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATTP.Controllers
{
    [Authorize]
    public class ProductVcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private IEnumerable<ProductCategory> ProductCategories =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private SelectList ParentProductCategoryList => new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");

        #region ProductCategory
        [ChildActionOnly]
        public ActionResult ListCategory()
        {
            var allcats = _unitOfWork.ProductCategoryRepository.Get(orderBy: q => q.OrderBy(a => a.CategorySort));
            return PartialView(allcats);
        }
        public ActionResult ProductCategory(string result = "", int parentId = 0, int categoryId = 0)
        {
            ViewBag.ArticleCat = result;
            var model = new InsertCategoryViewModel
            {
                ProductCategoryList = ParentProductCategoryList,
                ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "Name"),
                ProductCategory = new ProductCategory { CategorySort = 1 },
                ParentId = parentId,
                CategoryId = categoryId,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductCategory(InsertCategoryViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "ProductCategory.Image")
                    {
                        model.ProductCategory.Image = imgFile;
                    }
                }
                model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(null, model.ProductCategory.Url ?? model.ProductCategory.CategoryName);
                model.ProductCategory.ParentId = model.CategoryId ?? model.ParentId;
                _unitOfWork.ProductCategoryRepository.Insert(model.ProductCategory);
                _unitOfWork.Save();
                return RedirectToAction("ProductCategory", new { result = "success" });
            }
            model.ProductCategoryList = ParentProductCategoryList;
            model.ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "Name");
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        public ActionResult UpdateCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            var parentCat = ProductCategories.FirstOrDefault(a => a.Id == category.ParentId);
            if (category == null)
            {
                return RedirectToAction("ProductCategory");
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            var model = new InsertCategoryViewModel
            {
                ProductCategory = category,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateCategory(InsertCategoryViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var category = _unitOfWork.ProductCategoryRepository.GetById(model.ProductCategory.Id);
                if (category == null)
                {
                    return RedirectToAction("ProductCategory");
                }

                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "ProductCategory.Image")
                    {
                        model.ProductCategory.Image = imgFile;
                        category.Image = model.ProductCategory.Image;
                    }

                }
                if (model.ProductCategory.Image == null)
                {
                    category.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];

                }
                category.Url = HtmlHelpers.ConvertToUnSign(null, model.ProductCategory.Url ?? model.ProductCategory.CategoryName);
                category.ParentId = model.CategoryId ?? model.ProductCategory.ParentId;
                category.CategoryName = model.ProductCategory.CategoryName;
                category.CategorySort = model.ProductCategory.CategorySort;
                category.Description = model.ProductCategory.Description;
                category.CategoryActive = model.ProductCategory.CategoryActive;
                category.ShowMenu = model.ProductCategory.ShowMenu;
                category.ShowHome = model.ProductCategory.ShowHome;
                category.TitleMeta = model.ProductCategory.TitleMeta;
                category.DescriptionMeta = model.ProductCategory.DescriptionMeta;
                category.Body = model.ProductCategory.Body;
                category.Hot = model.ProductCategory.Hot;
                category.Filter = model.ProductCategory.Filter;
                _unitOfWork.Save();
                return RedirectToAction("ProductCategory", new { result = "update" });
            }
            model.ProductCategoryList = ParentProductCategoryList;
            model.ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "Name");
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            if (category == null)
            {
                return false;
            }
            _unitOfWork.ProductCategoryRepository.Delete(category);
            _unitOfWork.Save();
            return true;
        }
        public bool UpdateProductCat(int sort = 1, bool active = false, bool home = false, bool menu = false, int productCatId = 0)
        {
            var productCat = _unitOfWork.ProductCategoryRepository.GetById(productCatId);
            if (productCat == null)
            {
                return false;
            }
            productCat.CategorySort = sort;
            productCat.CategoryActive = active;
            productCat.ShowHome = home;
            productCat.ShowMenu = menu;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Product  
        public ActionResult ListProduct(int? page, string name, int? parentId, int catId = 0, int thirdId = 0, string sort = "date-desc", string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var products = _unitOfWork.ProductRepository.GetQuery().AsNoTracking();
            if (thirdId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == thirdId);
            }
            else if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            else if (parentId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == parentId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(l => l.Name.Contains(name));
            }

            switch (sort)
            {
                case "sort-asc":
                    products = products.OrderBy(a => a.Sort);
                    break;
                case "sort-desc":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "hot":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "date-asc":
                    products = products.OrderBy(a => a.CreateDate);
                    break;
                case "price-asc":
                    products = products.OrderBy(a => a.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(a => a.Price);
                    break;
                default:
                    products = products.OrderByDescending(a => a.CreateDate);
                    break;
            }
            var model = new ListProductViewModel
            {
                SelectCategories = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                Products = products.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                ParentId = parentId,
                Name = name,
                Sort = sort,
                ThirdId = thirdId,
            };
            if (parentId.HasValue)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == parentId), "Id", "CategoryName");
            }
            if (catId > 0)
            {
                model.ThirdCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == catId), "Id", "CategoryName");
            }
            return View(model);
        }
        public ActionResult Product()
        {
            var model = new InsertProductViewModel
            {
                Product = new Product { Sort = 1, Active = true },
                Categories = ProductCategories,
            };
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Product(InsertProductViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                if (model.Price != null)
                {
                    model.Product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
                }
                if (model.PriceSale != null)
                {
                    model.Product.PriceSale = Convert.ToDecimal(model.PriceSale.Replace(",", ""));
                }

                model.Product.ListImage = fc["Pictures"];
                model.Product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                model.Product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);

                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == model.Product.Url).Count();
                if (count > 1)
                {
                    model.Product.Url += "-" + DateTime.Now.Millisecond;
                    _unitOfWork.Save();
                }
                _unitOfWork.ProductRepository.Insert(model.Product);
                _unitOfWork.Save();
                return RedirectToAction("ListProduct", new { result = "success" });
            }

            model.Categories = ProductCategories;
            return View(model);
        }
        public ActionResult UpdateProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }
            var model = new InsertProductViewModel
            {
                Product = product,
                Categories = ProductCategories,
                Price = product.Price?.ToString("N0"),
                PriceSale = product.PriceSale?.ToString("N0"),
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateProduct(InsertProductViewModel model, FormCollection fc)
        {
            var product = _unitOfWork.ProductRepository.GetById(model.Product.Id);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }
            if (ModelState.IsValid)
            {
                if (model.Price != null)
                {
                    product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
                }
                else
                {
                    product.Price = null;
                }
                if (model.PriceSale != null)
                {
                    product.PriceSale = Convert.ToDecimal(model.PriceSale.Replace(",", ""));
                }
                else
                {
                    product.PriceSale = null;
                }

                product.ListImage = fc["Pictures"] == "" ? null : fc["Pictures"];
                product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                product.Name = model.Product.Name;
                product.Body = model.Product.Body;
                product.Active = model.Product.Active;
                product.Description = model.Product.Description;
                product.Home = model.Product.Home;
                product.TitleMeta = model.Product.TitleMeta;
                product.DescriptionMeta = model.Product.DescriptionMeta;
                product.Sort = model.Product.Sort;
                product.Saler = model.Product.Saler;
                product.New = model.Product.New;
                product.Quantity = model.Product.Quantity;
                product.Hot = model.Product.Hot;
                product.Selling = model.Product.Selling;
                product.Specifications = model.Product.Specifications;
                _unitOfWork.Save();
                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == product.Url).Count();
                if (count > 1)
                {
                    product.Url += "-" + DateTime.Now.Millisecond;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListProduct", new { result = "update" });
            }

            model.Categories = ProductCategories;
            return View(model);
        }
        [HttpPost]
        public bool DeleteProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            _unitOfWork.ProductRepository.Delete(product);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool QuickUpdate(int? quantity, bool? status, bool active, bool home, bool newProduct, int sort = 0, int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            if (status != null)
            {
                product.Active = Convert.ToBoolean(status);
            }
            if (sort >= 0)
            {
                product.Sort = sort;
            }
            product.Active = active;
            product.Home = home;
            _unitOfWork.Save();
            return true;
        }
        #endregion


        public JsonResult GetProductCategory(int? parentId)
        {
            var categories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(categories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChildCategory(int parentId = 0)
        {
            var childCategories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(childCategories.Select(a => new { a.Id, a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetThirdProductCategory(int? childId)
        {
            var thirdCategories = ProductCategories.Where(a => a.ParentId == childId);
            return Json(thirdCategories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}