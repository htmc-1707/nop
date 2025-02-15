using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Connect;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string BackgroundColor { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public List<Category> categories { get; set; }
    }
}