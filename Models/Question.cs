using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ATTP.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Display(Name = "Câu hỏi", Description = "Câu hỏi"), Required(ErrorMessage = "Hãy điền câu hỏi"), UIHint("TextBox")]
        public string Ques { get; set; }
        [DisplayName("Trả lời"), Required(ErrorMessage = "Hãy nhập câu trả lời"), UIHint("TextArea")]
        public string Reply { get; set; }
        [Display(Name = "Hoạt động", Description = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự")
        , RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        public Question()
        {
            Active = true;
            Sort = 1;
        }
    }
}