using ATTP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATTP.ViewModel
{
    public class AddressViewModel
    {
        public IEnumerable<Address> Addresses { get; set; }
        public Address Address { get; set; }
        [Display(Name = "Tỉnh/Thành phố"), Required(ErrorMessage = "Bạn hãy chọn Tỉnh/Thành phố")]
        public int? CityId { get; set; }
        [Display(Name = "Quận/Huyện"), Required(ErrorMessage = "Bạn hãy chọn Quận/Huyện")]
        public int? DistrictId { get; set; }
        public SelectList CitySelectList { get; set; }
        public SelectList DistrictSelectList { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<District> Districts { get; set; }
        public AddressViewModel()
        {
            DistrictSelectList = new SelectList(new List<District>(), "Id", "Name");
        }
    }

    public class UserOrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        public decimal CartTotal { get; set; }

    }
}