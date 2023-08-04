using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using Doris.Models;
using WebGrease.Css.Ast.Selectors;

namespace Doris.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly DataEntities _context = new DataEntities();
        private GenericRepository<Admin> _adminRepository;
        private GenericRepository<ArticleCategory> _articategoryRepository;
        private GenericRepository<Article> _articleRepository;
        private GenericRepository<Banner> _bannerRepository;
        private GenericRepository<Contact> _contactRepository;
        private GenericRepository<ConfigSite> _configRepository;
        private GenericRepository<Product> _productRepository;
        private GenericRepository<ProductCategory> _productCategoryRepository;
        private GenericRepository<Cart> _cartRepository;
        private GenericRepository<City> _cityRepository;
        private GenericRepository<District> _districtRepository;
        private GenericRepository<Order> _orderRepository;
        private GenericRepository<OrderDetail> _orderdetailRepository;
        private GenericRepository<Ward> _wardRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<Subcribe> _subcribeRepository;
        private GenericRepository<Brand> _brandRepository;
        private GenericRepository<Address> _addressRepository;
        private GenericRepository<Bank> _bankRepository;
        private GenericRepository<BankUser> _bankUserRepository;
        private GenericRepository<ProductUser> _productUserRepository;

        public GenericRepository<ProductUser> ProductUserRepository =>
            _productUserRepository ?? (_productUserRepository = new GenericRepository<ProductUser>(_context));
        public GenericRepository<Bank> BankRepository =>
            _bankRepository ?? (_bankRepository = new GenericRepository<Bank>(_context));
        public GenericRepository<BankUser> BankUserRepository =>
            _bankUserRepository ?? (_bankUserRepository = new GenericRepository<BankUser>(_context));
        public GenericRepository<Address> AddressRepository =>
            _addressRepository ?? (_addressRepository = new GenericRepository<Address>(_context));
        public GenericRepository<Brand> BrandRepository =>
            _brandRepository ?? (_brandRepository = new GenericRepository<Brand>(_context));
        public GenericRepository<Subcribe> SubcribeRepository =>
            _subcribeRepository ?? (_subcribeRepository = new GenericRepository<Subcribe>(_context));
        public GenericRepository<User> UserRepository =>
            _userRepository ?? (_userRepository = new GenericRepository<User>(_context));
        public GenericRepository<Ward> WardRepository =>
            _wardRepository ?? (_wardRepository = new GenericRepository<Ward>(_context));
        public GenericRepository<OrderDetail> OrderDetailRepository =>
            _orderdetailRepository ?? (_orderdetailRepository = new GenericRepository<OrderDetail>(_context));
        public GenericRepository<Order> OrderRepository =>
            _orderRepository ?? (_orderRepository = new GenericRepository<Order>(_context));
        public GenericRepository<District> DistrictRepository =>
            _districtRepository ?? (_districtRepository = new GenericRepository<District>(_context));
        public GenericRepository<City> CityRepository =>
            _cityRepository ?? (_cityRepository = new GenericRepository<City>(_context));
        public GenericRepository<Cart> CartRepository =>
            _cartRepository ?? (_cartRepository = new GenericRepository<Cart>(_context));
        public GenericRepository<Product> ProductRepository =>
            _productRepository ?? (_productRepository = new GenericRepository<Product>(_context));
        public GenericRepository<ProductCategory> ProductCategoryRepository =>
            _productCategoryRepository ?? (_productCategoryRepository = new GenericRepository<ProductCategory>(_context));
        public GenericRepository<ConfigSite> ConfigSiteRepository =>
            _configRepository ?? (_configRepository = new GenericRepository<ConfigSite>(_context));
        public GenericRepository<Contact> ContactRepository =>
            _contactRepository ?? (_contactRepository = new GenericRepository<Contact>(_context));
        public GenericRepository<Banner> BannerRepository =>
            _bannerRepository ?? (_bannerRepository = new GenericRepository<Banner>(_context));
        public GenericRepository<Article> ArticleRepository =>
            _articleRepository ?? (_articleRepository = new GenericRepository<Article>(_context));
        public GenericRepository<ArticleCategory> ArticleCategoryRepository =>
            _articategoryRepository ?? (_articategoryRepository = new GenericRepository<ArticleCategory>(_context));
        public GenericRepository<Admin> AdminRepository =>
            _adminRepository ?? (_adminRepository = new GenericRepository<Admin>(_context));
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