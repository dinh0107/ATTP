using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ATTP.Models
{
    public class Member
    {
        public int Id { get; set; }
        [Display(Name = "Họ tên", Description = "Tên dài tối đa 100 ký tự"),
       Required(ErrorMessage = "Hãy nhập Họ tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Chức vụ", Description = "Dài tối đa 100 ký tự"),
         StringLength(100, ErrorMessage = "Tối đa 100 ký tự"),
         UIHint("TextBox")]
        public string Classify { get; set; }
        [Display(Name = "Mô tả ngắn"), UIHint("TextArea")]
        public string Description { get; set; }
        [Display(Name = "Chi tiết"), UIHint("EditorBox")]
        public string Body { get; set; }

        [Display(Name = "Ảnh đại diện"), UIHint("ImageMember")]
        public string Image { get; set; }

        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Hiện bên trái")]
        public bool Right { get; set; }
        public Member()
        {
            Active = true;
            Sort = 1;
        }
    }
}