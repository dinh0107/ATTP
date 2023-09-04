using System;
using ATTP.Models;

namespace ATTP.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly DataEntities _context = new DataEntities();
        private GenericRepository<Admin> _adminRepository;
        private GenericRepository<ArticleCategory> _artcategoryRepository;
        private GenericRepository<Article> _articleRepository;
        private GenericRepository<Banner> _bannerRepository;
        private GenericRepository<Contact> _contactRepository;
        private GenericRepository<Cart> _cartRepository;
        private GenericRepository<ConfigSite> _configRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<Feedback> _feedbackRepository;
        private GenericRepository<Subcribe> _subcribeRepository;
        private GenericRepository<Member> _memberRepository;
        private GenericRepository<ProductCategory> _productCategoryRepository;
        private GenericRepository<Product> _productRepository;
        private GenericRepository<Order> _orderCateogryRepository;
        private GenericRepository<OrderDetail> _orderDetailRepository;
        private GenericRepository<City> _cityRepository;
        private GenericRepository<District> _districtRepository;
        private GenericRepository<Ward> _WardRepository;
        private GenericRepository<Question> _questionRepository;
        private GenericRepository<Address> _addressRepository;
        private GenericRepository<ProductReview> _productReviewRepository;

        public GenericRepository<ProductReview> ProductReviewRepository =>
      _productReviewRepository ?? (_productReviewRepository = new GenericRepository<ProductReview>(_context));
        public GenericRepository<Address> AddressRepository =>
       _addressRepository ?? (_addressRepository = new GenericRepository<Address>(_context));
        public GenericRepository<Question> QuestionRepository =>
       _questionRepository ?? (_questionRepository = new GenericRepository<Question>(_context));
        public GenericRepository<Cart> CartRepository =>
        _cartRepository ?? (_cartRepository = new GenericRepository<Cart>(_context));
        public GenericRepository<City> CityRepository =>
          _cityRepository ?? (_cityRepository = new GenericRepository<City>(_context));
        public GenericRepository<District> DistrictRepository =>
          _districtRepository ?? (_districtRepository = new GenericRepository<District>(_context));
        public GenericRepository<Ward> WardRepository =>
          _WardRepository ?? (_WardRepository = new GenericRepository<Ward>(_context));
        public GenericRepository<ProductCategory> ProductCategoryRepository =>
           _productCategoryRepository ?? (_productCategoryRepository = new GenericRepository<ProductCategory>(_context));
        public GenericRepository<Product> ProductRepository => _productRepository ?? (_productRepository = new GenericRepository<Product>(_context));
        public GenericRepository<Order> OrderRepository => _orderCateogryRepository ?? (_orderCateogryRepository = new GenericRepository<Order>(_context));
        public GenericRepository<OrderDetail> OrderDetailRepository => _orderDetailRepository ?? (_orderDetailRepository = new GenericRepository<OrderDetail>(_context));
        public GenericRepository<Member> MemberRepository => _memberRepository ?? (_memberRepository = new GenericRepository<Member>(_context));
        public GenericRepository<Subcribe> SubcribeRepository => _subcribeRepository ?? (_subcribeRepository = new GenericRepository<Subcribe>(_context));
        public GenericRepository<User> UserRepository => _userRepository ?? (_userRepository = new GenericRepository<User>(_context));
        public GenericRepository<Feedback> FeedbackRepository => _feedbackRepository ?? (_feedbackRepository = new GenericRepository<Feedback>(_context));
        public GenericRepository<ConfigSite> ConfigSiteRepository => _configRepository ?? (_configRepository = new GenericRepository<ConfigSite>(_context));
        public GenericRepository<Contact> ContactRepository => _contactRepository ?? (_contactRepository = new GenericRepository<Contact>(_context));
        public GenericRepository<Banner> BannerRepository => _bannerRepository ?? (_bannerRepository = new GenericRepository<Banner>(_context));
        public GenericRepository<Article> ArticleRepository => _articleRepository ?? (_articleRepository = new GenericRepository<Article>(_context));
        public GenericRepository<ArticleCategory> ArticleCategoryRepository => _artcategoryRepository ?? (_artcategoryRepository = new GenericRepository<ArticleCategory>(_context));
        public GenericRepository<Admin> AdminRepository => _adminRepository ?? (_adminRepository = new GenericRepository<Admin>(_context));
        public void Save()
        {
            _context.SaveChanges();
        }
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}