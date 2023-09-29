using ATTP.DAL;
using ATTP.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

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

        public ActionResult ReportProduct(int? page, string submit, int? cityId, string fromDate, string toDate, int? status = 2)
        {
            var orderDetails = _unitOfWork.OrderDetailRepository.GetQuery(a => a.Order.Status == status, q => q.OrderByDescending(a => a.Id));

            //if (status.HasValue)
            //{
            //    orderDetails = orderDetails.Where(a => a.Order.Status == status);
            //}
            if (cityId.HasValue)
            {
                orderDetails = orderDetails.Where(a => a.Order.CityId == cityId);
            }
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

            if (submit == "export")
            {
                var dt = new DataTable();
                dt.Columns.Add("STT");
                dt.Columns.Add("Ngày đặt hàng");
                dt.Columns.Add("Mã đơn hàng");
                dt.Columns.Add("Mã SP");
                dt.Columns.Add("Tên SP");
                dt.Columns.Add("Số lượng");
                dt.Columns.Add("Giá tiền");
                dt.Columns.Add("Thành tiền");

                var filename = $"thong-ke-san-pham.xlsx";
                var i = 1;
                foreach (var item in orderDetails)
                {
                    var totalItem = item.Quantity * item.Price;
                    dt.Rows.Add(i, item.Order.CreateDate.ToString("dd/MM/yyyy HH:mm"), item.Order.MaDonHang, item.ProductId, item.Product.Name, item.Quantity, item.Quantity, totalItem?.ToString("N0"));
                    i++;
                }
                using (var pck = new ExcelPackage())
                {
                    //Create the worksheet
                    var ws = pck.Workbook.Worksheets.Add("Danh sách đơn hàng");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(dt, true);

                    //Format the header for column 1-8
                    using (var rng = ws.Cells["A1:H1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    //Example how to Format Column 7 as numeric
                    //using (var col = ws.Cells[2, 7, 2 + dt.Rows.Count, 7])
                    //{
                    //    col.Style.Numberformat.Format = "#,##0";
                    //    col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //}

                    //Write it back to the client
                    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;  filename=" + filename + "");
                    //Response.BinaryWrite(pck.GetAsByteArray());

                    return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
            }
            //var groups = orderDetails.GroupBy(a => new { a.Product }).Select(a => new ReportProductViewModel.ReportProductItem
            //{
            //    TotalSale = a.Sum(c => c.Quantity),
            //    Product = a.Key.Product
            //});

            var pageNumber = page ?? 1;

            var model = new ReportProductViewModel
            {
                CityId = cityId,
                CitySelectList = new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name"),
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                OrderDetails = orderDetails.ToPagedList(pageNumber, 50),
                TotalAmount = orderDetails.Sum(c => c.Quantity * c.Price) ?? 0m
                //ReportProductItems = groups.OrderBy(a => a.Product.Sort).ToPagedList(pageNumber, 50)
            };

            return View(model);
        }

        public ActionResult ReportOrder(int? page, string submit, int? cityId, string fromDate, string toDate, int? status = 2)
        {
            var orders = _unitOfWork.OrderRepository.GetQuery(a => a.Status == status, q => q.OrderByDescending(a => a.Id));

            if (status.HasValue)
            {
                orders = orders.Where(a => a.Status == status);
            }
            if (cityId.HasValue)
            {
                orders = orders.Where(a => a.CityId == cityId);
            }
            if (DateTime.TryParse(fromDate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var fd))
            {
                orders = orders.Where(a =>
                    DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(fd));
            }
            if (DateTime.TryParse(toDate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var td))
            {
                orders = orders.Where(a => DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(td));
            }
            if (submit == "export")
            {
                var dt = new DataTable();
                dt.Columns.Add("STT");
                dt.Columns.Add("Ngày đặt hàng");
                dt.Columns.Add("Mã đơn hàng");
                dt.Columns.Add("Số tiền");
                dt.Columns.Add("Số lượng");
                dt.Columns.Add("Trạng thái");

                var filename = $"thong-ke-san-pham.xlsx";
                var i = 1;
                foreach (var item in orders)
                {
                    string statusName;
                    switch (item.Status)
                    {
                        case 1:
                            statusName = "Đang giao hàng";
                            break;
                        case 2:
                            statusName = "Hoàn tất";
                            break;
                        case 3:
                            statusName = "Hủy đơn";
                            break;
                        default:
                            statusName = "Chờ xử lý";
                            break;
                    }
                    dt.Rows.Add(i, item.CreateDate.ToString("dd/MM/yyyy HH:mm"), item.MaDonHang, item.Total(), item.OrderDetails.Sum(c => c.Quantity), statusName);
                    i++;
                }
                using (var pck = new ExcelPackage())
                {
                    //Create the worksheet
                    var ws = pck.Workbook.Worksheets.Add("Danh sách đơn hàng");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(dt, true);

                    //Format the header for column 1-8
                    using (var rng = ws.Cells["A1:F1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    //Example how to Format Column 7 as numeric
                    //using (var col = ws.Cells[2, 7, 2 + dt.Rows.Count, 7])
                    //{
                    //    col.Style.Numberformat.Format = "#,##0";
                    //    col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //}

                    //Write it back to the client
                    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;  filename=" + filename + "");
                    //Response.BinaryWrite(pck.GetAsByteArray());

                    return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
            }

            var pageNumber = page ?? 1;

            var model = new ReportOrderViewModel
            {
                CityId = cityId,
                CitySelectList = new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name"),
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                Orders = orders.ToPagedList(pageNumber, 50),
                TotalAmount = orders.Sum(c => c.OrderDetails.Sum(d => d.Quantity * d.Price)) ?? 0m
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