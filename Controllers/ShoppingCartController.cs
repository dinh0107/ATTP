using ATTP.Controllers;
using ATTP.DAL;
using ATTP.Models;
using ATTP.ViewModel;
using Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using ATTP.Filters;

namespace BepSaoViet.Controllers
{
    [RoutePrefix("gio-hang"), MemberLoginFilter]
    public class ShoppingCartController : BaseController
    {
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];

        [Route("thong-tin")]
        public ActionResult Index(string returnUrl)
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            var carts = cart.GetCartItems();

            var itemCarts = carts.Select(a => new ShoppingCartViewModel.CartItem
            {
                CartItems = a,
            });


            var viewModel = new ShoppingCartViewModel
            {
                CartItems = itemCarts,
                CartTotal = cart.GetTotal()
            };
            ViewBag.ReturnUrl = returnUrl;
            return View(viewModel);
        }
        [HttpPost, Route("thong-tin")]
        public ActionResult Index(FormCollection fc)
        {
            var records = fc.GetValues("RecordId");
            var quantities = fc.GetValues("Quantity");

            if (records == null || quantities == null)
            {
                return RedirectToActionPermanent("Index");
            }
            for (var i = 0; i < records.Length; i++)
            {
                var recordId = Convert.ToInt32(records[i]);
                var quantity = Convert.ToInt32(quantities[i]);

                var cartItem = _unitOfWork.CartRepository.GetById(recordId);
                if (cartItem == null || cartItem.Count == quantity || quantity < 1) continue;

                cartItem.Count = quantity;
                _unitOfWork.Save();
            }
            return RedirectToActionPermanent("Index");
        }
        [Route("thanh-toan")]
        public ActionResult CheckOut()
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            var userId = Convert.ToInt32(RouteData.Values["Id"]);
            var address = _unitOfWork.AddressRepository.Get(a => a.UserId == userId && a.Default == true).FirstOrDefault();
            if (!cart.GetCartItems().Any())
            {
                return RedirectToAction("Index");
            }
            var carts = cart.GetCartItems();

            var itemCarts = carts.Select(a => new CheckOutViewModel.CartItem
            {
                CartItems = a,
            });
            var model = new CheckOutViewModel
            {
                Order = new Order
                {
                    TypePay = 1,
                    ShipFee = 30000
                },
                CartItems = itemCarts,
                CartTotal = cart.GetTotal(),
                Addresses = _unitOfWork.AddressRepository.Get(a => a.UserId == userId, o => o.OrderBy(a => a.Id)),
                Address = address,
                User = _unitOfWork.UserRepository.GetQuery(a => a.Id == userId).FirstOrDefault(),
                Carts = carts,
                CitySelectList = CitySelectList,
            };
            return View(model);
        }
        [Route("thanh-toan")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CheckOut(CheckOutViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var cart = ShoppingCart.GetCart(HttpContext);
                var item = cart.GetCartItems();
                var userId = Convert.ToInt32(RouteData.Values["Id"]);
                var user = _unitOfWork.UserRepository.GetQuery(a => a.Id == userId).FirstOrDefault();
                var address = _unitOfWork.AddressRepository.GetQuery(a => a.Id == model.AddressId).FirstOrDefault();

                model.Order.CityId = model.CityId;
                model.Order.DistrictId = model.DistrictId;
                if (userId > 0)

                {
                    model.Order.UserId = userId;
                    model.Order.OrderMemberId = userId;

                    model.Order.CustomerInfo = new CustomerInfo
                    {
                        Fullname = user.FullName,
                        Mobile = user.PhoneNumber,
                        Email = user.Email,
                        Address = address.SpecificAddress + address.District.Name + address.City.Name,
                    };
                }
                _unitOfWork.OrderRepository.Insert(model.Order);
                _unitOfWork.Save();
                model.Order.MaDonHang = DateTime.Now.ToString("yyyyMMddHHmm") + "C" + model.Order.Id;

                foreach (var odetails in from cart1 in item
                                         let product = _unitOfWork.ProductRepository.GetById(cart1.ProductId)
                                         select new OrderDetail
                                         {
                                             OrderId = model.Order.Id,
                                             ProductId = product.Id,
                                             Quantity = cart1.Count,
                                             Price = cart1.Price
                                         })
                {
                    _unitOfWork.OrderDetailRepository.Insert(odetails);
                }
                _unitOfWork.Save();

                var tongtien = 0m;
                var district = _unitOfWork.DistrictRepository.GetById(model.DistrictId);

                var typepay = "Thanh toán khi nhận hàng";
                switch (model.Order.TypePay)
                {
                    case 1:
                        typepay = "Chuyển khoản";
                        break;
                    case 2:
                        typepay = "Tiền mặt";
                        break;
                }

                var sb = "<p>Cảm ơn quý khách đã đặt hàng tại website " + Request.Url?.Host + "</p>" +
                         "<p style='font-size:16px'>Thông tin đơn hàng gửi từ website " + Request.Url?.Host + "</p>";
                sb += "<p>Mã đơn hàng: <strong>" + model.Order.CreateDate.ToString("yyMMddHHmmss") + "</strong></p>";

                if (userId > 0)
                {
                    sb += "<p>Họ và tên: <strong>" + user.FullName + "</strong></p>";
                    sb += "<p>Địa chỉ: <strong>" + address.SpecificAddress + ", " + address.District.Name + ", " + address.City.Name + "</strong></p>";
                    sb += "<p>Email: <strong>" + user.Email + "</strong></p>";
                    sb += "<p>Điện thoại: <strong>" + user.PhoneNumber + "</strong></p>";
                }
                else
                {
                    sb += "<p>Họ và tên: <strong>" + model.Order.CustomerInfo.Fullname + "</strong></p>";
                    sb += "<p>Địa chỉ: <strong>" + model.Order.CustomerInfo.Address + ", " + district?.Name + ", " + district?.City.Name + "</strong></p>";
                    sb += "<p>Email: <strong>" + model.Order.CustomerInfo.Email + "</strong></p>";
                    sb += "<p>Điện thoại: <strong>" + model.Order.CustomerInfo.Mobile + "</strong></p>";
                    sb += "<p>Yêu cầu thêm: <strong>" + model.Order.CustomerInfo.Body + "</strong></p>";
                }

                sb += "<p>Ngày đặt hàng: <strong>" + model.Order.CreateDate.ToString("dd-MM-yyyy HH:ss") + "</strong></p>";
                sb += "<p>Hình thức thanh toán: <strong>" + typepay + "</strong></p>";
                sb += "<p>Thông tin đơn hàng</p>";
                sb += "<table border='1' cellpadding='10' style='border:1px #ccc solid;border-collapse: collapse'>" +

                      "<tr>" +
                      "<th>Ảnh sản phẩm</th>" +
                      "<th>Tên sản phẩm</th>" +
                      "<th>Số lượng</th>" +
                      "<th>Giá tiền</th>" +
                      "<th>Thành tiền</th>" +
                      "</tr>";
                foreach (var odetails in model.Order.OrderDetails)
                {
                    var thanhtien = Convert.ToDecimal(odetails.Price * odetails.Quantity);
                    tongtien += thanhtien;

                    var img = "NO PICTURE";
                    if (odetails.Product.ListImage != null)
                    {
                        img = "<img src='https://" + Request.Url?.Host + "/images/products/" + odetails.Product.ListImage.Split(',')[0] + "?w=100' />";
                    }
                    sb += "<tr>" +
                          "<td>" + img + "</td>" +
                          "<td><a href='https://" + Request.Url?.Host + Url.Action("Product", "Home", new { proId = odetails.ProductId, name = HtmlHelpers.ConvertToUnSign(null, odetails.Product.Name) }) + "' >" + odetails.Product.Name + "</a>";
                    sb += "</td>" +
                          "<td style='text-align:center'>" + odetails.Quantity + "</td>" +
                          "<td style='text-align:center'>" + Convert.ToDecimal(odetails.Price).ToString("N0") + "</td>" +
                          "<td style='text-align:center'>" + thanhtien.ToString("N0") + " đ</td>" +
                          "</tr>";
                }
                sb += "<tr><td colspan='5' style='text-align:right'><strong>Tổng tiền: " + tongtien.ToString("N0") + " đ</strong></td></tr>";
                sb += "</table>";
                sb += "<p>Cảm ơn bạn đã tin tưởng và mua hàng của chúng tôi.</p>";

                var orderId = model.Order.Id;
                Task.Run(() =>
                {
                    HtmlHelpers.SendEmail("gmail", "[" + orderId + "] Đơn đặt hàng từ website " + Request.Url?.Host, sb,
                        ConfigSite.Email, Email, Email, Password, "Đặt Hàng Online", model.Order.CustomerInfo.Email, ConfigSite.Email);
                });

                return RedirectToAction("CheckOutComplete", new { orderId });
            }
            return View(model);
        }

        [Route("thanh-toan-thanh-cong")]
        public ActionResult CheckOutComplete(int orderId)
        {
            EmptyCart();
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            return View(order);
        }

        public ActionResult EmptyCart()
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            cart.EmptyCart();
            return RedirectToAction("Index");
        }

        [Route("them-vao-gio-hang")]
        public JsonResult AddToCart(int? tayThuanId, int? loftId, int? flexId, int productId, string returnUrl, int quantity = 1)
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            decimal? price = null;

            var addedProduct = _unitOfWork.ProductRepository.GetQuery(a => a.Id == productId).SingleOrDefault();
            if (addedProduct?.PriceSale != null)
            {
                price = addedProduct.PriceSale;
            }
            else if (addedProduct?.Price != null)
            {
                price = addedProduct.Price;
            }
            try
            {
                cart.AddToCart(productId, price, quantity);
                var data = new
                {
                    result = 1,
                    count = cart.GetCount()
                };
                return Json(data);
            }
            catch
            {
                var data = new
                {
                    result = 0,
                    count = cart.GetCount()
                };
                return Json(data);
            }
        }

        [HttpPost]
        public void AddProduct(int sid = 0, int pid = 0, int quantity = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(pid);
            if (product == null) return;
            var cart = _unitOfWork.CartRepository.GetById(sid);
            if (cart == null) return;
            cart.Count = quantity;
            _unitOfWork.Save();
        }
        [HttpPost]
        public JsonResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(HttpContext);

            // Get the name of the album to display confirmation
            var productName = _unitOfWork.CartRepository.GetById(id).Product.Name;

            // Remove from cart
            var itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = productName + " đã được xóa khỏi giỏ hàng của bạn.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                Status = itemCount,
                DeleteId = id
            };
            return Json(results);
        }
        public PartialViewResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            var model = new CartSummaryViewModel
            {
                Carts = cart.GetCartItems(),
                Count = cart.GetCount(),
                TotalMoney = cart.GetTotal()
            };
            return PartialView("CartSummary", model);
        }
        [HttpPost]
        public JsonResult UpdateCartV2(int productId, int changeValue)
        {
            try
            {
                var cart = ShoppingCart.GetCart(HttpContext);
                var addedProduct = cart.GetCartItems().FirstOrDefault(a => a.RecordId == productId);
                if (addedProduct != null)
                {
                    var itemCount = cart.UpdateToCart(addedProduct, changeValue);
                    var totalMoneyItem = addedProduct.Price * itemCount;
                    var statistic = new CartStatistic
                    {
                        Status = 0,
                        Msg = "Cập nhật thành công",
                        itemCont = itemCount,
                        totalItem = cart.GetCount(),
                        totalMoneyItem = totalMoneyItem ?? 0,
                        totalMoney = cart.GetTotal(),
                    };
                    return Json(statistic);
                }
                return Json(new CartStatistic
                {
                    Status = 0,
                    totalItem = 0,
                    totalMoney = 0,
                    Msg = "Cập nhật thành công",
                });
            }
            catch (Exception)
            {
                return Json(new CartStatistic
                {
                    Status = 1,
                    totalMoney = 0,
                    itemCont = 0,
                    totalItem = 0,
                    Msg = "Cập nhật không thành công",
                });
            }
        }
        public PartialViewResult BankInfo()
        {
            return PartialView();
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}