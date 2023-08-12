using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Doris.DAL;

namespace Doris.Models
{
    public class ShoppingCart
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }
        public void AddToCart(int proId, decimal? price, int quantity = 1)
        {
            var cartItem = _unitOfWork.CartRepository.GetQuery
                (c => c.CartId == ShoppingCartId && c.ProductId == proId).SingleOrDefault();

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    ProductId = proId,
                    Price = price,
                    PriceUser = price,
                    CartId = ShoppingCartId,
                    Count = quantity,
                    DateCreated = DateTime.Now
                };
                _unitOfWork.CartRepository.Insert(cartItem);
            }
            else
            {
                cartItem.Count += quantity;
            }
            _unitOfWork.Save();
        }
        public int RemoveFromCart(int id)
        {
            var cartItem = _unitOfWork.CartRepository.Get(cart => cart.CartId == ShoppingCartId && cart.RecordId == id).SingleOrDefault();
            if (cartItem != null)
            {
                _unitOfWork.CartRepository.Delete(cartItem);
                _unitOfWork.Save();
                return 1;
            }
            return 0;
        }
        public void EmptyCart()
        {
            var cartItems = _unitOfWork.CartRepository.Get(cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                _unitOfWork.CartRepository.Delete(cartItem);
            }
            _unitOfWork.Save();
        }
        public List<Cart> GetCartItems()
        {
            return _unitOfWork.CartRepository.Get(cart => cart.CartId == ShoppingCartId).ToList();
        }
        public int GetCount()
        {
            var count = (from cartItems in _unitOfWork.CartRepository.Get()
                         where cartItems.CartId == ShoppingCartId
                         select (int?)cartItems.Count).Sum();
            return Convert.ToInt32(count);
        }
        public decimal GetTotal()
        {
            var total = (from cartItems in _unitOfWork.CartRepository.Get()
                         where cartItems.CartId == ShoppingCartId
                         select (int?)cartItems.Count * cartItems.Price ?? cartItems.Product.Price).Sum();

            return Convert.ToDecimal(total);
        }
        public decimal GetTotalUser()
        {
            var total = (from cartItems in _unitOfWork.CartRepository.Get()
                         where cartItems.CartId == ShoppingCartId
                         select (int?)cartItems.Count * cartItems.PriceUser).Sum();

            return Convert.ToDecimal(total);
        }
        public decimal GetTotalShipFee()
        {
            decimal totalShipFee = 0;
            if (GetTotal() > 0)
            {
                totalShipFee = GetTotal() + 30000;

            }
            return totalShipFee;
        }
        public decimal GetTotalUserShipFee()
        {
            decimal totalShipFee = 0;
            if (GetTotalUser() > 0)
            {
                totalShipFee = GetTotalUser() + 30000;
            }
            return totalShipFee;
        }
        public int CreateOrder(Order order)
        {
            var cartItems = GetCartItems();
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                    Price = Convert.ToDecimal(item.Product.Price),
                    Quantity = item.Count
                };

                _unitOfWork.OrderDetailRepository.Insert(orderDetail);
            }
            _unitOfWork.Save();
            EmptyCart();
            return order.Id;
        }
        public string GetCartId(HttpContextBase context)
        {
            var cookie = context.Request.Cookies[".ASPXAUTHMEMBER"];
            if (cookie != null)
            {
                var ticketInfo = FormsAuthentication.Decrypt(cookie.Value);
                context.Session[CartSessionKey] = ticketInfo?.Name;
            }
            else if (context.Session[CartSessionKey] == null)
            {
                var tempCartId = Guid.NewGuid();
                context.Session[CartSessionKey] = tempCartId.ToString();
            }
            return context.Session[CartSessionKey].ToString();
        }
        //public void MigrateCart(string userName)
        //{
        //    var user = _unitOfWork.UserRepository.GetQuery(a => a.Email == userName).Single();
        //    var shoppingCart = _unitOfWork.CartRepository.Get(c => c.CartId == ShoppingCartId);

        //    foreach (var item in shoppingCart)
        //    {
        //        item.CartId = userName;
        //    }
        //    _unitOfWork.Save();
        //}
    }
}