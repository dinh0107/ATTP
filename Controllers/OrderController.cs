using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ATTP.DAL;
using ATTP.Models;
using ATTP.ViewModel;

namespace ATTP.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        // GET: Order
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult ListOrder(int? page, int? cityId, string madonhang, string fromdate, string todate, string customerName, string customerEmail, string customerMobile, int status = -1, int payment = 0, int pageSize = 50)
        {
            var pageNumber = page ?? 1;
            var orders = _unitOfWork.OrderRepository.GetQuery(orderBy: q => q.OrderByDescending(a => a.Id));

            if (!string.IsNullOrEmpty(madonhang))
            {
                orders = orders.Where(a => a.MaDonHang.Contains(madonhang));
            }
            if (!string.IsNullOrEmpty(customerName))
            {
                orders = orders.Where(a => a.CustomerInfo.Fullname.ToLower().Contains(customerName.ToLower()));
            }
            if (!string.IsNullOrEmpty(customerEmail))
            {
                orders = orders.Where(a => a.CustomerInfo.Email.ToLower().Contains(customerEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(customerMobile))
            {
                orders = orders.Where(a => a.CustomerInfo.Mobile.Contains(customerMobile));
            }
            if (cityId.HasValue)
            {
                orders = orders.Where(a => a.CityId == cityId);
            }
            if (DateTime.TryParse(fromdate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var fd))
            {
                orders = orders.Where(a => DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(fd));
            }
            if (DateTime.TryParse(todate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var td))
            {
                orders = orders.Where(a => DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(td));
            }
            if (status >= 0)
            {
                orders = orders.Where(a => a.Status == status);
            }
            else
            {
                orders = orders.Where(a => a.Status != 3);
            }
            if (payment > 0)
            {
                switch (payment)
                {
                    case 1:
                        orders = orders.Where(a => !a.Payment);
                        break;
                    case 2:
                        orders = orders.Where(a => a.Payment);
                        break;
                }
            }

            var model = new ListOrderViewModel
            {
                Orders = orders.ToPagedList(pageNumber, pageSize),
                MaDonhang = madonhang,
                Status = status,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerMobile = customerMobile,
                FromDate = fromdate,
                ToDate = todate,
                PageSize = pageSize,
                Payment = payment,
                CityId = cityId,
                CitySelectList = new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name")
            };

            return View(model);
        }
        public PartialViewResult LoadOrder(int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);

            var model = new OrderViewModel
            {
                Order = order
            };
            return PartialView(model);
        }

        public ActionResult ReportProduct(int? page, int? cityId, string fromDate, string toDate, int? status = 2)
        {
            var orderDetails = _unitOfWork.OrderDetailRepository.GetQuery();

            if (status.HasValue)
            {
                orderDetails = orderDetails.Where(a => a.Order.Status == status);
            }
            if (cityId.HasValue)
            {
                orderDetails = orderDetails.Where(a => a.Order.CityId == cityId);
            }

            var products = _unitOfWork.ProductRepository.GetQuery();


            // from date
            if (DateTime.TryParse(fromDate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var fd))
            {
                orderDetails = orderDetails.Where(a =>
                    DbFunctions.TruncateTime(a.Order.CreateDate) >= DbFunctions.TruncateTime(fd));
            }
            // to date
            if (DateTime.TryParse(toDate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var td))
            {
                orderDetails = orderDetails.Where(a => DbFunctions.TruncateTime(a.Order.CreateDate) <= DbFunctions.TruncateTime(td));
            }

            var groups = orderDetails.GroupBy(a => new { a.ProductId }).Select(a => new ReportProductViewModel.ReportProductItem
            {
                TotalSale = a.Sum(c => c.Quantity),
                Product = products.FirstOrDefault(s => s.Id == a.Key.ProductId),
            });

            var pageNumber = page ?? 1;

            var model = new ReportProductViewModel
            {
                CityId = cityId,
                CitySelectList = new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name"),
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                ReportProductItems = groups.OrderBy(a => a.Product.Sort).ToPagedList(pageNumber, 50)
            };

            return View(model);
        }

        [HttpPost]
        public bool UpdateOrder(string notice, int payment = 0, int status = 0, int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            var orderdetails = _unitOfWork.OrderDetailRepository.GetQuery(a => a.OrderId == order.Id);
            if (order == null)
            {
                return false;
            }
            if (status >= 0)
            {
                order.Status = status;
            }
            if (status == 2)
            {
                foreach (var item in orderdetails)
                {
                    item.Product.Quantity = item.Product.Quantity - item.Quantity;
                    if (item.Product.Quantity < 0)
                    {
                        item.Product.Quantity = 0;
                    }
                }
            }
            if (payment > 0)
            {
                switch (payment)
                {
                    case 1:
                        order.Payment = false;
                        break;
                    case 2:
                        order.Payment = true;
                        break;
                }
            }

            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool UpdateOrderNotice(string notice, int thanhtoantruoc = 0, int ship = 0, int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null)
            {
                return false;
            }

            order.ShipFee = ship;
            order.ThanhToanTruoc = thanhtoantruoc;
            order.CustomerInfo.Body = notice;
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool DeleteOrder(int orderId = 0)
        {

            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = 3;
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool ParmanentDeleteOrder(int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null || order.Status != 3)
            {
                return false;
            }
            _unitOfWork.OrderRepository.Delete(order);
            _unitOfWork.Save();
            return true;

        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}