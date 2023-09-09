using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATTP.Models
{
    public class ArticleCategory
    {
        public int Id { get; set; }
        [Display(Name = "Tên danh mục"), Required(ErrorMessage = "Hãy nhập tên danh mục"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string CategoryName { get; set; }
        [Display(Name = "Nội dung bài viết"), UIHint("TextArea")]
        public string Description { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int CategorySort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool CategoryActive { get; set; }
        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }
        [Display(Name = "Hiển thị menu")]
        public bool ShowMenu { get; set; }
        [Display(Name = "Hiển thị trang chủ")]
        public bool Home { get; set; }
        [Display(Name = "Hiển thị chân trang")]
        public bool ShowFooter { get; set; }
        //[Display(Name = "Hiển menu đầu trang")]
        //public bool SubMenu { get; set; }
        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        [Display(Name = "Ảnh bìa"), StringLength(500), UIHint("ImageArticleCat")]
        public string Image { get; set; }
        [Display(Name = "Ảnh Icon"), StringLength(500), UIHint("ImageArticleCat")]
        public string ImageIcon { get; set; }
        [Display(Name = "Loại danh mục")]
        public TypePost TypePost { get; set; }
        public virtual ICollection<Article> Articles { get; set; }

        public ArticleCategory()
        {
            CategoryActive = true;
        }
    }

    public enum TypePost
    {
        [Display(Name = "Tin tức")]
        Article,
        [Display(Name = "Sản phẩm")]
        Product,
        [Display(Name = "Chính sách")]
        Policy,
        [Display(Name = "Tuyển dụng")]
        Recruitment,
    }
}