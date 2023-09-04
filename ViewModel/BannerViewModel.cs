using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATTP.Models;

namespace ATTP.ViewModel
{
    public class BannerViewModel
    {
        public Banner Banner { get; set; }
        public SelectList SelectGroup { get; set; }
        public BannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner" },
                { 2, "Con số biết nói" },
                { 3, "Đối tác" },
                { 4, "Video" },
                { 5, "Chính sách" },
                { 6, "Tầm nhìn sứ mệnh GTCL" },
                { 7, "Ảnh" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
    public class ListBannerViewModel
    {
        public PagedList.IPagedList<Banner> Banners { get; set; }
        public int? GroupId { get; set; }
        public SelectList SelectGroup { get; set; }
        public ListBannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner" },
                { 2, "Con số biết nói" },
                { 3, "Đối tác" },
                { 4, "Video" },
                { 5, "Chính sách" },
                { 6, "Tầm nhìn sứ mệnh GTCL" },
                { 7, "Ảnh" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
}