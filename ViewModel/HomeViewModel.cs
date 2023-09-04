using ATTP.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;

namespace ATTP.ViewModel
{
    public class HeaderViewModel
    {
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
    }
    public class FooterViewModel
    {
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
    }
    public class HomeViewModel
    {
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Product> ProductSaler { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<CategoryItem> CategoryItems { get; set; }
        public class CategoryItem
        {
            public ProductCategory ProductCategory { get; set; }
            public IEnumerable<Product> Products { get; set; }
        }
    }
    public class GetProductHot
    {
        public IEnumerable<Product> Products { get; set; }
    }
    public class CategoryProductViewModel
    {
        public int? CatId { get; set; }
        public ProductCategory Category { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IPagedList<Product> Products { get; set; }
        public string Url { get; set; }
        public string Sort { set; get; }
        public string Keywords { get; set; }
        public int ProductResult { get; set; }
        public int BeginCount { get; set; }
        public int EndCount { get; set; }
    }
    public class ProductDetailViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public Product Product { get; set; }
        public ProductReview ProductReview { get; set; }
        public bool HasPurchased { get; set; }
        //public IEnumerable<ProductCategory> Categories { get; set; }
    }
    public class AllArticleViewModel
    {
        public IPagedList<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> Categories { get; set; }
    }
    public class ArticleCategoryViewModel
    {
        public ArticleCategory RootCategory { get; set; }
        public ArticleCategory Category { get; set; }
        public IPagedList<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> Categories { get; set; }
    }
    public class ArticleDetailsViewModel
    {
        public Article Article { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
    public class AboutViewModel
    {
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Member> Members { get; set; }
        public IEnumerable<Question> Question { get; set; }
        public IEnumerable<Feedback> Feedbacks { get; set; }
    }
    public class ReviewViewModel
    {
        public ProductReview ProductReview { get; set; }
    }
    public class ReviewFormViewModel
    {
        public ProductReview ProductReview { get; set; }
        public Product Product { get; set; }
        public User User { get; set; }
    }
    public class GetReviewViewModel
    {
        public IEnumerable<ProductReview> ProductReviews { get; set; }
        public Product Product { get; set; }
    }
    public class SearchProductViewModel
    {
        public string Keywords { get; set; }
        public IPagedList<Product> Products { get; set; }
    }
    public class ViewMemberViewModel
    {
        public IEnumerable<Member> Members { get; set; }
    }
    public class ReplyViewModel
    {
        public ProductReview ProductReview { get; set; }
        public User User { get; set; }
        public int CommentId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
    }

    public class DasboardReview
    {
        public double PercentageOneStar { get; set; }
        public double PercentageTwoStar { get; set; }
        public double PercentageThreeStar { get; set; }
        public double PercentageFourStar { get; set; }
        public double PercentageFiveStar { get; set; }
        public double Arverage { get; set; }
        public int TotalReview { get; set; }
        public string ListImg { get; set; }
        public int CountImg { get; set; }
    }
}