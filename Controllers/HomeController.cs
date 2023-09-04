using ATTP.DAL;
using ATTP.Filters;
using ATTP.Models;
using ATTP.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace ATTP.Controllers
{
    [MemberLoginFilter]

    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];
        private IEnumerable<ProductCategory> ProductCategories() =>
          _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<ArticleCategory> ArticleCategories() =>
            _unitOfWork.ArticleCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));

        [ChildActionOnly]
        public PartialViewResult Header()
        {
            var productCategory = ProductCategories().Where(a => a.ShowMenu);
            var articleCategory = ArticleCategories().Where(a => a.ShowMenu);
            var model = new HeaderViewModel
            {
                ProductCategories = productCategory,
                ArticleCategories = articleCategory
            };
            return PartialView(model);
        }
        public ActionResult Index()
        {
            var banner = _unitOfWork.BannerRepository.GetQuery(a => a.Active && a.GroupId == 1, o => o.OrderBy(a => a.Sort));
            var categoryHot = ProductCategories().Where(a => a.Hot);
            var productSaler = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Saler, o => o.OrderByDescending(a => a.CreateDate));
            var categories = ProductCategories().Where(a => a.ShowHome);
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Home, o => o.OrderByDescending(a => a.CreateDate));
            var articleCategory = ArticleCategories().Where(a => a.Home);
            var question = _unitOfWork.QuestionRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
            var categoryItems = categories.Select(a => new HomeViewModel.CategoryItem
            {
                ProductCategory = a,
                Products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.Home && (p.ProductCategoryId == a.Id || p.ProductCategory.ParentId == a.Id),
                    c => c.OrderByDescending(l => l.CreateDate), 10),
            });
            var model = new HomeViewModel
            {
                CategoryItems = categoryItems,
                ProductCategories = categoryHot,
                ArticleCategories = articleCategory,
                Articles = article,
                Banners = banner,
                ProductSaler = productSaler,
                Questions = question,
            };
            return View(model);
        }
        public PartialViewResult GetProductHot(int catId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Hot).Take(10);
            if (catId > 0)
            {
                product = product.Where(a => a.ProductCategoryId == catId);
            }
            var model = new GetProductHot
            {
                Products = product,
            };
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var articleCat = ArticleCategories().Where(a => a.ShowFooter);
            var model = new FooterViewModel
            {
                ArticleCategories = articleCat
            };
            return PartialView(model);
        }
        [Route("gioi-thieu")]
        public ActionResult About()
        {
            var banner = _unitOfWork.BannerRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
            var member = _unitOfWork.MemberRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
            var question = _unitOfWork.QuestionRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
            var feedback = _unitOfWork.FeedbackRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
            var model = new AboutViewModel
            {
                Banners = banner,
                Members = member,
                Question = question,
                Feedbacks = feedback,
            };
            return View(model);
        }
        #region Product 
        [Route("san-pham")]
        public ActionResult AllProduct(int? page, string keywords, string sort = "date-desc", int catId = 0)
        {
            var pageNumber = page ?? 1;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active, o => o.OrderByDescending(p => p.CreateDate));
            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, 9),
                ProductResult = products.Count(),
                CatId = catId,
                Keywords = keywords,
                Sort = sort,
                Categories = ProductCategories().Where(a => a.Filter),
            };
            return View(model);
        }
        public PartialViewResult GetProduct(int? page, string keywords, string sort = "date-desc", int catId = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Active, c => c.OrderByDescending(a => a.CreateDate)).FirstOrDefault();
            var products = _unitOfWork.ProductRepository.GetQuery(l => l.Active, c => c.OrderByDescending(a => a.CreateDate)).AsNoTracking();
            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (!String.IsNullOrEmpty(keywords))
            {
                products = products.Where(a => a.Name.Contains(keywords));
            }
            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                ProductResult = products.Count(),
                CatId = catId,
                Sort = sort,
                Keywords = keywords,
            };
            return PartialView(model);
        }
        [Route("san-pham/{url:regex(^(?!.*vcms).*$)}", Order = 1)]
        public ActionResult ProductCategory(int? page, string keywords, string url, int catId = 0, string sort = "date-desc")
        {
            var pageNumber = page ?? 1;
            var category = ProductCategories().FirstOrDefault(a => a.Url == url);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var products = _unitOfWork.ProductRepository.GetQuery(
                p => p.Active && (p.ProductCategoryId == category.Id || p.ProductCategory.ParentId == category.Id),
                c => c.OrderByDescending(p => p.CreateDate));

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, 9),
                CatId = catId,
                Category = category,
                Keywords = keywords,
                Sort = sort,
                Categories = ProductCategories().Where(a => a.Filter),
                Url = url,
            };
            return View(model);
        }
        public PartialViewResult GetProductCateogory(int? page, string keywords, string url, int catId = 0, string sort = "date-desc")
        {
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var category = ProductCategories().FirstOrDefault(a => a.Url == url);

            if (category == null)
            {
                category = ProductCategories().FirstOrDefault(a => a.Id == catId);
            }
            var product = _unitOfWork.ProductRepository.GetQuery(l => l.Active, c => c.OrderByDescending(a => a.CreateDate)).AsNoTracking();
            var products = _unitOfWork.ProductRepository.GetQuery(
                p => p.Active && (p.ProductCategoryId == category.Id || p.ProductCategory.ParentId == category.Id),
                c => c.OrderByDescending(p => p.CreateDate));
            if (catId > 0)
            {
                products = product.Where(a => a.ProductCategoryId == catId);
            }
            if (!String.IsNullOrEmpty(keywords))
            {
                products = products.Where(a => a.Name.Contains(keywords));
            }
            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Category = category,
                ProductResult = products.Count(),
                Url = url,
                CatId = catId,
                Keywords = keywords,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };
            return PartialView(model);
        }
        [Route("san-pham/{url}.html")]
        public ActionResult ProductDetail(string url)
        {
            var userId = Convert.ToInt32(RouteData.Values["Id"]);
            var product = _unitOfWork.ProductRepository.GetQuery(p => p.Url == url).FirstOrDefault();
            var userOrder = _unitOfWork.OrderRepository.GetQuery(a => a.UserId == userId);
            bool hasPurchased = userOrder.Any(a => a.Status == 2 && a.OrderDetails.Any(o => o.ProductId == product.Id));
            var products = _unitOfWork.ProductRepository.GetQuery(
                    p => p.Id != product.Id && p.Active && (p.ProductCategoryId == product.ProductCategoryId || p.ProductCategory.ParentId == product.ProductCategoryId),
                    o => o.OrderByDescending(p => Guid.NewGuid()), 4);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            var model = new ProductDetailViewModel
            {
                Product = product,
                Products = products,
                HasPurchased = hasPurchased,
            };
            return View(model);
        }
        [Route("tim-kiem")]
        public ActionResult Search(int? page, string keywords)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.Name.Contains(keywords),
                            o => o.OrderByDescending(p => p.CreateDate));
            var model = new SearchProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Keywords = keywords,
            };
            return View(model);
        }
        #endregion
        #region Article
        [Route("blog", Order = 1)]
        public ActionResult AllArticle(int? page)
        {
            var pageNumber = page ?? 1;
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
            var model = new AllArticleViewModel
            {
                Articles = article.ToPagedList(pageNumber, 9)
            };
            return View(model);
        }
        [Route("blog/{url}", Order = 0)]
        public ActionResult ArticleCategory(string url, int? page)
        {
            var category = _unitOfWork.ArticleCategoryRepository.GetQuery(a => a.CategoryActive && a.Url == url).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            var articles = _unitOfWork.ArticleRepository.GetQuery(
                a => a.Active && (a.ArticleCategoryId == category.Id || a.ArticleCategory.ParentId == category.Id),
                q => q.OrderByDescending(a => a.CreateDate));
            var pageNumber = page ?? 1;

            if (articles.Count() == 1)
            {
                var fi = articles.First();
                return RedirectToAction("ArticleDetail", new { url = fi.Url });
            }
            var model = new ArticleCategoryViewModel
            {
                Category = category,
                Articles = articles.ToPagedList(pageNumber, 9),
                Categories = ArticleCategories(),
            };

            if (category.ParentId != null)
            {
                model.RootCategory = _unitOfWork.ArticleCategoryRepository.GetById(category.ParentId);
            }
            return View(model);
        }
        [Route("blog/{url}.html", Order = 0)]
        public ActionResult ArticleDetail(string url)
        {
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Url == url && a.Active).FirstOrDefault();
            if (article == null)
            {
                return RedirectToAction("Index");
            }
            var model = new ArticleDetailsViewModel
            {
                Article = article,
                Articles = _unitOfWork.ArticleRepository.GetQuery(p =>
                p.Active, q => q.OrderByDescending(a => a.CreateDate), 6),
            };
            return View(model);
        }
        #endregion

        [Route("lien-he")]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Contact(Contact model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            _unitOfWork.ContactRepository.Insert(model);
            _unitOfWork.Save();
            var subject = "Email liên hệ từ website: " + Request.Url?.Host;
            var body = $"<p>Tên người liên hệ: {model.FullName},</p>" +
                        $"<p>Email liên hệ: {model.Email},</p>" +
                        $"<p>Số điện thoại: {model.Mobile},</p>" +
                        $"<p>Nội dung:{model.Body}</p>" +
                        $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";

            Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, ConfigSite.Email, Email, Email, Password, ConfigSite.Title));
            return Json(new { status = true, msg = "Gửi liên hệ thành công.\nChúng tôi sẽ liên lạc lại với bạn sớm nhất có thể." });
        }
        public ActionResult Cart()
        {
            return View();
        }
        public PartialViewResult ViewMember(int id)
        {
            var member = _unitOfWork.MemberRepository.GetQuery(a => a.Id == id);
            var model = new ViewMemberViewModel
            {
                Members = member,
            };
            return PartialView(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SubcribeForm(Subcribe model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            _unitOfWork.SubcribeRepository.Insert(model);
            _unitOfWork.Save();
            var subject = "Email liên hệ từ website: " + Request.Url?.Host;
            var body =
                       $"<p>Email: {model.Email},</p>" +
                       $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";
            Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, ConfigSite.Email, Email, Email, Password, ConfigSite.Title));

            return Json(new { status = true, msg = "Gửi liên hệ thành công.\nChúng tôi sẽ liên lạc với bạn sớm nhất có thể." });
        }
        #region Review
        [ChildActionOnly]
        public PartialViewResult ReviewForm(string url)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            var userId = Convert.ToInt32(RouteData.Values["Id"]);
            var model = new ReviewFormViewModel
            {
                User = _unitOfWork.UserRepository.GetQuery(a => a.Id == userId).FirstOrDefault(),
                Product = product,
            };
            return PartialView(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SubmitReview(ReviewViewModel model, FormCollection fc)
        {
            var product = _unitOfWork.ProductRepository.GetById(model.ProductReview.ProductId);
            if (product != null)
            {
                model.ProductReview.Product = product;
            }
            if (ModelState.IsValid)
            {
                var files = Request.Files.GetMultiple("ImageFile");
                var imageFileNames = new List<string>();

                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                        {
                            ModelState.AddModelError("", "Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                            return Json(new { status = false, msg = "Ảnh không đúng định dạng jpg, png, gif, jpeg, svg" });
                        }
                        else
                        {
                            var imgPath = "/images/review/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);
                            imageFileNames.Add(DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName);
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }

                if (imageFileNames.Count > 0)
                {
                    model.ProductReview.ListImage = string.Join(",", imageFileNames);
                }
                else
                {
                    model.ProductReview.ListImage = null;
                }

                model.ProductReview.ParentReviewId = 0;

                _unitOfWork.ProductReviewRepository.Insert(model.ProductReview);
                _unitOfWork.Save();
                return RedirectToAction("ProductDetail", new { url = model.ProductReview.Product.Url });
            }

            return RedirectToAction("ProductDetail", new { url = model.ProductReview.Product.Url });
        }
        public PartialViewResult GetReview(string url, int dateValue = 0)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            var review = _unitOfWork.ProductReviewRepository.Get().OrderByDescending(a => a.CreatedAt);
            switch (dateValue)
            {
                case 1:
                    review = review.OrderByDescending(a => a.CreatedAt);
                    break;
                case 2:
                    review = review.OrderByDescending(a => a.CreatedAt);
                    break;
                case 4:
                    review = review.OrderByDescending(a => a.Rating == 5);
                    break;
                case 5:
                    review = review.OrderByDescending(a => a.Rating == 4);
                    break;
                case 6:
                    review = review.OrderByDescending(a => a.Rating == 3);
                    break;
                case 7:
                    review = review.OrderByDescending(a => a.Rating == 2);
                    break;
                case 8:
                    review = review.OrderByDescending(a => a.Rating == 1);
                    break;

            }
            var model = new GetReviewViewModel
            {
                ProductReviews = review,
                Product = product,
            };
            return PartialView(model);
        }
        public JsonResult Reply(ReplyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(RouteData.Values["Id"]);
                model.ProductReview.ProductId = model.ProductId;
                model.ProductReview.Likes = 0;
                model.ProductReview.Rating = 0;
                model.ProductReview.UserId = userId;
                model.ProductReview.ParentReviewId = model.CommentId;
                _unitOfWork.ProductReviewRepository.Insert(model.ProductReview);
                _unitOfWork.Save();
                return Json(new { status = true, msg = "Bình luận thành công" });
            }
            return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
        }
        public PartialViewResult DasboardReview(string url)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            var reviews = _unitOfWork.ProductReviewRepository.GetQuery(a => a.ProductId == product.Id).ToList();
            double totalRatingSum = reviews.Sum(review => review.Rating);
            List<string> allListImages = new List<string>();
            int totalReviewCount = reviews.Count;
            double averageRating = totalReviewCount > 0 ? totalRatingSum / totalReviewCount : 0.0;
            averageRating = Math.Round(averageRating, 1);
            Dictionary<int, int> ratingCounts = new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },
                { 5, 0 }
            };
            foreach (var review in reviews)
            {
                if (ratingCounts.ContainsKey(review.Rating))
                {
                    ratingCounts[review.Rating]++;
                }
            }
            foreach (var review in reviews)
            {
                string[] listImages = review.ListImage.Split(',');
                allListImages.AddRange(listImages);
            }
            var model = new DasboardReview
            {
                Arverage = averageRating,
                TotalReview = _unitOfWork.ProductReviewRepository.GetQuery(a => a.ProductId == product.Id).Count(),
                PercentageOneStar = Math.Round((double)ratingCounts[1] / totalReviewCount * 100, 1),
                PercentageTwoStar = Math.Round((double)ratingCounts[2] / totalReviewCount * 100, 1),
                PercentageThreeStar = Math.Round((double)ratingCounts[3] / totalReviewCount * 100, 1),
                PercentageFourStar = Math.Round((double)ratingCounts[4] / totalReviewCount * 100, 1),
                PercentageFiveStar = Math.Round((double)ratingCounts[5] / totalReviewCount * 100, 1),
                ListImg = string.Join(",", allListImages),
                CountImg = allListImages.Count - 6,
            };
            return PartialView(model);
        }
        [HttpPost]
        public JsonResult ToggleLikeForReview(int reviewId)
        {
            var likedReviews = Session["LikedReviews"] as List<int>;
            if (likedReviews == null)
            {
                likedReviews = new List<int>();
            }

            if (likedReviews.Contains(reviewId))
            {
                likedReviews.Remove(reviewId);

                var review = _unitOfWork.ProductReviewRepository.GetById(reviewId);
                if (review != null)
                {
                    review.Likes--;
                    _unitOfWork.Save();
                }

                Session["LikedReviews"] = likedReviews;
                return Json(new { liked = false, Likes = review.Likes });
            }
            else
            {
                likedReviews.Add(reviewId);
                var review = _unitOfWork.ProductReviewRepository.GetById(reviewId);
                if (review != null)
                {
                    review.Likes++;
                    _unitOfWork.Save();
                }

                Session["LikedReviews"] = likedReviews;
                return Json(new { liked = true, Likes = review.Likes });
            }
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}