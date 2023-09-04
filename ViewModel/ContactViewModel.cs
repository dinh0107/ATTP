using System.Collections.Generic;
using ATTP.Models;

namespace ATTP.ViewModel
{
    public class ListContactViewModel
    {
        public PagedList.IPagedList<Contact> Contacts { get; set; }
        public string Name { get; set; }
    }

    public class ListSubcribeViewModel
    {
        public PagedList.IPagedList<Subcribe> Subcribes { get; set; }
        public string Name { get; set; }
    }
    public class ListFeedbackViewModel
    {
        public PagedList.IPagedList<Feedback> Feedbacks { get; set; }
        public string Name { get; set; }
    }
    public class LisQuestionViewModel
    {
        public PagedList.IPagedList<Question> Questions { get; set; }
        public string Name { get; set; }
    }
    public class ListUserViewModel
    {
        public PagedList.IPagedList<User> Users { get; set; }
        public string Email { get; set; }
    }
    public class LoadUserOrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
    }
    public class ListMemberViewModel
    {
        public PagedList.IPagedList<Member> Members { get; set; }
        public string Name { get; set; }
    }
}