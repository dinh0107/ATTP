using ATTP.DAL;
using ATTP.Models;
using ATTP.ViewModel;
using Helpers;
using PagedList;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ATTP.Controllers
{
    public class MemberController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Member
        public ActionResult ListMember(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 10;
            var members = _unitOfWork.MemberRepository.Get(orderBy: l => l.OrderBy(a => a.Sort));
            var model = new ListMemberViewModel
            {
                Members = members.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }

        public ActionResult Member()
        {

            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Member(Member model)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/member/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            model.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = System.Drawing.Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                        }
                    }
                }
                _unitOfWork.MemberRepository.Insert(model);
                _unitOfWork.Save();
                return RedirectToAction("ListMember", new { result = "success" });
            }
            return View(model);
        }

        public ActionResult UpdateMember(int memberId = 0)
        {


            var member = _unitOfWork.MemberRepository.GetById(memberId);
            if (member == null)
            {
                return RedirectToAction("ListMember");
            }
            return View(member);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateMember(Member model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/member/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            if (System.IO.File.Exists(Server.MapPath("/images/member/" + model.Image)))
                            {
                                System.IO.File.Delete(Server.MapPath("/images/member/" + model.Image));
                            }
                            model.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = System.Drawing.Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                        }
                    }
                }
                _unitOfWork.MemberRepository.Update(model);
                _unitOfWork.Save();

                return RedirectToAction("ListMember", new { result = "update" });
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteMember(int memberId = 0)
        {


            var member = _unitOfWork.MemberRepository.GetById(memberId);
            if (member == null)
            {
                return false;
            }
            _unitOfWork.MemberRepository.Delete(memberId);
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