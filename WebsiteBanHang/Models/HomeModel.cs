using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteBanHang.Connect;

namespace WebsiteBanHang.Models
{
    public class HomeModel
    {
        public List<Category> categories { get; set; }
        public List<Product> products {  get; set; }
        public List<Brand> brands { get; set; }
    }
}