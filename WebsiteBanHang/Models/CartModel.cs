using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteBanHang.Connect;

namespace WebsiteBanHang.Models
{
    public class CartModel
    {
        public Product product { get; set; }
        public Stock stocks { get; set; }
        public Brand brands { get; set; }
        public int Quantity { get; set; }
    }
}