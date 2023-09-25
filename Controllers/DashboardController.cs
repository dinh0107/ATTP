using ATTP.Filters;
using ATTP.Models;
using ATTP.ViewModel;
using Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ATTP.Controllers
{
    [MemberFilter]
    public class DashboardController : BaseController
    {
        private string Username => RouteData.Values["Username"].ToString();
        private string Email => RouteData.Values["Email"].ToString();
        private new User User => _unitOfWork.UserRepository.Get(a => (a.Email == Email || a.Username == Username)).SingleOrDefault();

        [ChildActionOnly]
        public PartialViewResult UserNav()
        {
            return PartialView(User);
        }
        #region UserInfo
        [Route("thong-tin-ca-nhan")]
        public ActionResult UserInfo(string result = "")
        {
            ViewBag.Result = result;
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Id == User.Id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        [Route("thong-tin-ca-nhan")]
        [HttpPost, ValidateInput(false)]
        public ActionResult UserInfo(User model)
        {
            var user = _unitOfWork.UserRepository.Get(a => a.Id == User.Id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Avatar"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        isPost = false;
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            isPost = false;
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/users/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            model.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                            user.Avatar = model.Avatar;
                        }
                    }

                }
                if (isPost)
                {
                    user.FullName = model.FullName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    _unitOfWork.Save();

                    var userData = user.Username + "|" + user.Avatar + "|" + user.Id + "|" + user.Email;
                    var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);

                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    // Create the cookie.
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));

                    return RedirectToAction("UserInfo", "Dashboard", new { result = "success" });
                }
            }
            return View(model);
        }
        #endregion

        #region UserAddress
        [Route("quan-ly-dia-chi")]
        public ActionResult UserAddress(string result = "")
        {
            ViewBag.Result = result;
            var addresses = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Id));

            var model = new AddressViewModel
            {
                Addresses = addresses,
                CitySelectList = CitySelectList,
            };
            return View(model);
        }
        [Route("quan-ly-dia-chi")]
        [HttpPost]
        public ActionResult UserAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Address.CityId = model.CityId;
                model.Address.DistrictId = model.DistrictId;
                _unitOfWork.AddressRepository.Insert(model.Address);
                _unitOfWork.Save();

                var count = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id).Count();
                var fa = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id).FirstOrDefault();
                if (count <= 1)
                {
                    model.Address.Default = fa.Default = true;
                }
                _unitOfWork.Save();
                return RedirectToAction("UserAddress", new { result = "success" });
            }
            return View(model);
        }
        [Route("cap-nhat-dia-chi")]
        public ActionResult UpdateAddress(int addressId = 0)
        {
            var address = _unitOfWork.AddressRepository.GetById(addressId);
            if (address == null)
            {
                return RedirectToAction("UserAddress");
            }
            var model = new AddressViewModel
            {
                Address = address,
                Cities = _unitOfWork.CityRepository.Get(a => a.Active, o => o.OrderBy(a => a.Sort)),
                Districts = _unitOfWork.DistrictRepository.Get(a => a.Active, o => o.OrderBy(a => a.Sort)),
            };
            return View(model);
        }
        [Route("cap-nhat-dia-chi")]
        [HttpPost]
        public ActionResult UpdateAddress(AddressViewModel model)
        {
            var address = _unitOfWork.AddressRepository.GetById(model.Address.Id);
            if (address == null)
            {
                return RedirectToAction("UserAddress");
            }
            if (ModelState.IsValid)
            {
                address.Fullname = model.Address.Fullname;
                address.Mobile = model.Address.Mobile;
                address.CityId = model.CityId;
                address.DistrictId = model.DistrictId;
                address.SpecificAddress = model.Address.SpecificAddress;

                _unitOfWork.Save();
                return RedirectToAction("UserAddress", new { result = "update" });
            }
            return View(model);
        }
        public bool DefaultAddress(bool addressDefault = false, int addressId = 0)
        {
            var address = _unitOfWork.AddressRepository.GetById(addressId);
            if (address == null)
            {
                return false;
            }
            var addresses = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id);
            foreach (var item in addresses)
            {
                item.Default = false;
            }
            address.Default = addressDefault;

            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool DeleteAddress(int addressId = 0)
        {
            var address = _unitOfWork.AddressRepository.GetById(addressId);
            var fa = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id).FirstOrDefault();
            if (address == null)
            {
                return false;
            }
            if (address.Default == true)
            {
                fa.Default = true;
            }

            _unitOfWork.AddressRepository.Delete(address);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region UserOrder        
        [Route("don-hang-cua-toi")]
        public ActionResult UserOrder()
        {
            return View();
        }
        public PartialViewResult GetUserOrder(int status = 0)
        {
            var orders = _unitOfWork.OrderRepository.GetQuery(a => a.OrderMemberId == User.Id && a.Status == status, o => o.OrderBy(a => a.Id));

            var model = new UserOrderViewModel
            {
                Orders = orders,
                OrderDetails = _unitOfWork.OrderDetailRepository.GetQuery(),
            };

            return PartialView(model);
        }
        [HttpPost]
        public bool CancelOrder(int orderId = 0)
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
        #endregion

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}