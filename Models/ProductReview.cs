using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ATTP.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        [Display(Name = "Nội dung"), UIHint("TextArea")]
        public string Comment { get; set; }
        public int Likes { get; set; }
        public string ListImage { get; set; }
        public int ParentReviewId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
        public ProductReview()
        {
            CreatedAt = DateTime.Now;
            Likes = 0;
        }
    }
}