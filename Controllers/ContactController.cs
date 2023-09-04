using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATTP.DAL;
using ATTP.Models;
using ATTP.ViewModel;
using System.Data.Entity;

namespace ATTP.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Contact
        public ActionResult ListContact(int? page, string name)
        {
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var contact = _unitOfWork.ContactRepository.GetQuery(orderBy: l => l.OrderByDescending(a => a.Id));

            if (!string.IsNullOrEmpty(name))
            {
                contact = contact.Where(l => l.FullName.Contains(name));
            }
            var model = new ListContactViewModel
            {
                Contacts = contact.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        [HttpPost]
        public bool DeleteContact(int contactId = 0)
        {
            var contact = _unitOfWork.ContactRepository.GetById(contactId);
            if (contact == null)
            {
                return false;
            }
            _unitOfWork.ContactRepository.Delete(contact);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Subcribe
        public ActionResult ListSubcribe(int? page, string name)
        {
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var subcribes = _unitOfWork.SubcribeRepository.GetQuery(orderBy: l => l.OrderByDescending(a => a.Id));
            if (!string.IsNullOrEmpty(name))
            {
                subcribes = subcribes.Where(l => l.Email.ToLower().Contains(name.ToLower()));
            }
            var model = new ListSubcribeViewModel
            {
                Subcribes = subcribes.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        [HttpPost]
        public bool DeleteSubcribe(int subId = 0)
        {
            var subcribe = _unitOfWork.SubcribeRepository.GetById(subId);
            if (subcribe == null)
            {
                return false;
            }
            _unitOfWork.SubcribeRepository.Delete(subcribe);
            _unitOfWork.Save();
            return true;
        }
        #endregion
        #region Feedback
        public ActionResult ListFeedback(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 10;
            var feedback = _unitOfWork.FeedbackRepository.GetQuery(orderBy: l => l.OrderBy(a => a.Sort));

            if (!string.IsNullOrEmpty(name))
            {
                feedback = feedback.Where(l => l.Name.Contains(name));
            }
            var model = new ListFeedbackViewModel
            {
                Feedbacks = feedback.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        public ActionResult Feedback()
        {
            var model = new Feedback
            {
                Sort = 1,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Feedback(Feedback model)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/feedbacks/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            model.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                        }
                    }
                }
                if (isPost)
                {
                    _unitOfWork.FeedbackRepository.Insert(model);
                    _unitOfWork.Save();

                    return RedirectToAction("ListFeedback", new { result = "success" });
                }
            }
            return View(model);
        }
        public ActionResult UpdateFeedback(int feedbackId = 0)
        {
            var feedback = _unitOfWork.FeedbackRepository.GetById(feedbackId);
            if (feedback == null)
            {
                return RedirectToAction("ListFeeback");
            }
            return View(feedback);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateFeedback(Feedback model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var feedback = _unitOfWork.FeedbackRepository.GetById(model.Id);

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/feedbacks/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            feedback.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }

                if (isPost)
                {
                    feedback.Name = model.Name;
                    feedback.Sort = model.Sort;
                    feedback.Content = model.Content;
                    feedback.Active = model.Active;
                    feedback.Classify = model.Classify;


                    _unitOfWork.FeedbackRepository.Update(feedback);
                    _unitOfWork.Save();

                    return RedirectToAction("ListFeedback", new { result = "update" });
                }
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteFeedback(int feedbackId = 0)
        {
            var feedback = _unitOfWork.FeedbackRepository.GetById(feedbackId);
            if (feedback == null)
            {
                return false;
            }
            _unitOfWork.FeedbackRepository.Delete(feedback);
            _unitOfWork.Save();
            return true;
        }
        #endregion
        #region Question
        public ActionResult ListQuestion(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 10;
            var question = _unitOfWork.QuestionRepository.GetQuery(orderBy: l => l.OrderBy(a => a.Id));

            if (!string.IsNullOrEmpty(name))
            {
                question = question.Where(l => l.Ques.Contains(name));
            }
            var model = new LisQuestionViewModel
            {
                Questions = question.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        public ActionResult Question()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Question(Question model)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                if (isPost)
                {
                    _unitOfWork.QuestionRepository.Insert(model);
                    _unitOfWork.Save();

                    return RedirectToAction("ListQuestion", new { result = "success" });
                }
            }
            return View(model);
        }
        public ActionResult UpdateQuestion(int questionId = 0)
        {
            var question = _unitOfWork.QuestionRepository.GetById(questionId);
            if (question == null)
            {
                return RedirectToAction("ListQuestion");
            }
            return View(question);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateQuestion(Question model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var question = _unitOfWork.QuestionRepository.GetById(model.Id);
                if (isPost)
                {
                    question.Ques = model.Ques;
                    question.Reply = model.Reply;
                    question.Active = model.Active;


                    _unitOfWork.QuestionRepository.Update(question);
                    _unitOfWork.Save();

                    return RedirectToAction("ListQuestion", new { result = "update" });
                }
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteQuestion(int questionId = 0)
        {
            var question = _unitOfWork.QuestionRepository.GetById(questionId);
            if (question == null)
            {
                return false;
            }
            _unitOfWork.QuestionRepository.Delete(question);
            _unitOfWork.Save();
            return true;
        }
        #endregion


        #region User
        public ActionResult ListUser(int? page, string email, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 10;
            var user = _unitOfWork.UserRepository.Get(orderBy: l => l.OrderBy(a => a.CreateDate));
            var model = new ListUserViewModel
            {
                Users = user.ToPagedList(pageNumber, pageSize),
                Email = email,
            };
            return View(model);
        }
        public bool DeleteUser(int IdUser)
        {
            var userName = _unitOfWork.UserRepository.GetById(IdUser);
            if (userName == null)
            {
                return false;
            }
            _unitOfWork.UserRepository.Delete(userName);
            _unitOfWork.Save();
            return true;
        }
        public PartialViewResult GetOrderUser(int IdUser)
        {
            var order = _unitOfWork.OrderRepository.GetQuery(a => a.UserId == IdUser);
            if (order == null)
            {
                return PartialView(order);
            }
            var model = new LoadUserOrderViewModel
            {
                Orders = order
            };
            return PartialView(model);
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}