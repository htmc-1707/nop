using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Connect;
using WebsiteBanHang.Models;
using PagedList;

namespace WebsiteBanHang.Controllers
{
    public class CategoryController : Controller
    {
        WebsiteBanHangEntities objWebsiteBanHangEntities = new WebsiteBanHangEntities();
        // GET: Category
        public ActionResult Index()
        {
            var lstcategory = objWebsiteBanHangEntities.Categories.ToList();

            // Danh sách màu tùy ý
            var colors = new List<string> { "#ffd7d7", "#FFF68D", "#bcffb8", "#c9fff3", "#ddffeb", "#dee4ff", "#ddffeb", "#dee4ff" };
            int index = 0;

            // Chuyển đổi sang CategoryModel
            var categoryModels = lstcategory.Select(category => new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image,
                BackgroundColor = colors[index++ % colors.Count] // Gán màu từ danh sách
            }).ToList();

            return View(categoryModels);
        }
        public ActionResult ProductCategory(int id, int? page)
        {
            if (page == null) page = 1;
            var listProduct = objWebsiteBanHangEntities.Products.Where(n => n.CategoryId == id).ToList();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(listProduct.ToPagedList(pageNumber, pageSize));
        }
    }
}